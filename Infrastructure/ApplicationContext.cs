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

        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options) { }
    }
}
