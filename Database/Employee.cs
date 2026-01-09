using System;
using System.Collections.Generic;
using System.Text;

namespace Staff.Database
{
    internal class Employee
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public float Salary { get; set; }
        public int DepartmentId { get; set; }
        public Department Department { get; set; } = null!;
    }
}
