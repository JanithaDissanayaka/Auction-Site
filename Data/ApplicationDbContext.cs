using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Project_1.Models;

namespace Project_1.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Listing> Listings { get; set; }
        public DbSet<Bid> Bids { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Payment> Payments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Cascade delete Bids when a Listing is deleted
            modelBuilder.Entity<Bid>()
                .HasOne(b => b.Listing)
                .WithMany(l => l.Bids)
                .HasForeignKey(b => b.ListingId)
                .OnDelete(DeleteBehavior.Cascade);

            // Cascade delete Comments when a Listing is deleted
            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Listing)
                .WithMany(l => l.Comments)
                .HasForeignKey(c => c.ListingId)
                .OnDelete(DeleteBehavior.Cascade);

            // Optional: Cascade delete Payments when a Listing is deleted
            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Listing)
                .WithMany()
                .HasForeignKey(p => p.ListingId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
