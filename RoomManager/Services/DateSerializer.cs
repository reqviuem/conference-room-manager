using System.Globalization;

namespace RoomManager.Services;

public class DateSerializer
{
    private const string DateFormat = "dd.MM.yyyy";
    private const string TimeFormat = "HH:mm";

    public DateTime StartAt(string requestDate, string requestFrom)
    {
        var date = ParseDate(requestDate);
        var from = ParseTime(requestFrom);

        return date.ToDateTime(from);
    }

    public DateTime EndAt(string requestDate, string requestUntil)
    {
        var date = ParseDate(requestDate);
        var until = ParseTime(requestUntil);

        return date.ToDateTime(until);
    }

    public DateTime BuildEndAtFromDuration(DateTime startAt, decimal durationHours)
    {
        return startAt.AddHours((double)durationHours);
    }

    public bool CheckPastDate(string requestDate)
    {
        var date = ParseDate(requestDate);
        return date < DateOnly.FromDateTime(DateTime.Today);
    }

    private static DateOnly ParseDate(string requestDate)
    {
        if (!DateOnly.TryParseExact(
                requestDate,
                DateFormat,
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out var date))
        {
            throw new FormatException();
        }

        return date;
    }

    private static TimeOnly ParseTime(string requestTime)
    {
        if (!TimeOnly.TryParseExact(
                requestTime,
                TimeFormat,
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out var time))
        {
            throw new FormatException();
        }

        return time;
    }
}