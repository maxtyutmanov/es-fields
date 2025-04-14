using Nest;
using WorkflowApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure Elasticsearch
var settings = new ConnectionSettings(new Uri("http://localhost:9200"))
    .DefaultIndex("workflows");

var client = new ElasticClient(settings);
builder.Services.AddSingleton<IElasticClient>(client);

// Register services
builder.Services.AddScoped<IWorkflowService, ElasticsearchWorkflowService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
