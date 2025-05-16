using AucWebAPI.Enums;
namespace AucWebAPI.DTOs.AuctionDTOs;
public class UpdateAuctionResponseDTO
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
}
