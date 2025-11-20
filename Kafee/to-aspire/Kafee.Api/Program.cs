using Azure.Identity;
using Kafee.Api.Data;
using Kafee.Api.Endpoints;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddProblemDetails();

builder.Services.AddOpenApi();

builder.Services.AddDbContext<KafeeDbContext>((_, options) =>
{
    options.UseNpgsql("Host=localhost;Port=5432;Username=postgres;Password=postgres;Database=pre-aspire");
});

builder.Services.AddAzureClients(clientBuilder =>
{
    clientBuilder.AddServiceBusClient("Endpoint=sb://localhost;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=SAS_KEY_VALUE;UseDevelopmentEmulator=true;");
});


var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapOrderEndpoints();
app.MapMenuEndpoints();

app.Run();