using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UrlShortener.Data.Entities.Users;

namespace UrlShortener.Data.DbContext.Configuration
{
    public class UserRoleMappingConfiguration : IEntityTypeConfiguration<UserRoleMapping>
    {
        public void Configure(EntityTypeBuilder<UserRoleMapping> builder)
        {
            builder.ToTable("USER_ROLES")
                .HasKey(rm => new { rm.UserId, rm.RoleId });

            builder.Property(rm => rm.UserId)
                .HasColumnName("user_id");

            builder.Property(rm => rm.RoleId)
                .HasColumnName("role_id");

            builder.HasOne(rm => rm.User)
                .WithMany(u => u.RoleMappings)
                .HasForeignKey(rm => rm.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(rm => rm.RoleType)
                .WithMany(r => r.UserMappings)
                .HasForeignKey(rm => rm.RoleId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
