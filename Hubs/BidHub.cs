using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace Project1.Hubs
{
    public class BidHub : Hub
    {
        // This method will be called from your backend (e.g., your Bid controller)
        // to broadcast the new highest bid to all connected clients.
        public async Task UpdateHighestBid(int listingId, string newHighestBidAmount)
        {
            // 'Groups.AddToGroupAsync' is great for this, but for simplicity, 
            // we'll broadcast to a group of clients interested in a specific listing.
            await Clients.Group($"listing-{listingId}").SendAsync("ReceiveNewHighestBid", newHighestBidAmount);
        }

        // This method is called by the client when they connect to a specific listing page.
        public async Task JoinListingGroup(int listingId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"listing-{listingId}");
        }

        // You might also want a method to remove them when they leave.
        public async Task LeaveListingGroup(int listingId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"listing-{listingId}");
        }
    }
}