using System.ComponentModel.DataAnnotations;

namespace RoomManager.Models;

public class Service
{
    public Guid Id { get; set; }
    
    [Required] public string Name { get; set; } = null!;

    [Required] public decimal Price { get; set; }
}