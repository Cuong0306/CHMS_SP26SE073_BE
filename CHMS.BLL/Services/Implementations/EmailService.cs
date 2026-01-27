using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MimeKit;

namespace CHMS.BLL.Services.Implementations
{
    public class EmailService
    {
        public async Task SendEmailAsync(string toEmail, string subject, string messageBody)
        {
            var email = new MimeMessage();

            // ⚠️ THAY EMAIL CỦA BẠN VÀO DƯỚI ĐÂY
            email.From.Add(new MailboxAddress("Coastal Homestay", "minhhieunguyenquoc2@gmail.com"));
            email.To.Add(new MailboxAddress("", toEmail));
            email.Subject = subject;

            email.Body = new TextPart("html")
            {
                Text = messageBody
            };

            using var smtp = new SmtpClient();
            // Kết nối Gmail (Port 587, TLS)
            await smtp.ConnectAsync("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);

            // ⚠️ QUAN TRỌNG: Dùng "Mật khẩu ứng dụng" (App Password), KHÔNG PHẢI mật khẩu đăng nhập Gmail thường
            await smtp.AuthenticateAsync("minhhieunguyenquoc2@gmail.com", "qnlj iscu tzcf airx");

            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }
    }
}
