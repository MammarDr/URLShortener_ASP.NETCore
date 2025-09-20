using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UrlShortener.Data.Entities.Urls;

namespace UrlShortener.Data.DbContext.Configuration
{
    public class URLConfiguration : IEntityTypeConfiguration<URL>
    {
        public void Configure(EntityTypeBuilder<URL> builder)
        {
            builder.ToTable("URLS").HasKey(u => u.ID);

            builder.Property(u => u.ShortCode)
                .HasColumnName("short_code")
                .IsRequired();

            builder.Property(u => u.Source)
                .HasColumnName("source")
                .IsRequired();

            builder.Property(u => u.Title)
                .HasColumnName("title")
                .IsRequired(false);

            builder.Property(u => u.VisitCount)
                .HasColumnName("visit_count")
                .HasDefaultValue(0)
                .ValueGeneratedOnAdd(); 

            builder.Property(u => u.CreatedAt)
                .HasColumnName("created_at")
                .ValueGeneratedOnAdd();

            builder.Property(u => u.CreatedBy)
                .HasColumnName("created_by")
                .IsRequired();

            builder.Property(u => u.LastModified)
                .HasColumnName("last_modified")
                .ValueGeneratedOnAdd();

            builder.Property(u => u.ExpiresAt)
                .HasColumnName("expires_at")
                .IsRequired();

            builder.Property(u => u.isActive)
                .HasColumnName("is_active")
                .ValueGeneratedOnAdd();


            builder.HasMany(u => u.Visits)
                .WithOne(v => v.Url)
                .HasForeignKey(v => v.UrlId);
        }
    }
}
