namespace SmartAppointmentScheduler.Domain.Entities
{
    public class User
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string PasswordHash { get; set; } = null!;

        public string Role { get; set; } = "User"; // User or Admin

        // Navigation Property
        //public ICollection<Appointment> Appointments { get; set; }
        //    = new List<Appointment>();
    }
}
