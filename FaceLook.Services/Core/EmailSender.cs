using FaceLook.Data.Entities;
using FaceLook.Services.Models;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;
using System.Text;

namespace FaceLook.Services.Core;

public class EmailSender(IOptions<MailServerOptions> mailServerOptionsAccessor, ILogger<EmailSender> logger, UserManager<User> userManager, IHttpContextAccessor httpContextAccessor) : IEmailSender, IEmailSender<User>
{
    public readonly MailServerOptions MailServerOptions = mailServerOptionsAccessor.Value;

    public async Task SendConfirmationLinkAsync(User user, string email, string confirmationLink)
    {
        await SendEmailAsyncInternal("Confirm your email", $"Please confirm your account by <a href='{confirmationLink}'>clicking here</a>.", email);
    }

    public async Task SendEmailAsync(string toEmail, string subject, string htmlMessage)
    {
        await SendEmailAsyncInternal(subject, htmlMessage, toEmail);
    }

    public async Task SendPasswordResetCodeAsync(User user, string email, string resetCode)
    {
        await SendEmailAsyncInternal("Reset your password", $"Reset your password using this code: {resetCode}", email);
    }

    public async Task SendPasswordResetLinkAsync(User user, string email, string resetLink)
    {
        await SendEmailAsyncInternal("Reset your password", $"Reset your password by <a href='{resetLink}'>clicking here</a>.", email);
    }

    private async Task SendEmailAsyncInternal(string subject, string htmlMessage, string? toEmail)
    {
        ArgumentNullException.ThrowIfNull(toEmail);

        try
        {
            var email = new MimeMessage()
            {
                Subject = subject,
            };


            var claims = httpContextAccessor.HttpContext?.User;
            User? currentUser = null;
            if (claims is not null)
            {
                var userFromDb = await userManager.GetUserAsync(claims);
                if (userFromDb is not null)
                {
                    currentUser = userFromDb;
                }
            }

            var fromSenderName = currentUser?.UserName ?? MailServerOptions.SenderName;
            var fromSenderEmail = currentUser?.Email ?? MailServerOptions.SenderEmail;
            email.From.Add(new MailboxAddress(fromSenderName, fromSenderEmail));

            var userToSend = await userManager.FindByEmailAsync(toEmail);
            var toSenderName = userToSend?.UserName ?? MailServerOptions.RecepientName;
            email.To.Add(new MailboxAddress(toSenderName, toEmail));

            var contentBytres = Encoding.UTF8.GetBytes(htmlMessage);
            var contentStream = new MemoryStream(contentBytres);
            email.Body = new TextPart(TextFormat.Html)
            {
                Content = new MimeContent(contentStream)
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
            throw;
        }
    }
}