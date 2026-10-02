# 🏥 MediSync — AI-Powered Healthcare Coordination Platform

> Solving medical context fragmentation — patients see multiple doctors
> but no single doctor sees the complete clinical picture.

## Problem
Patients visit multiple doctors across different hospitals — yet no
doctor ever sees the complete medical history. This causes duplicate
diagnostics, dangerous drug interactions, and delayed emergency care.

## Solution
MediSync is a patient-owned medical record coordination platform where:
- Patients grant consent to doctors to view their records
- Doctors see complete cross-provider clinical picture
- AI detects dangerous drug interactions in real time
- Patient can chat with their own health records using AI

## Architecture

```
MediSync.Web (ASP.NET Core MVC)
        ↓
MediSync.ApiGateway (Ocelot)
        ↓
┌─────────────────────────────────────────┐
│ Auth.API          → JWT, Roles          │
│ MedicalRecord.API → Patient Records     │
│ Prescription.API  → Drug Management     │
│ Notification.API  → Email Alerts        │
│ AI.API            → Groq + Qdrant + RAG │
└─────────────────────────────────────────┘
```

## AI Features
- **Drug Interaction Checker** — blocks dangerous prescriptions in real time
- **Patient Health Chat** — RAG-powered conversation with health records
- **Medical Record Summarizer** — clinical summary for new doctors
- **Prescription Info** — plain language drug explanations

## Tech Stack
| Layer | Technology |
|---|---|
| Runtime | .NET 10 |
| Architecture | Clean Architecture + DDD + CQRS |
| Auth | ASP.NET Identity + JWT + Cookie Auth |
| Database | SQL Server + Entity Framework Core |
| Gateway | Ocelot |
| AI Orchestration | Semantic Kernel |
| LLM | Groq (Llama / GPT-OSS) |
| Embeddings | Cohere embed-english-v3.0 |
| Vector Store | Qdrant Cloud |
| Email | MailKit / SMTP |
| Frontend | ASP.NET Core MVC |

## Patterns Implemented
- ✅ Clean Architecture
- ✅ Domain-Driven Design (Aggregates, Value Objects, Domain Events)
- ✅ CQRS with MediatR + Pipeline Behaviors
- ✅ Result Pattern for explicit error handling
- ✅ Repository Pattern
- ✅ Factory Method, Strategy, Decorator
- ✅ RAG (Retrieval Augmented Generation)
- ✅ Outbox Pattern (event publishing)

## How To Run

### Prerequisites
- .NET 10 SDK
- SQL Server
- Groq API key (free at console.groq.com)
- Cohere API key (free at dashboard.cohere.com)
- Qdrant Cloud account (free at cloud.qdrant.io)

### Setup
```bash
# 1. Clone
git clone https://github.com/amIsachin/MediSync.git

# 2. Configure each service
# Copy appsettings.json → appsettings.Development.json
# Fill in your API keys and connection strings

# 3. Run migrations
dotnet ef database update --project MediSync.Auth.Infrastrucure
    --startup-project MediSync.Auth.Presentation

dotnet ef database update
    --project MediSync.MedicalRecord.Infrastructure
    --startup-project MediSync.MedicalRecord.Presentation

dotnet ef database update
    --project MediSync.Prescription.Infrastructure
    --startup-project MediSync.Prescription.Presentation

# 4. Start all projects
# Run in order:
# 1. MediSync.Auth.Presentation        (port 7002)
# 2. MediSync.MedicalRecord.Presentation (port 7003)
# 3. MediSync.Prescription.Presentation  (port 7004)
# 4. MediSync.Notification.API           (port 7005)
# 5. MediSync.AI.Presentation            (port 7006)
# 6. MediSync.ApiGateway                 (port 7000)
# 7. MediSync.Web                        (port 7007)
```

## API Documentation
Each service exposes Swagger UI in Development mode:
- Auth API: `https://localhost:7002/swagger`
- MedicalRecord API: `https://localhost:7003/swagger`
- Prescription API: `https://localhost:7004/swagger`
- AI API: `https://localhost:7006/swagger`

## Status
- [x] Auth.API
- [x] MedicalRecord.API
- [x] Prescription.API
- [x] Notification.API
- [x] AI.API — Drug Interaction, RAG Chat, Summarizer
- [x] MVC Frontend — Patient + Doctor
- [ ] Azure Deployment (coming soon)
- [ ] xUnit Tests (coming soon)