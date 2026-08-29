namespace RoomManager.Dtos.Requests;

public class RoomUpdateRequestDto
{
    public required Guid Id { get; set; }
    
    public required decimal BasePricePerHour { get; set; }
    
    public required List<string> Services { get; set; }
}