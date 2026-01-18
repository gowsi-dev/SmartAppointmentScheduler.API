using Microsoft.EntityFrameworkCore;
using SmartAppointmentScheduler.Domain.Entities;

namespace Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<TimeSlot> TimeSlots { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
    }
}
