using System.ComponentModel.DataAnnotations;

namespace RoomManager.Models;

public class Services
{
    public Guid Id { get; set; }

    [Required] public string Name { get; set; } = null!;

    [Required] public int Price { get; set; }
}