using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AucWebAPI.Services.Interfaces;
using AucWebAPI.Core;
using AucWebAPI.DTOs.AuctionDTOs;
namespace AucWebAPI.Controllers;
[Route("api/")]
[ApiController]
public class AuctionController : ControllerBase
{
    private readonly IAuctionService _auctionService;
    public AuctionController(IAuctionService auctionService)
    {
        _auctionService = auctionService;
    }

    [HttpPost("auctions")]
    public async Task<IActionResult> CreateAuction([FromBody] AddAuctionDTO dto)
    {
        var result = await _auctionService.AddAuctionAsync(dto);
        return StatusCode(result.Status, result);
    }

    [HttpGet("auctions")]
    public async Task<IActionResult> GetAuctions([FromQuery] GetAuctionsFilterDTO filter)
    {
        var result = await _auctionService.GetFilteredAuctionsAsync(filter);
        return StatusCode(result.Status, result);
    }

    [HttpGet("auctions/{id}")]
    public async Task<IActionResult> GetAuctionById(int id)
    {
        var result = await _auctionService.GetAuctionByIdAsync(id);
        return StatusCode(result.Status, result);
    }

    [HttpPut("auctions/{id}")]
    public async Task<IActionResult> UpdateAuction(int id, [FromBody] UpdateAuctionDTO dto)
    {
        var result = await _auctionService.UpdateAuctionAsync(id, dto);
        return StatusCode(result.Status, result);
    }

    [HttpDelete("auctions/{id}")]
    public async Task<IActionResult> DeleteAuction(int id)
    {
        var result = await _auctionService.DeleteAuctionAsync(id);
        return StatusCode(result.Status, result);
    }

    [HttpGet("auctions/active")]
    public async Task<IActionResult> GetActiveAuctions()
    {
        var result = await _auctionService.GetActiveAuctionsAsync();
        return StatusCode(result.Status, result);
    }

    [HttpGet("auctions/{userId:int}")]
    public async Task<IActionResult> GetUserAuctions(int userId)
    {
        var result = await _auctionService.GetAuctionsByUserIdAsync(userId);
        return StatusCode(result.Status, result);
    }

    [HttpGet("auctions/{id}/bids")]
    public async Task<IActionResult> GetBidsByAuctionId(int id)
    {
        var response = await _auctionService.GetBidsByAuctionIdAsync(id);
        return StatusCode(response.Status, response);
    }

}
