using MediSync.AI.Presentation.Services;
using Microsoft.SemanticKernel;
using Qdrant.Client;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Semantic Kernel with Groq
builder.Services.AddSingleton(sp =>
{
    var kernelBuilder = Kernel.CreateBuilder();

    // Groq uses OpenAI-compatible API
    kernelBuilder.AddOpenAIChatCompletion(
        modelId: "openai/gpt-oss-20b",
        apiKey: builder.Configuration["AI:GroqApiKey"]!,
        endpoint: new Uri("https://api.groq.com/openai/v1")
    );

    return kernelBuilder.Build();
});

//Qdrant client
builder.Services.AddSingleton(sp =>
{
    var url = builder.Configuration["AI:QdrantUrl"]!;
    var apiKey = builder.Configuration["AI:QdrantApiKey"]!;

    return new QdrantClient(
        host: new Uri(url).Host,
        https: true,
        apiKey: apiKey
    );
});

// Cohere HttpClient
builder.Services.AddHttpClient<IEmbeddingService, EmbeddingService>(client =>
{
    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {builder.Configuration["AI:CohereApiKey"]}");
    client.Timeout = TimeSpan.FromMinutes(5);
});

builder.Services.AddScoped<IDrugInteractionService, DrugInteractionService>();
builder.Services.AddScoped<IVectorStoreService, VectorStoreService>();
builder.Services.AddScoped<IIndexService, IndexService>();
builder.Services.AddScoped<IChatService, ChatService>();
builder.Services.AddScoped<ISummaryService, SummaryService>();
builder.Services.AddScoped<IPrescriptionInfoService, PrescriptionInfoService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var vectorStore = scope.ServiceProvider
        .GetRequiredService<IVectorStoreService>();

    await vectorStore.EnsureCollectionExistsAsync();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();