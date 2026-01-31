using AirRouteManagementSystem.Repository.IRepository;

namespace AirRouteManagementSystem.UnitOfWork.Interface
{
    public interface IUnitOfWork
    {
        public IRepository<ApplicationUserOTP> ApplicationUserOTPrepository { get; }
        public Task Commit();
    }
}
