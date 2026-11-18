namespace Luqma.Domain.Helpers
{
    public static class WorkingHours
    {
        // Returns true when the given local time falls inside the working window.
        // A window whose start equals its end means the shop works around the clock,
        // and a window that ends before it starts crosses midnight (e.g. 10:00 -> 02:00).
        public static bool IsOpen(TimeOnly openTime, TimeOnly closeTime, TimeOnly time)
        {
            if (openTime == closeTime)
                return true;

            if (openTime < closeTime)
                return time >= openTime && time < closeTime;

            return time >= openTime || time < closeTime;
        }
    }
}
