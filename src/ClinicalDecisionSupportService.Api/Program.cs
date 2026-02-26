using ClinicalDecisionSupportService.Api.Endpoints.NewsScore;
using ClinicalDecisionSupportService.Api.Extensions;
using ClinicalDecisionSupportService.Api.Middleware;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAppConfiguration(builder.Configuration);

var app = builder.Build();

app.UseApiMiddleware();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.MapNewsScoreEndpoints();

app.Run();

public partial class Program { }
