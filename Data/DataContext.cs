using Microsoft.EntityFrameworkCore;
using AucWebAPI.Models;

namespace AucWebAPI.Data
{
    public class DataContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<Auction> Auctions { get; set; }
        public DbSet<Bid> Bids { get; set; }
        public DbSet<EmailVerification> EmailVerifications { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)   /// MY AWS RDS CONNECTION STRING
        {
            optionsBuilder.UseSqlServer(@"Server=database-auction.c3sawaum20mz.eu-north-1.rds.amazonaws.com,3306;Database=database-1;User ID=admin;Password=ItsFreeBro1%;Encrypt=True;TrustServerCertificate=True;Connection Timeout=30;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Item>()
                .HasOne(i => i.Seller)
                .WithMany()
                .HasForeignKey(i => i.SellerId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<EmailVerification>()
                .HasOne(ev => ev.User)
                .WithMany()
                .HasForeignKey(ev => ev.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Auction>()
                .HasOne(a => a.Item)
                .WithMany()
                .HasForeignKey(a => a.ItemId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Auction>()
                .HasOne(a => a.Winner)
                .WithMany()
                .HasForeignKey(a => a.WinnerId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Bid>()
                .HasOne(b => b.Auction)
                .WithMany()
                .HasForeignKey(b => b.AuctionId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Bid>()
                .HasOne(b => b.Bidder)
                .WithMany()
                .HasForeignKey(b => b.BidderId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
