using Microsoft.EntityFrameworkCore;
using DAL.DAO;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace DAL
{
    public class OrganizationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        public OrganizationDbContext(DbContextOptions<OrganizationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Country> Countries { get; set; }
        public DbSet<ApplicationUser> ApplicationUser { get; set; }
        public DbSet<ApplicationRole> ApplicationRoles { get; set; }
        public DbSet<ApplicationUserRole> ApplicationUserRoles { get; set; }
        public DbSet<IdentityUserRole<string>> ApplicationUserRole { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Set Country table name as Country
            modelBuilder.Entity<Country>().ToTable("Country");

            // Set CountryId as primary key
            modelBuilder.Entity<Country>().HasKey(c => c.CountryId);

            // Set Country Name as required
            modelBuilder.Entity<Country>().Property(c => c.CountryName).IsRequired();

            // Optional: Uncomment to enforce unique constraint
            // modelBuilder.Entity<Country>().HasIndex(c => c.CountryName).IsUnique();
        }
    }
}


