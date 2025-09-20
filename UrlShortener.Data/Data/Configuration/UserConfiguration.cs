using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UrlShortener.Data.Entities.Users;

namespace UrlShortener.Data.DbContext.Configuration
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("USERS").HasKey(u => u.ID);

            builder.Property(u => u.Email)
                .HasColumnName("email")
                .IsRequired();

            builder.Property(u => u.Password)
                .HasColumnName("password")
                .IsRequired();

            builder.Property(u => u.PlanId)
                .HasColumnName("plan_id")
                .IsRequired();

            builder.Property(u => u.PlanExpiresAt)
                .HasColumnName("plan_expires_at")
                .IsRequired(false);

            builder.Property(u => u.CreatedAt)
                .HasColumnName("created_at")
                .ValueGeneratedOnAdd()
                .IsRequired(false);

            builder.HasOne(u => u.Plan)
                 .WithMany(p => p.Users)
                 .HasForeignKey(u => u.PlanId);


            // Principal : User
            // Dependent : URls
            // If user got deleted all his urls will be gone
            builder.HasMany(u => u.URLs)
                .WithOne(ur => ur.CreatedByUser)
                .HasForeignKey(u => u.CreatedBy)
                .OnDelete(DeleteBehavior.Cascade); // DeleteBehavior.SetNull
        }
    }
}
