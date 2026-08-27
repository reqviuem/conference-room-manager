using System.ComponentModel.DataAnnotations;

namespace RoomManager.Models;

public class Room
{
    public Guid Id { get; set; }

    [Required] public string Name { get; set; } = null!;
    
    [Required] public int Capacity { get; set; }

    [Required] public List<string> AvailableServices { get; set; } = null!;
    
    [Required] public int PricePerHour { get; set; }
    
    
}