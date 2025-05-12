using AucWebAPI.Core;
using AucWebAPI.DTOs.UserDTOs;
using AucWebAPI.Services.Implementations;
using AucWebAPI.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AucWebAPI.Controllers;

[Route("api/")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService) 
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserDTO dto)
    {
        var result = await _authService.RegisterUserAsync(dto);
        return StatusCode(result.Status, result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDTO loginDto)
    {
        var response = await _authService.LoginAsync(loginDto);
        return StatusCode(response.Status, response); 
    }
}
