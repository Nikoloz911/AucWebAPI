namespace AucWebAPI.DTOs.AuctionDTOs;
public class AddAuctionDTO
{
    public int ItemId { get; set; }
    public decimal StartPrice { get; set; }
    public decimal CurrentPrice { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string Status { get; set; }
    public int? WinnerId { get; set; }
    public decimal MinimumBidIncrement { get; set; }
}
