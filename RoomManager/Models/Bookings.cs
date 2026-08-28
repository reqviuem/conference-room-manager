using System.ComponentModel.DataAnnotations;

namespace RoomManager.Models;

public class Bookings
{
    public Guid Id { get; set; }
    
    [Required] public int RoomsId { get; set; }

    public Rooms? Room { get; set; }

    [Required] public DateOnly DateWhenAvailable { get; set; }

    [Required]public TimeOnly AvailableFrom { get; set; }

    [Required]public TimeOnly AvailableUntil { get; set; }
    
    [Required] public BookingStatuses Statuses { get; set; }

    [Required] public Services SelectedServices { get; set; } = null!;
}