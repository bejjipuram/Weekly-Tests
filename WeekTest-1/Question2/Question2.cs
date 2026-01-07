using System;
    /// <summary>
    /// This is the main Class PatientBill
    /// </summary>
public class PatientBill
{
        /// <summary>
        /// Variable Declaration
        /// </summary>
        public string? BillId;
        public string? PatientName;
        public bool HasInsurance;
        public double ConsultationFee;
        public double LabCharges;
        public double MedicineCharges;
        public double GrossAmount;
        public double DiscountAmount;
        public double FinalPayable;
        public static bool HasLastBill;
        public static PatientBill? LastBill;

        /// <summary>
        /// Function to register Patient
        /// </summary>
        public void Register()
        {
            PatientBill b = new PatientBill();
            Console.Write("Enter bill id: ");
            b.BillId = Console.ReadLine()!;
            Console.Write("Enter Patient name: ");
            b.PatientName = Console.ReadLine()!;
            Console.Write("Enter (Y/N) has insurance: ");
            string? insurance = Console.ReadLine();
            b.HasInsurance = insurance == "Y" ? true : false;
            Console.Write("Enter consultation fee: ");
            while (!double.TryParse(Console.ReadLine(), out b.ConsultationFee) || b.ConsultationFee <= 0)
            {
                Console.WriteLine("Enter Value greater than 0. ReEnter");
            }
            Console.Write("Enter Lab fee: ");
            while (!double.TryParse(Console.ReadLine(), out b.LabCharges) || b.LabCharges < 0)
            {
                Console.WriteLine("Enter Value greater than 0. ReEnter");
            }
            Console.Write("Enter medical fee: ");
            while (!double.TryParse(Console.ReadLine(), out b.MedicineCharges) || b.MedicineCharges < 0)
            {
                Console.WriteLine("Enter Value greater than 0. ReEnter");
            }

            b.GrossAmount = b.ConsultationFee + b.LabCharges + b.MedicineCharges;
            if (HasInsurance)
            {
                DiscountAmount = GrossAmount * 0.10;
            }
            else
            {
                DiscountAmount = 0;
            }

            b.FinalPayable = Math.Round(GrossAmount - DiscountAmount, 2);
            Console.WriteLine("Bill Created Successfully");
            LastBill = b;
            HasLastBill = true;
        }

        /// <summary>
        /// Function to view last bill
        /// </summary>
        public void View()
        {
            if (!HasLastBill)
            {
                Console.WriteLine("No bill Available. Please Create a new bill first");
            }
            else
            {
                Console.WriteLine("YOUR LAST BILL: ");
                Console.WriteLine("BillId : " + LastBill?.BillId);
                Console.WriteLine("Patient name: " + LastBill?.PatientName);
                Console.WriteLine("Amount You paid : " + LastBill?.FinalPayable);
            }
        }

        /// <summary>
        /// Function to clear the Last Transaction
        /// </summary>
        public void Clear()
        {
            LastBill?.BillId = "";
            LastBill?.PatientName = "";
            LastBill?.FinalPayable = 0;
            LastBill?.ConsultationFee = 0;
            LastBill?.DiscountAmount = 0;
            LastBill?.GrossAmount = 0;
            LastBill?.HasInsurance = false;
            HasLastBill = false;
            Console.WriteLine("Last Bill Cleared.");
        }
}
public class Program
{
    static void Main(string[] args)
    {
        #region MediSure (This is the main program for PatientBill.cs)
        bool exit = false;
        while (!exit)
        {
            Console.WriteLine("------------------------------MediSure-----------------------------");
            Console.WriteLine("1. Create New Bill");
            Console.WriteLine("2. View Last Bill");
            Console.WriteLine("3. Clear Last Bill");
            Console.WriteLine("4. Exit");
            Console.WriteLine("Enter your choice: ");
            int choice = int.Parse(Console.ReadLine()!);
            PatientBill P = new PatientBill();
            switch (choice)
            {
                case 1:
                    P.Register();
                    break;
                case 2:
                    P.View();
                    break;
                case 3:
                    P.Clear();
                    break;
                case 4:
                    exit = true;
                    break;
            }
        }
    }
}
#endregion