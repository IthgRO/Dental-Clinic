using Dental_Clinic.Dtos;
using Infrastructure.Dtos;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure
{
    public class ApplicationContext : DbContext
    {

        public DbSet<UserDto> Users { get; set; }

        public DbSet<ClinicDto> Clinics { get; set; }

        public DbSet<AppointmentDto> Appointments { get; set; }

        public DbSet<ServiceDto> Services { get; set; }

        public DbSet<ReminderDto> Reminders { get; set; }

        public DbSet<ReminderSettingsDto> ReminderSettings {  get; set; }

        public DbSet<DentistScheduleDto> DentistSchedules { get; set; }

        public DbSet<PasswordChangeAttempt> PasswordChangeAttempts { get; set; }

        public DbSet<DoctorLoginAttemptDto> DoctorLoginAttempts { get; set; }

        public DbSet<InvitationDto> Invitations { get; set; }

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

            modelBuilder.Entity<AppointmentDto>()
                .HasOne(x => x.Dentist)
                .WithMany(x => x.Appointments)
                .HasForeignKey(x => x.DentistId);

            modelBuilder.Entity<AppointmentDto>()
                .HasOne(x => x.Clinic)
                .WithMany(x => x.Appointments)
                .HasForeignKey(x => x.ClinicId);

            modelBuilder.Entity<AppointmentDto>()
                .HasOne(x => x.Service)
                .WithMany(x => x.Appointments)
                .HasForeignKey(x => x.ServiceId);

            base.OnModelCreating(modelBuilder);

            // Configure Reminder
            modelBuilder.Entity<ReminderDto>()
                .HasKey(r => r.Id);

            modelBuilder.Entity<ReminderDto>()
                .Property(r => r.SendAt)
                .IsRequired();

            // Configure One-to-One Relationship
            modelBuilder.Entity<ReminderDto>()
                .HasOne(r => r.Appointment) // Reminder references Appointment
                .WithOne()  // Appointment references Reminder
                .HasForeignKey<ReminderDto>(r => r.AppointmentId) // Foreign key
                .OnDelete(DeleteBehavior.Cascade); 


        }
       

        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options) { }
    }
}
