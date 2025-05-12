using AucWebAPI.Core;
using AucWebAPI.Data;
using AucWebAPI.DTOs.UserDTOs;
using AucWebAPI.Enums;
using AucWebAPI.Models;
using AucWebAPI.request;
using AutoMapper;
using AucWebAPI.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using AucWebAPI.JWT;

namespace AucWebAPI.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly IMapper _mapper;
        private readonly DataContext _context;
        private readonly IJWTService _jwtService;

        public AuthService(IMapper mapper, DataContext context, IJWTService jwtService)
        {
            _mapper = mapper;
            _context = context;
            _jwtService = jwtService;
        }

        public async Task<ApiResponse<RegisterUserResponseDTO>> RegisterUserAsync(RegisterUserDTO dto)
        {
            if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
            {
                return new ApiResponse<RegisterUserResponseDTO>
                {
                    Status = StatusCodes.Status409Conflict,
                    Message = "Email already exists",
                    Data = null
                };
            }

            if (await _context.Users.AnyAsync(u => u.UserName == dto.UserName))
            {
                return new ApiResponse<RegisterUserResponseDTO>
                {
                    Status = StatusCodes.Status409Conflict,
                    Message = "Username already exists",
                    Data = null
                };
            }

            if (!Enum.TryParse<USER_ROLE>(dto.Role, true, out var parsedRole))
            {
                return new ApiResponse<RegisterUserResponseDTO>
                {
                    Status = StatusCodes.Status400BadRequest,
                    Message = "Invalid role provided. Please provide 'user' or 'admin'.",
                    Data = null
                };
            }

            var registerUser = _mapper.Map<RegisterUser>(dto);
            registerUser.Role = parsedRole;
            registerUser.RegistrationDate = DateTime.UtcNow;
            registerUser.IsEmailConfirmed = false;

            using var hmac = new HMACSHA256();
            registerUser.Salt = Convert.ToBase64String(hmac.Key);
            var passwordBytes = Encoding.UTF8.GetBytes(dto.Password);
            var hashedPassword = hmac.ComputeHash(passwordBytes);
            registerUser.Password = Convert.ToBase64String(hashedPassword);

            var user = _mapper.Map<User>(registerUser);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var responseDto = _mapper.Map<RegisterUserResponseDTO>(user);
            return new ApiResponse<RegisterUserResponseDTO>
            {
                Status = StatusCodes.Status200OK,
                Message = "User registered successfully",
                Data = responseDto
            };
        }

        public async Task<ApiResponse<LoginResponseDTO>> LoginAsync(LoginDTO dto)
        {
            User user = null;

            if (await _context.Users.AnyAsync(u => u.Email == dto.Identifier))
            {
                user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Identifier);
            }
            else if (await _context.Users.AnyAsync(u => u.UserName == dto.Identifier))
            {
                user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == dto.Identifier);
            }

            if (user == null)
            {
                return new ApiResponse<LoginResponseDTO>
                {
                    Status = StatusCodes.Status404NotFound,
                    Message = "User not found. Please check the email or username.",
                    Data = null
                };
            }

            using var hmac = new HMACSHA256(Convert.FromBase64String(user.Salt));
            var passwordBytes = Encoding.UTF8.GetBytes(dto.Password);
            var hashedPassword = hmac.ComputeHash(passwordBytes);
            var storedPassword = Convert.FromBase64String(user.Password);

            if (!hashedPassword.SequenceEqual(storedPassword))
            {
                return new ApiResponse<LoginResponseDTO>
                {
                    Status = StatusCodes.Status401Unauthorized,
                    Message = "Incorrect password. Please try again.",
                    Data = null
                };
            }

            var token = _jwtService.GetToken(user);

            var responseDto = new LoginResponseDTO
            {
                UserToken = token.UserToken,
                FullName = $"{user.FirstName} {user.LastName}",
                Role = user.Role.ToString()
            };

            return new ApiResponse<LoginResponseDTO>
            {
                Status = StatusCodes.Status200OK,
                Message = "Login successful",
                Data = responseDto
            };
        }




    }
}