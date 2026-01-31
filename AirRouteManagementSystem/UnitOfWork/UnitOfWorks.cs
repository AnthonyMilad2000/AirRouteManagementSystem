using System.Threading.Tasks;

namespace AirRouteManagementSystem.UnitOfWork
{
    public class UnitOfWorks : IUnitOfWork
    {
        private readonly ApplicationDBContext _dBContext;

        public UnitOfWorks(ApplicationDBContext dBContext)
        {
            _dBContext = dBContext;
        }

        public void Dispose()
        {
            _dBContext.Dispose();
        }
        public async Task Commit()
        {
            await _dBContext.SaveChangesAsync();
        }

    }
}
