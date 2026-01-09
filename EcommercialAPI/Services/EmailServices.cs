using EcommercialAPI.Helper;
using EcommercialAPI.Respository;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using System;
namespace EcommercialAPI.Services
{
    public class EmailServices : IEmailServices
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;
        public EmailServices(IConfiguration configuration, IWebHostEnvironment env)
        {
            _configuration = configuration;
            _env = env;
        }

        public async Task<APIResponse> SendEmail(string email, string otp, string request, string type)
        {
            try
            {
                var emailSettings = _configuration.GetSection("EmailSettings");
                if (string.IsNullOrEmpty(emailSettings["SmtpServer"]) ||
                    string.IsNullOrEmpty(emailSettings["SenderEmail"]) ||
                    string.IsNullOrEmpty(emailSettings["SenderPassword"]))
                {
                    return new APIResponse
                    {
                        ResponseCode = 500,
                        Result = "Email configuration missing"
                    };
                }
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(
                emailSettings["SenderName"] ?? "EnaNguyen",
                emailSettings["SenderEmail"]));
                message.To.Add(new MailboxAddress("", email));
                message.Subject = request;
                var bodyBuilder = new BodyBuilder();
                bodyBuilder.TextBody = "";
                string templatePath = "";
                string cssPath = "";
                if (type == "OTP")
                {
                    templatePath = Path.Combine(_env.WebRootPath, "template", "Email", "Validation", "validation.html");
                    cssPath = Path.Combine(_env.WebRootPath, "template", "Email", "Validation", "validation.css");
                    bodyBuilder.TextBody = $"Mã OTP của bạn là: {otp}. Có hiệu lực trong 5 phút.";
                }
                if (type == "Password Reset")
                {
                    templatePath = Path.Combine(_env.WebRootPath, "template", "Email", "Notification", "ResetPassword.html");
                    cssPath = Path.Combine(_env.WebRootPath, "template", "Email", "Notification", "ResetPassword.css");
                    bodyBuilder.TextBody = $"Mật khẩu của bạn đã được thay đổi thành {otp}. Vui lòng đổi lại mật khẩu sau khi đăng nhập";
                }
                if (!File.Exists(templatePath) || !File.Exists(cssPath))
                {
                    return new APIResponse { ResponseCode = 500, Result = "Email template not found" };
                }
                string htmlTemplate = await File.ReadAllTextAsync(templatePath);
                string cssContent = await File.ReadAllTextAsync(cssPath);

                string finalHtml = htmlTemplate
                    .Replace("<link rel=\"stylesheet\" href=\"TwoFactor.css\">", $"<style>{cssContent}</style>")
                    .Replace("@OTP_CODE@", otp);
                bodyBuilder.HtmlBody = finalHtml;
                message.Body = bodyBuilder.ToMessageBody();

                using var client = new SmtpClient();
                await client.ConnectAsync(
                    emailSettings["SmtpServer"],
                    int.Parse(emailSettings["SmtpPort"]),
                    SecureSocketOptions.StartTls);

                await client.AuthenticateAsync(
                    emailSettings["SenderEmail"],
                    emailSettings["SenderPassword"]);

                await client.SendAsync(message);
                await client.DisconnectAsync(true);

                return new APIResponse
                {
                    ResponseCode = 200,
                    Result = "Email Sent Successfully"
                };
            }
            catch (Exception ex)
            {
                return new APIResponse
                {
                    ResponseCode = 500,
                    Result = "Failed to send email",
                    ErrorMessage = ex.Message
                };
            }
        }
    }
}
