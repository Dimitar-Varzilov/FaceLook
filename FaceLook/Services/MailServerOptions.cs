namespace FaceLook.Services;

public record MailServerOptions
{
    public required string Host { get; set; }
    public required int Port { get; set; }
    public required bool UseSsl { get; set; }
    public required string Username { get; set; }
    public required string Password { get; set; }
    public required string SenderName { get; set; }
    public required string SenderEmail { get; set; }
    public required string RecepientName { get; set; }
}