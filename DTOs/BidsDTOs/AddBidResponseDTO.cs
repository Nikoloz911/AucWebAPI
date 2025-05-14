namespace AucWebAPI.DTOs.BidsDTOs;
public class AddBidResponseDTO
{
    public int AuctionId { get; set; }
    public int BidderId { get; set; }
    public decimal Amount { get; set; }
    public DateTime BidTime { get; set; }
    public bool IsWinning { get; set; }
}
