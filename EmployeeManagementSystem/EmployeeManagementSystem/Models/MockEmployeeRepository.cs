namespace EmployeeManagementSystem.Models
{
    public class MockEmployeeRepository : IEmployeeRepository
    {
        private List<Employee> employees;

        public MockEmployeeRepository()
        {
            employees = new List<Employee>()
            {
                new Employee(){Id = 1, Name = "Yang", Email = "Yangryu321@gmail.com", Department = Dpt.IT},
                new Employee(){Id = 2, Name = "Tomo", Email = "Tomo@gmail.com", Department = Dpt.Security},
                new Employee(){Id = 3, Name = "Charlie", Email = "Charlie@gmail.com", Department = Dpt.Payroll},
                new Employee(){Id = 4, Name = "Misheru", Email = "mychlyn@hotmail.com", Department = Dpt.Insurance}
            };
                
        }

 
        public Employee Get(int id)
        {
            return employees.FirstOrDefault(x => x.Id == id);
            
        }

        public IEnumerable<Employee> GetAll()
        {
            return employees;
        }

        public Employee Add(Employee employee)
        {
            if (employee is not null)
            {
                employee.Id = employees.Count + 1;
                employees.Add(employee);
            }

            return employee;
        }

        public Employee Update(Employee employee)
        {
            Employee model = employees.FirstOrDefault(x => x.Id == employee.Id);
            if (model != null)
            {
                model.Email = employee.Email;
                model.Department = employee.Department;
                model.Name = employee.Name;
            }

            return employee;

        }

        public Employee Delete(int id)
        {
            Employee model = employees.FirstOrDefault(x => x.Id == id);
            if(model != null)
                employees.Remove(model);

            return model;
        }
    }
}
