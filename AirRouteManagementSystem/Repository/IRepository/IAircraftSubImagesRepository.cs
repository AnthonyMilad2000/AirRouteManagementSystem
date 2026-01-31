using AirRouteManagementSystem.Model;

namespace AirRouteManagementSystem.Repository.IRepository
{
    public interface IAircraftSubImagesRepository : IRepository<AirCraftSubImg>
    {
        void RemoveRange(IEnumerable<AirCraftSubImg> airCrafts );
    }
}
