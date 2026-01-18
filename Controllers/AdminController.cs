using Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartAppointmentScheduler.Authentication.DTOs.Auth;
using SmartAppointmentScheduler.Domain.Entities;
using SmartAppointmentScheduler.Domain.Enums;
using System.Data;

namespace SmartAppointmentScheduler.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly AppDbContext _context;
        public AdminController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("CreateSlot")]
        public async Task<IActionResult> CreateTimeSlot(TimeSlotDTO _timeSlot)
        {
            TimeSlot timeSlot = new TimeSlot();
            timeSlot.Date = _timeSlot.Date;
            timeSlot.StartTime = TimeSpan.FromHours(Convert.ToDouble(_timeSlot.StartTime));
            timeSlot.EndTime = TimeSpan.FromHours(Convert.ToDouble(_timeSlot.EndTime));
            timeSlot.IsAvailable = true;
            _context.TimeSlots.Add(timeSlot);
            await _context.SaveChangesAsync();
            return Ok("Save Changes Successfully");
        }
        [HttpGet("GetAppointment")]
        public async Task<IActionResult> GetPendingAppointment()
        {
            List<Appointment> appointmentDTOs = new List<Appointment>();
            appointmentDTOs = _context.Appointments.Where(x => x.Status == Domain.Enums.AppointmentStatus.Pending).ToList();
            return Ok(appointmentDTOs);
        }
        [HttpPost("UpdatePending")]
        public async Task<IActionResult> UpdateAppointment(int timeSlotId, int value)
        {
            Appointment appointment = new Appointment();
            appointment = _context.Appointments.Where(x => x.TimeSlotId == timeSlotId).FirstOrDefault();
            if (appointment == null)
            {
                return BadRequest();
            } else
            {
                AppointmentStatus status = (AppointmentStatus)value;
                appointment.Status = status;
                if (status == AppointmentStatus.Approved)
                {
                    TimeSlot timeSlot = _context.TimeSlots.Where(x => x.Id == appointment.TimeSlotId).FirstOrDefault();
                    if (timeSlot != null) { return BadRequest(); }
                    timeSlot.IsAvailable = false;
                } 
                //_context.Appointments.M
                await _context.SaveChangesAsync();
            }
            return Ok("Save Changes Successfully");
        }
        [HttpPost("MarkCompleted")]
        public async Task<IActionResult> CompletedAppointment()
        {
            List<Appointment> appointments = _context.Appointments.ToList();
            List<int> timeSlots = _context.TimeSlots.Where(x => x.Date <= DateTime.Now && x.StartTime < DateTime.Now.TimeOfDay).Select(x => x.Id).ToList();
            appointments = appointments.Where(x => timeSlots.Contains(x.TimeSlotId)).ToList();
            appointments = appointments.Where(x=> x.Status == AppointmentStatus.Completed).ToList();
            if(appointments.Count == 0) { return BadRequest(); }
            appointments.Select(x => { x.Status = AppointmentStatus.Completed; return x; }).ToList();
            await _context.SaveChangesAsync();
            return Ok("Save Changes Successfully");
        }
    }
}








