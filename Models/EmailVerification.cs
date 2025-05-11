namespace AucWebAPI.Models;
public class EmailVerification
{
    public int Id { get; set; }
    public int UserId { get; set; } // FK TO USER
    public string Token { get; set; }
    public DateTime ExpirationDate { get; set; }
    public User User { get; set; }
}
