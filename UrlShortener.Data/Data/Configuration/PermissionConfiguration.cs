using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UrlShortener.Data.Entities.Permissions;

namespace UrlShortener.Data.DbContext.Configuration
{
    public class PermissionConfiguration : IEntityTypeConfiguration<Permission>
    {
        public void Configure(EntityTypeBuilder<Permission> builder)
        {
            builder.ToTable("PERMISSIONS")
                .HasKey(p => new {p.RoleId, p.ActionId, p.ResourceId});

            builder.Property(r => r.RoleId)
                .HasColumnName("role_id");

            builder.Property(r => r.ActionId)
                .HasColumnName("action_id");

            builder.Property(r => r.ResourceId)
                .HasColumnName("resource_id");

            builder.HasOne(p => p.RoleType)
                .WithMany(r => r.Permissions) // Keep navigation for Role
                .HasForeignKey(p => p.RoleId);

            builder.HasOne(p => p.Action)
                   .WithMany() // no collection on Action, not asking "what permissions have this action"
                   .HasForeignKey(p => p.ActionId);

            builder.HasOne(p => p.Resource)
                   .WithMany() // no collection on Resource
                   .HasForeignKey(p => p.ResourceId);
        }
    }
}
