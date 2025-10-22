using EmployeeManagement.Cadastro.Domain.Entities;

namespace EmployeeManagement.Cadastro.Domain.Interfaces;

public interface IEmployeeRepository
{
    Task<Employee?> GetByIdAsync(Guid id);
    Task<IEnumerable<Employee>> GetAllAsync();
    Task<IEnumerable<Employee>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<IEnumerable<IGrouping<string, Employee>>> GetGroupedByDepartmentAsync(DateTime? startDate = null, DateTime? endDate = null);
    Task<Employee> AddAsync(Employee employee);
    Task UpdateAsync(Employee employee);
    Task DeleteAsync(Guid id);
}
