namespace Luqma.Domain.Helpers
{
    public static class GeoDistance
    {
        private const double EarthRadiusKm = 6371.0;

        // Returns the distance in kilometers between two map points, or null when either
        // point is not set (0,0). The result is rounded to 2 decimals for display.
        public static double? Kilometers(double fromLatitude, double fromLongitude, double toLatitude, double toLongitude)
        {
            if (!IsSet(fromLatitude, fromLongitude) || !IsSet(toLatitude, toLongitude))
                return null;

            var fromLatRadians = DegreesToRadians(fromLatitude);
            var toLatRadians = DegreesToRadians(toLatitude);
            var latitudeDelta = DegreesToRadians(toLatitude - fromLatitude);
            var longitudeDelta = DegreesToRadians(toLongitude - fromLongitude);

            var a = Math.Sin(latitudeDelta / 2) * Math.Sin(latitudeDelta / 2)
                    + Math.Cos(fromLatRadians) * Math.Cos(toLatRadians)
                      * Math.Sin(longitudeDelta / 2) * Math.Sin(longitudeDelta / 2);

            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return Math.Round(EarthRadiusKm * c, 2);
        }

        public static bool IsSet(double latitude, double longitude)
        {
            if (latitude == 0 && longitude == 0)
                return false;

            return latitude is >= -90 and <= 90 && longitude is >= -180 and <= 180;
        }

        private static double DegreesToRadians(double degrees) => degrees * Math.PI / 180.0;
    }
}
