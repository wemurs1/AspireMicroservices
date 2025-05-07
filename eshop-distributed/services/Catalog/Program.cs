var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.AddServiceDefaults();

builder.AddNpgsqlDbContext<ProductDbContext>(connectionName: "catalogDb");

builder.Services.AddScoped<ProductService>();

builder.Services.AddScoped<ProductAIService>();

builder.Services.AddMassTransitWithAssemblies(Assembly.GetExecutingAssembly());

// Register Ollama-based chat and embedding
builder.AddOllamaApiClient("ollama-llama3-2").AddChatClient();
builder.AddOllamaApiClient("ollama-all-minilm").AddEmbeddingGenerator();

// register an in-memory vector store
builder.Services.AddInMemoryVectorStoreRecordCollection<int, ProductVector>("products");

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapDefaultEndpoints();

app.UseMigration();

app.MapProductEndpoints();

app.UseHttpsRedirection();

app.Run();