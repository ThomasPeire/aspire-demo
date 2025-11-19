using Kafee.Api.Data;
using Kafee.MigrationService;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddHostedService<Worker>();

builder.AddNpgsqlDbContext<KafeeDbContext>(connectionName: "kafee-database");

var host = builder.Build();
host.Run();