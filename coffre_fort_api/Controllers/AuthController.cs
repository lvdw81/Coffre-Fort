using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using coffre_fort_api.Data;
using coffre_fort_api.Models;
using coffre_fort_api.Utils;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;
using System.Net;
using System.Collections.Generic;

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
                return BadRequest("Champs requis manquants.");

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Identifiant == request.Identifiant);
            if (user == null || !PasswordHasher.Verify(request.MotDePasse, user.MotDePasseHash))
                return Unauthorized("Identifiant ou mot de passe incorrect.");

            var code = new Random().Next(100000, 999999).ToString();
            Services.A2F.Codes[user.Identifiant] = code;

            EnvoyerCodeParEmail(user.Identifiant, code);

            return Ok(new
            {
                message = "Code 2FA envoyé",
                identifiant = user.Identifiant
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

        private void EnvoyerCodeParEmail(string destinataire, string code)
        {
            var smtp = new SmtpClient("smtp.gmail.com") // ou autre serveur SMTP
            {
                Port = 587,
                Credentials = new NetworkCredential("vaulta2F@gmail.com", "kbrr fjfd qwgo lfyg"),
                EnableSsl = true
            };

            smtp.Send("vaulta2F@gmail.com", destinataire, "Code de confirmation", $"Voici votre code : {code}");
        }

        public class VerificationCodeModel
        {
            public string Identifiant { get; set; }
            public string Code { get; set; }
        }

        [HttpPost("verifier-code")]
        public async Task<IActionResult> VerifierCode([FromBody] VerificationCodeModel model)
        {
            if (Services.A2F.Codes.TryGetValue(model.Identifiant, out var codeAttendu)
                && codeAttendu == model.Code)
            {
                Services.A2F.Codes.TryRemove(model.Identifiant, out _);


                var user = await _context.Users.FirstOrDefaultAsync(u => u.Identifiant == model.Identifiant);
                if (user == null) return NotFound("Utilisateur introuvable.");

                var token = GenerateJwtToken(user);
                return Ok(new
                {
                    token,
                    identifiant = user.Identifiant,
                    userId = user.Id
                });
            }

            return Unauthorized("Code invalide.");
        }


    }
}
