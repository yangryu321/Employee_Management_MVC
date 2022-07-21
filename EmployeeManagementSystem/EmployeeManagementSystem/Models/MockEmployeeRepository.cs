namespace EmployeeManagementSystem.Models
{
    public class MockEmployeeRepository : IEmployeeRepository
    {
        private List<Employee> employees;

        public MockEmployeeRepository()
        {
            employees = new List<Employee>()
            {
                new Employee(){Id = 1, Name = "Yang", Email = "Yangryu321@gmail.com", Department = "IT"},
                new Employee(){Id = 2, Name = "Tomo", Email = "Tomo@gmail.com", Department = "Security"},
                new Employee(){Id = 3, Name = "Charlie", Email = "Charlie@gmail.com", Department = "Payroll"},
                new Employee(){Id = 4, Name = "Misheru", Email = "mychlyn@hotmail.com", Department = "Insurance"}
            };
                
        }
        public Employee Get(int id)
        {
            return employees.FirstOrDefault(x => x.Id == id);
            
        }

    }
}
