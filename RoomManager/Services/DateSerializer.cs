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

        var startAt = date.ToDateTime(from);
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

        var endAt = date.ToDateTime(until);

        return endAt;
    }
    
    public DateTime BuildEndAtFromDuration(DateTime startAt, decimal durationHours)
    {
        return startAt.AddHours((double)durationHours);
    }

    public bool CheckPastDate(string requestDate)
    {
        var dateIsPast = false;
        
        var date = DateTime.ParseExact(
            requestDate,
            "dd.MM.yyyy",
            CultureInfo.InvariantCulture);

        if (date < DateTime.Now)
        {
            dateIsPast = true;
        }

        return dateIsPast;
    }
}