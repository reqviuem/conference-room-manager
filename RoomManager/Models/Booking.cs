using System.ComponentModel.DataAnnotations;

namespace RoomManager.Models;

public class Booking
{
    public Guid Id { get; set; }
    
    [Required] public int RoomId { get; set; }

    public Room? Room { get; set; }

    [Required] public DateOnly DateWhenAvailable { get; set; }

    [Required]public TimeOnly AvailableFrom { get; set; }

    [Required]public TimeOnly AvailableUntil { get; set; }
    
    [Required] public BookingStatus Status { get; set; }

    [Required] public List<string> SelectedServices { get; set; } = null!;
}