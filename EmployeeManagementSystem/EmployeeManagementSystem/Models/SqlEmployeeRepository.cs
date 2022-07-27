namespace EmployeeManagementSystem.Models
{
    public class SqlEmployeeRepository : IEmployeeRepository
    {
        private readonly AppDBContext dBContext;

        public SqlEmployeeRepository(AppDBContext dBContext)
        {
            this.dBContext = dBContext;
        }
        public Employee Add(Employee employee)
        {
            dBContext.Employees.Add(employee);
            dBContext.SaveChanges();    
            return employee;
        }

        public Employee Delete(int id)
        {
            Employee employee = dBContext.Employees.Find(id);
            dBContext.Employees.Remove(employee);
            dBContext.SaveChanges();
            return employee ;
        }

        public Employee Get(int id)
        {
            return dBContext.Employees.Find(id);
        }

        public IEnumerable<Employee> GetAll()
        {
            return dBContext.Employees.ToList();
        }

        public Employee Update(Employee employee)
        {
            var model = dBContext.Employees.Attach(employee);
            model.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            dBContext.SaveChanges();
            return employee;

        }
    }
}
