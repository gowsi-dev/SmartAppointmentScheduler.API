using SmartAppointmentScheduler.Domain.Entities;

namespace SmartAppointmentScheduler.Service
{
    public interface ITokenService
    {
        string GenerateAccessToken(User user, string roles, out string jwtId);
    }
}
