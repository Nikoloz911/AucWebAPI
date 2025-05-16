using AucWebAPI.Core;
using AucWebAPI.Data;
using AucWebAPI.DTOs.AuctionDTOs;
using AucWebAPI.Enums;
using AucWebAPI.Models;
using AucWebAPI.Services.Interfaces;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace AucWebAPI.Services.Implementations;
public class AuctionService : IAuctionService
{
    private readonly DataContext _context;
    private readonly IMapper _mapper;
    public AuctionService(DataContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    // ADD AUCTION
    public async Task<ApiResponse<AddAuctionResponseDTO>> AddAuctionAsync(AddAuctionDTO dto)
    {
        if (dto.EndTime <= dto.StartTime)
        {
            return new ApiResponse<AddAuctionResponseDTO>
            {
                Status = 400,
                Message = "End time must be after start time.",
                Data = null!
            };
        }
        if (!Enum.TryParse<AUCTION_STATUS>(dto.Status, true, out var parsedStatus))
        {
            return new ApiResponse<AddAuctionResponseDTO>
            {
                Status = 400,
                Message = $"Invalid auction status: {dto.Status}",
                Data = null!
            };
        }
        var itemExists = await _context.Items.AnyAsync(i => i.Id == dto.ItemId);
        if (!itemExists)
        {
            return new ApiResponse<AddAuctionResponseDTO>
            {
                Status = 404,
                Message = $"Item with ID {dto.ItemId} not found.",
                Data = null!
            };
        }
        if (dto.WinnerId.HasValue)
        {
            if (dto.WinnerId.Value == 0)
            {
                return new ApiResponse<AddAuctionResponseDTO>
                {
                    Status = 400,
                    Message = "WinnerId cannot be 0.",
                    Data = null!
                };
            }

            var winnerExists = await _context.Users.AnyAsync(u => u.Id == dto.WinnerId.Value);
            if (!winnerExists)
            {
                return new ApiResponse<AddAuctionResponseDTO>
                {
                    Status = 404,
                    Message = $"User with ID {dto.WinnerId.Value} not found.",
                    Data = null!
                };
            }
        }
        var auction = _mapper.Map<Auction>(dto);
        auction.Status = parsedStatus;

        _context.Auctions.Add(auction);
        await _context.SaveChangesAsync();

        var responseDto = _mapper.Map<AddAuctionResponseDTO>(auction);
        return new ApiResponse<AddAuctionResponseDTO>
        {
            Status = 200,
            Message = "Auction created successfully.",
            Data = responseDto
        };
    }
    // GET AUCTIONS WITH FILTER
    public async Task<ApiResponse<List<GetAllAuctionsDTO>>> GetFilteredAuctionsAsync(GetAuctionsFilterDTO filter)
    {
        IQueryable<Auction> query = _context.Auctions;

        if (filter.Id.HasValue)
            query = query.Where(a => a.Id == filter.Id.Value);

        if (filter.StartPrice.HasValue)
            query = query.Where(a => a.StartPrice >= filter.StartPrice.Value);

        if (filter.CurrentPrice.HasValue)
            query = query.Where(a => a.CurrentPrice >= filter.CurrentPrice.Value);

        if (filter.Status.HasValue)
            query = query.Where(a => a.Status == filter.Status.Value);

        if (filter.WinnerId.HasValue)
            query = query.Where(a => a.WinnerId == filter.WinnerId.Value);

        if (filter.MinimumBidIncrement.HasValue)
            query = query.Where(a => a.MinimumBidIncrement == filter.MinimumBidIncrement.Value);

        var auctions = await query.ToListAsync();

        if (!auctions.Any())
        {
            return new ApiResponse<List<GetAllAuctionsDTO>>
            {
                Status = 404,
                Message = "No auctions found matching the filter criteria.",
                Data = null!
            };
        }

        var dtos = _mapper.Map<List<GetAllAuctionsDTO>>(auctions);
        return new ApiResponse<List<GetAllAuctionsDTO>>
        {
            Status = 200,
            Message = "Filtered auctions retrieved successfully.",
            Data = dtos
        };
    }
    // GET AUCTION BY ID
    public async Task<ApiResponse<GetAuctionsByIdDTO>> GetAuctionByIdAsync(int id)
    {
        var auction = await _context.Auctions.FindAsync(id);
        if (auction == null)
        {
            return new ApiResponse<GetAuctionsByIdDTO>
            {
                Status = 404,
                Message = $"Auction with ID {id} not found.",
                Data = null!
            };
        }
        var dto = _mapper.Map<GetAuctionsByIdDTO>(auction);
        return new ApiResponse<GetAuctionsByIdDTO>
        {
            Status = 200,
            Message = "Auction retrieved successfully.",
            Data = dto
        };
    }
    // UPDATE AUCTION BY ID 

    public async Task<ApiResponse<UpdateAuctionResponseDTO>> UpdateAuctionAsync(int id, UpdateAuctionDTO dto)
    {
        var auction = await _context.Auctions.FindAsync(id);
        if (auction == null)
        {
            return new ApiResponse<UpdateAuctionResponseDTO>
            {
                Status = 404,
                Message = $"Auction with ID {id} not found.",
                Data = null!
            };
        }
        if (dto.EndTime <= dto.StartTime)
        {
            return new ApiResponse<UpdateAuctionResponseDTO>
            {
                Status = 400,
                Message = "End time must be after start time.",
                Data = null!
            };
        }
        if (!Enum.TryParse<AUCTION_STATUS>(dto.Status, true, out var parsedStatus))
        {
            return new ApiResponse<UpdateAuctionResponseDTO>
            {
                Status = 400,
                Message = $"Invalid auction status: {dto.Status}",
                Data = null!
            };
        }
        var itemExists = await _context.Items.AnyAsync(i => i.Id == dto.ItemId);
        if (!itemExists)
        {
            return new ApiResponse<UpdateAuctionResponseDTO>
            {
                Status = 404,
                Message = $"Item with ID {dto.ItemId} not found.",
                Data = null!
            };
        }
        if (dto.WinnerId.HasValue && dto.WinnerId.Value == 0)
        {
            return new ApiResponse<UpdateAuctionResponseDTO>
            {
                Status = 400,
                Message = "WinnerId cannot be 0.",
                Data = null!
            };
        }
        _mapper.Map(dto, auction); 
        auction.Status = parsedStatus;
        await _context.SaveChangesAsync();
        var responseDto = _mapper.Map<UpdateAuctionResponseDTO>(auction);
        return new ApiResponse<UpdateAuctionResponseDTO>
        {
            Status = 200,
            Message = "Auction updated successfully.",
            Data = responseDto
        };
    }
    // DELETE AUCTION BY ID
    public async Task<ApiResponse<string>> DeleteAuctionAsync(int id)
    {
        var auction = await _context.Auctions.FindAsync(id);
        if (auction == null)
        {
            return new ApiResponse<string>
            {
                Status = 404,
                Message = $"Auction with ID {id} not found.",
                Data = null!
            };
        }
        _context.Auctions.Remove(auction);
        await _context.SaveChangesAsync();
        return new ApiResponse<string>
        {
            Status = 200,
            Message = "Auction deleted successfully.",
            Data = $"Auction with ID {id} deleted."
        };
    }
    // GET ACTIVE AUCTIONS
    public async Task<ApiResponse<List<GetActiveAuctionsDTO>>> GetActiveAuctionsAsync()
    {
        var activeAuctions = await _context.Auctions
            .Where(a => a.Status == AUCTION_STATUS.Active)
            .ToListAsync();

        if (!activeAuctions.Any())
        {
            return new ApiResponse<List<GetActiveAuctionsDTO>>
            {
                Status = 404,
                Message = "No active auctions found.",
                Data = null!
            };
        }

        var dtos = _mapper.Map<List<GetActiveAuctionsDTO>>(activeAuctions);
        return new ApiResponse<List<GetActiveAuctionsDTO>>
        {
            Status = 200,
            Message = "Active auctions retrieved successfully.",
            Data = dtos
        };
    }
    // GET USERS AUCTIONS
    public async Task<ApiResponse<List<GetUsersAuctionsDTO>>> GetAuctionsByUserIdAsync(int userId)
    {
        var userExists = await _context.Users.AnyAsync(u => u.Id == userId);
        if (!userExists)
        {
            return new ApiResponse<List<GetUsersAuctionsDTO>>
            {
                Status = 404,
                Message = $"User with ID {userId} not found.",
                Data = null!
            };
        }
        var auctions = await _context.Auctions
            .Where(a => a.WinnerId == userId)
            .ToListAsync();
        if (!auctions.Any())
        {
            return new ApiResponse<List<GetUsersAuctionsDTO>>
            {
                Status = 404,
                Message = "No auctions found for this user.",
                Data = null!
            };
        }

        var dtoList = _mapper.Map<List<GetUsersAuctionsDTO>>(auctions);
        return new ApiResponse<List<GetUsersAuctionsDTO>>
        {
            Status = 200,
            Message = "User's auctions retrieved successfully.",
            Data = dtoList
        };
    }
    // GET BIDS BY AUCTION ID
    public async Task<ApiResponse<List<GetBidDTO>>> GetBidsByAuctionIdAsync(int auctionId)
    {
        var auctionExists = await _context.Auctions.AnyAsync(a => a.Id == auctionId);
        if (!auctionExists)
        {
            return new ApiResponse<List<GetBidDTO>>
            {
                Status = 404,
                Message = $"Auction with ID {auctionId} not found.",
                Data = null!
            };
        }

        var bids = await _context.Bids
            .Where(b => b.AuctionId == auctionId)
            .OrderByDescending(b => b.BidTime)
            .ToListAsync();

        if (bids.Count == 0)
        {
            return new ApiResponse<List<GetBidDTO>>
            {
                Status = 404,
                Message = "No bids found for this auction.",
                Data = null!
            };
        }

        var dtos = _mapper.Map<List<GetBidDTO>>(bids);
        return new ApiResponse<List<GetBidDTO>>
        {
            Status = 200,
            Message = "Bids retrieved successfully.",
            Data = dtos
        };
    }
}
