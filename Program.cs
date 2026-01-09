/*
 * PROGRAM:     staff
 * 
 * PURPOSE:     Implments an in-memory staff database; used as an exerciser for Entity Framework
 *              and LINQ.
 * 
 * AUTHOR:      Mark Parker
 *              January, 2026
 * 
 */

using Microsoft.EntityFrameworkCore;
using Staff.Database;

StaffDbContext? db = null;
var exit = false;

//  Command loop
while (!exit)
{

    // Give the command options
    Console.WriteLine("");
    Console.WriteLine("Commands");
    Console.WriteLine("");
    Console.WriteLine("    0. Exit program                            1. Create database");
    Console.WriteLine("    2. Add one department                      3. Add one employee");
    Console.WriteLine("    4. Add a standard batch of depts/employees 5. Delete an employee");
    Console.WriteLine("    6. List departments in alpha order         7. List employees in alpha order");
    Console.WriteLine("    8. Count employees                         9. Get highest earning employee");
    Console.WriteLine("   10. Get number of employees per dept       11. Get dept with most employees");
    Console.WriteLine("   12. Get total wage bill per dept           13. Get highest earning employee per dept");
    Console.WriteLine("");

    // Get the command
    var command = string.Empty;
    while (string.IsNullOrWhiteSpace(command))
    {
        Console.Write("Enter command: ");
        command = Console.ReadLine();
    }

    try
    {
        switch (command.Trim())
        {
            case "0":
                {
                    // Exit
                    exit = true;
                    break;
                }

            case "1":
                {
                    // Create database
                    var options = new DbContextOptionsBuilder<StaffDbContext>()
                        .UseInMemoryDatabase("TestDb")
                        .Options;

                    db = new StaffDbContext(options);

                    Console.WriteLine("Database created!");
                    break;
                }

            case "2":
                {
                    // Add one department
                    var department = new Department();
                    department.Name = getans("Enter new department name");

                    db.Add(department);
                    db.SaveChanges();

                    Console.Write($"Department '{department.Name}' added!");
                    break;
                }

            case "3":
                {
                    //  Add one employee
                    var employee = new Employee();
                    employee.Name = getans("Name of employee");
                    employee.Salary = float.Parse(getans("Salary"));

                    var deptName = getans("Department");
                    employee.DepartmentId = db.Departments.Single(d => d.Name == deptName).Id;

                    db.Add(employee);
                    db.SaveChanges();

                    Console.WriteLine($"\nEmployee '{employee.Name}' (id {employee.Id}) added!");
                    break;
                }

            case "4":
                {
                    //  Bulk add depts and employees
                    var hr = new Department { Name = "HR" };
                    var it = new Department { Name = "IT" };
                    var sales = new Department { Name = "Sales" };
                    var accounts = new Department { Name = "Accounts" };

                    db.Departments.AddRange(hr, it, sales, accounts);
                    db.SaveChanges();                                               // ensures IDs are generated

                    var newEmployees = new List<Employee>
                    {
                        new Employee { Name = "Jim Seller",      Salary=500000, Department = sales },
                        new Employee { Name = "Fred Closes",     Salary=425000, Department = sales },
                        new Employee { Name = "Sally Smooth",    Salary=120500, Department = sales },
                        new Employee { Name = "Nancy Nice",      Salary=320450, Department = sales },
                        new Employee { Name = "Pete Nerd",       Salary=95000,  Department = it },
                        new Employee { Name = "Steve N. Cables", Salary=45000,  Department = it },
                        new Employee { Name = "Willy Wonga",     Salary=108000, Department = accounts },
                        new Employee { Name = "Bill Cash",       Salary=195990, Department = accounts },
                        new Employee { Name = "Teresa Green",    Salary=85000,  Department = accounts },
                        new Employee { Name = "Dippy Docky",     Salary=23000,  Department = hr },
                        new Employee { Name = "Lucy Draper",     Salary=19000,  Department = hr }
                    };

                    db.Employees.AddRange(newEmployees);
                    db.SaveChanges();

                    Console.WriteLine($"Bulk add complete!");
                    break;
                }

            case "5":
                {
                    //  Delete an employee
                    var employeeId = getans("Employee ID");

                    var employeeToDelete = db.Employees.Single(x => x.Id == int.Parse(employeeId));
                    db.Employees.Remove(employeeToDelete);
                    db.SaveChanges();

                    Console.WriteLine($"{employeeToDelete.Name} gone!");
                    break;
                }

            case "6":
                {
                    //  List departments
                    var depts = db.Departments.OrderBy(x => x.Name).ToList();

                    Console.WriteLine();
                    Console.WriteLine($"Id         Name");
                    Console.WriteLine($"------------------------------------------");

                    foreach (var d in depts)
                    {
                        Console.WriteLine($"{d.Id,-10} {d.Name}");
                    }
                    Console.WriteLine($"");
                    break;
                }

            case "7":
                {
                    //  List employees
                    var employees = db.Employees.Include(x => x.Department).OrderBy(x => x.Name).ToList();

                    Console.WriteLine();
                    Console.WriteLine($"Id         Name                 Salary         Department");
                    Console.WriteLine($"---------------------------------------------------------");

                    foreach (var e in employees)
                    {
                        Console.WriteLine($"{e.Id,-10} {e.Name,-16} {e.Salary,15:C2}    {e.Department.Name}");
                    }
                    Console.WriteLine($"");
                    break;
                }

            case "8":
                {
                    //  Count the employees
                    var headCount = db.Employees.Count();

                    Console.WriteLine();
                    Console.WriteLine(headCount == 1
                        ? $"There is 1 employee"
                        : $"There are {headCount} employees");

                    break;
                }

            case "9":
                {
                    //  Get highest earning employee
                    var hee = db.Employees.Include(x => x.Department).OrderByDescending(x => x.Salary).Take(1).Single();

                    Console.WriteLine();
                    Console.WriteLine($"Highest earning employee: {hee.Name}, salary {hee.Salary:C2}, dept {hee.Department.Name}");
                    Console.WriteLine();

                    break;
                }

            case "10":
                {
                    //  Get number of employees per dept
                    var numEmpPerDept = db.Employees
                        .Include(d => d.Department)
                        .GroupBy(e => e.Department)
                        .Select(g => new { Department = g.Key.Name, NumberOfEmployees = g.Count() })
                        .OrderByDescending(o => o.NumberOfEmployees)
                        .ToList();

                    Console.WriteLine();
                    Console.WriteLine("Department         NumberOfEmployees");
                    Console.WriteLine("------------------------------------");

                    foreach (var nepd in numEmpPerDept)
                    {
                        Console.WriteLine($"{nepd.Department,-20} {nepd.NumberOfEmployees}");
                    }

                    Console.WriteLine();
                    break;
                }

            case "11":
                {
                    //  Get department with most employees
                    var dept = db.Employees
                        .Include(d => d.Department)
                        .GroupBy(e => e.Department)
                        .Select(g => new { Department = g.Key.Name, NumberOfEmployees = g.Count() })
                        .OrderByDescending(o => o.NumberOfEmployees)
                        .Take(1)
                        .Single();

                    Console.WriteLine();
                    Console.WriteLine($"Department with most employees is '{dept.Department}', with {dept.NumberOfEmployees} staff");
                    Console.WriteLine();
                    break;
                }

            case "12":
                {
                    //  Get total wage bill per dept
                    var depts = db.Employees
                        .Include(d => d.Department)
                        .GroupBy(e => e.Department)
                        .Select(g => new { g.Key.Name, NumberOfEmployees = g.Count(), TotalWageBill = g.Sum(w => w.Salary) })
                        .OrderByDescending(o => o.NumberOfEmployees)
                        .ToList();

                    Console.WriteLine();
                    Console.WriteLine($"Department    NumberOfEmployees     TotalWageBill");
                    Console.WriteLine($"-------------------------------------------------");

                    foreach (var dept in depts)
                    {
                        Console.WriteLine($"{dept.Name,-15} {dept.NumberOfEmployees,-20} {dept.TotalWageBill:C2}");
                    }

                    Console.WriteLine();
                    break;
                }

            case "13":
                {
                    //  Get the highest earning employee per department
                    var highestEarners = db.Employees
                        .GroupBy(e => e.Department)
                        .Select(g => new
                        {
                            Department = g.Key.Name,
                            Name = g.OrderByDescending(e => e.Salary).First().Name,
                            Salary = g.Max(e => e.Salary)
                        });

                    Console.WriteLine();
                    Console.WriteLine($"HighestEarner     Department                 Salary");
                    Console.WriteLine($"-----------------------------------------------------");

                    foreach (var hes in highestEarners)
                    {
                        Console.WriteLine($"{hes.Name,-15}   {hes.Department,-20} {hes.Salary,12:C2}");
                    }

                    Console.WriteLine();
                    break;
                }

            default:
                Console.Error.WriteLine($"\nCommand {command} not implemented yet.");
                break;
        }
    }
    catch (Exception ex)
    {
        Console.Error.WriteLine($"\nCommand failed: {ex.Message}\n");
    }
}

static string getans(string prompt)
{
    var answer = string.Empty;

    while (string.IsNullOrEmpty(answer))
    {
        Console.Write(prompt + ": ");
        answer = Console.ReadLine().Trim();
    }

    return answer;
}
