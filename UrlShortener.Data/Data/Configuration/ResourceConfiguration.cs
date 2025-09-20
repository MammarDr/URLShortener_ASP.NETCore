using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UrlShortener.Data.Entities.Permissions;

namespace UrlShortener.Data.DbContext.Configuration
{
    public class ResourceConfiguration : IEntityTypeConfiguration<Resource>
    {
        public void Configure(EntityTypeBuilder<Resource> builder)
        {
            builder.ToTable("RESOURCES").HasKey(r => r.ID);

            builder.Property(r => r.ID)
                .HasColumnName("id")
                .ValueGeneratedOnAdd();

            builder.Property(r => r.Name)
                .HasColumnName("name")
                .IsRequired();
        }
    }
}
