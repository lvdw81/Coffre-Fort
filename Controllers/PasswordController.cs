using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using coffre_fort_api.Data;
using coffre_fort_api.Models;

namespace coffre_fort_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PasswordController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PasswordController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/password
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PasswordEntry>>> GetAll()
        {
            return await _context.PasswordEntries.ToListAsync();
        }

        // GET: api/password/user/3
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<PasswordEntry>>> GetPasswordsByUser(int userId)
        {
            return await _context.PasswordEntries
                .Where(p => p.UserId == userId)
                .ToListAsync();
        }

        // GET: api/password/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PasswordEntry>> GetById(int id)
        {
            var entry = await _context.PasswordEntries.FindAsync(id);
            if (entry == null)
                return NotFound();

            return entry;
        }

        // POST: api/password
        [HttpPost]
        public async Task<ActionResult<PasswordEntry>> Create([FromBody] PasswordEntry entry)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _context.PasswordEntries.Add(entry);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = entry.Id }, entry);
        }

        // PUT: api/password/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] PasswordEntry updated)
        {
            if (id != updated.Id)
                return BadRequest("IDs mismatch");

            var existing = await _context.PasswordEntries.FindAsync(id);
            if (existing == null)
                return NotFound();

            existing.NomApplication = updated.NomApplication;
            existing.Identifiant = updated.Identifiant;
            existing.MotDePasse = updated.MotDePasse;
            existing.Tags = updated.Tags;

            await _context.SaveChangesAsync();
            return Ok(existing);
        }

        // DELETE: api/password/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var entry = await _context.PasswordEntries.FindAsync(id);
            if (entry == null)
                return NotFound();

            _context.PasswordEntries.Remove(entry);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/password/share
        [HttpPost("share")]
        public async Task<IActionResult> ShareAllPasswords([FromBody] PasswordShare share)
        {
            var sourceUser = await _context.Users.FindAsync(share.SourceUserId);
            var targetUser = await _context.Users.FindAsync(share.SharedWithUserId);

            if (sourceUser == null || targetUser == null)
                return NotFound("Utilisateur source ou cible introuvable.");

            // Empêche le doublon de partage
            var dejaPartage = await _context.PasswordShares.AnyAsync(ps =>
                ps.SourceUserId == share.SourceUserId && ps.SharedWithUserId == share.SharedWithUserId);

            if (dejaPartage)
                return Conflict("Les donnees de cet utilisateur sont deja partagees avec ce destinataire.");

            share.DatePartage = DateTime.UtcNow;

            _context.PasswordShares.Add(share);
            await _context.SaveChangesAsync();

            return Ok();
        }

        // GET: api/password/shared-from-users/5
        [HttpGet("shared-from-users/{targetUserId}")]
        public async Task<ActionResult<IEnumerable<PasswordEntry>>> GetAllSharedPasswords(int targetUserId)
        {
            var sharedUserIds = await _context.PasswordShares
                .Where(ps => ps.SharedWithUserId == targetUserId)
                .Select(ps => ps.SourceUserId)
                .Distinct()
                .ToListAsync();

            var sharedEntries = await _context.PasswordEntries
                .Where(entry => sharedUserIds.Contains(entry.UserId))
                .ToListAsync();

            return Ok(sharedEntries);
        }
    }
}
