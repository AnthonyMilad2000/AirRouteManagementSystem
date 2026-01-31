using AirRouteManagementSystem.Model;
using AirRouteManagementSystem.Repository.IRepository;

namespace AirRouteManagementSystem.Repository
{
    public class AircraftSubImageRepository : Repository<AirCraftSubImg>, IAircraftSubImagesRepository
    {
        private ApplicationDBContext _context;

        public AircraftSubImageRepository(ApplicationDBContext context) : base(context)
        {
            _context = context;
        }
      
        public void RemoveRange(IEnumerable<AirCraftSubImg> airCrafts)
        {
            _context.RemoveRange(airCrafts);
        }
    }
}
