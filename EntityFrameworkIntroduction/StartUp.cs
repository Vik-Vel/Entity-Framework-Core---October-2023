using Microsoft.VisualBasic;
using SoftUni.Data;
using SoftUni.Models;
using System.Globalization;
using System.Text;
using System.Xml.Linq;

namespace SoftUni
{
    public class StartUp
    {
        static void Main(string[] args)
        {
            var context = new SoftUniContext();
            string output = string.Empty;

            //03.
            //output = GetEmployeesFullInformation(context);

            //04.
            //output = GetEmployeesWithSalaryOver50000(context);

            //05.
            //output = GetEmployeesFromResearchAndDevelopment(context);

            //06.
            //output = AddNewAddressToEmployee(context);

            //07.
            //output = GetEmployeesInPeriod(context);

            //08.
            //output = GetAddressesByTown(context);

            //09.
            //output = GetEmployee147(context);

            //10.
            //output = GetDepartmentsWithMoreThan5Employees(context);

            //11.
            //output = GetLatestProjects(context);

            //12.
            //output = IncreaseSalaries(context);

            //13.
            //output = GetEmployeesByFirstNameStartingWithSa(context);

            //14.
            //output = DeleteProjectById(context);

            //15.
            output = RemoveTown(context);

            Console.WriteLine(output);
        }

        //03.Employees Full Information
        public static string GetEmployeesFullInformation(SoftUniContext context)
        {
            var employees = context.Employees
                .Select(e => new
                {
                    e.FirstName,
                    e.LastName,
                    e.MiddleName,
                    e.JobTitle,
                    e.Salary
                }).ToList();


            //Console.WriteLine(employees.ToQueryString());

            string result = string.Join(Environment.NewLine,
                employees.Select(e => $"{e.FirstName} {e.LastName} {e.MiddleName} {e.JobTitle} {e.Salary:f2}"));

            return result;
        }

        //04.Employees with Salary Over 50,000
        public static string GetEmployeesWithSalaryOver50000(SoftUniContext context)
        {
            var employees = context.Employees
                .Select(e => new
                {
                    e.FirstName,
                    e.Salary
                })
                .Where(e => e.Salary > 50000)
                .OrderBy(e => e.FirstName)
                .ToList();

            string result = string.Join(Environment.NewLine,
                employees.Select(e => $"{e.FirstName} - {e.Salary:f2}"));

            return result;
        }

        //05.Employees from Research and Development
        public static string GetEmployeesFromResearchAndDevelopment(SoftUniContext context)
        {
            var employees = context.Employees
                .Where(e => e.Department.Name == "Research and Development")
                .Select(e => new
                {
                    e.FirstName,
                    e.LastName,
                    e.Department.Name,
                    e.Salary
                })
                .OrderBy(e => e.Salary)
                .ThenByDescending(e => e.FirstName);

            string result = string.Join(Environment.NewLine,
                employees.Select(e => $"{e.FirstName} {e.LastName} from {e.Name} - ${e.Salary:f2}"));

            return result;
        }

        ////06.Adding a New Address and Updating Employee
        public static string AddNewAddressToEmployee(SoftUniContext context)
        {
            Address address = new Address()
            {
                AddressText = "Vitoshka 15",
                TownId = 4
            };

            var employee = context.Employees
                .FirstOrDefault(e => e.LastName == "Nakov");

            employee.Address = address;

            context.SaveChanges();

            var employees = context.Employees
                .Select(e => new
                {
                    e.AddressId,
                    e.Address.AddressText
                })
                .OrderByDescending(e => e.AddressId)
                .Take(10)
                .ToList();

            return string.Join(Environment.NewLine, employees.Select(e => $"{e.AddressText}"));


        }

        //07.Employees and Projects
        public static string GetEmployeesInPeriod(SoftUniContext context)
        {
            var EmployeeInfo = context.Employees
                .Take(10)
                .Select(e => new
                {
                    e.FirstName,
                    e.LastName,
                    ManagerFirstName = e.Manager.FirstName,
                    ManagerLastName = e.Manager.LastName,
                    Projects = e.EmployeesProjects.Where(ep => ep.Project.StartDate.Year >= 2001 & ep.Project.StartDate.Year <= 2003)
                        .Select(ep => new
                        {
                            ProjectName = ep.Project.Name,
                            StartDate = ep.Project.StartDate.ToString("M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture),
                            EndDate = ep.Project.EndDate != null
                                ? ep.Project.EndDate.Value.ToString("M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture)
                                : "not finished"
                        })
                })
                .ToList();

            StringBuilder sb = new StringBuilder();

            foreach (var e in EmployeeInfo)
            {
                sb.AppendLine($"{e.FirstName} {e.LastName} - Manager: {e.ManagerFirstName} {e.ManagerLastName}");

                if (e.Projects.Any())
                {
                    sb.AppendLine(String.Join(Environment.NewLine, e.Projects
                        .Select(p => $"--{p.ProjectName} - {p.StartDate} - {p.EndDate}")));
                }
            }

            return sb.ToString().TrimEnd();
        }

        //08.Addresses by Town
        public static string GetAddressesByTown(SoftUniContext context)
        {
            //string[] addressesInfo = context.Addresses
            //    .OrderByDescending(a => a.Employees.Count)
            //    .ThenBy(a => a.Town.Name)
            //    .ThenBy(a => a.AddressText)
            //    .Take(10)
            //    .Select(a => $"{a.AddressText}, {a.Town.Name} - {a.Employees.Count} employees")
            //    .ToArray();

            //return string.Join(Environment.NewLine, addressesInfo);


            var addressInfo = context.Addresses
                .Select(a => new
                {
                    a.AddressText,
                    a.Town.Name,
                    a.Employees.Count
                })
                .OrderByDescending(a => a.Count)
                .ThenBy(a => a.Name)
                .ThenBy(a => a.AddressText)
                .Take(10)
                .ToList();

            return string.Join(Environment.NewLine, addressInfo
                .Select(a=>$"{a.AddressText}, {a.Name} - {a.Count} employees"));

        }

        //09.Employee 147
        public static string GetEmployee147(SoftUniContext context)
        {
            var employee147Info = context.Employees
                .Where(e => e.EmployeeId == 147)
                .Select(x => new
                {
                    x.FirstName,
                    x.LastName,
                    x.JobTitle,
                    Projects = x.EmployeesProjects.Select(p => new { p.Project.Name }).OrderBy(p => p.Name).ToArray()
                })
                .FirstOrDefault();

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"{employee147Info.FirstName} {employee147Info.LastName} - {employee147Info.JobTitle}");
            sb.Append(string.Join(Environment.NewLine, employee147Info.Projects.Select(p => p.Name)));

           
             return sb.ToString().TrimEnd();
        }
        //10. Departments with More Than 5 Employees 
        public static string GetDepartmentsWithMoreThan5Employees(SoftUniContext context)
        {
            var departments = context.Departments
                .Where(d => d.Employees.Count > 5)
                .OrderBy(e => e.Employees.Count)
                .ThenBy(e => e.Name)
                .Select(e => new
                {
                    DepartmentName = e.Name,
                    ManagerName = e.Manager.FirstName + " " + e.Manager.LastName,
                    Employees = e.Employees
                        .OrderBy(e => e.FirstName)
                        .ThenBy(e => e.LastName)
                        .Select(e => new
                        {
                            EmployeeData = $"{e.FirstName} {e.LastName} - {e.JobTitle}"
                        })
                        .ToArray()
                })
                .ToArray();

            StringBuilder sb = new StringBuilder();

            foreach (var d in departments)
            {
                sb.AppendLine($"{d.DepartmentName} - {d.ManagerName}");
                sb.Append(string.Join(Environment.NewLine, d.Employees.Select(e => e.EmployeeData)));
            }
            //sb.AppendLine($"{employee147Info.FirstName} {employee147Info.LastName} - {employee147Info.JobTitle}");: Добавя в StringBuilder ред с първо име, фамилия и длъжност на служителя.
            //sb.Append(string.Join(Environment.NewLine, employee147Info.Projects.Select(p => p.Name)));: Добавя имената на проектите, на които работи служителят, като ги обединява с нов ред (Environment.NewLine се използва за нов ред) и ги добавя към StringBuilder.

            return sb.ToString().TrimEnd();
        }

        //11. Find Latest 10 Projects 
        public static string GetLatestProjects(SoftUniContext context)
        {
            var lastProject = context.Projects
                .OrderByDescending(p => p.StartDate)
                .Take(10)
                .OrderBy(p => p.Name)
                .Select( p => new
                {
                    p.Name,
                    p.Description,
                    StartDate = p.StartDate.ToString("M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture),
                })
                .ToArray();

            StringBuilder sb = new StringBuilder();

            foreach (var d in lastProject)
            {
                sb.AppendLine(d.Name);
                sb.AppendLine(d.Description);
                sb.AppendLine(d.StartDate);
            }
                

            return sb.ToString().TrimEnd();
        }

        //12.	Increase Salaries
        public static string IncreaseSalaries(SoftUniContext context)
        {
            decimal salaryModifier = 1.12m;
            string[] departmentNames = new string[]{ "Engineering", "Tool Design", "Marketing", "Information Services" };

            var employeesForSalaryIncrease = context.Employees
                .Where(e => departmentNames.Contains(e.Department.Name))
                .ToArray();

            foreach (var e in employeesForSalaryIncrease)
            {
                e.Salary *= salaryModifier;
            }

            context.SaveChanges();

            string[] employeesInfo = context.Employees
                .Where(e => departmentNames.Contains(e.Department.Name))
                .OrderBy(e => e.FirstName)
                .ThenBy(e => e.LastName)
                .Select(e => $"{e.FirstName} {e.LastName} (${e.Salary:f2})")
                .ToArray();

            return string.Join(Environment.NewLine, employeesInfo);
        }

        //13.Find Employees by First Name Starting With Sa
        public static string GetEmployeesByFirstNameStartingWithSa(SoftUniContext context)
        {
            var employee = context.Employees
                .Take(10)
                .Select(e => new
                {
                    e.FirstName,
                    e.LastName,
                    e.JobTitle,
                    e.Salary
                })
                .Where(e => e.FirstName.StartsWith("Sa")) //Where(e=>EF.Functions.Like(e.FirstName,'sa%'));
                .OrderBy(e => e.FirstName)
                .ThenBy(e => e.LastName)
                .ToList();

            return string.Join(Environment.NewLine,
                employee.Select(e => $"{e.FirstName} {e.LastName} - {e.JobTitle} - (${e.Salary:f2})"));
        }

        //14.	Delete Project by Id
        public static string DeleteProjectById(SoftUniContext context)
        {
             int projectIdd = 2;


             var employeessProjectsToDelete = context.EmployeesProjects
                 .Where(p => p.ProjectId == projectIdd);

             context.EmployeesProjects.RemoveRange(employeessProjectsToDelete);

            var projectsToDelete = context.Projects
                 .Where(p => p.ProjectId == projectIdd);

             context.Projects.RemoveRange(projectsToDelete);



             context.SaveChanges();

             string[] returned = context.Projects
                 .Take(10)
                 .Select(p => p.Name)
                 .ToArray();

             return string.Join(Environment.NewLine,returned);
        }

        //15.	Remove Town

        public static string RemoveTown(SoftUniContext context)
        {
            string nameOfDeletedTown = "Seattle";

            var deletedTown = context.Towns
                .Where(t => t.Name == nameOfDeletedTown);

            var addresses = context.Addresses
                .Where(a => a.Town.Name == nameOfDeletedTown);

            var employeesToRemoveAddressFrom = context.Employees
                .Where(e => addresses
                    .Contains(e.Address))
                .ToArray();

            int count = addresses.Count();

            foreach (Employee e in employeesToRemoveAddressFrom)
            {
                e.AddressId = null;
            }

            context.Addresses.RemoveRange(addresses);
            context.Towns.RemoveRange(deletedTown);

            context.SaveChanges();


            return $"{count} addresses in Seattle were deleted";


            /*Town townToDelete = context.Towns
                .Where(t => t.Name == "Seattle")
                .FirstOrDefault();

        Address[] addressesToDelete = context.Addresses
            .Where(a => a.TownId == townToDelete.TownId)
            .ToArray();

        Employee[] employeesToRemoveAddressFrom = context.Employees
            .Where(e => addressesToDelete
            .Contains(e.Address))
            .ToArray();

        foreach (Employee e in employeesToRemoveAddressFrom)
        {
            e.AddressId = null;
        }

        context.Addresses.RemoveRange(addressesToDelete);
        context.Towns.Remove(townToDelete);
        context.SaveChanges();

        return $"{addressesToDelete.Count()} addresses in Seattle were deleted";*/
        }
    }

}