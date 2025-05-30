﻿using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using AucWebAPI.Core;
using AucWebAPI.Data;
using AucWebAPI.DTOs.UserDTOs;
using AucWebAPI.Enums;
using AucWebAPI.JWT;
using AucWebAPI.Models;
using AucWebAPI.request;
using AucWebAPI.Services.Interfaces;
using AucWebAPI.SMTP;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

namespace AucWebAPI.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly IMapper _mapper;
        private readonly DataContext _context;
        private readonly IJWTService _jwtService;
        private readonly IDistributedCache _cache;
        private readonly TimeSpan _cacheExpiration = TimeSpan.FromMinutes(10);
        public AuthService(IMapper mapper, DataContext context, IJWTService jwtService, IDistributedCache cache)
        {
            _mapper = mapper;
            _context = context;
            _jwtService = jwtService;
            _cache = cache;
        }

        // REGISTER
        // REGISTER
        public async Task<ApiResponse<RegisterUserResponseDTO>> RegisterUserAsync(
            RegisterUserDTO dto
        )
        {
            if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
            {
                return new ApiResponse<RegisterUserResponseDTO>
                {
                    Status = StatusCodes.Status409Conflict,
                    Message = "Email already exists",
                    Data = null,
                };
            }

            if (await _context.Users.AnyAsync(u => u.UserName == dto.UserName))
            {
                return new ApiResponse<RegisterUserResponseDTO>
                {
                    Status = StatusCodes.Status409Conflict,
                    Message = "Username already exists",
                    Data = null,
                };
            }

            if (!Enum.TryParse<USER_ROLE>(dto.Role, true, out var parsedRole))
            {
                return new ApiResponse<RegisterUserResponseDTO>
                {
                    Status = StatusCodes.Status400BadRequest,
                    Message = "Invalid role provided. Please provide 'user' or 'admin'.",
                    Data = null,
                };
            }
            if (
                !dto.Email.Contains("@")
                || !dto.Email.EndsWith(".com", StringComparison.OrdinalIgnoreCase)
            )
            {
                return new ApiResponse<RegisterUserResponseDTO>
                {
                    Status = StatusCodes.Status400BadRequest,
                    Message = "Invalid email format. Email must contain '@' and end with '.com'.",
                    Data = null,
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
            string verificationToken = SMTP_Registration.GenerateVerificationCode();
            DateTime expirationDate = DateTime.UtcNow.AddMinutes(5);

            var emailVerification = new EmailVerification
            {
                UserId = user.Id,
                Token = verificationToken,
                ExpirationDate = expirationDate,
            };

            _context.EmailVerifications.Add(emailVerification);
            await _context.SaveChangesAsync();

            SMTP_Registration.EmailSender(
                user.Email,
                user.FirstName,
                user.LastName,
                verificationToken
            );

            var responseDto = _mapper.Map<RegisterUserResponseDTO>(user);
            return new ApiResponse<RegisterUserResponseDTO>
            {
                Status = StatusCodes.Status200OK,
                Message =
                    "User registered successfully. Please check your email for verification code.",
                Data = responseDto,
            };
        }

        // LOGIN
        // LOGIN
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
                    Data = null,
                };
            }

            if (!user.IsEmailConfirmed)
            {
                return new ApiResponse<LoginResponseDTO>
                {
                    Status = StatusCodes.Status403Forbidden,
                    Message = "Email not verified. Please verify your email before logging in.",
                    Data = null,
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
                    Data = null,
                };
            }
            var token = _jwtService.GetToken(user);
            var responseDto = new LoginResponseDTO
            {
                UserToken = token.UserToken,
                FullName = $"{user.FirstName} {user.LastName}",
                Role = user.Role.ToString(),
            };

            return new ApiResponse<LoginResponseDTO>
            {
                Status = StatusCodes.Status200OK,
                Message = "Login successful",
                Data = responseDto,
            };
        }

        // VERIFY EMAIL
        // VERIFY EMAIL
        public async Task<ApiResponse<string>> VerifyEmailAsync(string email, string code)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
            {
                return new ApiResponse<string>
                {
                    Status = StatusCodes.Status404NotFound,
                    Message = "User not found",
                    Data = null,
                };
            }

            if (user.IsEmailConfirmed)
            {
                return new ApiResponse<string>
                {
                    Status = StatusCodes.Status400BadRequest,
                    Message = "Email already verified",
                    Data = null,
                };
            }

            var verification = await _context
                .EmailVerifications.Where(v =>
                    v.UserId == user.Id && v.Token == code && v.ExpirationDate > DateTime.UtcNow
                )
                .FirstOrDefaultAsync();

            if (verification == null)
            {
 
                var expiredVerification = await _context
                    .EmailVerifications.Where(v =>
                        v.UserId == user.Id
                        && v.Token == code
                        && v.ExpirationDate <= DateTime.UtcNow
                    )
                    .FirstOrDefaultAsync();

                if (expiredVerification != null)
                {
                    return new ApiResponse<string>
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Message = "Verification code expired. Please request a new one.",
                        Data = null,
                    };
                }

                return new ApiResponse<string>
                {
                    Status = StatusCodes.Status400BadRequest,
                    Message = "Invalid verification code",
                    Data = null,
                };
            }
            user.IsEmailConfirmed = true;
            _context.EmailVerifications.Remove(verification);
            await _context.SaveChangesAsync();
            return new ApiResponse<string>
            {
                Status = StatusCodes.Status200OK,
                Message = "Email verified successfully",
                Data = "Email verification successful",
            };
        }

        // RESEND VERIFICATION CODE
        public async Task<ApiResponse<string>> ResendVerificationCodeAsync(string email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
            {
                return new ApiResponse<string>
                {
                    Status = StatusCodes.Status404NotFound,
                    Message = "User not found",
                    Data = null,
                };
            }

            if (user.IsEmailConfirmed)
            {
                return new ApiResponse<string>
                {
                    Status = StatusCodes.Status400BadRequest,
                    Message = "Email already verified",
                    Data = null,
                };
            }
            var existingVerifications = await _context
                .EmailVerifications.Where(v => v.UserId == user.Id)
                .ToListAsync();

            if (existingVerifications.Any())
            {
                _context.EmailVerifications.RemoveRange(existingVerifications);
            }
            string verificationToken = SMTP_Registration.GenerateVerificationCode();
            DateTime expirationDate = DateTime.UtcNow.AddMinutes(5);
            var emailVerification = new EmailVerification
            {
                UserId = user.Id,
                Token = verificationToken,
                ExpirationDate = expirationDate,
            };

            _context.EmailVerifications.Add(emailVerification);
            await _context.SaveChangesAsync();

            SMTP_Registration.EmailSender(
                user.Email,
                user.FirstName,
                user.LastName,
                verificationToken
            );

            return new ApiResponse<string>
            {
                Status = StatusCodes.Status200OK,
                Message = "Verification code sent successfully",
                Data = "New verification code has been sent to your email",
            };
        }

        // SEND FORGOT PASSWORD CODE
        // SEND FORGOT PASSWORD CODE
        public async Task<ApiResponse<string>> ForgotPasswordAsync(string email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
                return new ApiResponse<string>
                {
                    Status = StatusCodes.Status404NotFound,
                    Message = "User not found",
                    Data = null,
                };
            }
            var existingVerifications = await _context.EmailVerifications
                .Where(v => v.UserId == user.Id)
                .ToListAsync();

            if (existingVerifications.Any())
            {
                _context.EmailVerifications.RemoveRange(existingVerifications);
            }
            string resetToken = SMTP_Registration.GenerateVerificationCode();
            DateTime expirationDate = DateTime.UtcNow.AddMinutes(5);
            var emailVerification = new EmailVerification
            {
                UserId = user.Id,
                Token = resetToken,
                ExpirationDate = expirationDate
            };
            _context.EmailVerifications.Add(emailVerification);
            await _context.SaveChangesAsync();
            SMTP_ResetPassword.EmailSender(user.Email, user.FirstName, user.LastName, resetToken);

            return new ApiResponse<string>
            {
                Status = StatusCodes.Status200OK,
                Message = "Password reset code sent successfully",
                Data = "A password reset code has been sent to your email",
            };
        }
        // RESET PASSWORD
        // RESET PASSWORD
        public async Task<ApiResponse<string>> ResetPasswordAsync(
            string email,
            string code,
            string newPassword
        )
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user is null)
            {
                return new ApiResponse<string>
                {
                    Status = StatusCodes.Status404NotFound,
                    Message = "User not found",
                    Data = null,
                };
            }
            if (user.IsEmailConfirmed == false)
            {
                return new ApiResponse<string>
                {
                    Status = StatusCodes.Status401Unauthorized,
                    Message =
                        "Email not verified. Please verify your email before resetting the password.",
                    Data = null,
                };
            }
            var verification = await _context.EmailVerifications
                .Where(v => v.UserId == user.Id && v.Token == code)
                .FirstOrDefaultAsync();
            if (verification == null)
            {
                return new ApiResponse<string>
                {
                    Status = StatusCodes.Status400BadRequest,
                    Message = "No reset code found. Please request a new one.",
                    Data = null,
                };
            }
            if (verification.Token != code)
            {
                return new ApiResponse<string>
                {
                    Status = StatusCodes.Status400BadRequest,
                    Message = "Invalid reset code",
                    Data = null,
                };
            }

            if (verification.ExpirationDate < DateTime.UtcNow)
            {
                _context.EmailVerifications.Remove(verification);
                await _context.SaveChangesAsync();

                return new ApiResponse<string>
                {
                    Status = StatusCodes.Status400BadRequest,
                    Message = "Reset code expired. Please request a new one.",
                    Data = null,
                };
            }

            using var hmac = new HMACSHA256(Convert.FromBase64String(user.Salt));
            var passwordBytes = Encoding.UTF8.GetBytes(newPassword);
            var hashedPassword = hmac.ComputeHash(passwordBytes);
            user.Password = Convert.ToBase64String(hashedPassword);
            _context.EmailVerifications.Remove(verification);
            await _context.SaveChangesAsync();
            return new ApiResponse<string>
            {
                Status = StatusCodes.Status200OK,
                Message = "Password has been reset successfully",
                Data = "Password reset successful",
            };
        }

        // GET USER BY ID
        // GET USER BY ID
        public async Task<ApiResponse<GetUserDTO>> GetUserProfileAsync(int id)
        {
            string cacheKey = $"UserProfile_{id}";

            var cachedUser = await _cache.GetStringAsync(cacheKey);
            if (!string.IsNullOrEmpty(cachedUser))
            {
                var userFromCache = JsonSerializer.Deserialize<GetUserDTO>(cachedUser);
                return new ApiResponse<GetUserDTO>
                {
                    Status = StatusCodes.Status200OK,
                    Message = "User profile retrieved from cache",
                    Data = userFromCache
                };
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return new ApiResponse<GetUserDTO>
                {
                    Status = StatusCodes.Status404NotFound,
                    Message = "User not found",
                    Data = null,
                };
            }
            var userDTO = _mapper.Map<GetUserDTO>(user);
            var serializedData = JsonSerializer.Serialize(userDTO);
            await _cache.SetStringAsync(cacheKey, serializedData, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
            });
            return new ApiResponse<GetUserDTO>
            {
                Status = StatusCodes.Status200OK,
                Message = "User profile retrieved successfully",
                Data = userDTO,
            };
        }

        // UPDATE USER BY ID
        // UPDATE USER BY ID
        public async Task<ApiResponse<UpdateUserResponseDTO>> UpdateUserAsync(
            int id,
            UpdateUserDTO dto
        )
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return new ApiResponse<UpdateUserResponseDTO>
                {
                    Status = StatusCodes.Status404NotFound,
                    Message = "User not found",
                    Data = null,
                };
            }

            var existingUserByUsername = await _context
                .Users.Where(u => u.Id != id && u.UserName == dto.UserName)
                .FirstOrDefaultAsync();

            if (existingUserByUsername != null)
            {
                return new ApiResponse<UpdateUserResponseDTO>
                {
                    Status = StatusCodes.Status409Conflict,
                    Message = "Username is already taken by another user",
                    Data = null,
                };
            }

            var existingUserByEmail = await _context
                .Users.Where(u => u.Id != id && u.Email == dto.Email)
                .FirstOrDefaultAsync();

            if (existingUserByEmail != null)
            {
                return new ApiResponse<UpdateUserResponseDTO>
                {
                    Status = StatusCodes.Status409Conflict,
                    Message = "Email is already registered by another user",
                    Data = null,
                };
            }

            _mapper.Map(dto, user);
            await _context.SaveChangesAsync();
            var responseDto = _mapper.Map<UpdateUserResponseDTO>(user);
            return new ApiResponse<UpdateUserResponseDTO>
            {
                Status = StatusCodes.Status200OK,
                Message = "User updated successfully",
                Data = responseDto,
            };
        }
    }
}
