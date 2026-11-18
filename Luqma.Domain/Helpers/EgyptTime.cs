using System;
using System.Runtime.InteropServices;

namespace Luqma.Domain.Helpers
{
    public static class EgyptTime
    {
        public static DateTime Now => DateTime.UtcNow;

        public static DateTime NowLocal
        {
            get
            {
                string tzId = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                    ? "Egypt Standard Time"
                    : "Africa/Cairo";
                TimeZoneInfo egyptTimeZone = TimeZoneInfo.FindSystemTimeZoneById(tzId);
                return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, egyptTimeZone);
            }
        }
    }
}
