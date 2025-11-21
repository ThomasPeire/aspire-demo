# Migrating a .NET Application to .NET Aspire: Live Demo Guide

## Part 1: Initial Setup with Docker (Pre-Aspire)

### Understanding the Starting Point

Before migrating to Aspire, the application uses Docker to run PostgreSQL locally.

**Build the PostgreSQL Docker image:**

```pwsh
docker build -f postgres.dockerfile -t pre-aspire .
```

**Run the PostgreSQL container:**

```pwsh
docker run --name pre-aspire-container -p 5432:5432 pre-aspire
```

**Create and apply Entity Framework migrations:**

```pwsh
dotnet ef migrations add init --project .\Kafee.Api
```

```pwsh
dotnet ef database update --project .\Kafee.Api
```

## Part 2: Adding .NET Aspire Orchestration

### Step 1: Add Aspire to the Solution

1. Right-click on the solution in Solution Explorer
2. Select **Add > .NET Aspire Orchestration**
3. Review the changes added to your solution (optionally use Git compare to see what was generated)

This creates an AppHost project that will orchestrate all services and resources.

### Step 2: Start and Explore the Dashboard

1. Run the application from the AppHost project
2. Explore the Aspire Dashboard features:
   - Console output from all services
   - Structured logs with filtering
   - Distributed traces
   - Metrics and performance data

## Part 3: Integrating Additional Projects

### Step 3: Configure Launch Settings

Prevent automatic browser launch by modifying launch settings to avoid interrupting the demo flow.

### Step 4: Clean Up Pre-Aspire Docker Resources

Remove the manual Docker containers since Aspire will manage these resources:

```pwsh
docker stop pre-aspire-container
docker rm pre-aspire-container
```

## Part 4: Establishing Service References

### Step 5: Remove Hardcoded Frontend-Backend Connection

Replace hardcoded URLs and ports with Aspire service references in the AppHost `Program.cs`:

```csharp
var migrationService = builder.AddProject<Projects.Kafee_MigrationService>("kafee-migrationservice");

var api = builder.AddProject<Projects.Kafee_Api>("kafee-api")
    .WaitForCompletion(migrationService)
    .WithChildRelationship(migrationService);

builder.AddProject<Projects.Kafee_Client>("kafee-client")
    .WithReference(api)
    .WaitFor(api);
```

**What this accomplishes:**

- Establishes service discovery between frontend and backend
- Ensures migration service completes before API starts
- Creates proper startup dependencies

Show dashboard and service discovery resolving

## Part 5: Migrating PostgreSQL to Aspire

### Step 6: Add PostgreSQL Hosting Package

Add the following package to the AppHost project:

```
Aspire.Hosting.PostgreSQL
```

### Step 7: Configure PostgreSQL in AppHost

Add PostgreSQL configuration to the AppHost `Program.cs`:

```csharp
var postgresPassword = builder.AddParameter("kafee-postgres-password", secret: true);

var postgres = builder.AddPostgres("kafee-postgres")
    .WithPgAdmin()
    .WithPassword(postgresPassword)
    .WithChildRelationship(postgresPassword);

var kafeedeebee = postgres.AddDatabase("kafee-database");

var migrationService = builder.AddProject<Projects.Kafee_MigrationService>("kafee-migrationservice")
    .WithReference(kafeedeebee)
    .WaitFor(kafeedeebee);

var api = builder.AddProject<Projects.Kafee_Api>("kafee-api")
    .WithReference(kafeedeebee)
    .WaitFor(kafeedeebee)
    .WaitForCompletion(migrationService)
    .WithChildRelationship(migrationService);
```

**What this accomplishes:**

- Manages PostgreSQL as an Aspire resource
- Includes PgAdmin for database management
- Securely handles database password as a parameter
- Ensures database is ready before dependent services start

### Step 8: Update API and Migration Service for Aspire PostgreSQL

**Remove hardcoded connection strings** from `appsettings.json` in both Kafee.Api and Kafee.MigrationService projects.

**In Kafee.Api project:**

1. Remove the `Npgsql.EntityFrameworkCore.PostgreSQL` package
2. Add the `Aspire.Npgsql.EntityFrameworkCore.PostgreSQL` package
3. Replace the existing DbContext registration in `Program.cs` with:

```csharp
builder.AddNpgsqlDbContext<KafeeDbContext>(connectionName: "kafee-database");
```

**Repeat for Kafee.MigrationService project.**

### Step 9: Run and Configure Database Password

1. Start the application
2. You will see an unresolved parameter prompt for `kafee-postgres-password`
3. Enter a secure password
4. Observe the application running with Aspire-managed PostgreSQL

### Step 10: Make PostgreSQL Container Persistent

Stop the application and observe that containers are destroyed by default. To persist the database between runs, add the following to the PostgreSQL configuration:

```csharp
var postgres = builder.AddPostgres("kafee-postgres")
    .WithPgAdmin()
    .WithPassword(postgresPassword)
    .WithLifetime(ContainerLifetime.Persistent)
    .WithChildRelationship(postgresPassword);
```

## Part 6: Integrating Azure Service Bus

### Step 11: Add Service Bus Hosting Packages

Add the following packages to the AppHost project:

```
Aspire.Hosting.Azure.ServiceBus
Aspire.Hosting.Azure.Functions (PRERELEASE)
```

### Step 12: Configure Azure Service Bus in AppHost

Add Service Bus configuration with emulator support for local development:

```csharp
var serviceBus = builder.AddAzureServiceBus("kafee-servicebus")
    .RunAsEmulator(c => c.WithLifetime(ContainerLifetime.Persistent));

var queue = serviceBus.AddServiceBusQueue("mailbrewerqueue");
```

**Add Azure Functions project reference:**

apphost project needs reference to functionapp project

```csharp
var functions = builder.AddAzureFunctionsProject<Projects.Kafee_AzureFunctions>("function-app")
    .WithReference(serviceBus)
    .WaitFor(serviceBus);
```

**Update API service to reference Service Bus:**

```csharp
var api = builder.AddProject<Projects.Kafee_Api>("kafee-api")
    .WithReference(kafeedeebee)
    .WithReference(serviceBus)
    .WaitFor(kafeedeebee)
    .WaitFor(serviceBus)
    .WaitForCompletion(migrationService)
    .WithChildRelationship(migrationService);
```

### Step 13: Update Azure Functions Project

Add Aspire service defaults to the Kafee.AzureFunctions `Program.cs`:

```csharp
builder.AddServiceDefaults();
```

**Update the Service Bus trigger** in your function to use the connection name:

```csharp
[ServiceBusTrigger("mailbrewerqueue", Connection = "kafee-servicebus")]
```

### Step 14: Update API Project for Service Bus

Add the following package to the Kafee.Api project:

```
Aspire.Azure.Messaging.ServiceBus
```

```

builder.AddAzureServiceBusClient(connectionName: "kafee-servicebus");
```

This enables the API to use Aspire's Service Bus client configuration automatically.

## Conclusion

The application is now fully migrated to .NET Aspire with:

- Centralized orchestration through the AppHost
- Managed PostgreSQL with persistence
- Azure Service Bus emulator for local development
- Azure Functions integration
- Service discovery and dependency management
- Comprehensive observability through the Aspire Dashboard

# Deploying to Azure

open terminal

```powershell

pwsh

gco -f
git clean -fd

rm .azure, rm nextsteps, rm azure.yaml

azd init


azd pipeline config

=> cancel

azd up

uitleg, cancelen en reeds deployede versie tonen


```

1. Toon service discovery
