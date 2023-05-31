using Example.Domain.Entitys;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Example.Data.Mappings
{
    public abstract class BaseEntityTypeConfiguration<TModel> : IEntityTypeConfiguration<TModel> where TModel : EntityBase
    {
        public virtual void Configure(EntityTypeBuilder<TModel> builder)
        {
            // Primary Key
            builder.HasKey(x => x.Id);

            // Properties
            builder.Property(x => x.Id)
             .IsRequired()
             .ValueGeneratedOnAdd();

            builder.Property(x => x.CreatedAt)
                   .HasColumnType("datetime(6)");

            builder.Property(x => x.UpdatedAt)
                   .HasColumnType("datetime(6)");
        }
    }
}