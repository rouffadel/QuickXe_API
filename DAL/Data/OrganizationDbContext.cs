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
        public DbSet<Customer> Customers { get; set; }
        public DbSet<SendEmail> SendEmails { get; set; }
        public DbSet<CustomerOTP> CustomerOTPs { get; set; }
        public DbSet<CountriesMaster> CountriesMaster { get; set; }
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


            // Set SendEmail table name as SendEmail
            modelBuilder.Entity<SendEmail>().ToTable("SendEmail");

            // Set Id as primary key
            modelBuilder.Entity<SendEmail>().HasKey(c => c.Id);

            modelBuilder.Entity<SendEmail>().HasOne(c => c.ApplicationUser).WithMany(u => u.SendEmails).HasForeignKey(c => c.UserId);



            // Set CustomerOTP table name as CustomerOTP
            modelBuilder.Entity<CustomerOTP>().ToTable("CustomerOTP");

            // Set Id as primary key
            modelBuilder.Entity<CustomerOTP>().HasKey(c => c.Id);

            //modelBuilder.Entity<CustomerOTP>().HasOne(c => c.Customers).WithMany(u => u.CustomerOTPs).HasForeignKey(c => c.CustomerId);



            // Set Customer table name as Customer
            modelBuilder.Entity<Customer>().ToTable("Customer");

            // Set CustomerId as primary key
            modelBuilder.Entity<Customer>().HasKey(c => c.CustomerId);

            // Set Customer Name as required
            modelBuilder.Entity<Customer>().Property(c => c.Name).IsRequired();

            // Set Customer Phone Number as required
            modelBuilder.Entity<Customer>().Property(c => c.PhoneNumber).IsRequired();

            // Set Customer Email as required
            modelBuilder.Entity<Customer>().Property(c => c.Email).IsRequired();

            // Set Customer Address as required
            modelBuilder.Entity<Customer>().Property(c => c.Address).IsRequired();



            // Set CountriesMaster table name as CountriesMaster
            modelBuilder.Entity<CountriesMaster>().ToTable("CountriesMaster");

            // Set CountryId as primary key
            modelBuilder.Entity<CountriesMaster>().HasKey(c => c.CountryId);

            modelBuilder.Entity<Country>().HasOne(c => c.ApplicationUser).WithMany(u => u.Countries).HasForeignKey(c => c.TenantId);
            // Optional: Uncomment to enforce unique constraint
            // modelBuilder.Entity<Country>().HasIndex(c => c.CountryName).IsUnique();
        }
    }
}


