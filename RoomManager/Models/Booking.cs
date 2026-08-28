using System.ComponentModel.DataAnnotations;

namespace RoomManager.Models;

public class Booking
{
    public Guid Id { get; set; }
    
    [Required] public Guid RoomId { get; set; }
    
    [Required] public DateTime StartAt { get; set; }
    
    [Required] public DateTime EndAt { get; set; }
    
    [Required] public decimal TotalPrice { get; set; }
    
    [Required] public List<BookingService> BookingServices { get; set; } = null!;
}