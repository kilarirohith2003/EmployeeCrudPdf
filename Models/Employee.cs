using System.ComponentModel.DataAnnotations;

public class Employee
{
    public int Id { get; set; }

    [Required, StringLength(100)]
    public string Name { get; set; } = "";

    [Required, StringLength(100)]
    public string Department { get; set; } = "";

    [Required, EmailAddress, StringLength(255)]
    public string Email { get; set; } = "";

    [Range(0, 100000000)]
    public decimal Salary { get; set; }
}
