using AucWebAPI.Enums;
namespace AucWebAPI.DTOs.AuctionDTOs;
public class GetAllAuctionsDTO
{
    public int Id { get; set; }
    public decimal StartPrice { get; set; }
    public decimal CurrentPrice { get; set; }
    public AUCTION_STATUS Status { get; set; }
    public int? WinnerId { get; set; }
    public decimal MinimumBidIncrement { get; set; }
}
