using Azure.Core;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SmartAppointmentScheduler.Authentication.DTOs.Auth;
using SmartAppointmentScheduler.Domain.Entities;
using SmartAppointmentScheduler.Service;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace SmartAppointmentScheduler.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly ITokenService _tokenService;
        private static string accessToken = string.Empty;
        private static readonly string baseUrl = "https://localhost:44371";
        public AuthController (AppDbContext context, ITokenService tokenService, IConfiguration configuration)
        {
            _context = context;
            _tokenService = tokenService;
            _configuration = configuration;
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            if (_context.Users.Any(u => u.Email == dto.Email))
                return BadRequest("Email already exist");

            var user = new User
            {
                Email = dto.Email,
                Name = dto.Username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password)
            };
            
            // Add user
            _context.Users.Add(user);

            // Save the user in database
            await _context.SaveChangesAsync();
            return Ok("User Register Successfully");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            //"email": "test1@gmail.com",
            //"password": "sara"
            // Retrieve user by email with roles eagerly loaded; only active users allowed
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == dto.Email);

            // Verify user exists and password matches the stored hashed password
            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                return Unauthorized(); // Invalid credentials

            // Generate JWT access token with user details, roles, and client info
            accessToken = _tokenService.GenerateAccessToken(user, user.Role, out string jwtId);

            // Read access token expiration duration from config or fallback to 15 minutes
            var accessTokenExpiryMinutes = int.TryParse(_configuration["JwtSettings:AccessTokenExpirationMinutes"], out var val) ? val : 15;
            // Return the tokens and expiry info encapsulated in AuthResponseDTO
            //var data =
            //var data = await CallProtectedApiAsync();
          
            return Ok(new 
            {
                Access= accessToken,
                AccessTokenExpiryMinutes  = accessTokenExpiryMinutes
            });
        }
        static async Task<string> CallProtectedApiAsync()
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                var resp = await client.GetAsync($"{baseUrl}/api/Protected/userdata");
                if (resp.IsSuccessStatusCode)
                {
                    var body = await resp.Content.ReadAsStringAsync();
                    return body;
                }
                else
                {
                    Console.WriteLine($"\n[Protected API] Failed: {resp.StatusCode}");
                    var body = await resp.Content.ReadAsStringAsync();
                    return body;
                }
            }
        }
    }
}
