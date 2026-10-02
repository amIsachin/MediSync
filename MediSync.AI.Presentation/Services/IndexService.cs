using MediSync.AI.Presentation.Models;
using Qdrant.Client;

namespace MediSync.AI.Presentation.Services;

public class IndexService : IIndexService
{
    private readonly IVectorStoreService _vectorStoreService;
    private readonly ILogger<IndexService> _logger;
    private readonly QdrantClient _qdrantClient;
    private readonly IConfiguration _configuration;
    public IndexService(IVectorStoreService vectorStoreService, IEmbeddingService embeddingService, ILogger<IndexService> logger, QdrantClient qdrantClient, IConfiguration configuration)
    {
        _vectorStoreService = vectorStoreService;
        _logger = logger;
        _qdrantClient = qdrantClient;
        _configuration = configuration;
    }

    private string CollectionName
        => _configuration["AI:QdrantCollection"]
            ?? "medisync_patient_records";

    public async Task IndexPatientAsync(IndexPatientRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Indexing patient {PatientId} into Qdrant", request.PatientId);

        // Step 1 — Delete existing records (re-index fresh)
        await _vectorStoreService.DeletePatientRecordsAsync(request.PatientId, cancellationToken);

        if (!await _qdrantClient.CollectionExistsAsync(CollectionName, cancellationToken))
        {
            await _vectorStoreService.EnsureCollectionExistsAsync(cancellationToken);
        }

        // Step 2 — Index all record types in parallel
        var tasks = new List<Task>();

        tasks.AddRange(request.Allergies.Select(a => IndexAllergyAsync(request.PatientId, a, cancellationToken)));

        tasks.AddRange(request.Diagnoses.Select(d => IndexDiagnosisAsync(request.PatientId, d, cancellationToken)));

        tasks.AddRange(request.Encounters.Select(e => IndexEncounterAsync(request.PatientId, e, cancellationToken)));

        tasks.AddRange(request.Prescriptions.Select(p => IndexPrescriptionAsync(request.PatientId, p, cancellationToken)));

        await Task.WhenAll(tasks);

        _logger.LogInformation(
            "Indexed {Allergies} allergies, {Diagnoses} diagnoses, " +
            "{Encounters} encounters, {Prescriptions} prescriptions for patient {PatientId}",
            request.Allergies.Count,
            request.Diagnoses.Count,
            request.Encounters.Count,
            request.Prescriptions.Count,
            request.PatientId);
    }

    private async Task IndexAllergyAsync(Guid patientId, AllergyRecord allergy, CancellationToken cancellationToken)
    {
        var content = $"Patient {patientId} has a {allergy.Severity} allergy to {allergy.Substance}. {(string.IsNullOrEmpty(allergy.Notes) ? string.Empty : allergy.Notes)} Recorded on {allergy.RecordedAt: dd MMM yyyy}";

        var chunk = new PatientRecordChunk(
            PatientId: patientId,
            RecordType: "allergy",
            Content: content,
            Metadata: new Dictionary<string, string>
            {
                { "substance", allergy.Substance },
                { "severity", allergy.Severity },
                { "recorded_at", allergy.RecordedAt.ToString("yyyy-MM-dd") }
            }
        );

        await _vectorStoreService.UpsertAsync(chunk, cancellationToken);

    }

    private async Task IndexDiagnosisAsync(Guid patientId, DiagnosisRecord diagnosis, CancellationToken cancellationToken)
    {
        var status = diagnosis.IsActive ? "Active" : "Resolved";

        var content = $"Patient {patientId} has been diagnosed with {diagnosis.Description} (ICD code: {diagnosis.IcdCode}) on {diagnosis.DiagnosedAt:dd MMM yyyy}. This condition is currently {status}. Diagnosed by doctor {diagnosis.DoctorId}";

        var chunk = new PatientRecordChunk(
            PatientId: patientId,
            RecordType: "diagnosis",
            Content: content,
            Metadata: new Dictionary<string, string>
            {
                ["icd_code"] = diagnosis.IcdCode,
                ["description"] = diagnosis.Description,
                ["is_active"] = diagnosis.IsActive.ToString(),
                ["diagnosed_at"] = diagnosis.DiagnosedAt.ToString("yyyy-MM-dd")
            }
        );

        await _vectorStoreService.UpsertAsync(chunk, cancellationToken);
    }

    private async Task IndexEncounterAsync(Guid patientId, EncounterRecord encounter, CancellationToken cancellationToken)
    {
        var content = $"Patient {patientId} had a {encounter.EncounterType} encounter with doctor {encounter.DoctorId} {(string.IsNullOrEmpty(encounter.Facility) ? "" : $"at {encounter.Facility} ")} on {encounter.VisitDate:dd MMM yyyy}. Chief complaint: {encounter.ChiefComplaint}. {(string.IsNullOrEmpty(encounter.Notes) ? "" : $"Notes: {encounter.Notes}.")}";

        var chunk = new PatientRecordChunk(
            PatientId: patientId,
            RecordType: "encounter",
            Content: content,
            Metadata: new Dictionary<string, string>
            {
                ["encounter_type"] = encounter.EncounterType,
                ["visit_date"] = encounter.VisitDate.ToString("yyyy-MM-dd"),
                ["chief_complaint"] = encounter.ChiefComplaint,
                ["facility"] = encounter.Facility ?? ""
            });

        await _vectorStoreService.UpsertAsync(chunk, cancellationToken);
    }

    private async Task IndexPrescriptionAsync(Guid patientId, PrescriptionRecord prescription, CancellationToken cancellationToken)
    {
        var content = $"Patient {patientId} is prescribed {prescription.DrugName} {prescription.Dosage} taken {prescription.Route} {prescription.Frequency} for {prescription.DurationDays} days. Status: {prescription.Status}. Expires on {prescription.ExpiresAt:dd MMM yyyy}. {(string.IsNullOrEmpty(prescription.Notes) ? "" : $"Notes: {prescription.Notes}.")}";

        var chunk = new PatientRecordChunk(
            PatientId: patientId,
            RecordType: "prescription",
            Content: content,
            Metadata: new Dictionary<string, string>
            {
                ["drug_name"] = prescription.DrugName,
                ["dosage"] = prescription.Dosage,
                ["status"] = prescription.Status,
                ["expires_at"] = prescription.ExpiresAt.ToString("yyyy-MM-dd")
            });

        await _vectorStoreService.UpsertAsync(chunk, cancellationToken);
    }
}