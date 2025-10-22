using EmployeeManagement.Cadastro.Domain.Interfaces;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using MimeKit;

namespace EmployeeManagement.Cadastro.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendWelcomeEmailAsync(string recipientEmail, string recipientName, DateTime startDate)
    {
        var subject = "Bem-vindo à Empresa!";
        var body = $@"
            <html>
            <body>
                <h2>Olá, {recipientName}!</h2>
                <p>Seja bem-vindo(a) à nossa empresa!</p>
                <p>Sua data de início é: <strong>{startDate:dd/MM/yyyy}</strong></p>
                <p>Estamos ansiosos para trabalhar com você!</p>
                <br/>
                <p>Atenciosamente,</p>
                <p><strong>Equipe de RH</strong></p>
            </body>
            </html>
        ";

        await SendEmailAsync(recipientEmail, recipientName, subject, body);
    }

    public async Task SendStartDateUpdatedEmailAsync(string recipientEmail, string recipientName, DateTime oldStartDate, DateTime newStartDate)
    {
        var subject = "Data de Início Atualizada";
        var body = $@"
            <html>
            <body>
                <h2>Olá, {recipientName}!</h2>
                <p>Sua data de início foi atualizada.</p>
                <p>Data anterior: <strong>{oldStartDate:dd/MM/yyyy}</strong></p>
                <p>Nova data: <strong>{newStartDate:dd/MM/yyyy}</strong></p>
                <br/>
                <p>Atenciosamente,</p>
                <p><strong>Equipe de RH</strong></p>
            </body>
            </html>
        ";

        await SendEmailAsync(recipientEmail, recipientName, subject, body);
    }

    private async Task SendEmailAsync(string recipientEmail, string recipientName, string subject, string htmlBody)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(
            _configuration["EmailSettings:SenderName"],
            _configuration["EmailSettings:SenderEmail"]
        ));
        message.To.Add(new MailboxAddress(recipientName, recipientEmail));
        message.Subject = subject;

        var bodyBuilder = new BodyBuilder { HtmlBody = htmlBody };
        message.Body = bodyBuilder.ToMessageBody();

        using var client = new SmtpClient();
        await client.ConnectAsync(
            _configuration["EmailSettings:SmtpServer"],
            int.Parse(_configuration["EmailSettings:SmtpPort"] ?? "587"),
            MailKit.Security.SecureSocketOptions.StartTls
        );

        await client.AuthenticateAsync(
            _configuration["EmailSettings:Username"],
            _configuration["EmailSettings:Password"]
        );

        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }
}
