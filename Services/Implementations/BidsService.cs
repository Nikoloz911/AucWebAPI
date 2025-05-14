using AucWebAPI.Core;
using AucWebAPI.Data;
using AucWebAPI.DTOs.BidsDTOs;
using AucWebAPI.Models;
using AucWebAPI.Services.Interfaces;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace AucWebAPI.Services.Implementations;

public class BidsService : IBidsService
{
    private readonly DataContext _context;
    private readonly IMapper _mapper;

    public BidsService(DataContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ApiResponse<AddBidResponseDTO>> AddBidAsync(AddBidDTO bidDto)
    {
            var auction = await _context.Auctions.FindAsync(bidDto.AuctionId);
            if (auction == null)
                return new ApiResponse<AddBidResponseDTO> 
                {
                    Status = 404,
                    Message = "Auction not found",
                    Data = null
                };

            var bidder = await _context.Users.FindAsync(bidDto.BidderId);
            if (bidder == null)
                return new ApiResponse<AddBidResponseDTO> 
                { 
                    Status = 404, 
                    Message = "Bidder not found",
                    Data = null
                };

            var bid = _mapper.Map<Bid>(bidDto);
            bid.BidTime = DateTime.UtcNow;

            if (bidDto.IsWinning)
            {
                var previousWinningBids = await _context.Bids
                    .Where(b => b.AuctionId == bidDto.AuctionId && b.IsWinning)
                    .ToListAsync();

                foreach (var previousBid in previousWinningBids)
                {
                    previousBid.IsWinning = false;
                }
            }

            await _context.Bids.AddAsync(bid);
            await _context.SaveChangesAsync();
            var responseDto = _mapper.Map<AddBidResponseDTO>(bid);
            return new ApiResponse<AddBidResponseDTO>
            {
                Status = 200,
                Message = "Bid added successfully",
                Data = responseDto
            };  
    }

    public async Task<ApiResponse<List<GetBidByIdDTO>>> GetUserBidsAsync(int userId)
    {
      
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return new ApiResponse<List<GetBidByIdDTO>> 
                {
                    Status = 404,
                    Message = "User not found",
                    Data = null
                };

            var bids = await _context.Bids
                .Where(b => b.BidderId == userId)
                .OrderByDescending(b => b.BidTime)
                .ToListAsync();

            var bidDtos = _mapper.Map<List<GetBidByIdDTO>>(bids);

            return new ApiResponse<List<GetBidByIdDTO>>
            {
                Status = 200,
                Message = "User bids retrieved successfully",
                Data = bidDtos
            };    
    }
}