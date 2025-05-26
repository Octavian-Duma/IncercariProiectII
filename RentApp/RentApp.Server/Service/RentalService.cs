using Microsoft.EntityFrameworkCore;
using RentApp.Data;
using RentApp.Models;
using RentApp.Server.Models;
using RentApp.Server.Models.DTO.Rental;
using RentApp.Server.Models.DTO.Review;

namespace RentApp.Server.Service
{
    public interface IRentalService
    {
        Task<int> RentProductAsync(RentalRequestDTO dto, int userId);
        Task<IEnumerable<RentalListItemDTO>> GetMyRentalsAsync(int userId);
        Task<bool> CancelRentalAsync(int rentalId, int userId);
        Task<ProductRentalDetailsDTO?> GetProductDetailsAsync(int productId);
        Task<IEnumerable<RentalListItemDTO>> GetAllRentalsAsync();
        Task<IEnumerable<RentalWithUserDTO>> GetAllRentalsWithUsersAsync();
    }

    public class RentalService : IRentalService
    {
        private readonly RentDbContext _context;

        public RentalService(RentDbContext context)
        {
            _context = context;
        }

        public async Task<int> RentProductAsync(RentalRequestDTO dto, int userId)
        {
            var product = await _context.Products.FindAsync(dto.ProductId);
            if (product == null)
                throw new Exception("Produsul nu a fost găsit!");

            // 1. Verificare suprapunere cu alte închirieri ACTIVE (Confirmed sau InProgress)
            var overlappingRental = await _context.Rentals
                .Where(r => r.ProductId == dto.ProductId
                    && (r.Status == States.Confirmed || r.Status == States.InProgress)
                    && r.EndDate > dto.StartDate
                    && r.StartDate < dto.EndDate)
                .FirstOrDefaultAsync();

            if (overlappingRental != null)
                throw new Exception("Produsul nu este disponibil în perioada selectată!");

            var days = (dto.EndDate - dto.StartDate).Days;
            if (days <= 0)
                throw new Exception("Perioada de închiriere este invalidă!");

            // 2. Creează noua închiriere FĂRĂ să setezi product.Available = false
            var rental = new Rental
            {
                ProductId = dto.ProductId,
                UserId = userId,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                RegisterDate = DateTime.UtcNow,
                TotalPrice = product.PricePerDay * days,
                Status = States.Confirmed
            };

            _context.Rentals.Add(rental);
            await _context.SaveChangesAsync();
            return rental.RentalId ?? 0;
        }


        public async Task<IEnumerable<RentalListItemDTO>> GetMyRentalsAsync(int userId)
        {
            var rentals = await _context.Rentals
                .Include(r => r.product)
                .Where(r => r.UserId == userId)
                .ToListAsync();

            return rentals.Select(r => new RentalListItemDTO
            {
                RentalId = r.RentalId ?? 0,
                ProductId = r.ProductId,
                UserId = r.UserId,
                ProductName = r.product.Name,
                StartDate = r.StartDate,
                EndDate = r.EndDate,
                TotalPrice = r.TotalPrice,
                Status = r.Status switch
                {
                    States.Confirmed => "Inactiv",
                    States.InProgress => "Activ",
                    States.Completed => "Finalizat",
                    States.Cancelled => "Anulat",
                    _ => r.Status.ToString()
                }
            });
        }

        public async Task<bool> CancelRentalAsync(int rentalId, int userId)
        {
            var rental = await _context.Rentals
                .Include(r => r.product)
                .FirstOrDefaultAsync(r => r.RentalId == rentalId && r.UserId == userId);

            if (rental == null)
                return false;

            // 1. Dacă perioada s-a terminat și nu e deja Completed, marchează ca Completed & produs disponibil
            if (rental.Status != States.Completed && rental.EndDate.Date < DateTime.UtcNow.Date)
            {
                rental.Status = States.Completed;
                rental.product.Available = true;
                await _context.SaveChangesAsync();
            }

            // 2. Dacă statusul este Completed (Finalizat) SAU Cancelled (Anulat) – șterge complet închirierea!
            if (rental.Status == States.Completed || rental.Status == States.Cancelled)
            {
                _context.Rentals.Remove(rental);
                rental.product.Available = true;
                await _context.SaveChangesAsync();
                return true;
            }

            // 3. Poți șterge complet dacă este Confirmed (Inactiv) și ai >= 3 zile întregi până la start
            var nowDate = DateTime.UtcNow.Date;
            var startDate = rental.StartDate.Date;
            var daysUntilStart = (startDate - nowDate).Days;

            if (rental.Status == States.Confirmed && daysUntilStart >= 3)
            {
                _context.Rentals.Remove(rental);
                rental.product.Available = true;
                await _context.SaveChangesAsync();
                return true;
            }

            // 4. Nu poți anula/șterge în alte cazuri!
            throw new Exception("Poți șterge o închiriere finalizată, sau poți anula cu minim 3 zile înainte de data de început (ștergerea se consideră tot anulare)!");
        }





        public async Task<ProductRentalDetailsDTO?> GetProductDetailsAsync(int productId)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null)
                return null;

            return new ProductRentalDetailsDTO
            {
                Id = product.Id,
                Name = product.Name,
                PricePerDay = product.PricePerDay,
                Description = product.Description,
                Available = product.Available
            };
        }

        public async Task<IEnumerable<RentalListItemDTO>> GetAllRentalsAsync()
        {
            var rentals = await _context.Rentals
                .Include(r => r.product)
                .ToListAsync();

            return rentals.Select(r => new RentalListItemDTO
            {
                RentalId = r.RentalId ?? 0,
                ProductId = r.ProductId,
                UserId = r.UserId,
                ProductName = r.product.Name,
                StartDate = r.StartDate,
                EndDate = r.EndDate,
                TotalPrice = r.TotalPrice,
                Status = r.Status switch
                {
                    States.Confirmed => "Inactiv",
                    States.InProgress => "Activ",
                    States.Completed => "Finalizat",
                    States.Cancelled => "Anulat",
                    _ => r.Status.ToString()
                }
            });
        }

        public async Task<IEnumerable<RentalWithUserDTO>> GetAllRentalsWithUsersAsync()
        {
            var rentals = await _context.Rentals
                .Include(r => r.product)
                .Include(r => r.user)
                .ToListAsync();

            return rentals.Select(r => new RentalWithUserDTO
            {
                RentalId = r.RentalId ?? 0,
                ProductId = r.ProductId,
                UserId = r.UserId,
                ProductName = r.product.Name,
                StartDate = r.StartDate,
                EndDate = r.EndDate,
                TotalPrice = r.TotalPrice,
                Status = r.Status switch
                {
                    States.Confirmed => "Inactiv",
                    States.InProgress => "Activ",
                    States.Completed => "Finalizat",
                    States.Cancelled => "Anulat",
                    _ => r.Status.ToString()
                },
                UserName = r.user.Name,
                UserEmail = r.user.email,
                UserTelephone = r.user.telephoneNumber
            });
        }
    }
}
