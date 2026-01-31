using AirRouteManagementSystem.Repository.IRepository;
using System.Threading.Tasks;

namespace AirRouteManagementSystem.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDBContext _dBContext;

        public IRepository<ApplicationUserOTP> ApplicationUserOTPrepository { get; }

        public UnitOfWork(ApplicationDBContext dBContext, IRepository<ApplicationUserOTP> ApplicationUserOTPrepository)
        {
            _dBContext = dBContext;
            this.ApplicationUserOTPrepository = ApplicationUserOTPrepository;
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
