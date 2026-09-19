namespace MediSync.AI.Presentation.Services;

public interface IVectorStoreService
{
    // Store a chunk into Qdrant
    Task UpsertAsync(PatientRecordChunk chunk, CancellationToken cancellationToken = default);

    // Search for relevant chunks by patient
    Task<List<PatientRecordChunk>> SearchAsync(float[] queryVector, Guid patientId, int topK = 5, CancellationToken cancellationToken = default);

    // Delete all chunks for a patient (when profile updated)
    Task DeletePatientRecordsAsync(Guid patientId, CancellationToken cancellationToken = default);

    Task EnsureCollectionExistsAsync(CancellationToken cancellationToken = default);
}

public record PatientRecordChunk(
    Guid PatientId,
    string RecordType,    // "allergy", "diagnosis", "encounter", "prescription"
    string Content,       // original text
    Dictionary<string, string> Metadata  // extra info
);