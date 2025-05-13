using System.Net.Mail;
using System.Net;

namespace AucWebAPI.SMTP
{
    public class SMTP_ResetPassword
    {
        public static void EmailSender(string toAddress, string firstName, string lastName, string resetCode)
        {
            string senderEmail = "nikalobjanidze014@gmail.com";
            string appPassword = "uqbo hbxk ikls pfmb"; // APP PASSWORD

            string htmlContent = $@"
<!DOCTYPE html>
<html lang='en'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Password Reset</title>
</head>
<body style='font-family: Segoe UI, Roboto, Helvetica, sans-serif; margin: 0; padding: 0; background-color: #f4f4f4;'>
    <table style='width: 100%; max-width: 600px; margin: 0 auto; background-color: #ffffff; border-radius: 8px; box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);'>
        <tr>
            <td style='background-color: #7B2CBF; color: #ffffff; text-align: center; padding: 20px;'>
                <h1 style='font-size: 26px; margin: 0;'>AuctionMaster Password Reset</h1>
                <div style='height: 3px; width: 80px; background: linear-gradient(90deg, #9D4EDD 0%, #E0AAFF 100%); margin: 15px auto 0;'></div>
            </td>
        </tr>
        
        <tr>
            <td style='padding: 20px 40px; text-align: left;'>
                <p style='font-size: 18px; line-height: 1.6; margin: 0 0 20px;'>Dear {firstName} {lastName},</p>
                <p style='font-size: 18px; line-height: 1.6; margin: 0 0 25px;'>We received a request to reset your password for your AuctionMaster account. Please use the verification code below to proceed:</p>
                
                <div style='background-color: #121212; border-radius: 8px; padding: 25px; margin: 30px 0; text-align: center; border: 1px solid #333;'>
                    <div style='font-size: 42px; font-weight: 700; letter-spacing: 3px; color: #9D4EDD;'>{resetCode}</div>
                    <div style='font-size: 14px; color: #aaa; margin-top: 10px;'>This code expires in 5 minutes</div>
                </div>
                
                <p style='font-size: 16px; line-height: 1.6; margin: 0 0 15px; color: #666;'>If you didn't request a password reset, you can safely ignore this email.</p>
            </td>
        </tr>
        
        <tr>
            <td style='padding: 20px; text-align: center; border-top: 1px solid #ddd;'>
                <p style='font-size: 14px; color: #999; margin: 0 0 10px;'>Need help? Contact our support team anytime.</p>
                <p style='font-size: 14px; color: #999; margin: 0;'>© 2025 AuctionMaster. All rights reserved.</p>
            </td>
        </tr>
    </table>
</body>
</html>";

            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(senderEmail);
            mail.To.Add(toAddress);
            mail.Subject = "AuctionMaster - Password Reset Code";
            mail.Body = htmlContent;
            mail.IsBodyHtml = true;

            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                EnableSsl = true,
                Credentials = new NetworkCredential(senderEmail, appPassword),
            };

            smtpClient.Send(mail);
        }
    }
}
