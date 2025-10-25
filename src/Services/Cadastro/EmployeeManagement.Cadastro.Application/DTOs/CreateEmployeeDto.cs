namespace EmployeeManagement.Cadastro.Application.DTOs;

public class CreateEmployeeDto
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public string Department { get; set; } = string.Empty;
}