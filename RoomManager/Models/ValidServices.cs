using System.ComponentModel.DataAnnotations;

namespace RoomManager.Models;

public static class ValidServices
{
    public static readonly Dictionary<string, decimal> Catalog = new()
    {
        { "Проєктор", 500 },
        { "Wi-Fi", 300 },
        { "Звук", 700 }
    };
}