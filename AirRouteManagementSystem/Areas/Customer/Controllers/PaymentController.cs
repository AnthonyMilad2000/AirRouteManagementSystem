using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AirRouteManagementSystem.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Route("[Area]/[controller]")]
    [ApiController]
    [Authorize]
    public class PaymentController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public PaymentController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        [HttpGet]
        [Route("Pay")]
        public async Task<IActionResult> Pay()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
                return Unauthorized();

            var carts = await _unitOfWork.CartRepository.GetAsync(c => c.UserId==userId,
                [V => V.Flight]);

            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>
                {

                },
                Mode = "payment",
                SuccessUrl = $"{Request.Scheme}://{Request.Host}/Customer/Booking/Create",
                CancelUrl = $"{Request.Scheme}://{Request.Host}/Customer/Booking/PaymentIssue",
            };



            foreach (var cart in carts)
            {
                options.LineItems.Add(new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = "EGP",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = cart.Flight.FlightNumber,
                        },
                        UnitAmount = (long)cart.Price,
                    },
                    Quantity = cart.Quantity,
                });
            }

            var service = new SessionService();
            var session = service.Create(options);
            return Ok(new { sessionId = session.Id });
        }
    }
}