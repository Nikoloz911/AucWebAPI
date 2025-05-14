using AucWebAPI.DTOs.ItemDTOs;
using AucWebAPI.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AucWebAPI.Controllers;

[Route("api/")]
[ApiController]
public class ItemController : ControllerBase
{
    private readonly IItemService _itemService;

    public ItemController(IItemService itemService)
    {
        _itemService = itemService;
    }
    [HttpPost("item")]
    public async Task<IActionResult> AddItem([FromBody] AddItemDTO dto)
    {
        var result = await _itemService.AddItemAsync(dto);
        return StatusCode(result.Status, result);
    }
    [HttpGet("items")]
    public async Task<IActionResult> GetAllItems([FromQuery] string? category, [FromQuery] string? condition)
    {
        var result = await _itemService.GetAllItemsAsync(category, condition);
        return StatusCode(result.Status, result);
    }

    [HttpGet("items/{id}")]
    public async Task<IActionResult> GetItemById(int id)
    {
        var result = await _itemService.GetItemByIdAsync(id);
        return StatusCode(result.Status, result);
    }
    [HttpPut("items/{id}")]
    public async Task<IActionResult> UpdateItem(int id, [FromBody] UpdateItemDTO dto)
    {
        var result = await _itemService.UpdateItemAsync(id, dto);
        return StatusCode(result.Status, result);
    }
    [HttpDelete("items/{id}")]
    public async Task<IActionResult> DeleteItem(int id)
    {
        var result = await _itemService.DeleteItemAsync(id);
        return StatusCode(result.Status, result);
    }

}
