using AucWebAPI.Core;
using AucWebAPI.DTOs.ItemDTOs;
namespace AucWebAPI.Services.Interfaces;
public interface IItemService
{
    Task<ApiResponse<AddItemResponseDTO>> AddItemAsync(AddItemDTO dto);
    Task<ApiResponse<List<AddItemResponseDTO>>> GetAllItemsAsync(string? category = null, string? condition = null);
    Task<ApiResponse<AddItemResponseDTO>> GetItemByIdAsync(int id);
    Task<ApiResponse<UpdateItemResponseDTO>> UpdateItemAsync(int id, UpdateItemDTO dto);
    Task<ApiResponse<string>> DeleteItemAsync(int id);
}
