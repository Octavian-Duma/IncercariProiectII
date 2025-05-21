namespace RentApp.Server.Models.DTO.Rental
{
    
        public class RentalWithUserDTO
        {
            public int RentalId { get; set; }
            public int ProductId { get; set; }
            public int UserId { get; set; }
            public string ProductName { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
            public decimal TotalPrice { get; set; }
            public string Status { get; set; }
            // USER INFO
            public string UserName { get; set; }
            public string UserEmail { get; set; }
            public string UserTelephone { get; set; }
        }
    }


