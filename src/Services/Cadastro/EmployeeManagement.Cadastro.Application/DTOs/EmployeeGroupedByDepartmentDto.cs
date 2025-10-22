namespace EmployeeManagement.Cadastro.Application.DTOs;

public class EmployeeGroupedByDepartmentDto
{
    public string Department { get; set; } = string.Empty;
    public int EmployeeCount { get; set; }
    public List<EmployeeDto> Employees { get; set; } = new();
}