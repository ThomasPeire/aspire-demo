using Microsoft.Extensions.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

var postgresPassword = builder.AddParameter("kafee-postgres-password", secret: true);  
  
var postgres = builder.AddPostgres("kafee-postgres")  
    .WithPgAdmin(c=>c.WithLifetime(ContainerLifetime.Persistent))  
    .WithPassword(postgresPassword)  
    .WithChildRelationship(postgresPassword)
    .WithLifetime(ContainerLifetime.Persistent);
  
var kafeedeebee = postgres.AddDatabase("kafee-database");

var migrationService = builder.AddProject<Projects.Kafee_MigrationService>("kafee-migrationservice")
    .WithReference(kafeedeebee)  
    .WaitFor(kafeedeebee);

var serviceBus = builder.Environment.IsDevelopment() 
    ? builder.AddAzureServiceBus("kafee-servicebus").RunAsEmulator(c => c.WithLifetime(ContainerLifetime.Persistent))
    : builder.AddAzureServiceBus("kafee-servicebus");

var queue = serviceBus.AddServiceBusQueue("mailbrewerqueue");

var api = builder.AddProject<Projects.Kafee_Api>("kafee-api")
    .WithReference(kafeedeebee)   
    .WaitFor(kafeedeebee)
    .WithReference(serviceBus)
    .WaitFor(serviceBus)
    .WaitForCompletion(migrationService)
    .WithChildRelationship(migrationService);

builder.AddProject<Projects.Kafee_Client>("kafee-client")
    .WithReference(api)
    .WaitFor(api);

builder.AddAzureFunctionsProject<Projects.Kafee_AzureFunctions>("functionapp")
    .WithReference(serviceBus)
    .WaitFor(serviceBus);

builder.Build().Run();
