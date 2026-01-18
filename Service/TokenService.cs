using Microsoft.IdentityModel.Tokens;
using SmartAppointmentScheduler.Domain.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SmartAppointmentScheduler.Service
{
    public class TokenService : ITokenService
    {// IConfiguration to access appsettings.json values like issuer and token expiry times
        private readonly IConfiguration _configuration;
        // Constructor injects IConfiguration dependency
        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        // Generates a JWT Access Token for the authenticated user.
        public string GenerateAccessToken(User user, string role, out string jwtId)
        {
            var secretKey = _configuration["SecretKey"];
            // Initialize JWT token handler which creates and serializes tokens
            var tokenHandler = new JwtSecurityTokenHandler();
            // Decode the Base64-encoded client secret key into byte array for signing
            var keyBytes = Encoding.UTF8.GetBytes(secretKey);
            var key = new SymmetricSecurityKey(keyBytes);
            // Generate a new unique identifier for the JWT token (jti claim)
            jwtId = Guid.NewGuid().ToString();
            // Read issuer and token expiration from configuration, with default fallback values
            var issuer = _configuration["JwtSettings:Issuer"] ?? "DefaultIssuer";
            var accessTokenExpirationMinutes = int.TryParse(_configuration["JwtSettings:AccessTokenExpirationMinutes"], out var val) ? val : 15;
            // Define the claims to be embedded in the JWT token
            var claims = new List<Claim>
            {
                // Subject claim represents user identifier
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                // JWT ID claim for unique token identification (used to link refresh tokens)
                new Claim(JwtRegisteredClaimNames.Jti, jwtId),
                // User email claim for identification purposes
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                // Issuer claim indicating the token issuer
                new Claim(JwtRegisteredClaimNames.Iss, issuer)
            };
            // Add role claims for authorization and role-based access control
            claims.Add(new Claim(ClaimTypes.Role, role));
            // Create signing credentials with the symmetric security key and HMAC SHA256 algorithm
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);
            // Define the JWT token descriptor containing claims, expiration, signing credentials, issuer, and audience
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims), // Claims identity constructed from claims list
                Expires = DateTime.UtcNow.AddMinutes(accessTokenExpirationMinutes), // Token expiration time
                SigningCredentials = creds, // Signing credentials using client secret key
                Issuer = issuer, // Token issuer
            };
            // Create the JWT token based on the descriptor
            var token = tokenHandler.CreateToken(tokenDescriptor);
            // Serialize the JWT token to compact JWT format string
            return tokenHandler.WriteToken(token);
        }
    }
}
