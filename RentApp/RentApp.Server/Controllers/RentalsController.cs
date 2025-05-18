using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentApp.Server.Models.DTO.Rental;
using RentApp.Server.Service;
using System.Security.Claims;

namespace RentApp.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RentalsController : ControllerBase
    {
        private readonly IRentalService _rentalService;

        public RentalsController(IRentalService rentalService)
        {
            _rentalService = rentalService;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> RentProduct([FromBody] RentalRequestDTO dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
                return Unauthorized(new { message = "Utilizator neautorizat" });

            try
            {
                var rentalId = await _rentalService.RentProductAsync(dto, userId);
                return Ok(new { message = "Inchiriere realizata cu succes", rentalId });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("my")]
        [Authorize]
        public async Task<IActionResult> GetMyRentals()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
                return Unauthorized();

            var rentals = await _rentalService.GetMyRentalsAsync(userId);
            return Ok(rentals);
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> CancelRental(int id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
                return Unauthorized();

            try
            {
                var result = await _rentalService.CancelRentalAsync(id, userId);
                if (!result)
                    return NotFound();

                return Ok(new { message = "Inchirierea a fost anulata" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("product/{id}")]
        public async Task<IActionResult> GetProductDetails(int id)
        {
            var product = await _rentalService.GetProductDetailsAsync(id);
            if (product == null)
                return NotFound(new { message = "Produsul nu a fost gasit" });

            return Ok(product);
        }
    }
}