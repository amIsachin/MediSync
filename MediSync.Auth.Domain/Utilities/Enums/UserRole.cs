namespace MediSync.Auth.Domain.Utilities.Enums;

public enum UserRole
{
    // Start from 1 — never 0
    // If you use 0, then default(UserRole) == Patient
    // which means an uninitialized variable accidentally becomes a Patient
    // Starting from 1 means uninitialized = 0 = nothing = bug is visible

    Patient = 1,  // can view own records, grant consent
    Doctor = 2,  // can view consented patient records, write prescriptions
    Admin = 3,  // manages users, views audit logs
    LabTechnician = 4,  // processes lab orders, enters test results
    Pharmacist = 5,  // views prescriptions, marks as dispensed
    Receptionist = 6,  // books appointments, manages schedules
    SuperAdmin = 7   // full system access — only for Anthropic/your team
}
