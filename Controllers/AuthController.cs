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

    [HttpPost("auth/register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserDTO dto)
    {
        var result = await _authService.RegisterUserAsync(dto);
        return StatusCode(result.Status, result);
    }

    [HttpPost("auth/login")]
    public async Task<IActionResult> Login([FromBody] LoginDTO loginDto)
    {
        var response = await _authService.LoginAsync(loginDto);
        return StatusCode(response.Status, response);
    }

    [HttpGet("auth/verify-email")]
    public async Task<IActionResult> VerifyEmail([FromQuery] string email, [FromQuery] string code)
    {
        var result = await _authService.VerifyEmailAsync(email, code);
        return StatusCode(result.Status, result);
    }

    [HttpGet("auth/resend-verification")]
    public async Task<IActionResult> ResendVerification([FromQuery] string email)
    {
        var result = await _authService.ResendVerificationCodeAsync(email);
        return StatusCode(result.Status, result);
    }

    [HttpGet("auth/forgot-password")]
    public async Task<IActionResult> SendResetCode([FromQuery] string email)
    {
        var result = await _authService.ForgotPasswordAsync(email);
        return StatusCode(result.Status, result);
    }

    [HttpPost("auth/reset-password")]
    public async Task<IActionResult> ResetPassword(
        [FromForm] string email,
        [FromForm] string code,
        [FromForm] string newPassword
    )
    {
        var result = await _authService.ResetPasswordAsync(email, code, newPassword);
        return StatusCode(result.Status, result);
    }

    [HttpGet("users/profile/{id}")]
    public async Task<IActionResult> GetUserProfile(int id)
    {
        var response = await _authService.GetUserProfileAsync(id);
        return StatusCode(response.Status, response);
    }

    [HttpPut("users/profile/{id}")]
    public async Task<IActionResult> UpdateUserProfile(int id, [FromBody] UpdateUserDTO dto)
    {
        var response = await _authService.UpdateUserAsync(id, dto);
        return StatusCode(response.Status, response);
    }
}
