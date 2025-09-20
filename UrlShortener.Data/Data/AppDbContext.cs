using Microsoft.EntityFrameworkCore;
using UrlShortener.Data.DbContext.Configuration;
using UrlShortener.Data.Entities.Permissions;
using UrlShortener.Data.Entities.Plans;
using UrlShortener.Data.Entities.Urls;
using UrlShortener.Data.Entities.Users;

namespace UrlShortener.Data
{
    public class AppDbContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public DbSet<User>              Users           { get; set; }
        public DbSet<UserRoleType>      RolesType       { get; set; }
        public DbSet<UserRoleMapping>   RoleMappings    { get; set; }
        public DbSet<RefreshToken>      RefreshTokens   { get; set; }

        public DbSet<Plan>      Plans   { get; set; }

        public DbSet<URL>       Urls    { get; set; }
        public DbSet<URLVisit>  Visits  { get; set; }

        public DbSet<UserAction> Actions        { get; set; }
        public DbSet<Resource>   Resources      { get; set; }
        public DbSet<Permission> Permissions    { get; set; }


        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(UserConfiguration).Assembly);
        }
    }
}
