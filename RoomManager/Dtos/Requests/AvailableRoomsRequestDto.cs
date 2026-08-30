namespace RoomManager.Dtos.Requests;

public class AvailableRoomsRequestDto
{
    public required string Date { get; set; }
    public required string From { get; set; }
    public required string Until { get; set; }
    public required int Capacity { get; set; }
}