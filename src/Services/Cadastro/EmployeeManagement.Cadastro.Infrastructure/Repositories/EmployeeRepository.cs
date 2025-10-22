namespace EmployeeManagement.Cadastro.Infrastructure.Repositories;

public class EmployeeRepository : IEmployeeRepository
{
    private readonly ApplicationDbContext _context;

    public EmployeeRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Employee?> GetByIdAsync(Guid id)
    {
        return await _context.Employees.FindAsync(id);
    }

    public async Task<IEnumerable<Employee>> GetAllAsync()
    {
        return await _context.Employees
            .OrderByDescending(e => e.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Employee>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _context.Employees
            .Where(e => e.StartDate >= startDate && e.StartDate <= endDate)
            .OrderBy(e => e.StartDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<IGrouping<string, Employee>>> GetGroupedByDepartmentAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        var query = _context.Employees.AsQueryable();

        if (startDate.HasValue)
            query = query.Where(e => e.StartDate >= startDate.Value);

        if (endDate.HasValue)
            query = query.Where(e => e.StartDate <= endDate.Value);

        var employees = await query.ToListAsync();
        return employees.GroupBy(e => e.Department);
    }

    public async Task<Employee> AddAsync(Employee employee)
    {
        await _context.Employees.AddAsync(employee);
        await _context.SaveChangesAsync();
        return employee;
    }

    public async Task UpdateAsync(Employee employee)
    {
        _context.Employees.Update(employee);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var employee = await GetByIdAsync(id);
        if (employee != null)
        {
            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();
        }
    }
}