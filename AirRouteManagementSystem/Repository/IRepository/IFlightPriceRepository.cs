namespace AirRouteManagementSystem.Repository.IRepository
{
    public interface IFlightPriceRepository: IRepository<FlightPrice>
    {
        void RemoveRange(IEnumerable<FlightPrice> flightPrices);
    }
   
}
