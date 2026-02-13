using AirRouteManagementSystem.DTOs.Customer.Request;
using AirRouteManagementSystem.DTOs.Customer.Response;
using AirRouteManagementSystem.DTOs.Response;
using AirRouteManagementSystem.Model.Customer;
using AirRouteManagementSystem.Repository.IRepository;
using AirRouteManagementSystem.UnitOfWork.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AirRouteManagementSystem.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Route("[area]/[controller]")]
    [ApiController]
    [Authorize]
    public class BookingController : ControllerBase
    {
        private readonly IRepository<Cart> _cartRepository;
        private readonly IRepository<Booking> _bookingRepository;
        private readonly IRepository<Promotion> _promotionRepository;
        private readonly IUnitOfWork _unitOfWork;

        public BookingController(

            IRepository<Cart> cartRepository,
            IRepository<Booking> bookingRepository,
            IRepository<Promotion> promotionRepository,
            IUnitOfWork unitOfWork
        )
        {
            _cartRepository = cartRepository;
            _bookingRepository = bookingRepository;
            _promotionRepository = promotionRepository;
            _unitOfWork = unitOfWork;
        }

        // POST: /Customer/Booking
        [HttpPost]
        public async Task<IActionResult> Create(
            CreateBookingRequest request,
            //string userId,
            CancellationToken cancellationToken
        )
        {
            var cartItem = await _cartRepository.GetOneAsync(
                //c => c.Id == request.CartId && c.UserId == userId,
                c => c.Id == request.CartId && c.UserId == "9fe9d12c-a5f2-465e-bddd-53db5fb9231b",

                Include: [c => c.Flight.FlightPrice!],
                cancellationToken: cancellationToken
            );

            if (cartItem is null)
                return NotFound("Cart not found");


            var flightPrice = cartItem.Flight.FlightPrice?.Min(fp => fp.Price) ?? 0;
            var subPrice = flightPrice * cartItem.Quantity;
            decimal discount = 0;

            if (!string.IsNullOrEmpty(request.PromotionCode))
            {
                var promo = await _promotionRepository.GetOneAsync(
                    p =>
                        p.PromotionCode == request.PromotionCode &&
                        p.Status == "Active" &&
                        p.ExpireDate > DateTime.UtcNow,
                    cancellationToken: cancellationToken
                );

                if (promo is not null && promo.UsedCount < promo.MaxUse)
                {
                    if (promo.DiscountType == "Percentage")
                        discount = subPrice * (promo.DiscountValue / 100);
                    else
                        discount = promo.DiscountValue;

                    promo.UsedCount++;
                    _promotionRepository.Update(promo);
                }
            }

            var booking = new Booking
            {
                //UserId = userId,
                UserId = "9fe9d12c-a5f2-465e-bddd-53db5fb9231b",
                FlightId = cartItem.FlightId,
                Quantity = cartItem.Quantity,
                SubPrice = subPrice,
                Discount = discount,
                FinalPrice = subPrice - discount,
                Status = "Pending"
            };

            await _bookingRepository.CreateAsync(booking, cancellationToken);

            _cartRepository.Remove(cartItem);

            await _unitOfWork.Commit();

            return Ok(new BookingResponse
            {
                BookingId = booking.Id,
                SubPrice = booking.SubPrice,
                Discount = booking.Discount,
                FinalPrice = booking.FinalPrice,
                Status = booking.Status
            });
        }


        [HttpPost("my")]
        public async Task<IActionResult> GetMyBookings(
            BookingFilterRequest request,
            string userId,
            CancellationToken cancellationToken
        )
        {
            var bookings = await _bookingRepository.GetAsync(
                b =>
                    b.UserId == userId &&
                    (string.IsNullOrEmpty(request.Status) || b.Status == request.Status) &&
                    (!request.FromDate.HasValue || b.CreatedAt >= request.FromDate) &&
                    (!request.ToDate.HasValue || b.CreatedAt <= request.ToDate),
                tracking: false,
                cancellationToken: cancellationToken
            );

            var totalCount = bookings.Count();

            var data = bookings
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(b => new BookingResponse
                {
                    BookingId = b.Id,
                    SubPrice = b.SubPrice,
                    Discount = b.Discount,
                    FinalPrice = b.FinalPrice,
                    Status = b.Status
                })
                .ToList();

            return Ok(new PagedResponse<BookingResponse>
            {
                TotalCount = totalCount,
                Page = request.Page,
                PageSize = request.PageSize,
                Data = data
            });
        }


        // POST: /Customer/Booking/{id}/cancel
        [HttpPost("{id}/cancel")]
        public async Task<IActionResult> CancelBooking(
            int id,
            string userId,
            CancellationToken cancellationToken
        )
        {
            var booking = await _bookingRepository.GetOneAsync(
                b => b.Id == id && b.UserId == userId,
                cancellationToken: cancellationToken        
            );

            if (booking is null)
                return NotFound("Booking not found");

            // مسموح بالإلغاء فقط لو لسه Pending
            if (booking.Status != "Pending")
                return BadRequest("Booking cannot be cancelled at this stage");

            booking.Status = "Cancelled";

            _bookingRepository.Update(booking);
            await _unitOfWork.Commit();

            return Ok(new
            {
                Message = "Booking cancelled successfully",
                BookingId = booking.Id,
                Status = booking.Status
            });
        }

        [HttpGet]
        public async Task<IActionResult> Get(int page = 1, int pageSize = 10, string? search = null, CancellationToken cancellationToken = default)
        {
            var bookings = await _bookingRepository.GetAsync(Include: [e=>e.Flight, e=>e.Flight.FlightPrice, e=>e.User],tracking: false, cancellationToken: cancellationToken);

            var query = bookings.AsQueryable();

            if (bookings is null)
                return NotFound(new ErrorModel
                {
                    Code = "Not Found",
                    Description = "No Booking Found",
                });

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(a => a.Flight.FlightNumber.Contains(search));
            }

            var total = query.Count();

            var data = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var result = new PagedResponse<Booking>
            {
                Data = data,
                Page = page,
                PageSize = pageSize,
                TotalCount = total
            };

            return Ok(result);
        }

        [HttpGet("PaymentIssue")]
        public IActionResult PaymentIssue()
        {
            return Ok(new ErrorModel
            {
                Code = "Payment Issue",
                Description = "Payment Not success",
            });
        }

    }
}
