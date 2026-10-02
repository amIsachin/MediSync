using Qdrant.Client;
using Qdrant.Client.Grpc;

namespace MediSync.AI.Presentation.Services;

public class VectorStoreService : IVectorStoreService
{
    private readonly ILogger<VectorStoreService> _logger;
    private readonly QdrantClient _qdrantClient;
    private readonly IConfiguration _configuration;
    private readonly IEmbeddingService _embeddingService;

    public VectorStoreService(ILogger<VectorStoreService> logger, QdrantClient qdrantClient, IConfiguration configuration, IEmbeddingService embeddingService)
    {
        _logger = logger;
        _qdrantClient = qdrantClient;
        _configuration = configuration;
        _embeddingService = embeddingService;
    }

    private string CollectionName
        => _configuration["AI:QdrantCollection"]
            ?? "medisync_patient_records";

    public async Task DeletePatientRecordsAsync(Guid patientId, CancellationToken cancellationToken = default)
    {
        try
        {
            var filter = new Filter
            {
                Must =
                {
                    new Condition
                    {
                        Field  = new FieldCondition
                        {
                            Key="patient_id",
                            Match = new Match
                            {
                                Keyword = patientId.ToString()
                            }
                        }
                    }
                }
            };

            if (await _qdrantClient.CollectionExistsAsync(CollectionName, cancellationToken))
            {
                await _qdrantClient.DeleteAsync(CollectionName, filter: filter, cancellationToken: cancellationToken);
            }

            _logger.LogInformation("Deleted all records for patient {PatientId}", patientId);

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Delete failed for patient {PatientId}", patientId);
        }
    }

    public async Task EnsureCollectionExistsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var isExists = await _qdrantClient.CollectionExistsAsync(CollectionName, cancellationToken);

            if (isExists)
            {
                _logger.LogInformation("Collection {Collection} already exists", CollectionName);
                await _qdrantClient.DeleteCollectionAsync(CollectionName, cancellationToken: cancellationToken);
                return;
            }

            VectorParams vectorParams = new VectorParams
            {
                Size = 1024,
                Distance = Distance.Cosine
            };

            await _qdrantClient.CreateCollectionAsync(CollectionName, vectorParams, cancellationToken: cancellationToken);

            _logger.LogInformation("Collection {Collection} created", CollectionName);

            await _qdrantClient.CreatePayloadIndexAsync(
            CollectionName,
            fieldName: "patient_id",
            schemaType: PayloadSchemaType.Uuid,  // ← Keyword for string matching
            cancellationToken: cancellationToken);

            _logger.LogInformation("Index created on patient_id");

            await _qdrantClient.CreatePayloadIndexAsync(
                CollectionName,
                fieldName: "record_type",
                schemaType: PayloadSchemaType.Keyword,
                cancellationToken: cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to ensure collection {Collection} exists", CollectionName);
        }
    }

    public async Task<List<PatientRecordChunk>> GetAllPatientRecordsAsync(Guid patientId, CancellationToken cancellationToken = default)
    {
        if (!await _qdrantClient.CollectionExistsAsync(CollectionName, cancellationToken))
        {
            return new List<PatientRecordChunk>();
        }

        var filter = new Filter
        {
            Must =
            {
                new Condition
                {
                    Field = new FieldCondition
                    {
                        Key = "patient_id",
                        Match = new Match
                        {
                            Keyword = patientId.ToString()
                        }
                    }
                }
            }
        };

        // Scroll through ALL records — not just top 5
        var results = await _qdrantClient.ScrollAsync(
            CollectionName,
            filter: filter,
            limit: 100,   // max 100 chunks per patient
            payloadSelector: new WithPayloadSelector { Enable = true },
            vectorsSelector: new WithVectorsSelector { Enable = false }, // no need for vectors here
            cancellationToken: cancellationToken);

        return results.Result.Select(r => new PatientRecordChunk(
             PatientId: Guid.Parse(r.Payload["patient_id"].StringValue),
             RecordType: r.Payload["record_type"].StringValue,
             Content: r.Payload["content"].StringValue,
             Metadata: r.Payload
                 .Where(p => p.Key != "patient_id" &&
                             p.Key != "record_type" &&
                             p.Key != "content")
                 .ToDictionary(
                     p => p.Key,
                     p => p.Value.StringValue)
        )).ToList();
    }

    public async Task<List<PatientRecordChunk>> SearchAsync(float[] queryVector, Guid patientId, int topK = 5, CancellationToken cancellationToken = default)
    {
        try
        {
            if (!await _qdrantClient.CollectionExistsAsync(CollectionName, cancellationToken))
            {
                _logger.LogWarning("Collection {Collection} does not exist — returning empty", CollectionName);
                return new List<PatientRecordChunk>();
            }

            // Filter by patient_id — only search this patient's records
            var filter = new Filter
            {
                Must =
                {
                    new Condition
                    {
                        Field  = new FieldCondition
                        {
                            Key="patient_id",
                            Match = new Match
                            {
                                Keyword = patientId.ToString()
                            }
                        }
                    }
                }
            };

            var results = await _qdrantClient.QueryAsync(CollectionName, queryVector, filter: filter, limit: (ulong)topK, cancellationToken: cancellationToken);

            var pId = results.Select(r => r.Payload["patient_id"].StringValue).FirstOrDefault();

            return results.Select(r => new PatientRecordChunk
            (
                PatientId: Guid.Parse(r.Payload["patient_id"].StringValue),
                RecordType: r.Payload["record_type"].StringValue,
                Content: r.Payload["content"].StringValue,
                Metadata: r.Payload
                        .Where(p => p.Key != "patient_id" &&
                                    p.Key != "record_type" &&
                                    p.Key != "content")
                        .ToDictionary(p => p.Key, p => p.Value.StringValue)

            )).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Search failed for patient {PatientId}", patientId);
            return new List<PatientRecordChunk>();
        }
    }

    public async Task UpsertAsync(PatientRecordChunk chunk, CancellationToken cancellationToken = default)
    {
        try
        {
            float[] vector = await _embeddingService.EmbedDocumentAsync(chunk.Content, cancellationToken);

            if (vector.Length == 0)
            {
                _logger.LogWarning("Embedding failed for {RecordType} — skipping upsert", chunk.RecordType);
            }

            // Step 2 — Build payload
            var payload = new Dictionary<string, Value>
            {
                ["patient_id"] = new Value { StringValue = chunk.PatientId.ToString().ToLower() },
                ["record_type"] = new Value { StringValue = chunk.RecordType },
                ["content"] = new Value { StringValue = chunk.Content }
            };

            foreach (var (key, value) in chunk.Metadata)
            {
                payload[key] = new Value { StringValue = value };
            }

            // Step 3 — Build point
            var point = new PointStruct
            {
                Id = new PointId { Uuid = Guid.NewGuid().ToString() },
                Vectors = new Vectors
                {
                    Vector = new Vector { Data = { vector } }
                },
                Payload = { payload }
            };

            // Step 4 — Upsert into Qdrant
            await _qdrantClient.UpsertAsync(CollectionName, new[] { point }, cancellationToken: cancellationToken);

            _logger.LogInformation("Upserted {RecordType} for patient {PatientId}", chunk.RecordType, chunk.PatientId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Upsert failed for patient {PatientId}", chunk.PatientId);
        }
    }

    private async Task UpsertWithVectorAsync(PatientRecordChunk chunk, float[] vector, CancellationToken cancellationToken = default)
    {
        try
        {
            var payload = new Dictionary<string, Value>
            {
                ["patient_id"] = new Value { StringValue = chunk.PatientId.ToString() },
                ["record_type"] = new Value { StringValue = chunk.RecordType },
                ["content"] = new Value { StringValue = chunk.Content }
            };

            // Add extra metadata
            foreach (var (key, value) in chunk.Metadata)
            {
                payload[key] = new Value { StringValue = value };
            }

            var point = new PointStruct
            {
                Id = new PointId { Uuid = Guid.NewGuid().ToString() },
                Vectors = new Vectors { Vector = new Vector { Data = { vector } } },
                Payload = { payload }
            };

            await _qdrantClient.UpsertAsync(CollectionName, new List<PointStruct> { point }, cancellationToken: cancellationToken);

            _logger.LogInformation("Upserted {RecordType} for patient {PatientId}", chunk.RecordType, chunk.PatientId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Upsert failed");
        }
    }
}
