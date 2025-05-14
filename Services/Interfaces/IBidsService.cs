using AucWebAPI.Core;
using AucWebAPI.DTOs.BidsDTOs;
namespace AucWebAPI.Services.Interfaces;

public interface IBidsService
{
    Task<ApiResponse<AddBidResponseDTO>> AddBidAsync(AddBidDTO bidDto);
    Task<ApiResponse<List<GetBidByIdDTO>>> GetUserBidsAsync(int userId);
}