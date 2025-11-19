using Azure.Identity;
using Kafee.Api.Data;
using Kafee.Api.Endpoints;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddProblemDetails();

builder.Services.AddOpenApi();

builder.AddNpgsqlDbContext<KafeeDbContext>(connectionName: "kafee-database");

builder.AddAzureServiceBusClient(connectionName: "kafee-servicebus");

var app = builder.Build();

app.MapDefaultEndpoints();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapOrderEndpoints();
app.MapMenuEndpoints();

app.Run();