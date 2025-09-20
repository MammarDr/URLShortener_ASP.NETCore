using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UrlShortener.Data.Entities.Urls;

namespace UrlShortener.Data.DbContext.Configuration
{
    public class URLVisitConfiguration : IEntityTypeConfiguration<URLVisit>
    {
        public void Configure(EntityTypeBuilder<URLVisit> builder)
        {
            builder.ToTable("URL_VISIT")
                .HasKey(u => new { u.UrlId, u.VisitorIP, u.VisitedAt });

            builder.Property(u => u.UrlId)
                .HasColumnName("url_id")
                .IsRequired();

            builder.Property(u => u.VisitorIP)
                .HasColumnName("visitor_ip")
                .IsRequired();

            builder.Property(u => u.UserAgent)
                .HasColumnName("user_agent")
                .IsRequired(false);

            builder.Property(u => u.VisitedAt)
                .HasColumnName("visited_at")
                .ValueGeneratedOnAdd();
        }
    }
}
