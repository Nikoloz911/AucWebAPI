using AucWebAPI.Core;
using AucWebAPI.DTOs.UserDTOs;

namespace AucWebAPI.Services.Interfaces;
public interface IAuthService
{
     Task<ApiResponse<RegisterUserResponseDTO>> RegisterUserAsync(RegisterUserDTO dto);
     Task<ApiResponse<LoginResponseDTO>> LoginAsync(LoginDTO dto);
    Task<ApiResponse<string>> VerifyEmailAsync(string email, string code);
    Task<ApiResponse<string>> ResendVerificationCodeAsync(string email);
    Task<ApiResponse<string>> ForgotPasswordAsync(string email);
    Task<ApiResponse<string>> ResetPasswordAsync(string email, string code, string newPassword);
    Task<ApiResponse<GetUserDTO>> GetUserProfileAsync(int id);
    Task<ApiResponse<UpdateUserResponseDTO>> UpdateUserAsync(int id, UpdateUserDTO dto);
}
