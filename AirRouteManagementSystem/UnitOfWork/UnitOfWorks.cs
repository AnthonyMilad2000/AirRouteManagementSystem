using AirRouteManagementSystem.Model.Customer;
using AirRouteManagementSystem.Repository.IRepository;
using System.Threading.Tasks;

namespace AirRouteManagementSystem.UnitOfWork
{
    public class UnitOfWorks : IUnitOfWork
    {
        private readonly ApplicationDBContext _dBContext;

        public IRepository<ApplicationUserOTP> ApplicationUserOTPrepository { get; }
        public IRepository<Cart> CartRepository { get; }

        public UnitOfWorks(ApplicationDBContext dBContext, IRepository<ApplicationUserOTP> ApplicationUserOTPrepository, IRepository<Cart> cartRepository)
        {
            _dBContext = dBContext;
            this.ApplicationUserOTPrepository = ApplicationUserOTPrepository;
            CartRepository=cartRepository;
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
