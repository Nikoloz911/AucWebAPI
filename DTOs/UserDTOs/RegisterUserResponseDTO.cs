﻿namespace AucWebAPI.DTOs.UserDTOs;
public class RegisterUserResponseDTO
{
    public string UserName { get; set; }
    public string Email { get; set; }
    public string FullName { get; set; }
    public string Role { get; set; }
    public DateTime RegistrationDate { get; set; }
}
