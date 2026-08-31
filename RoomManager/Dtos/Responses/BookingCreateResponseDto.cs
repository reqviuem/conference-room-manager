namespace RoomManager.Dtos.Responses;

public class BookingCreateResponseDto
{
    public Guid Id { get; set; }
    
    public Guid RoomId { get; set; }
    
    public DateTime StartAt { get; set; }
    
    public DateTime EndAt { get; set; }
    
    public decimal RoomPrice { get; set; }
    
    public decimal ServicesPrice { get; set; }
    
    public decimal TotalPrice { get; set; }
    
    public List<string> Services { get; set; } = [];
}