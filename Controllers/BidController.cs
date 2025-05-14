using AucWebAPI.Core;
using AucWebAPI.DTOs.BidsDTOs;
using AucWebAPI.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AucWebAPI.Controllers;
[Route("api/")]
[ApiController]
public class BidController : ControllerBase
{
    private readonly IBidsService _bidsService;

    public BidController(IBidsService bidsService)
    {
        _bidsService = bidsService;
    }

    [HttpPost("bids")]
    public async Task<ActionResult<ApiResponse<AddBidResponseDTO>>> AddBid([FromBody] AddBidDTO bidDto)
    {
        var response = await _bidsService.AddBidAsync(bidDto);
        return StatusCode(response.Status, response);
    }

    [HttpGet("bids/{userId}")]
    public async Task<ActionResult<ApiResponse<List<GetBidByIdDTO>>>> GetUserBids(int userId)
    {
        var response = await _bidsService.GetUserBidsAsync(userId);
        return StatusCode(response.Status, response);
    }
}
