using AirRouteManagementSystem.DTOs.Customer.Request;
using AirRouteManagementSystem.DTOs.Customer.Response;
using AirRouteManagementSystem.Model;
using AirRouteManagementSystem.Model.Customer;
using AirRouteManagementSystem.Repository.IRepository;
using AirRouteManagementSystem.UnitOfWork.Interface;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AirRouteManagementSystem.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Route("[area]/[controller]")]
    [ApiController]
    [Authorize]
    public class CartController : ControllerBase
    {
        private readonly IRepository<Cart> _cartRepository;
        private readonly IRepository<Flight> _flightRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CartController(
            IRepository<Cart> cartRepository,
            IRepository<Flight> flightRepository,
            IUnitOfWork unitOfWork
        )
        {
            _cartRepository = cartRepository;
            _flightRepository = flightRepository;
            _unitOfWork = unitOfWork;
        }

        // GET: /Customer/Cart/{userId}
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetCart(
            string userId,
            CancellationToken cancellationToken
        )
        {
            var cartItems = await _cartRepository.GetAsync(
                c => c.UserId == userId,
                Include: [c => c.Flight],
                tracking: false,
                cancellationToken: cancellationToken
            );

            var response = cartItems.Select(c => new CartResponse
            {
                CartId = c.Id,
                FlightId = c.FlightId,
                FlightNumber = c.Flight.FlightNumber,
                Price = c.Flight.FlightPrice!.Min(e=>e.Price),
                Quantity = c.Quantity
            });

            return Ok(response);
        }

        // POST: /Customer/Cart
        [HttpPost]
        public async Task<IActionResult> AddToCart( AddToCartRequest request, //string userId,
                                                                              CancellationToken cancellationToken
        )
        {
            var flight = await _flightRepository.GetOneAsync(
                f => f.Id == request.FlightId,
                cancellationToken: cancellationToken
            );

            if (flight is null)
                return NotFound("Flight not found");

            var cartItem = new Cart
            {
                //UserId = userId,
                UserId = "9fe9d12c-a5f2-465e-bddd-53db5fb9231b",
                FlightId = request.FlightId,
                Quantity = request.Quantity,
                Price = request.Price
            };

            await _cartRepository.CreateAsync(cartItem, cancellationToken);
            await _unitOfWork.Commit();

            return Ok("Added to cart");
        }

        // DELETE: /Customer/Cart/{cartId}
        [HttpDelete("{cartId}")]
        public async Task<IActionResult> RemoveFromCart(
            int cartId,
            CancellationToken cancellationToken
        )
        {
            var cartItem = await _cartRepository.GetOneAsync(
                c => c.Id == cartId,
                cancellationToken: cancellationToken
            );

            if (cartItem is null)
                return NotFound("Cart item not found");

            _cartRepository.Remove(cartItem);
            await _unitOfWork.Commit();

            return NoContent();
        }
    }
}
