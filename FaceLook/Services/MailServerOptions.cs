namespace FaceLook.Services;

public record MailServerOptions
{
    public required string MailServerUserName { get; set; }
    public required string MailServerPassword { get; set; }
    public required string MailServerSenderName{ get; set; }
    public required string MailServerSenderEmail{ get; set; }
}