using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;

namespace FaceLook.Services;

public class EmailSender(IOptions<MailServerOptions> mailServerOptionsAccessor, ILogger<EmailSender> logger, IUserService userService) : IEmailSender
{
    public MailServerOptions MailServerOptions { get; } = mailServerOptionsAccessor.Value;

    public async Task SendEmailAsync(string toEmail, string subject, string htmlMessage)
    {
        await SendEmailAsyncInternal(subject, htmlMessage, toEmail);
    }

    private async Task SendEmailAsyncInternal(string subject, string htmlMessage, string toEmail)
    {
        try
        {
            var email = new MimeMessage()
            {
                Subject = subject,
            };

            var currentUser = await userService.GetCurrentUserAsync();
            var fromSenderName = currentUser?.UserName ?? MailServerOptions.SenderName;
            var fromSenderEmail = currentUser?.Email ?? MailServerOptions.SenderEmail;
            email.From.Add(new MailboxAddress(fromSenderName, fromSenderEmail));

            var userToSend = await userService.GetUserByEmailAsync(toEmail);
            var toSenderName = userToSend?.UserName ?? MailServerOptions.RecepientName;
            email.To.Add(new MailboxAddress(toSenderName, toEmail));

            email.Body = new TextPart(TextFormat.Html)
            {
                Text = htmlMessage
            };

            using var smtp = new SmtpClient();
            smtp.Connect(MailServerOptions.Host, MailServerOptions.Port, MailServerOptions.UseSsl);

            smtp.Authenticate(MailServerOptions.Username, MailServerOptions.Password);

            smtp.Send(email);
            smtp.Disconnect(true);

        }
        catch (Exception ex)
        {
            logger.LogError("An error occurred while sending email: {exceptionMessage}", ex.Message);
        }
    }
}