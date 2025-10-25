using EmployeeManagement.Cadastro.Domain.Enums;

namespace EmployeeManagement.Cadastro.Domain.Entities;

public class Employee
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public string Department { get; set; } = string.Empty;
    public EmployeeStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
