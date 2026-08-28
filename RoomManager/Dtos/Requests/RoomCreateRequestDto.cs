namespace RoomManager.Dtos.Requests;

public class RoomCreateRequestDto
{
    public required string Name { get; set; }
    
    public required int Capacity { get; set; }
    
    public required int BasePricePerHour { get; set; }
    
    public required List<string> Services { get; set; }
}