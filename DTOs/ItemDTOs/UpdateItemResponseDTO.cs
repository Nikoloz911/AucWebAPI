using AucWebAPI.Enums;
namespace AucWebAPI.DTOs.ItemDTOs;
public class UpdateItemResponseDTO
{
    public int Id { get; set; }
    public string Title { get; set; }
    public int SellerId { get; set; }
    public string Description { get; set; }
    public List<string> ImageUrls { get; set; }
    public string Category { get; set; }
    public ITEM_CONDITION Condition { get; set; }
    public DateTime CreatedAt { get; set; }
}
