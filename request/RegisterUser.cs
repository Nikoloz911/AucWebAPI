using AucWebAPI.Enums;
namespace AucWebAPI.request;
public class RegisterUser
{
    public string UserName { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string Salt { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public bool IsEmailConfirmed { get; set; }
    public DateTime RegistrationDate { get; set; }
    public USER_ROLE Role { get; set; }
}
