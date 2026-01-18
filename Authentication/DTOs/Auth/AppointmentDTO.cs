using SmartAppointmentScheduler.Domain.Entities;
using SmartAppointmentScheduler.Domain.Enums;

namespace SmartAppointmentScheduler.Authentication.DTOs.Auth
{
    public class AppointmentDTO
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public int TimeSlotId { get; set; }

        public AppointmentStatus Status { get; set; }
            = AppointmentStatus.Pending;

        public DateTime CreatedAt { get; set; }
            = DateTime.UtcNow;

        // Navigation Properties
        //public User User { get; set; }
        //public TimeSlotDTO TimeSlot { get; set; } = null!;
    }
}
