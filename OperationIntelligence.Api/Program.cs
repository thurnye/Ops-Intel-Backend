using OperationIntelligence.Api;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAppControllers();
builder.Services.AddAppSwagger();
builder.Services.AddAppAuthentication(builder.Configuration);
builder.Services.AddAppAuthorization();
builder.Services.AddAppValidation();
builder.Services.AddAppRepositories();
builder.Services.AddAppServices();
builder.Services.AddAppDatabase(builder.Configuration);
builder.Services.AddAppCors();
builder.Services.AddAppRedis(builder.Configuration);

var app = builder.Build();

app.UseAppPipeline();

app.Run();

public partial class Program
{
}