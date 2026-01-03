using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.SignalR;
using Project_1.Data;
using Project_1.Hubs;
using Project_1.Models;

namespace Project_1.Services
{
    public class BidExpirationService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<BidExpirationService> _logger;
        private readonly IHubContext<BidHub> _hubContext;

        public BidExpirationService(
            IServiceProvider serviceProvider,
            ILogger<BidExpirationService> logger,
            IHubContext<BidHub> hubContext)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _hubContext = hubContext;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("✅ BidExpirationService started and running.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var _context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                        // Find listings that are not sold and have expired closing times
                        var expiredListings = await _context.Listings
                            .Include(l => l.Bids)
                                .ThenInclude(b => b.User)
                            .Where(l => !l.IsSold && l.ClosingTime <= DateTime.Now)
                            .ToListAsync(stoppingToken);

                        foreach (var listing in expiredListings)
                        {
                            listing.IsSold = true;
                            _context.Update(listing);

                            var highestBid = listing.Bids?
                                .OrderByDescending(b => b.Price)
                                .FirstOrDefault();

                            if (highestBid != null)
                            {
                                _logger.LogInformation($"🏁 Listing '{listing.Title}' ended. Winner: {highestBid.User?.UserName} (${highestBid.Price}).");

                                // Notify all connected clients via SignalR
                                await _hubContext.Clients
                                    .Group($"listing-{listing.Id}")
                                    .SendAsync("AuctionEnded", new
                                    {
                                        listingId = listing.Id,
                                        winner = highestBid.User?.UserName,
                                        finalPrice = highestBid.Price
                                    });

                                // Optionally send email here if needed
                            }
                            else
                            {
                                _logger.LogInformation($"⚠️ Listing '{listing.Title}' ended with no bids.");
                            }
                        }

                        if (expiredListings.Any())
                            await _context.SaveChangesAsync(stoppingToken);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"❌ Error in BidExpirationService: {ex.Message}");
                }

                // Run every 1 minute
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }

            _logger.LogInformation("⏹️ BidExpirationService stopped.");
        }
    }
}
