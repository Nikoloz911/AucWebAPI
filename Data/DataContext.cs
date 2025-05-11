using Microsoft.EntityFrameworkCore;
using AucWebAPI.Models;
using AucWebAPI.Enums;

namespace AucWebAPI.Data
{
    public class DataContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<Auction> Auctions { get; set; }
        public DbSet<Bid> Bids { get; set; }
        public DbSet<EmailVerification> EmailVerifications { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Data Source=LENOVO\SQLEXPRESS01;Initial Catalog=Auction;Integrated Security=True;Connect Timeout=30;Encrypt=True;Trust Server Certificate=True;Application Intent=ReadWrite;Multi Subnet Failover=False");
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
