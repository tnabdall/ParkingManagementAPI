public static class RateCalculator
{
    public static double CalculateParkingFee(DateTime startTime, DateTime endTime, double ratePerMinute, double ratePerFiveMinutes)
    {
        if (endTime < startTime)
        {
            throw new ArgumentException("End time cannot be earlier than start time.");
        }
        if (ratePerMinute < 0)
        {
            throw new ArgumentException("Rate per minute cannot be negative.");
        }

        var durationMinutes = Math.Max((endTime - startTime).TotalMinutes, 0);
        var baseCharge = durationMinutes * ratePerMinute;
        var additionalCharge = Math.Floor(durationMinutes / 5) * ratePerFiveMinutes;
        var totalCharge = baseCharge + additionalCharge;
        // Round total charge to 2 decimal places
        totalCharge = Math.Round(totalCharge, 2);
        return totalCharge;
    }
}