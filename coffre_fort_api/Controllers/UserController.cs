
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using coffre_fort_api.Data;
using coffre_fort_api.Models;
using coffre_fort_api.Utils;
using Microsoft.AspNetCore.Authorization;

namespace coffre_fort_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UserController(AppDbContext context)
        {
            _context = context;
        }

        // POST: api/user
        [HttpPost]
        public async Task<ActionResult<User>> CreateUser([FromBody] User user)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            user.MotDePasseHash = PasswordHasher.Hash(user.MotDePasseHash);

            // Verifie si un utilisateur avec le même identifiant existe deja
            if (await _context.Users.AnyAsync(u => u.Identifiant == user.Identifiant))
                return Conflict("Identifiant deja utilise");

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUserByIdentifiant), new { identifiant = user.Identifiant }, user);
        }

        // GET: api/user/by-identifiant/lucas
        [AllowAnonymous]
        [HttpGet("by-identifiant/{identifiant}")]
        public async Task<ActionResult<User>> GetUserByIdentifiant(string identifiant)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Identifiant == identifiant);

            if (user == null)
                return NotFound();

            return user;
        }

        

        // GET: api/user/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUserById(int id)
        {
            var user = await _context.Users
                .Include(u => u.PasswordEntries)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
                return NotFound();

            return user;
        }

    }
}
