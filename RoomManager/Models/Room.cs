using System.ComponentModel.DataAnnotations;

namespace RoomManager.Models;

public class Room
{
    public Guid Id { get; set; }
    
    [Required] public string Name { get; set; } = null!;
    
    [Required] public int Capacity { get; set; }
    
    [Required] public List<RoomService> RoomServices { get; set; } = null!;
    
    [Required] public decimal PricePerHour { get; set; }
}