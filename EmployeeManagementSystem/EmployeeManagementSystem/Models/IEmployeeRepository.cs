namespace EmployeeManagementSystem.Models
{
    public interface IEmployeeRepository
    {
        Employee Get(int id);
        IEnumerable<Employee> GetAll();
        Employee Add(Employee employee);
    }
}
