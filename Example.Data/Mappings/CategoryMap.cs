using Example.Domain.Entitys;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Example.Data.Mappings;

public class CategoryMap : BaseEntityTypeConfiguration<Category>
{
    public override void Configure(EntityTypeBuilder<Category> builder)
    {
        // Properties
        builder.Property(x => x.Name)
               .IsRequired()
               .HasMaxLength(80);

        builder.Property(x => x.ImageUrl)
               .IsRequired()
               .HasMaxLength(300);
        
        builder.Navigation("Products");
        builder.ToTable("Category");

        base.Configure(builder);
    }
}
