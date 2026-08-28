namespace RoomManager.Models;

public class BookingService
{
    public Guid BookingId { get; set; }
    
    public Guid ServiceId { get; set; }
    
    public decimal PriceAtBooking { get; set; }
}