using AucWebAPI.Enums;

namespace AucWebAPI.Models;
public class Auction
{
    public int Id { get; set; }
    public int ItemId { get; set; }
    public decimal StartPrice { get; set; }
    public decimal CurrentPrice { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public AUCTION_STATUS Status { get; set; }
    public int? WinnerId { get; set; }   
    public decimal MinimumBidIncrement { get; set; }
    public User Winner { get; set; }  
    public Item Item { get; set; }
}
