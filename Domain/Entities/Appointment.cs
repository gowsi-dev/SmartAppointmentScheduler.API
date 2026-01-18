using SmartAppointmentScheduler.Authentication.DTOs.Auth;
using SmartAppointmentScheduler.Domain.Enums;
namespace SmartAppointmentScheduler.Domain.Entities
{
    public class Appointment
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public int TimeSlotId { get; set; }

        public AppointmentStatus Status { get; set; }
            = AppointmentStatus.Pending;

        public DateTime CreatedAt { get; set; }
            = DateTime.UtcNow;

        // Navigation Properties
        public User User { get; set; }
        public TimeSlot TimeSlot { get; set; } = null!;
    }
}
