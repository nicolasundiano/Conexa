using Conexa.Api;
using Conexa.Application;
using Conexa.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddApplication()
    .AddInfrastructure(builder.Configuration)
    .AddPresentation();

var app = builder.Build();

await app.InitializeDatabaseAsync();

app.ConfigureRequestPipeline();

await app.RunAsync();

public partial class Program;
