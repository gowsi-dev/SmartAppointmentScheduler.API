using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartAppointmentScheduler.Authentication.DTOs.Auth;
using SmartAppointmentScheduler.Domain.Entities;
using SmartAppointmentScheduler.Domain.Enums;
using System.Security.Claims;

namespace SmartAppointmentScheduler.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AppointmentsController(AppDbContext context)
        {
            _context = context;
        }
        private int GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
                throw new UnauthorizedAccessException("User ID not found in token");

            return int.Parse(userIdClaim.Value);
        }
        [HttpPost("book")]
        public async Task<IActionResult> BookAppointment(int timeSlotID)
        {
            Appointment appointmentDTO = new Appointment();
            appointmentDTO.TimeSlotId = timeSlotID;
            appointmentDTO.Status = AppointmentStatus.Pending;
            appointmentDTO.UserId = GetUserId();
            appointmentDTO.CreatedAt = DateTime.UtcNow;

            _context.Appointments.Add(appointmentDTO);
            await _context.SaveChangesAsync();

            return Ok("Appointment booked successfully");
        }


        [HttpPost("GetTimeSlot")]
        public IActionResult AvailableTime(string dateString)
        {
            List<TimeSlot> timeSlots = new List<TimeSlot>();

            DateTime dateValue = DateTime.Parse(dateString);
            timeSlots = _context.TimeSlots.Where(x => x.Date == dateValue && x.IsAvailable).ToList();
            if (timeSlots.Count == 0) { return BadRequest(); }
            return Ok(timeSlots);
        }
        [HttpGet("GetAppointments")]
        public IActionResult UserAppointments()
        {
            List<Appointment> appointment = _context.Appointments.Where(x => x.UserId == GetUserId()).ToList();
            if(appointment.Count > 0)
            {
                return Ok(appointment);
            }
            return BadRequest();
        }
        [HttpPost("CancelleAppointment")]
        public async Task<IActionResult> CancelleAppointment(int id)
        {
            Appointment appointment = _context.Appointments.Where(x => x.Id == id && x.UserId == GetUserId()).FirstOrDefault();
            if (appointment != null)
            {
                appointment.Status = AppointmentStatus.Cancelled;
                await _context.SaveChangesAsync();
                return Ok(appointment);
            }
            return BadRequest();
        }
    }
}
