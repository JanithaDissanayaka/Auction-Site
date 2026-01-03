using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using Project_1.Data;
using Project_1.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System;
using Microsoft.AspNetCore.Identity;

namespace Project_1.Controllers
{
    public class PaymentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public PaymentController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpPost]
        public IActionResult CreateCheckoutSession(decimal amount, int listingId)
        {
            var domain = "https://localhost:44348";

            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                CustomerEmail = User.Identity.Name,
                LineItems = new List<SessionLineItemOptions>
                {
                    new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmountDecimal = amount * 100,
                            Currency = "lkr",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = "Auction Payment"
                            }
                        },
                        Quantity = 1
                    }
                },
                Mode = "payment",
                SuccessUrl = domain + $"/Payment/Success?listingId={listingId}&amount={amount}",
                CancelUrl = domain + "/Payment/Cancel"
            };

            var service = new SessionService();
            var session = service.Create(options);

            return Redirect(session.Url);
        }

        [HttpGet]
        public async Task<IActionResult> Success(int listingId, decimal amount)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                return Unauthorized(); // user must be logged in
            }

            var payment = new Payment
            {
                ListingId = listingId,
                Amount = amount,
                PaymentDate = DateTime.Now,
                UserId = userId // ✅ assign the current user's ID
            };
            _context.Payments.Add(payment);

            var highestBid = await _context.Bids
                                    .Where(b => b.ListingId == listingId)
                                    .OrderByDescending(b => b.Price)
                                    .FirstOrDefaultAsync();

            if (highestBid != null)
            {
                highestBid.IsPaid = true;
                _context.Bids.Update(highestBid);

                var listing = await _context.Listings.FindAsync(listingId);
                if (listing != null)
                {
                    listing.IsSold = true;
                    listing.WinningBidId = highestBid.Id;
                    _context.Listings.Update(listing);
                }
            }

            await _context.SaveChangesAsync();

            ViewBag.Message = "Payment successful! Your bid is now marked as paid.";
            return View(payment);
        }

        [HttpGet]
        public IActionResult Cancel()
        {
            ViewBag.Message = "Payment cancelled. You can try again.";
            return View();
        }
    }
}
