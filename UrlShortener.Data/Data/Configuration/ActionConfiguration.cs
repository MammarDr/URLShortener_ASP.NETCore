using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UrlShortener.Data.Entities.Permissions;

namespace UrlShortener.Data.DbContext.Configuration
{
    public class ActionConfiguration : IEntityTypeConfiguration<UserAction>
    {
        public void Configure(EntityTypeBuilder<UserAction> builder)
        {
            builder.ToTable("ACTIONS").HasKey(a => a.ID);

            builder.Property(a => a.ID)
                .HasColumnName("id")
                .ValueGeneratedOnAdd();
        }
    }
}
