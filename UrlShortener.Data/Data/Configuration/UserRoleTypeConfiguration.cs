using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UrlShortener.Data.Entities.Users;

namespace UrlShortener.Data.DbContext.Configuration
{
    public class UserRoleTypeConfiguration : IEntityTypeConfiguration<UserRoleType>
    {
        public void Configure(EntityTypeBuilder<UserRoleType> builder)
        {
            builder.ToTable("ROLES")
                .HasKey(r => r.ID);

            builder.Property(r => r.ID)
                .HasColumnName("id")
                .ValueGeneratedOnAdd();

            builder.Property(r => r.Name)
                .HasColumnName("name")
                .IsRequired();

            builder.HasMany(r => r.Permissions)
                .WithOne(p => p.RoleType)
                .HasForeignKey(p => p.RoleId);
        }
    }
}
