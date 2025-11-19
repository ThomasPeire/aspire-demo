using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kafee.Api.Data;

internal class MenuItemConfiguration : IEntityTypeConfiguration<MenuItem>
{
    private static readonly Guid MenuItem1Guid = new("A89F6CD7-4693-457B-9009-02205DBBFE45");
    private static readonly Guid MenuItem2Guid = new("E4FA19BF-6981-4E50-A542-7C9B26E9EC31");
    private static readonly Guid MenuItem3Guid = new("17C61E41-3953-42CD-8F88-D3F698869B35");
    private static readonly Guid MenuItem4Guid = new("CA79E9B3-312C-43D4-A6F7-27AD7AC842E3");

    public void Configure(EntityTypeBuilder<MenuItem> builder)
    {
        builder.Property(p => p.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(p => p.AmountInStock)
            .HasDefaultValue(0m)
            .IsRequired();

        builder.HasData(GetSampleMenuItems());
    }

    private static IEnumerable<MenuItem> GetSampleMenuItems()
    {
        yield return MenuItem.Create(MenuItem1Guid, "Cola", 20, 2.80m);
        yield return MenuItem.Create(MenuItem2Guid, "Jupiler", 14, 3.00m);
        yield return MenuItem.Create(MenuItem3Guid, "Picon", 2, 5.50m);
        yield return MenuItem.Create(MenuItem4Guid, "Gin-Tonic", 0, 9.50m);
    }
}