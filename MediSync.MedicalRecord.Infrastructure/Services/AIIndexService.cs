using MediSync.MedicalRecord.Application.Abstraction;
using MediSync.MedicalRecord.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;

namespace MediSync.MedicalRecord.Infrastructure.Services;

internal class AIIndexService : IAIIndexService
{
    private readonly IPatientRepository _patientRepository;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<AIIndexService> _logger;

    public AIIndexService(IPatientRepository patientRepository, IHttpClientFactory httpClientFactory, ILogger<AIIndexService> logger)
    {
        _patientRepository = patientRepository;
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task IndexPatientAsync(Guid patientId, CancellationToken cancellationToken = default)
    {
        try
        {
            var patient = await _patientRepository.GetByIdAsync(patientId, cancellationToken);

            if (patient is null)
            {
                _logger.LogWarning("Patient {PatientId} not found — skipping AI index", patientId);
                return;
            }

            var payload = new
            {
                patientId = patient.Id,
                allergies = patient.Allergies.Select(a => new
                {
                    substance = a.Substance,
                    severity = a.Severity.ToString(),
                    notes = a.Notes,
                    recordedAt = a.RecordedAt
                }),
                diagnoses = patient.Diagnoses.Select(d => new
                {
                    icdCode = d.IcdCode.Value,
                    description = d.Description,
                    doctorId = d.DoctorId,
                    diagnosedAt = d.DiagnosedAt,
                    isActive = d.IsActive
                }),
                encounters = patient.Encounters.Select(e => new
                {
                    doctorId = e.DoctorId,
                    visitDate = e.VisitDate,
                    encounterType = e.Type.ToString(),
                    chiefComplaint = e.ChiefComplaint,
                    notes = e.Notes,
                    facility = e.Facility
                }),
                prescriptions = new List<object>()
            };

            var client = _httpClientFactory.CreateClient("AIApi");
            var response = await client.PostAsJsonAsync("/api/ai/index", payload, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Patient {PatientId} indexed successfully", patientId);
            }
            else
            {
                _logger.LogWarning("AI index failed for patient {PatientId} — Status: {Status}", patientId, response.StatusCode);
            }
        }
        catch (Exception ex)
        {
            // Never let indexing failure break main flow
            _logger.LogError(ex, "AI index failed for patient {PatientId}", patientId);
        }
    }
}
