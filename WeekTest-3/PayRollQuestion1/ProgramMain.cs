using System;
using System.Collections.Generic;
using System.Linq;

namespace PayRollQuestion1
{
    // ======================================================
    // ABSTRACT BASE CLASS : EMPLOYEE
    // Defines common properties and behavior for all employees
    // ======================================================
    public abstract class Employee
    {
        /// <summary>
        /// Unique identifier of the employee
        /// </summary>
        public int EmployeeId { get; }

        /// <summary>
        /// Name of the employee
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Employee category (Full-Time / Contract)
        /// </summary>
        public string EmployeeType { get; protected set; }

        /// <summary>
        /// Base constructor with validation
        /// </summary>
        protected Employee(int id, string name)
        {
            if (id <= 0)
                throw new ArgumentException("Employee ID must be positive");

            EmployeeId = id;
            Name = name;
        }

        /// <summary>
        /// Abstract method for salary calculation
        /// Implemented differently by derived classes
        /// </summary>
        public abstract PaySlip CalculatePay();
    }

    // ======================================================
    // FULL-TIME EMPLOYEE
    // Salary = Monthly Salary - (Tax + Insurance)
    // ======================================================
    public class FullTimeEmployee : Employee
    {
        public double MonthlySalary { get; }
        public double TaxRate { get; }
        public double Insurance { get; }

        /// <summary>
        /// Initializes a full-time employee
        /// </summary>
        public FullTimeEmployee(int id, string name, double salary,
                                double taxRate, double insurance)
            : base(id, name)
        {
            if (salary < 0)
                throw new ArgumentException("Salary cannot be negative");

            EmployeeType = "Full-Time";
            MonthlySalary = salary;
            TaxRate = taxRate;
            Insurance = insurance;
        }

        /// <summary>
        /// Calculates net salary for full-time employee
        /// </summary>
        public override PaySlip CalculatePay()
        {
            double tax = MonthlySalary * TaxRate;
            double deductions = tax + Insurance;
            double netSalary = MonthlySalary - deductions;

            return new PaySlip(EmployeeId, Name, EmployeeType,
                               MonthlySalary, deductions, netSalary);
        }
    }

    // ======================================================
    // CONTRACT EMPLOYEE
    // Salary = Daily Rate × Working Days
    // ======================================================
    public class ContractEmployee : Employee
    {
        public double DailyRate { get; }
        public int WorkingDays { get; }

        /// <summary>
        /// Initializes a contract employee
        /// </summary>
        public ContractEmployee(int id, string name, double rate, int days)
            : base(id, name)
        {
            if (rate < 0 || days < 0 || days > 31)
                throw new ArgumentException("Invalid rate or working days");

            EmployeeType = "Contract";
            DailyRate = rate;
            WorkingDays = days;
        }

        /// <summary>
        /// Calculates salary for contract employee
        /// </summary>
        public override PaySlip CalculatePay()
        {
            double grossSalary = DailyRate * WorkingDays;

            return new PaySlip(EmployeeId, Name, EmployeeType,
                               grossSalary, 0, grossSalary);
        }
    }

    // ======================================================
    // PAYSLIP
    // Holds payroll calculation result for an employee
    // ======================================================
    public class PaySlip
    {
        public int EmployeeId { get; }
        public string Name { get; }
        public string EmployeeType { get; }
        public double GrossSalary { get; }
        public double Deductions { get; }
        public double NetSalary { get; }

        /// <summary>
        /// Initializes a PaySlip
        /// </summary>
        public PaySlip(int id, string name, string type,
                       double gross, double deductions, double net)
        {
            EmployeeId = id;
            Name = name;
            EmployeeType = type;
            GrossSalary = gross;
            Deductions = deductions;
            NetSalary = net;
        }
    }

    // ======================================================
    // NOTIFICATION SERVICE
    // Handles notifications using delegates
    // ======================================================
    public static class NotificationService
    {
        /// <summary>
        /// Notifies HR department after salary processing
        /// </summary>
        public static void NotifyHR(PaySlip slip)
        {
            Console.WriteLine($"[HR] Salary processed for {slip.Name}");
        }

        /// <summary>
        /// Notifies Finance department for payment initiation
        /// </summary>
        public static void NotifyFinance(PaySlip slip)
        {
            Console.WriteLine($"[Finance] Payment initiated for {slip.Name}");
        }
    }

    // ======================================================
    // EMPLOYEE REPOSITORY (STATIC STORAGE)
    // Acts as an in-memory database
    // ======================================================
    public static class EmployeeRepository
    {
        private static Dictionary<int, Employee> _employees =
            new Dictionary<int, Employee>();

        /// <summary>
        /// Returns employees and loads hard-coded data once
        /// </summary>
        public static Dictionary<int, Employee> GetEmployees()
        {
            if (_employees.Count == 0)
            {
                _employees.Add(101, new FullTimeEmployee(101, "Rahul", 50000, 0.10, 2000));
                _employees.Add(102, new ContractEmployee(102, "Amit", 1200, 22));
                _employees.Add(103, new FullTimeEmployee(103, "Sneha", 65000, 0.12, 2500));
            }
            return _employees;
        }

        /// <summary>
        /// Adds a new employee to the repository
        /// </summary>
        public static void AddEmployee(Employee emp)
        {
            if (_employees.ContainsKey(emp.EmployeeId))
                throw new Exception("Duplicate Employee ID");

            _employees.Add(emp.EmployeeId, emp);
        }
    }

    // ======================================================
    // PAYROLL PROCESSOR
    // Handles payroll calculation and reporting
    // ======================================================
    public class PayrollProcessor
    {
        private Dictionary<int, Employee> _employees;
        private List<PaySlip> _paySlips = new List<PaySlip>();

        /// <summary>
        /// Delegate triggered after salary is processed
        /// </summary>
        public Action<PaySlip> SalaryProcessed;

        /// <summary>
        /// Initializes payroll processor
        /// </summary>
        public PayrollProcessor(Dictionary<int, Employee> employees)
        {
            _employees = employees;
        }

        /// <summary>
        /// Processes payroll using polymorphism
        /// </summary>
        public void ProcessPayroll()
        {
            foreach (var emp in _employees.Values)
            {
                PaySlip slip = emp.CalculatePay();
                _paySlips.Add(slip);

                Console.WriteLine(
                    $"Processed: {slip.Name} | Gross: {slip.GrossSalary} | Net: {slip.NetSalary}");

                // Invoke all subscribed notifications
                SalaryProcessed?.Invoke(slip);

                Console.WriteLine("*********************************");
            }

            GenerateSummary();
        }

        /// <summary>
        /// Generates payroll summary using LINQ
        /// </summary>
        private void GenerateSummary()
        {
            Console.WriteLine("\n===== PAYROLL SUMMARY =====");
            Console.WriteLine($"Total Employees: {_paySlips.Count}");
            Console.WriteLine($"Total Payout: {_paySlips.Sum(p => p.NetSalary)}");

            foreach (var group in _paySlips.GroupBy(p => p.EmployeeType))
            {
                Console.WriteLine($"{group.Key} Employees: {group.Count()}");
            }

            var highestPaid = _paySlips.OrderByDescending(p => p.NetSalary).First();
            Console.WriteLine($"Highest Paid Employee: {highestPaid.Name} ({highestPaid.NetSalary})");
        }
    }

    // ======================================================
    // PROGRAM ENTRY POINT
    // ======================================================
    public class ProgramMain
    {
        public static void Main()
        {
            // Load employees from repository
            var employees = EmployeeRepository.GetEmployees();

            // Initialize payroll processor
            PayrollProcessor processor = new PayrollProcessor(employees);

            // Subscribe notifications
            processor.SalaryProcessed += NotificationService.NotifyHR;
            processor.SalaryProcessed += NotificationService.NotifyFinance;

            Console.WriteLine("===== PAYROLL SYSTEM =====");
            Console.WriteLine("1. Use Hard-Coded Employees");
            Console.WriteLine("2. Add Employees Manually");
            Console.Write("Enter choice: ");

            int choice = int.Parse(Console.ReadLine());

            // Add employees if user chooses manual input
            if (choice == 2)
                AddEmployeesByUserInput();

            // Process payroll
            processor.ProcessPayroll();
        }

        /// <summary>
        /// Accepts user input and adds employees to repository
        /// </summary>
        private static void AddEmployeesByUserInput()
        {
            Console.Write("\nEnter number of employees to add: ");
            int count = int.Parse(Console.ReadLine());

            for (int i = 0; i < count; i++)
            {
                try
                {
                    Console.Write("\nEmployee ID: ");
                    int id = int.Parse(Console.ReadLine());

                    Console.Write("Name: ");
                    string name = Console.ReadLine();

                    Console.Write("Type (1 = Full-Time, 2 = Contract): ");
                    int type = int.Parse(Console.ReadLine());

                    Employee emp;

                    if (type == 1)
                    {
                        Console.Write("Monthly Salary: ");
                        double salary = double.Parse(Console.ReadLine());

                        Console.Write("Tax Rate: ");
                        double tax = double.Parse(Console.ReadLine());

                        Console.Write("Insurance: ");
                        double insurance = double.Parse(Console.ReadLine());

                        emp = new FullTimeEmployee(id, name, salary, tax, insurance);
                    }
                    else if (type == 2)
                    {
                        Console.Write("Daily Rate: ");
                        double rate = double.Parse(Console.ReadLine());

                        Console.Write("Working Days: ");
                        int days = int.Parse(Console.ReadLine());

                        emp = new ContractEmployee(id, name, rate, days);
                    }
                    else
                    {
                        Console.WriteLine("Invalid type.");
                        i--;
                        continue;
                    }

                    EmployeeRepository.AddEmployee(emp);
                    Console.WriteLine("Employee added successfully!");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                    i--;
                }
            }
        }
    }
}
