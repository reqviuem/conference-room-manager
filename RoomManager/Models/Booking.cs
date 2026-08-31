using System.ComponentModel.DataAnnotations;

namespace RoomManager.Models;

public class Booking
{
    public Guid Id { get; set; }
    
    public Guid RoomId { get; set; }
    
     [Required]public DateTime StartAt { get; set; }
    
     [Required]public DateTime EndAt { get; set; }
    
     public decimal TotalPrice { get; set; }
    
     public List<BookingService> BookingServices { get; set; } = new();
}