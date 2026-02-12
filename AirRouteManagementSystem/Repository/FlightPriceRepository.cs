using AirRouteManagementSystem.Repository.IRepository;

namespace AirRouteManagementSystem.Repository
{
    public class FlightPriceRepository : Repository<FlightPrice>, IFlightPriceRepository
    {
        private ApplicationDBContext _context;

        public FlightPriceRepository(ApplicationDBContext dBContext) : base(dBContext)
        {
            _context = dBContext;
        }

        public void RemoveRange(IEnumerable<FlightPrice> flightPrices)
        {
            _context.RemoveRange(flightPrices);
        }
    }
}
