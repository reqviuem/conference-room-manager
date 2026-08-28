namespace RoomManager.Dtos.Responses;

public class RoomCreateResponseDto
{
    public required Guid Id { get; set; }
    
    public required string Name { get; set; }
    
    public required int Capacity { get; set; }
    
    public required decimal BasePricePerHour { get; set; }
    
    public required List<string> Services { get; set; }
}