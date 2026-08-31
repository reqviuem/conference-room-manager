namespace RoomManager.Dtos.Requests;

public class BookingCreateRequestDto
{
    public Guid RoomId { get; set; }
    
    public required string Date { get; set; }
    
    public required string StartTime { get; set; }
    
    public decimal DurationHours { get; set; }
    
    public required List<string> Services { get; set; }
}