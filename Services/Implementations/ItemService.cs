using AucWebAPI.Core;
using AucWebAPI.Data;
using AucWebAPI.DTOs.ItemDTOs;
using AucWebAPI.Enums;
using AucWebAPI.Models;
using AucWebAPI.Services.Interfaces;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace AucWebAPI.Services.Implementations;
public class ItemService : IItemService
{
    private readonly DataContext _context;
    private readonly IMapper _mapper;

    public ItemService(DataContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    // ADD ITEM
    public async Task<ApiResponse<AddItemResponseDTO>> AddItemAsync(AddItemDTO dto)
    {
        if (!Enum.TryParse<ITEM_CONDITION>(dto.Condition, true, out var parsedCondition))
        {
            return new ApiResponse<AddItemResponseDTO>
            {
                Status = 400,
                Message = $"Invalid condition '{dto.Condition}'",
                Data = null
            };
        }
        var sellerExists = await _context.Users.AnyAsync(u => u.Id == dto.SellerId);
        if (!sellerExists)
        {
            return new ApiResponse<AddItemResponseDTO>
            {
                Status = 404,
                Message = $"Seller with ID {dto.SellerId} not found.",
                Data = null
            };
        }
        var item = _mapper.Map<Item>(dto);
        item.Condition = parsedCondition;
        item.CreatedAt = DateTime.UtcNow;
        await _context.Items.AddAsync(item);
        await _context.SaveChangesAsync();
        var responseDto = _mapper.Map<AddItemResponseDTO>(item);
        return new ApiResponse<AddItemResponseDTO>
        {
            Status = 200,
            Message = "Item created successfully.",
            Data = responseDto
        };
    }

    // GET ALL ITEMS WITH FILTERING
    public async Task<ApiResponse<List<AddItemResponseDTO>>> GetAllItemsAsync(string? category = null, string? condition = null)
    {
        var query = _context.Items.AsQueryable();

        if (!string.IsNullOrWhiteSpace(category))
            query = query.Where(i => i.Category.ToLower() == category.ToLower());

        if (!string.IsNullOrWhiteSpace(condition) &&
            Enum.TryParse<ITEM_CONDITION>(condition, true, out var parsedCondition))
        {
            query = query.Where(i => i.Condition == parsedCondition);
        }

        var items = await query
            .Select(i => new AddItemResponseDTO
            {
                Id = i.Id,
                Title = i.Title,
                SellerId = i.SellerId,
                Description = i.Description,
                ImageUrls = i.ImageUrls,
                Category = i.Category,
                Condition = i.Condition,
                CreatedAt = i.CreatedAt
            }).ToListAsync();

        return new ApiResponse<List<AddItemResponseDTO>>
        {
            Status = 200,
            Message = "Items retrieved successfully",
            Data = items
        };
    }
    // GET ITEM BY ID
    public async Task<ApiResponse<AddItemResponseDTO>> GetItemByIdAsync(int id)
    {
        var item = await _context.Items
            .Where(i => i.Id == id)
            .Select(i => new AddItemResponseDTO
            {
                Id = i.Id,
                Title = i.Title,
                SellerId = i.SellerId,
                Description = i.Description,
                ImageUrls = i.ImageUrls,
                Category = i.Category,
                Condition = i.Condition,
                CreatedAt = i.CreatedAt
            }).FirstOrDefaultAsync();

        if (item == null)
        {
            return new ApiResponse<AddItemResponseDTO>
            {
                Status = 404,
                Message = "Item not found",
                Data = null
            };
        }

        return new ApiResponse<AddItemResponseDTO>
        {
            Status = 200,
            Message = "Item retrieved successfully",
            Data = item
        };
    }
    // UPDATE ITEM
    public async Task<ApiResponse<UpdateItemResponseDTO>> UpdateItemAsync(int id, UpdateItemDTO dto)
    {
        var item = await _context.Items.FindAsync(id);
        if (item == null)
        {
            return new ApiResponse<UpdateItemResponseDTO>
            {
                Status = 404,
                Message = "Item not found",
                Data = null
            };
        }
        if (!Enum.TryParse<ITEM_CONDITION>(dto.Condition, true, out var parsedCondition))
        {
            return new ApiResponse<UpdateItemResponseDTO>
            {
                Status = 400,
                Message = $"Invalid condition '{dto.Condition}'",
                Data = null
            };
        }
        var sellerExists = await _context.Users.AnyAsync(u => u.Id == dto.SellerId);
        if (!sellerExists)
        {
            return new ApiResponse<UpdateItemResponseDTO>
            {
                Status = 404,
                Message = $"Seller with ID {dto.SellerId} not found.",
                Data = null
            };
        }
        _mapper.Map(dto, item);
        item.Condition = parsedCondition;
        _context.Items.Update(item);
        await _context.SaveChangesAsync();
        var responseDto = _mapper.Map<UpdateItemResponseDTO>(item);
        return new ApiResponse<UpdateItemResponseDTO>
        {
            Status = 200,
            Message = "Item updated successfully",
            Data = responseDto
        };
    }
    // DELETE ITEM
    public async Task<ApiResponse<string>> DeleteItemAsync(int id)
    {
        var item = await _context.Items.FindAsync(id);
        if (item == null)
        {
            return new ApiResponse<string>
            {
                Status = 404,
                Message = "Item not found",
                Data = null
            };
        }

        _context.Items.Remove(item);
        await _context.SaveChangesAsync();

        return new ApiResponse<string>
        {
            Status = 200,
            Message = "Item deleted successfully",
            Data = $"Item {id} removed"
        };
    }
}
