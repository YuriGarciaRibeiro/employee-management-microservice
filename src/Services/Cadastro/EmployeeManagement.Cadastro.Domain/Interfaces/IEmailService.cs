namespace EmployeeManagement.Cadastro.Domain.Interfaces;

public interface IEmailService
{
    Task SendWelcomeEmailAsync(string recipientEmail, string recipientName, DateTime startDate);
    Task SendStartDateUpdatedEmailAsync(string recipientEmail, string recipientName, DateTime oldStartDate, DateTime newStartDate);
}