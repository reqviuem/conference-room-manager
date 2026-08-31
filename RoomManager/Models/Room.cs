
namespace RoomManager.Models;

public class Room
{
    public Guid Id { get; set; }
    
    public string Name { get; set; } = null!;
    
   public int Capacity { get; set; }
    
    public List<RoomService> RoomServices { get; set; } = new();
    
    public decimal PricePerHour { get; set; }
}