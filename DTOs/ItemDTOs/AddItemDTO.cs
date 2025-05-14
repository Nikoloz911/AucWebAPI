namespace AucWebAPI.DTOs.ItemDTOs;
public class AddItemDTO
{
    public string Title { get; set; }
    public int SellerId { get; set; }
    public string Description { get; set; }
    public List<string> ImageUrls { get; set; }
    public string Category { get; set; }
    public string Condition { get; set; }
}
