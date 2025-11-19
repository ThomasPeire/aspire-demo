using System.Diagnostics;
using Kafee.Api;
using Kafee.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace Kafee.MigrationService;

public class Worker(
    IServiceProvider serviceProvider,
    IHostApplicationLifetime hostApplicationLifetime) : BackgroundService
{
    public const string ActivitySourceName = "Migrations";
    private static readonly ActivitySource s_activitySource = new(ActivitySourceName);

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        using var activity = s_activitySource.StartActivity("Migrating database", ActivityKind.Client);

        try
        {
            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<KafeeDbContext>();

            await RunMigrationAsync(dbContext, cancellationToken);
            await SeedDataAsync(dbContext, cancellationToken);
        }
        catch (Exception ex)
        {
            activity?.AddException(ex);
            throw;
        }

        hostApplicationLifetime.StopApplication();
    }

    private static async Task RunMigrationAsync(KafeeDbContext dbContext, CancellationToken cancellationToken)
    {
        var strategy = dbContext.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            // Run migration in a transaction to avoid partial migration if it fails.
            await dbContext.Database.MigrateAsync(cancellationToken);
        });
    }

    private static async Task SeedDataAsync(KafeeDbContext dbContext, CancellationToken cancellationToken)
    {
        var strategy = dbContext.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
            foreach (var sampleMenuItem in GetSampleMenuItems())
            {
                if (dbContext.MenuItems.Any(x => x.Id == sampleMenuItem.Id))
                {
                    continue;
                }
                await dbContext.MenuItems.AddAsync(sampleMenuItem, cancellationToken);
                await dbContext.SaveChangesAsync(cancellationToken);
                
            }
            await transaction.CommitAsync(cancellationToken);
        });
    }
    
    private static readonly Guid MenuItem1Guid = new("A89F6CD7-4693-457B-9009-02205DBBFE45");
    private static readonly Guid MenuItem2Guid = new("E4FA19BF-6981-4E50-A542-7C9B26E9EC31");
    private static readonly Guid MenuItem3Guid = new("17C61E41-3953-42CD-8F88-D3F698869B35");
    private static readonly Guid MenuItem4Guid = new("CA79E9B3-312C-43D4-A6F7-27AD7AC842E3");

    private static IEnumerable<MenuItem> GetSampleMenuItems()
    {
        yield return MenuItem.Create(MenuItem1Guid, "Cola", 20, 2.80m);
        yield return MenuItem.Create(MenuItem2Guid, "Jupiler", 14, 3.00m);
        yield return MenuItem.Create(MenuItem3Guid, "Picon", 2, 5.50m);
        yield return MenuItem.Create(MenuItem4Guid, "Gin-Tonic", 0, 9.50m);
    }
}
