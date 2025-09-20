using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UrlShortener.Data.Entities.Permissions;

namespace UrlShortener.Data.DbContext.Configuration
{
    public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            builder.ToTable("REFRESH_TOKEN").HasKey(x => x.Id);
            builder.Property(r => r.Token).HasColumnName("token").IsRequired();
            builder.Property(r => r.UserId).HasColumnName("user_id").IsRequired();
            builder.Property(r => r.ExpiresOnUtc).HasColumnName("expire_at").IsRequired();
            builder.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId);

        }
    }
}
