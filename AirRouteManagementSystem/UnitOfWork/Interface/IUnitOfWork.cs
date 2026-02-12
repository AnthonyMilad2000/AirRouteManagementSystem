using AirRouteManagementSystem.Model.Customer;
using AirRouteManagementSystem.Repository.IRepository;

namespace AirRouteManagementSystem.UnitOfWork.Interface
{
    public interface IUnitOfWork
    {
        public IRepository<ApplicationUserOTP> ApplicationUserOTPrepository { get; }

        public IRepository<Cart> CartRepository { get; }

        public Task Commit();
    }
}
