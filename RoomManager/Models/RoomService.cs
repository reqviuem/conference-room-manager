using System.ComponentModel.DataAnnotations;

namespace RoomManager.Models;

public class RoomService
{
    [Required] public  Guid RoomId { get; set; }
    public Room Room { get; set; } = null!;
    
    [Required] public  Guid ServiceId { get; set; }
    public Service Service { get; set; } = null!;
}