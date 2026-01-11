namespace Staff.Database
{
    internal class Employee
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Salary { get; set; }
        public int DepartmentId { get; set; }
        public Department Department { get; set; } = null!;
    }
}
