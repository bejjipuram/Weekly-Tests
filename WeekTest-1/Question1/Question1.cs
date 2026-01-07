using System;

/// <summary>
/// in this class we have defined all the member variable which are needed for the operations
/// </summary>
public class SaleTransaction
{
    public string InvoiceNo { get; set; }
    public string CustomerName { get; set; }
    public string ItemName { get; set; }
    public int Quantity { get; set; }
    public decimal PurchaseAmount { get; set; }
    public decimal SellingAmount { get; set; }
    public string ProfitOrLossStatus { get; set; }
    public decimal ProfitOrLossAmount { get; set; }
    public decimal ProfitMarginPercent { get; set; }
}

/// <summary>
/// in this class we are performing the operations
/// </summary>
public class TransactionManager
{
    public static SaleTransaction LastTransaction;
    public static bool HasLastTransaction = false;

    /// <summary>
    /// this static function helps to create a new transaction, static keyword is used so that the garbage collector will not interfere until the program is finished
    /// </summary>
    public static void CreateTransaction()
    {
        SaleTransaction transaction = new SaleTransaction();

        // taking the invoice number and if condition is checking if it is null or contains spaces
        Console.Write("Enter Invoice No: ");
        transaction.InvoiceNo = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(transaction.InvoiceNo))
        {
            Console.WriteLine("Invoice No cannot be empty.");
            return;
        }

        // taking customer name and checking if it is null or empty
        Console.Write("Enter Customer Name: ");
        transaction.CustomerName = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(transaction.CustomerName))
        {
            Console.WriteLine("Customer name cannot be empty.");
            return;
        }

        // taking item name and checking if it is null or empty
        Console.Write("Enter Item Name: ");
        transaction.ItemName = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(transaction.ItemName))
        {
            Console.WriteLine("Item name cannot be empty.");
            return;
        }

        // taking the item quantity and returning that the quantity must be greater than zero
        Console.Write("Enter Quantity: ");
        if (!int.TryParse(Console.ReadLine(), out int qty) || qty <= 0)
        {
            Console.WriteLine("Quantity must be a positive number.");
            return;
        }
        transaction.Quantity = qty;

        // taking the purchase amount as input and checking if it is less than zero
        Console.Write("Enter Purchase Amount (total): ");
        if (!decimal.TryParse(Console.ReadLine(), out decimal purchaseAmt) || purchaseAmt <= 0)
        {
            Console.WriteLine("Purchase amount must be greater than zero.");
            return;
        }
        transaction.PurchaseAmount = purchaseAmt;

        // taking the selling price and returning that the selling price can not be zero or less than that
        Console.Write("Enter Selling Amount (total): ");
        if (!decimal.TryParse(Console.ReadLine(), out decimal sellingAmt) || sellingAmt < 0)
        {
            Console.WriteLine("Selling amount cannot be negative.");
            return;
        }
        transaction.SellingAmount = sellingAmt;

        // Calculating the profit and loss amounts
        CalculateProfitLoss(transaction);

        LastTransaction = transaction;
        HasLastTransaction = true;

        Console.WriteLine("\nTransaction saved successfully.");
        PrintCalculation(transaction);
        Console.WriteLine("------------------------------------------------------");
    }

    /// <summary>
    /// this class performs to print the previous transaction that has performed
    /// </summary>
    public static void ViewLastTransaction()
    {
        //checking if the boolean value "haslasttransaction" is false and if it is false then printing that there is no previous transaction that has performed
        if (!HasLastTransaction)
        {
            Console.WriteLine("No transaction available. Please create a new transaction first.");
            return;
        }

        Console.WriteLine("\n-------------- Last Transaction --------------");
        Console.WriteLine($"Invoice No: {LastTransaction.InvoiceNo}");
        Console.WriteLine($"Customer: {LastTransaction.CustomerName}");
        Console.WriteLine($"Item: {LastTransaction.ItemName}");
        Console.WriteLine($"Quantity: {LastTransaction.Quantity}");
        Console.WriteLine($"Purchase Amount: {LastTransaction.PurchaseAmount:F2}");
        Console.WriteLine($"Selling Amount: {LastTransaction.SellingAmount:F2}");
        Console.WriteLine($"Status: {LastTransaction.ProfitOrLossStatus}");
        Console.WriteLine($"Profit/Loss Amount: {LastTransaction.ProfitOrLossAmount:F2}");
        Console.WriteLine($"Profit Margin (%): {LastTransaction.ProfitMarginPercent:F2}");
        Console.WriteLine("--------------------------------------------");
    }

    /// <summary>
    /// this is the option 3 in the menu that is recalculating the profit and loss of the previous transaction
    /// </summary>
    public static void RecalculateProfitLoss()
    {
        //if there is no previous transaction then it will go into the if condition and prints the message
        if (!HasLastTransaction)
        {
            Console.WriteLine("No transaction available. Please create a new transaction first.");
            return;
        }

        CalculateProfitLoss(LastTransaction);
        PrintCalculation(LastTransaction);
        Console.WriteLine("------------------------------------------------------");
    }

    /// <summary>
    /// here we are calculating the profit and loss and printing the status whether it is profit, loss or break-even
    /// </summary>
    /// <param name="transaction"></param>
    private static void CalculateProfitLoss(SaleTransaction transaction)
    {
        if (transaction.SellingAmount > transaction.PurchaseAmount)
        {
            transaction.ProfitOrLossStatus = "PROFIT";
            transaction.ProfitOrLossAmount =
                transaction.SellingAmount - transaction.PurchaseAmount;
        }
        else if (transaction.SellingAmount < transaction.PurchaseAmount)
        {
            transaction.ProfitOrLossStatus = "LOSS";
            transaction.ProfitOrLossAmount =
                transaction.PurchaseAmount - transaction.SellingAmount;
        }
        else
        {
            transaction.ProfitOrLossStatus = "BREAK-EVEN";
            transaction.ProfitOrLossAmount = 0;
        }

        if (transaction.PurchaseAmount > 0)
        {
            transaction.ProfitMarginPercent =
                (transaction.ProfitOrLossAmount / transaction.PurchaseAmount) * 100;
        }
        else
        {
            transaction.ProfitMarginPercent = 0;
        }
    }

    /// <summary>
    /// here we are fetching the values from above functions and printing them"
    /// </summary>
    /// <param name="transaction"></param>
    private static void PrintCalculation(SaleTransaction transaction)
    {
        Console.WriteLine($"Status: {transaction.ProfitOrLossStatus}");
        Console.WriteLine($"Profit/Loss Amount: {transaction.ProfitOrLossAmount:F2}");
        Console.WriteLine($"Profit Margin (%): {transaction.ProfitMarginPercent:F2}");
    }
}

/// <summary>
/// in this main class we are using switch cases to choose which operation we wants to perform
/// </summary>
class Question1
{
    public static void Main(string[] args)
    {
        bool running = true;

        while (running)
        {
            Console.WriteLine("\n================== QuickMart Traders ==================");
            Console.WriteLine("1. Create New Transaction");
            Console.WriteLine("2. View Last Transaction");
            Console.WriteLine("3. Calculate Profit/Loss (Recompute)");
            Console.WriteLine("4. Exit");
            Console.Write("Enter your option: ");

            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    TransactionManager.CreateTransaction();
                    break;

                case "2":
                    TransactionManager.ViewLastTransaction();
                    break;

                case "3":
                    TransactionManager.RecalculateProfitLoss();
                    break;

                case "4":
                    Console.WriteLine("Thank you. Application closed normally.");
                    running = false;
                    break;

                default:
                    Console.WriteLine("Invalid option. Please select a valid menu choice.");
                    break;
            }
        }
    }
}
