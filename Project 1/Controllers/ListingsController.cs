using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project_1.Models;
using Project_1.Data.Services;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Project_1.Data;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.SignalR;
using Project_1.Hubs;

namespace Project_1.Controllers
{
    public class ListingsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly ILogger<ListingsController> _logger;
        private readonly IHubContext<BidHub> _hubContext;
        private readonly IListingsService _listingsService;
        private readonly IBidsService _bidsService;
        private readonly ICommentsService _commentsService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ListingsController(
            ApplicationDbContext context,
            UserManager<IdentityUser> userManager,
            IEmailSender emailSender,
            ILogger<ListingsController> logger,
            IListingsService listingsService,
            IWebHostEnvironment webHostEnvironment,
            IBidsService bidsService,
            ICommentsService commentsService,
            IHubContext<BidHub> hubContext)
        {
            _context = context;
            _userManager = userManager;
            _emailSender = emailSender;
            _logger = logger;
            _listingsService = listingsService;
            _webHostEnvironment = webHostEnvironment;
            _bidsService = bidsService;
            _commentsService = commentsService;
            _hubContext = hubContext;
        }

        // ✅ Automatically close expired listings
        private async Task CheckAndCloseExpiredListings()
        {
            var expiredListings = await _context.Listings
                .Where(l => !l.IsSold && l.ClosingTime <= DateTime.Now)
                .Include(l => l.Bids)
                    .ThenInclude(b => b.User)
                .ToListAsync();

            if (expiredListings.Any())
            {
                foreach (var listing in expiredListings)
                {
                    listing.IsSold = true;

                    var highestBid = listing.Bids?.OrderByDescending(b => b.Price).FirstOrDefault();
                    if (highestBid != null && highestBid.User != null)
                    {
                        string subject = "Congratulations! You have won the bid!";
                        string message = $"Dear {highestBid.User.UserName},\n\n" +
                                         $"You have won the bid for '{listing.Title}' with Rs {highestBid.Price:N2}.\n\n" +
                                         "Please contact us to finalize the transaction.\n\n" +
                                         "Best regards,\nAuction Team";

                        try
                        {
                            await _emailSender.SendEmailAsync(highestBid.User.Email, subject, message);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError("Error sending email: " + ex.Message);
                        }
                    }

                    await _hubContext.Clients.All.SendAsync("BidClosed", listing.Id);
                }

                await _context.SaveChangesAsync();
            }
        }

        // GET: Listings
        public async Task<IActionResult> Index(int? pageNumber, string searchString)
        {
            await CheckAndCloseExpiredListings();
            var listings = _listingsService.GetAll();
            int pageSize = 3;

            if (!string.IsNullOrEmpty(searchString))
            {
                listings = listings.Where(a => a.Title.Contains(searchString));
            }

            return View(await PaginatedList<Listing>.CreateAsync(
                listings.Where(l => !l.IsSold).AsNoTracking(),
                pageNumber ?? 1, pageSize));
        }

        public async Task<IActionResult> MyListings(int? pageNumber)
        {
            await CheckAndCloseExpiredListings();
            var listings = _listingsService.GetAll();
            int pageSize = 3;

            return View("Index", await PaginatedList<Listing>.CreateAsync(
                listings.Where(l => l.IdentityUserId == User.FindFirstValue(ClaimTypes.NameIdentifier))
                        .AsNoTracking(),
                pageNumber ?? 1, pageSize));
        }

        public async Task<IActionResult> MyBids(int? pageNumber)
        {
            await CheckAndCloseExpiredListings();
            var bids = _bidsService.GetAll();
            int pageSize = 3;

            return View(await PaginatedList<Bid>.CreateAsync(
                bids.Where(l => l.IdentityUserId == User.FindFirstValue(ClaimTypes.NameIdentifier))
                    .AsNoTracking(),
                pageNumber ?? 1, pageSize));
        }

        // GET: Listings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            await CheckAndCloseExpiredListings();

            if (id == null) return NotFound();

            var listing = await _listingsService.GetById(id);
            if (listing == null) return NotFound();

            return View(listing);
        }

        // GET: Listings/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Listings/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ListingVM listing)
        {
            if (listing.Image != null)
            {
                string uploadDir = Path.Combine(_webHostEnvironment.WebRootPath, "Images");
                string fileName = listing.Image.FileName;
                string filePath = Path.Combine(uploadDir, fileName);

                using var fileStream = new FileStream(filePath, FileMode.Create);
                await listing.Image.CopyToAsync(fileStream);

                var identityUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (identityUserId == null)
                    return Unauthorized();

                // 🕒 Combine days, hours, and minutes properly
                var totalDuration = TimeSpan.FromDays(listing.ClosingDays)
                                   + TimeSpan.FromHours(listing.ClosingHours)
                                   + TimeSpan.FromMinutes(listing.ClosingMinutes);

                // ✅ Validate total duration
                if (totalDuration.TotalMinutes <= 0)
                {
                    ModelState.AddModelError("", "Please set a valid auction duration.");
                    return View(listing);
                }

                var listObj = new Listing
                {
                    Title = listing.Title,
                    Description = listing.Description,
                    Price = listing.Price,
                    IdentityUserId = identityUserId,
                    ImagePath = fileName,

                    // ✅ Now includes days too
                    ClosingTime = DateTime.Now.Add(totalDuration),

                    IsSold = false
                };

                await _listingsService.Add(listObj);
                return RedirectToAction("Index");
            }

            return View(listing);
        }



        // POST: Add Bid
        [HttpPost]
        public async Task<ActionResult> AddBid([Bind("Id, Price, ListingId, IdentityUserId")] Bid bid)
        {
            if (ModelState.IsValid)
            {
                await _bidsService.Add(bid);

                var listing = await _listingsService.GetById(bid.ListingId);
                if (listing != null && !listing.IsSold)
                {
                    listing.Price = bid.Price;
                    await _listingsService.SaveChanges();

                    string formattedBid = bid.Price.ToString("N2");
                    await _hubContext.Clients.Group($"listing-{bid.ListingId}")
                        .SendAsync("ReceiveNewHighestBid", formattedBid);
                }
            }

            return RedirectToAction("Details", new { id = bid.ListingId });
        }

        // POST: Close Bidding
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CloseBidding(int id)
        {
            var listing = await _context.Listings.Include(l => l.Bids)
                    .ThenInclude(b => b.User)
                .FirstOrDefaultAsync(l => l.Id == id);

            if (listing == null) return NotFound();

            listing.IsSold = true;
            _context.Update(listing);

            var highestBid = listing.Bids?.OrderByDescending(b => b.Price).FirstOrDefault();
            if (highestBid != null && highestBid.User != null)
            {
                string subject = "Congratulations! You have won the bid!";
                string message = $"Dear {highestBid.User.UserName},\n\n" +
                                 $"Congratulations! You have won the bid with Rs {highestBid.Price:N2}.\n\n" +
                                 "Best regards,\nAuction Team";

                try
                {
                    await _emailSender.SendEmailAsync(highestBid.User.Email, subject, message);
                }
                catch (Exception ex)
                {
                    _logger.LogError("Error sending email: " + ex.Message);
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Details", new { id = listing.Id });
        }

        // POST: Delete Listing (Cancel without selling)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteListing(int id)
        {
            var listing = await _context.Listings
                .Include(l => l.Bids)
                .FirstOrDefaultAsync(l => l.Id == id);

            if (listing == null) return NotFound();

            // Only allow deleting if the listing is still open
            if (listing.IsSold) return BadRequest("Cannot delete a closed listing.");

            if (listing.Bids != null && listing.Bids.Any())
                _context.Bids.RemoveRange(listing.Bids);

            _context.Listings.Remove(listing);
            await _context.SaveChangesAsync();

            await _hubContext.Clients.All.SendAsync("ListingDeleted", id);

            return RedirectToAction("MyListings");
        }

        // POST: Delete an individual bid (before closing)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteBid(int id)
        {
            var bid = await _context.Bids
                .Include(b => b.Listing)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (bid == null) return NotFound();

            if (bid.Listing.IsSold)
                return BadRequest("Cannot delete a bid for a closed listing.");

            _context.Bids.Remove(bid);
            await _context.SaveChangesAsync();

            await _hubContext.Clients.Group($"listing-{bid.ListingId}")
                .SendAsync("BidDeleted", bid.Id);

            TempData["Success"] = "Bid deleted successfully.";
            return RedirectToAction("MyBids");
        }

        // POST: Add Comment
        [HttpPost]
        public async Task<ActionResult> AddComment([Bind("Id, Content, ListingId, IdentityUserId")] Comment comment)
        {
            if (ModelState.IsValid)
            {
                await _commentsService.Add(comment);
            }

            var listing = await _listingsService.GetById(comment.ListingId);
            return View("Details", listing);
        }

        // GET: Won Bids
        public async Task<IActionResult> WonBids()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var wonBids = await _bidsService.GetWonBidsAsync(userId);
            return View(wonBids);
        }
    }
}
