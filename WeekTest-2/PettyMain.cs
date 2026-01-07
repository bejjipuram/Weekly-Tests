using System;
using System.Collections.Generic;
using System.Linq;

#region Interfaces and Base Classes

/// <summary>
/// Interface that forces implementing classes
/// to provide a summary of their data.
/// </summary>
public interface IReportable
{
    /// <summary>
    /// Returns a formatted summary string.
    /// </summary>
    string GetSummary();
}

/// <summary>
/// Abstract base class representing a financial transaction.
/// Implements IReportable to enforce summary behavior.
/// </summary>
public abstract class Transaction : IReportable
{
    /// <summary>
    /// Unique transaction identifier.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Date and time of the transaction.
    /// </summary>
    public DateTime Date { get; set; }

    /// <summary>
    /// Amount involved in the transaction.
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// Short description of the transaction.
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Abstract method to be implemented by child classes
    /// to return transaction-specific summaries.
    /// </summary>
    public abstract string GetSummary();
}

#endregion

#region Transaction Types

/// <summary>
/// Represents an expense transaction.
/// </summary>
public class ExpenseTransaction : Transaction
{
    /// <summary>
    /// Category of the expense (Office, Food, Travel, etc.).
    /// </summary>
    public string Category { get; set; }

    /// <summary>
    /// Returns formatted expense summary.
    /// </summary>
    public override string GetSummary()
    {
        return $"[ID: {Id}] Expense: {Amount:C} | {Date:yyyy-MM-dd} | {Description} | Category: {Category}";
    }
}

/// <summary>
/// Represents an income transaction.
/// </summary>
public class IncomeTransaction : Transaction
{
    /// <summary>
    /// Source of income (Bank, Cash, Salary, etc.).
    /// </summary>
    public string Source { get; set; }

    /// <summary>
    /// Returns formatted income summary.
    /// </summary>
    public override string GetSummary()
    {
        return $"[ID: {Id}] Income: {Amount:C} | {Date:yyyy-MM-dd} | {Description} | Source: {Source}";
    }
}

#endregion

#region Helper Classes

/// <summary>
/// Static helper class responsible for ledger calculations.
/// </summary>
public static class LedgerCalculator
{
    /// <summary>
    /// Calculates total amount from a list of transactions.
    /// </summary>
    /// <typeparam name="T">Transaction type</typeparam>
    /// <param name="transactions">List of transactions</param>
    /// <returns>Total amount</returns>
    public static decimal CalculateTotal<T>(List<T> transactions) where T : Transaction
    {
        return transactions.Sum(t => t.Amount);
    }
}

/// <summary>
/// Generic ledger class that stores transactions.
/// </summary>
/// <typeparam name="T">Type of transaction</typeparam>
public class Ledger<T> where T : Transaction
{
    /// <summary>
    /// Internal list to store transactions.
    /// </summary>
    private List<T> transactions = new List<T>();

    /// <summary>
    /// Adds a new transaction to the ledger.
    /// </summary>
    public void AddEntry(T entry)
    {
        transactions.Add(entry);
    }

    /// <summary>
    /// Returns all transactions in the ledger.
    /// </summary>
    public List<T> GetAll()
    {
        return transactions;
    }

    /// <summary>
    /// Calculates total amount using helper class.
    /// </summary>
    public decimal CalculateTotal()
    {
        return LedgerCalculator.CalculateTotal(transactions);
    }
}

#endregion

#region Main Class

/// <summary>
/// Main program class.
/// </summary>
public class PettyMain
{
    /// <summary>
    /// Application entry point.
    /// Provides menu-driven interaction.
    /// </summary>
    public static void Main()
    {
        // Ledgers for income and expenses
        var incomeLedger = new Ledger<IncomeTransaction>();
        var expenseLedger = new Ledger<ExpenseTransaction>();

        bool running = true;

        Console.WriteLine("***** Digital Petty Cash Ledger *****");

        // Loop until user chooses to exit
        while (running)
        {
            Console.WriteLine("\nMenu:");
            Console.WriteLine("1. Add Income");
            Console.WriteLine("2. Add Expense");
            Console.WriteLine("3. Calculate Totals");
            Console.WriteLine("4. Print Transaction Summary");
            Console.WriteLine("5. Exit");
            Console.Write("Enter your choice: ");

            int choice = ReadInt();

            switch (choice)
            {
                case 1:
                    AddIncome(incomeLedger);
                    break;

                case 2:
                    AddExpense(expenseLedger);
                    break;

                case 3:
                    ShowTotals(incomeLedger, expenseLedger);
                    break;

                case 4:
                    PrintSummary(incomeLedger, expenseLedger);
                    break;

                case 5:
                    running = false;
                    Console.WriteLine("Exiting program...");
                    break;

                default:
                    Console.WriteLine("Invalid choice. Try again.");
                    break;
            }
        }
    }

    /// <summary>
    /// Adds an income transaction.
    /// </summary>
    private static void AddIncome(Ledger<IncomeTransaction> ledger)
    {
        Console.Write("Enter amount: ");
        decimal amount = ReadDecimal();

        Console.Write("Enter description: ");
        string description = Console.ReadLine();

        Console.Write("Enter source: ");
        string source = Console.ReadLine();

        var income = new IncomeTransaction
        {
            Id = ledger.GetAll().Count + 1,
            Date = DateTime.Now,
            Amount = amount,
            Description = description,
            Source = source
        };

        ledger.AddEntry(income);
        Console.WriteLine("Income added successfully.");
    }

    /// <summary>
    /// Adds an expense transaction.
    /// </summary>
    private static void AddExpense(Ledger<ExpenseTransaction> ledger)
    {
        Console.Write("Enter amount: ");
        decimal amount = ReadDecimal();

        Console.Write("Enter description: ");
        string description = Console.ReadLine();

        Console.Write("Enter category: ");
        string category = Console.ReadLine();

        var expense = new ExpenseTransaction
        {
            Id = ledger.GetAll().Count + 1,
            Date = DateTime.Now,
            Amount = amount,
            Description = description,
            Category = category
        };

        ledger.AddEntry(expense);
        Console.WriteLine("Expense added successfully.");
    }

    /// <summary>
    /// Displays total income, expenses, and net balance.
    /// </summary>
    private static void ShowTotals(
        Ledger<IncomeTransaction> incomeLedger,
        Ledger<ExpenseTransaction> expenseLedger)
    {
        decimal totalIncome = incomeLedger.CalculateTotal();
        decimal totalExpense = expenseLedger.CalculateTotal();

        Console.WriteLine("\n=== Totals ===");
        Console.WriteLine($"Total Income: {totalIncome:C}");
        Console.WriteLine($"Total Expense: {totalExpense:C}");
        Console.WriteLine($"Net Balance: {(totalIncome - totalExpense):C}");
    }

    /// <summary>
    /// Prints all transactions using polymorphism.
    /// </summary>
    private static void PrintSummary(
        Ledger<IncomeTransaction> incomeLedger,
        Ledger<ExpenseTransaction> expenseLedger)
    {
        Console.WriteLine("\n=== Transaction Summary ===");

        var allTransactions = new List<Transaction>();
        allTransactions.AddRange(incomeLedger.GetAll());
        allTransactions.AddRange(expenseLedger.GetAll());

        if (allTransactions.Count == 0)
        {
            Console.WriteLine("No transactions recorded.");
            return;
        }

        foreach (var transaction in allTransactions)
        {
            Console.WriteLine(transaction.GetSummary());
        }
    }

    /// <summary>
    /// Reads a valid non-negative integer from the user.
    /// </summary>
    private static int ReadInt()
    {
        while (true)
        {
            if (int.TryParse(Console.ReadLine(), out int value) && value >= 0)
                return value;

            Console.Write("Invalid input. Enter a valid integer: ");
        }
    }

    /// <summary>
    /// Reads a valid non-negative decimal value from the user.
    /// </summary>
    private static decimal ReadDecimal()
    {
        while (true)
        {
            if (decimal.TryParse(Console.ReadLine(), out decimal value) && value >= 0)
                return value;

            Console.Write("Invalid amount. Enter a valid decimal: ");
        }
    }
}

#endregion
