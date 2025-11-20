using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kafee.Api.Data;

internal class MenuItemConfiguration : IEntityTypeConfiguration<MenuItem>
{ public void Configure(EntityTypeBuilder<MenuItem> builder)
    {
        builder.Property(p => p.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(p => p.AmountInStock)
            .HasDefaultValue(0m)
            .IsRequired();
    }
}