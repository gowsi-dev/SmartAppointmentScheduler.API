namespace SmartAppointmentScheduler.Authentication.DTOs.Auth
{
    public class TimeSlotDTO
    {
        public int Id { get; set; }

        public DateTime Date { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }

        public bool IsAvailable { get; set; } = true;

    }
}
