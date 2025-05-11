namespace AucWebAPI.Models;
public class Bid
{
    public int Id { get; set; }
    public int AuctionId { get; set; } 
    public int BidderId { get; set; } 
    public decimal Amount { get; set; }
    public DateTime BidTime { get; set; }
    public bool IsWinning { get; set; }
    public Auction Auction { get; set; } 
    public User Bidder { get; set; }
}
