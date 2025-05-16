using AucWebAPI.Core;
using AucWebAPI.DTOs.AuctionDTOs;

namespace AucWebAPI.Services.Interfaces;
public interface IAuctionService
{
    Task<ApiResponse<AddAuctionResponseDTO>> AddAuctionAsync(AddAuctionDTO dto);
    Task<ApiResponse<List<GetAllAuctionsDTO>>> GetFilteredAuctionsAsync(GetAuctionsFilterDTO filter);
    Task<ApiResponse<GetAuctionsByIdDTO>> GetAuctionByIdAsync(int id);
    Task<ApiResponse<UpdateAuctionResponseDTO>> UpdateAuctionAsync(int id, UpdateAuctionDTO dto);
    Task<ApiResponse<string>> DeleteAuctionAsync(int id);
    Task<ApiResponse<List<GetActiveAuctionsDTO>>> GetActiveAuctionsAsync();
    Task<ApiResponse<List<GetUsersAuctionsDTO>>> GetAuctionsByUserIdAsync(int userId);
    Task<ApiResponse<List<GetBidDTO>>> GetBidsByAuctionIdAsync(int auctionId);
}
