namespace RoomManager.Models;

public static class AvailableServices
{
    public static readonly Dictionary<string, int> Services = new Dictionary<string, int>
    {
        { "Проєктор", 500 },
        { "Wi-Fi", 300 },
        { "Звук", 700 }
    };
}