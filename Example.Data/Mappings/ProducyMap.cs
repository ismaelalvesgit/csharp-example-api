using Example.Domain.Entitys;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Example.Data.Mappings;

public class ProductMap : BaseEntityTypeConfiguration<Product>
{
    public override void Configure(EntityTypeBuilder<Product> builder)
    {
        // Properties
        builder.Property(x => x.CategoryId)
                .IsRequired();

        builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(80);

        builder.Property(x => x.Description)
                .HasColumnType("longtext");   

        builder.Property(x => x.ImageUrl)
                .HasColumnType("longtext");

        builder.Property(x => x.Quantity)
                .IsRequired()
                .HasDefaultValue(0)
                .HasColumnType("float");

        builder.Property(x => x.Price)
                .IsRequired()
                .HasDefaultValue(0)
                .HasColumnType("decimal(10,2)");

        builder.HasIndex("CategoryId");

        // Relationships
        builder.HasOne(x => x.Category)
                .WithMany(x => x.Products)
                .HasForeignKey(x => x.CategoryId)
                .HasPrincipalKey(x => x.Id)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();
        builder.Navigation("Category");

        builder.ToTable("Product");

        base.Configure(builder);
    }
}
