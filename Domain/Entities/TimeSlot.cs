using System.Text.Json.Serialization;

namespace SmartAppointmentScheduler.Domain.Entities
{
    public class TimeSlot
    {
        public int Id { get; set; }

        public DateTime Date { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }

        public bool IsAvailable { get; set; } = true;

        // Navigation Property
        //public Appointment? Appointment { get; set; }
    }
}
