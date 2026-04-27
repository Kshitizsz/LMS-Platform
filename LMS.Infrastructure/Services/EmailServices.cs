using LMS.Domain.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;

namespace LMS.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _config;

    public EmailService(IConfiguration config) => _config = config;

    public async Task SendAsync(string to, string subject, string htmlBody,
        CancellationToken ct = default)
    {
        var message = new MimeMessage();
        message.From.Add(MailboxAddress.Parse(_config["Email:From"]!));
        message.To.Add(MailboxAddress.Parse(to));
        message.Subject = subject;

        message.Body = new BodyBuilder { HtmlBody = htmlBody }.ToMessageBody();

        using var smtp = new SmtpClient();
        await smtp.ConnectAsync(
            _config["Email:Host"]!,
            int.Parse(_config["Email:Port"]!),
            SecureSocketOptions.StartTls, ct);

        await smtp.AuthenticateAsync(
            _config["Email:Username"]!,
            _config["Email:Password"]!, ct);

        await smtp.SendAsync(message, ct);
        await smtp.DisconnectAsync(true, ct);
    }

   }