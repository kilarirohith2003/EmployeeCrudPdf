using System.Collections.Generic;
using System.Threading.Tasks;

public interface IEmployeeRepository
{
    Task<IEnumerable<Employee>> GetAllAsync();
    Task<Employee?> GetByIdAsync(int id);
    Task<int> CreateAsync(Employee emp);
    Task<bool> UpdateAsync(Employee emp);
    Task<bool> DeleteAsync(int id);
}
