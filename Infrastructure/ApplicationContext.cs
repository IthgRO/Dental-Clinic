using Dental_Clinic.Dtos;
using Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure
{
    public class ApplicationContext : DbContext
    {

        public DbSet<UserDto> Users { get; set; }

        public DbSet<ClinicDto> Clinics { get; set; }

        public DbSet<AppointmentDto> Appointments { get; set; }

        public DbSet<ServiceDto> Services { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserDto>()
                .HasOne(x => x.Clinic)
                .WithMany(x => x.Users)
                .HasForeignKey(x => x.ClinicId);

            modelBuilder.Entity<ServiceDto>()
                .HasOne(x => x.User)
                .WithMany(x => x.Services)
                .HasForeignKey(x => x.UserId);
        }
       

        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options) { }
    }
}
