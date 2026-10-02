using MediSync.Notification.Presentation.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Register services
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IPatientService, PatientService>();

builder.Services.AddHttpClient("MedicalRecordApi", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["MedicalRecordApi:BaseUrl"]! ?? "https://localhost:7004");
    client.Timeout = TimeSpan.FromSeconds(10);
});

var app = builder.Build(); 

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
