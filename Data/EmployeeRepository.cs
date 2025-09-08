using System.Diagnostics;
using Dapper;
using EmployeeCrudPdf.Exceptions;
using EmployeeCrudPdf.Models;

using EmployeeCrudPdf.Services.Logging;
using MySqlConnector;

namespace EmployeeCrudPdf.Data
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly string _connStr;
        private readonly IAppLogger _log;
        private readonly double _slowSeconds;

        public EmployeeRepository(IConfiguration cfg, IAppLogger log)
        {
            _connStr = cfg.GetConnectionString("MySql")!;
            _log = log;
            _slowSeconds = cfg.GetValue<double?>("Performance:SlowQuerySeconds") ?? 2.0;
        }

        private MySqlConnection Conn() => new MySqlConnection(_connStr);

        public async Task<IEnumerable<Employee>> GetAllAsync()
        {
            const string sql = "SELECT id, name, department, email, salary FROM employees ORDER BY id DESC;";
            var sw = Stopwatch.StartNew();
            try
            {
                await using var conn = Conn();
                var rows = await conn.QueryAsync<Employee>(sql);
                sw.Stop();

                if (sw.Elapsed.TotalSeconds > _slowSeconds)
                    _log.Warn(nameof(GetAllAsync), $"Slow query ({sw.Elapsed.TotalSeconds:F2}s) for GetAllAsync");

                _log.Info(nameof(GetAllAsync), $"Fetched {rows.Count()} employees");
                return rows;
            }
            catch (Exception ex)
            {
                sw.Stop();
                _log.Error(nameof(GetAllAsync), "Failed to fetch employees", ex);
                throw new DatabaseException("Error fetching employees", ex);
            }
        }

        public async Task<Employee> GetByIdAsync(int id)
        {
            const string sql = "SELECT id, name, department, email, salary FROM employees WHERE id = @id;";
            try
            {
                await using var conn = Conn();
                var emp = await conn.QueryFirstOrDefaultAsync<Employee>(sql, new { id });
                if (emp == null)
                {
                    _log.Warn(nameof(GetByIdAsync), $"Employee {id} not found");
                    throw new NotFoundException("Employee", id);
                }
                _log.Info(nameof(GetByIdAsync), $"Employee {id} fetched");
                return emp;
            }
            catch (AppException) { throw; }
            catch (Exception ex)
            {
                _log.Error(nameof(GetByIdAsync), $"Failed to fetch employee {id}", ex);
                throw new DatabaseException("Error fetching employee", ex);
            }
        }

        public async Task<int> CreateAsync(Employee emp)
        {
            const string sql = @"
                INSERT INTO employees (name, department, email, salary)
                VALUES (@Name, @Department, @Email, @Salary);
                SELECT LAST_INSERT_ID();";
            try
            {
                await using var conn = Conn();
                var id = await conn.ExecuteScalarAsync<int>(sql, emp);
                _log.Info(nameof(CreateAsync), $"Created employee #{id} ({emp.Email})");
                return id;
            }
            catch (MySqlException ex) when (ex.ErrorCode == MySqlErrorCode.DuplicateKeyEntry)
            {
                _log.Error(nameof(CreateAsync), $"Duplicate email {emp.Email}", ex);
                throw new DuplicateEntityException("Employee", "Email", emp.Email);
            }
            catch (Exception ex)
            {
                _log.Error(nameof(CreateAsync), $"Failed to create employee ({emp.Email})", ex);
                throw new DatabaseException("Error creating employee", ex);
            }
        }

        public async Task<bool> UpdateAsync(Employee emp)
        {
            const string sql = @"
                UPDATE employees
                SET name=@Name, department=@Department, email=@Email, salary=@Salary
                WHERE id=@Id;";
            try
            {
                await using var conn = Conn();
                var rows = await conn.ExecuteAsync(sql, emp);
                if (rows == 0)
                {
                    _log.Warn(nameof(UpdateAsync), $"No rows updated for employee #{emp.Id}");
                    throw new NotFoundException("Employee", emp.Id);
                }
                _log.Info(nameof(UpdateAsync), $"Updated employee #{emp.Id}");
                return true;
            }
            catch (AppException) { throw; }
            catch (Exception ex)
            {
                _log.Error(nameof(UpdateAsync), $"Failed to update employee #{emp.Id}", ex);
                throw new DatabaseException("Error updating employee", ex);
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            const string sql = "DELETE FROM employees WHERE id=@id;";
            try
            {
                await using var conn = Conn();
                var rows = await conn.ExecuteAsync(sql, new { id });
                if (rows == 0)
                {
                    _log.Warn(nameof(DeleteAsync), $"No rows deleted for employee #{id}");
                    throw new NotFoundException("Employee", id);
                }
                _log.Info(nameof(DeleteAsync), $"Deleted employee #{id}");
                return true;
            }
            catch (AppException) { throw; }
            catch (Exception ex)
            {
                _log.Error(nameof(DeleteAsync), $"Failed to delete employee #{id}", ex);
                throw new DatabaseException("Error deleting employee", ex);
            }
        }
    }
}
