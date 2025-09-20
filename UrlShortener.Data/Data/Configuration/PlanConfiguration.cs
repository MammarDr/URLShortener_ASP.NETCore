using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UrlShortener.Data.Entities.Plans;
using UrlShortener.Models.Enums;

namespace UrlShortener.Data.DbContext.Configuration
{
    public class PlanConfiguration : IEntityTypeConfiguration<Plan>
    {
        public void Configure(EntityTypeBuilder<Plan> builder)
        {
            builder.ToTable("PLANS").HasKey(p => p.ID);

            builder.Property(p => p.Name)
                .HasColumnName("name")
                .IsRequired();

            builder.Property(p => p.Price)
                .HasColumnName("price")
                .IsRequired();

            builder.Property(p => p.MaxDailyURL)
                .HasColumnName("max_daily_url")
                .IsRequired();

            builder.Property(p => p.UrlExpiresAfter)
                .HasColumnName("url_expires_after")
                .IsRequired();

            builder.Property(p => p.HasCustomSlugs)
                .HasColumnName("has_custom_slugs")
                .IsRequired();

            builder.Property(p => p.SupportLevel)
                .HasColumnName("support_level")
                .HasConversion(
                    p => p.ToString(),
                    p => (enSupportLevel)Enum.Parse(typeof(enSupportLevel), p)
                )
                .IsRequired();
        }
    }
}
