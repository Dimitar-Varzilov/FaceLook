using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;

namespace FaceLook.Services;

public class EmailSender(IOptions<MailServerOptions> optionsAccessor, ILogger<EmailSender> logger) : IEmailSender
{
    public MailServerOptions MailServerOptions { get; } = optionsAccessor.Value;

    public async Task SendEmailAsync(string toEmail, string subject, string message)
    {
        if (string.IsNullOrEmpty(MailServerOptions.MailServerPassword))
        {
            throw new Exception("Null SendGridKey");
        }
        await Execute(subject, message, toEmail);
    }

    private async Task Execute(string subject, string message, string toEmail)
    {
        try
        {
            var email = new MimeMessage()
            {
                Subject = subject,
            };

            email.From.Add(new MailboxAddress(MailServerOptions.MailServerSenderName, MailServerOptions.MailServerSenderEmail));
            email.To.Add(new MailboxAddress("Receiver Name", toEmail));

            email.Body = new TextPart(TextFormat.Html)
            {
                Text = message
            };

            using var smtp = new SmtpClient();
            smtp.Connect("smtp.gmail.com", 465, true);

            smtp.Authenticate(MailServerOptions.MailServerUserName, MailServerOptions.MailServerPassword);

            smtp.Send(email);
            smtp.Disconnect(true);

        }
        catch (Exception ex)
        {
            logger.LogError("An error occurred while sending email: {exceptionMessage}", ex.Message);
        }
    }
}