using System.Globalization;

namespace AirRouteManagementSystem.Services
{
    public class LocationService : ILocationService
    {
        public (double lat, double lng)? ExtractCoordinates(string url)
        {
            try
            {
                // أولًا: حاول نقرأ بعد @
                var atIndex = url.IndexOf("@");
                if (atIndex != -1)
                {
                    var coordsPart = url.Substring(atIndex + 1);
                    var parts = coordsPart.Split(',');
                    if (parts.Length >= 2)
                    {
                        double lat = double.Parse(parts[0], CultureInfo.InvariantCulture);
                        double lng = double.Parse(parts[1], CultureInfo.InvariantCulture);
                        return (lat, lng);
                    }
                }

                // لو ما حصلش، حاول نقرأ بعد !3d و !4d
                var latIndex = url.IndexOf("!3d");
                var lngIndex = url.IndexOf("!4d");
                if (latIndex != -1 && lngIndex != -1)
                {
                    var latStr = url.Substring(latIndex + 3, lngIndex - (latIndex + 3));
                    var lngStr = url.Substring(lngIndex + 3);
                    double lat = double.Parse(latStr, CultureInfo.InvariantCulture);
                    double lng = double.Parse(lngStr, CultureInfo.InvariantCulture);
                    return (lat, lng);
                }

                return null;
            }
            catch
            {
                return null;
            }
        }
        
        // Haversine Formula
        public double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            var R = 6371; // Earth radius in KM

            var dLat = ToRadians(lat2 - lat1);
            var dLon = ToRadians(lon2 - lon1);

            var a =
                Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(ToRadians(lat1)) *
                Math.Cos(ToRadians(lat2)) *
                Math.Sin(dLon / 2) *
                Math.Sin(dLon / 2);

            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return R * c;
        }

        private double ToRadians(double angle)
        {
            return angle * Math.PI / 180;
        }
    }
}
