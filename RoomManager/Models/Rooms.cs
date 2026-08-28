using System.ComponentModel.DataAnnotations;

namespace RoomManager.Models;

public class Rooms
{
    public Guid Id { get; set; }

    [Required] public string Name { get; set; } = null!;
    
    [Required] public int Capacity { get; set; }

    [Required] public Services Services { get; set; } = null!;
    
    [Required] public int PricePerHour { get; set; }
}