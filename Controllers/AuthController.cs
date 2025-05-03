using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using coffre_fort_api.Data;
using coffre_fort_api.Models;
using coffre_fort_api.Utils;
using Microsoft.EntityFrameworkCore;

namespace coffre_fort_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public class LoginRequest
        {
            public string Identifiant { get; set; }
            public string MotDePasse { get; set; }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Identifiant) || string.IsNullOrWhiteSpace(request.MotDePasse))
            {
                return BadRequest("Champs requis manquants.");
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Identifiant == request.Identifiant);

            if (user == null || !PasswordHasher.Verify(request.MotDePasse, user.MotDePasseHash))
            {
                return Unauthorized("Identifiant ou mot de passe incorrect.");
            }

            var token = GenerateJwtToken(user);
            return Ok(new
            {
                token,
                identifiant = user.Identifiant,
                userId = user.Id
            });
        }

        private string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
            new Claim(ClaimTypes.Name, user.Identifiant),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim("userId", user.Id.ToString())
        };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
