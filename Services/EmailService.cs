using CarrefourPolaire.Models;
using CarrefourPolaire.Services.Interfaces;
using MailKit.Net.Smtp;
using MimeKit;

namespace CarrefourPolaire.Services;

public class EmailService: IEmailService
{
    private readonly SmtpSettings _smtpSettings;

    public EmailService(SmtpSettings smtpSettings)
    {
        _smtpSettings = smtpSettings;
    }
    
    public async Task SendEmail(string toEmail, string subject, string body)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(subject, _smtpSettings.FromAddress));
        message.To.Add(new MailboxAddress("", toEmail));
        message.Subject = subject;
        message.Body = new TextPart("plain")
        {
            Text = body
        };

        using var client = new SmtpClient();
        await client.ConnectAsync(_smtpSettings.Host, _smtpSettings.Port, MailKit.Security.SecureSocketOptions.StartTls); //sue me, I don't want to make everything a var:_)
        await client.AuthenticateAsync(_smtpSettings.Username, _smtpSettings.Password);
        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }

}