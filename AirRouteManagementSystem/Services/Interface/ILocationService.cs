namespace AirRouteManagementSystem.Services
{
    public interface ILocationService
    {
        (double lat, double lng)? ExtractCoordinates(string url);
        double CalculateDistance(double lat1, double lon1, double lat2, double lon2);
    }
}
