namespace RoomManager.Services;

public class BookingPriceCalculator
{
    private decimal ApplyTimeBasedPrice(decimal basePrice, TimeSpan time)
    {
        if (time >= TimeSpan.FromHours(12) && time < TimeSpan.FromHours(14))
        {
            var markup = basePrice * 15 / 100;
            return basePrice + markup;
        }

        if (time >= TimeSpan.FromHours(18) && time < TimeSpan.FromHours(23))
        {
            var discount = basePrice * 20 / 100;
            return basePrice - discount;
        }

        if (time >= TimeSpan.FromHours(6) && time < TimeSpan.FromHours(9))
        {
            var discount = basePrice * 10 / 100;
            return basePrice - discount;
        }

        return basePrice;
    }

    public decimal CalculateRoomPrice(
        decimal basePricePerHour,
        DateTime startAt,
        DateTime endAt)
    {
        decimal total = 0;
        var current = startAt;

        while (current < endAt)
        {
            var next = current.AddHours(1);

            if (next > endAt)
            {
                next = endAt;
            }

            var hours = (decimal)(next - current).TotalHours;
            var priceForOneHour = ApplyTimeBasedPrice(basePricePerHour, current.TimeOfDay);

            total += priceForOneHour * hours;
            current = next;
        }

        return total;
    }
}