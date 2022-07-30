using Microsoft.EntityFrameworkCore;

namespace EmployeeManagementSystem.Models
{
    public static class ExtentionMethods
    {
        public static void Seed(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employee>().HasData(

                new Employee
                {
                    Id = 1,
                    Name = "Tomo",
                    Email = "Tomo@gmail.com",
                    Department = Dpt.Security
                },
                new Employee
                {
                    Id = 2,
                    Name = "Charlie",
                    Email = "Charlie@gmail.com",
                    Department = Dpt.Payroll
                }
                );

        }
    }
}
