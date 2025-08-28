using MySqlConnector;
using Dapper; // optional; if you prefer ADO manually, remove this line & use MySqlCommand
using Microsoft.Extensions.Configuration;

public class EmployeeRepository : IEmployeeRepository
{
    private readonly string _connStr;

    public EmployeeRepository(IConfiguration cfg)
    {
        _connStr = cfg.GetConnectionString("MySql")!;
    }

    private MySqlConnection Conn() => new MySqlConnection(_connStr);

    public async Task<IEnumerable<Employee>> GetAllAsync()
    {
        const string sql = "SELECT id, name, department, email, salary FROM employees ORDER BY id DESC;";
        using var conn = Conn();
        return await conn.QueryAsync<Employee>(sql);
    }

    public async Task<Employee?> GetByIdAsync(int id)
    {
        const string sql = "SELECT id, name, department, email, salary FROM employees WHERE id = @id;";
        using var conn = Conn();
        return await conn.QueryFirstOrDefaultAsync<Employee>(sql, new { id });
    }

    public async Task<int> CreateAsync(Employee emp)
    {
        const string sql = @"
            INSERT INTO employees (name, department, email, salary)
            VALUES (@Name, @Department, @Email, @Salary);
            SELECT LAST_INSERT_ID();";
        using var conn = Conn();
        var newId = await conn.ExecuteScalarAsync<int>(sql, emp);
        return newId;
    }

    public async Task<bool> UpdateAsync(Employee emp)
    {
        const string sql = @"
            UPDATE employees
            SET name=@Name, department=@Department, email=@Email, salary=@Salary
            WHERE id=@Id;";
        using var conn = Conn();
        var rows = await conn.ExecuteAsync(sql, emp);
        return rows > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        const string sql = "DELETE FROM employees WHERE id=@id;";
        using var conn = Conn();
        var rows = await conn.ExecuteAsync(sql, new { id });
        return rows > 0;
    }
}
