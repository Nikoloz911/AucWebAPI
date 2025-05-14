namespace AucWebAPI.DTOs.BidsDTOs;
public class AddBidDTO
{
    public int AuctionId { get; set; }
    public int BidderId { get; set; }
    public decimal Amount { get; set; }
    public bool IsWinning { get; set; }
}
