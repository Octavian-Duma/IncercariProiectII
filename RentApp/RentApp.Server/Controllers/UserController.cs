using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RentApp.Data;
using RentApp.Server.Models.DTO.User;
using System.Security.Claims;

namespace RentApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly RentDbContext _context;

        public UserController(RentDbContext context)
        {
            _context = context;
        }

        // GET: api/user/profile
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var userId = GetUserId();
            if (userId == null)
                return Unauthorized(new { error = "Utilizator neautorizat" });

            var user = await _context.Users
                .Where(u => u.UserId == userId)
                .Select(u => new
                {
                    u.UserId,
                    u.Name,
                    u.email,
                    u.telephoneNumber
                })
                .FirstOrDefaultAsync();

            if (user == null)
                return NotFound(new { error = "Utilizatorul nu a fost gasit" });

            return Ok(user);
        }

        // PUT: api/user/profile
        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateAccountDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = GetUserId();
            if (userId == null)
                return Unauthorized(new { error = "Neautorizat" });

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return NotFound(new { error = "Utilizatorul nu a fost gasit" });

            user.Name = dto.Name ?? user.Name;
            user.email = dto.Email ?? user.email;
            user.telephoneNumber = dto.TelephoneNumber ?? user.telephoneNumber;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Contul a fost actualizat cu succes" });
        }

        // PUT: api/user/change-password
        [HttpPut("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = GetUserId();
            if (userId == null)
                return Unauthorized(new { error = "Neautorizat" });

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return NotFound(new { error = "Utilizatorul nu a fost gasit" });

            if (!BCrypt.Net.BCrypt.Verify(dto.OldPassword, user.password))
                return BadRequest(new { error = "Parola veche este incorecta" });

            user.password = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Parola a fost schimbata cu succes" });
        }

        private int? GetUserId()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return userId != null ? int.Parse(userId) : (int?)null;
        }

        // DELETE: api/user
        [HttpDelete]
        public async Task<IActionResult> DeleteAccount()
        {
            var userId = GetUserId();
            if (userId == null)
                return Unauthorized(new { error = "Neautorizat" });

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return NotFound(new { error = "Utilizatorul nu a fost gasit." });

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Contul a fost sters cu succes" });
        }
    }
}
