using AucWebAPI.Core;
using AucWebAPI.DTOs.UserDTOs;

namespace AucWebAPI.Services.Interfaces;
public interface IAuthService
{
     Task<ApiResponse<RegisterUserResponseDTO>> RegisterUserAsync(RegisterUserDTO dto);
     Task<ApiResponse<LoginResponseDTO>> LoginAsync(LoginDTO dto);

}
