using System.Globalization;
using System.Runtime.InteropServices;
using RoomManager.Dtos.Requests;

namespace RoomManager.Services;

public class DateSerializer
{
    public DateTime StartAt(string requestDate, string requestFrom)
    {
        var date = DateOnly.ParseExact(
            requestDate,
            "dd.MM.yyyy",
            CultureInfo.InvariantCulture);

        var from = TimeOnly.ParseExact(
            requestFrom,
            "HH:mm",
            CultureInfo.InvariantCulture);

        var local = date.ToDateTime(from);
        var startAt = local.ToUniversalTime();
        return startAt;
    }


    public DateTime EndAt(string requestDate, string requestUntil)
    {
        var date = DateOnly.ParseExact(
            requestDate,
            "dd.MM.yyyy",
            CultureInfo.InvariantCulture);

        var until = TimeOnly.ParseExact(
            requestUntil,
            "HH:mm",
            CultureInfo.InvariantCulture);

        var local = date.ToDateTime(until);

        var endAt = local.ToUniversalTime();

        return endAt;
    }
    
    public DateTime BuildEndAtFromDuration(DateTime startAt, decimal durationHours)
    {
        return startAt.AddHours((double)durationHours);
    }
}