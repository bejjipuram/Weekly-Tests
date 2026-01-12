using System;
using System.Collections.Generic;

namespace OnlineOrderProcessingSystem
{
    #region ENUM

    /// <summary>
    /// Represents order lifecycle states.
    /// </summary>
    public enum OrderStatus
    {
        Created,
        Paid,
        Packed,
        Shipped,
        Delivered,
        Cancelled
    }

    #endregion

    #region ENTITIES

    /// <summary>
    /// Represents a product.
    /// </summary>
    public class Product
    {
        public int Id { get; }
        public string Name { get; }
        public decimal Price { get; }
        public string Category { get; }

        public Product(int id, string name, decimal price, string category)
        {
            Id = id;
            Name = name;
            Price = price;
            Category = category;
        }
    }

    /// <summary>
    /// Represents a customer.
    /// </summary>
    public class Customer
    {
        public int Id { get; }
        public string Name { get; }
        public string Email { get; }

        public Customer(int id, string name, string email)
        {
            Id = id;
            Name = name;
            Email = email;
        }
    }

    /// <summary>
    /// Represents an item in an order.
    /// </summary>
    public class OrderItem
    {
        public Product Product { get; }
        public int Quantity { get; }

        public OrderItem(Product product, int quantity)
        {
            Product = product;
            Quantity = quantity;
        }

        public decimal GetTotal()
        {
            return Product.Price * Quantity;
        }
    }

    /// <summary>
    /// Stores order status history.
    /// </summary>
    public class OrderStatusLog
    {
        public OrderStatus OldStatus { get; }
        public OrderStatus NewStatus { get; }
        public DateTime ChangedAt { get; }

        public OrderStatusLog(OrderStatus oldStatus, OrderStatus newStatus)
        {
            OldStatus = oldStatus;
            NewStatus = newStatus;
            ChangedAt = DateTime.Now;
        }
    }

    /// <summary>
    /// Core Order class that owns business rules.
    /// </summary>
    public class Order
    {
        public int OrderId { get; }
        public Customer Customer { get; }
        public OrderStatus Status { get; private set; }

        public List<OrderItem> Items { get; } = new();
        public List<OrderStatusLog> StatusHistory { get; } = new();

        public Order(int id, Customer customer)
        {
            OrderId = id;
            Customer = customer;
            Status = OrderStatus.Created;
        }

        public void AddItem(OrderItem item)
        {
            Items.Add(item);
        }

        public decimal CalculateTotal()
        {
            decimal total = 0;
            foreach (var item in Items)
                total += item.GetTotal();
            return total;
        }

        public bool ChangeStatus(OrderStatus newStatus, out OrderStatus oldStatus)
        {
            oldStatus = Status;

            if (!IsValidTransition(Status, newStatus))
            {
                Console.WriteLine($"❌ Invalid transition: {Status} → {newStatus}");
                return false;
            }

            Status = newStatus;
            StatusHistory.Add(new OrderStatusLog(oldStatus, newStatus));
            return true;
        }

        private bool IsValidTransition(OrderStatus oldS, OrderStatus newS)
        {
            return (oldS, newS) switch
            {
                (OrderStatus.Created, OrderStatus.Paid) => true,
                (OrderStatus.Paid, OrderStatus.Packed) => true,
                (OrderStatus.Packed, OrderStatus.Shipped) => true,
                (OrderStatus.Shipped, OrderStatus.Delivered) => true,
                _ => false
            };
        }
    }

    #endregion

    #region NOTIFICATIONS

    /// <summary>
    /// Customer notification logic.
    /// </summary>
    public static class CustomerNotification
    {
        public static void Notify(Order order, OrderStatus oldS, OrderStatus newS)
        {
            Console.WriteLine(
                $"Email to {order.Customer.Email}: Order {order.OrderId} changed from {oldS} to {newS}"
            );
        }
    }

    /// <summary>
    /// Logistics notification logic.
    /// </summary>
    public static class LogisticsNotification
    {
        public static void Notify(Order order, OrderStatus oldS, OrderStatus newS)
        {
            if (newS == OrderStatus.Shipped)
            {
                Console.WriteLine(
                    $"Logistics notified: Order {order.OrderId} ready for delivery"
                );
            }
        }
    }

    #endregion

    #region REPORTING

    /// <summary>
    /// Handles order reporting.
    /// </summary>
    public static class OrderReportPrinter
    {
        public static void Print(Order order)
        {
            Console.WriteLine("\n----------------------------");
            Console.WriteLine($"Order ID   : {order.OrderId}");
            Console.WriteLine($"Customer   : {order.Customer.Name}");
            Console.WriteLine($"Status     : {order.Status}");
            Console.WriteLine("Items:");

            foreach (var item in order.Items)
                Console.WriteLine($"- {item.Product.Name} x {item.Quantity} = {item.GetTotal()}");

            Console.WriteLine($"Total      : {order.CalculateTotal()}");
            Console.WriteLine("Status History:");

            foreach (var log in order.StatusHistory)
                Console.WriteLine($"{log.ChangedAt} : {log.OldStatus} → {log.NewStatus}");
        }
    }

    #endregion

    #region Main Class

    public class ProgramMain
    {
        public static void Main()
        {
            Console.WriteLine("===== ONLINE ORDER PROCESSING SYSTEM =====");
            Console.WriteLine("1. Use Hard-Coded Sample Data");
            Console.WriteLine("2. Enter Data Manually");
            Console.Write("Choose option (1 or 2): ");

            int choice = int.Parse(Console.ReadLine()!);

            var products = new Dictionary<int, Product>();

            // KEY CHANGE: Orders grouped per customer
            var ordersPerCustomer = new Dictionary<int, List<Order>>();

            if (choice == 1)
                LoadHardCodedData(products, ordersPerCustomer);
            else
                LoadUserInputData(products, ordersPerCustomer);

            // Multicast delegate
            Action<Order, OrderStatus, OrderStatus> notifier;
            notifier = CustomerNotification.Notify;
            notifier += LogisticsNotification.Notify;

            // Process all orders of all customers
            foreach (var customerOrders in ordersPerCustomer.Values)
            {
                foreach (var order in customerOrders)
                {
                    ChangeStatus(order, OrderStatus.Paid, notifier);
                    ChangeStatus(order, OrderStatus.Packed, notifier);
                    ChangeStatus(order, OrderStatus.Shipped, notifier);
                }
            }

            // Final report (customer-wise)
            foreach (var customerOrders in ordersPerCustomer.Values)
            {
                foreach (var order in customerOrders)
                {
                    OrderReportPrinter.Print(order);
                }
            }
        }

        /// <summary>
        /// Loads predefined products, customers and multiple orders.
        /// </summary>
        static void LoadHardCodedData(
            Dictionary<int, Product> products,
            Dictionary<int, List<Order>> ordersPerCustomer)
        {
            products.Add(1, new Product(1, "Laptop", 60000, "Electronics"));
            products.Add(2, new Product(2, "Mouse", 500, "Electronics"));
            products.Add(3, new Product(3, "Keyboard", 1500, "Electronics"));
            products.Add(4, new Product(4, "BackPack", 600, "Accessories"));
            products.Add(5, new Product(5, "Mobile", 35000, "Electronics"));

            var c1 = new Customer(1, "Indra", "indra@mail.com");
            var c2 = new Customer(2, "Viswa", "viswa@gmail.com");

            ordersPerCustomer[c1.Id] = new List<Order>();
            ordersPerCustomer[c2.Id] = new List<Order>();

            var o1 = new Order(101, c1);
            o1.AddItem(new OrderItem(products[1], 1));
            o1.AddItem(new OrderItem(products[2], 2));

            var o2 = new Order(102, c1);
            o2.AddItem(new OrderItem(products[3], 1));

            var o3 = new Order(201, c2);
            o3.AddItem(new OrderItem(products[5], 1));
            o3.AddItem(new OrderItem(products[4], 1));

            ordersPerCustomer[c1.Id].Add(o1);
            ordersPerCustomer[c1.Id].Add(o2);
            ordersPerCustomer[c2.Id].Add(o3);
        }

        /// <summary>
        /// Accepts user input for multiple customers and multiple orders.
        /// </summary>
        static void LoadUserInputData(
            Dictionary<int, Product> products,
            Dictionary<int, List<Order>> ordersPerCustomer)
        {
            Console.Write("Enter number of products: ");
            int pCount = int.Parse(Console.ReadLine()!);

            for (int i = 0; i < pCount; i++)
            {
                Console.Write("Product Id: ");
                int id = int.Parse(Console.ReadLine()!);
                Console.Write("Name: ");
                string name = Console.ReadLine()!;
                Console.Write("Price: ");
                decimal price = decimal.Parse(Console.ReadLine()!);
                Console.Write("Category: ");
                string category = Console.ReadLine()!;

                products[id] = new Product(id, name, price, category);
            }

            Console.Write("Enter number of customers: ");
            int customerCount = int.Parse(Console.ReadLine()!);

            for (int c = 0; c < customerCount; c++)
            {
                Console.Write("\nCustomer Id: ");
                int cid = int.Parse(Console.ReadLine()!);
                Console.Write("Customer Name: ");
                string cname = Console.ReadLine()!;
                Console.Write("Email: ");
                string email = Console.ReadLine()!;

                var customer = new Customer(cid, cname, email);
                ordersPerCustomer[cid] = new List<Order>();

                Console.Write("How many orders for this customer: ");
                int orderCount = int.Parse(Console.ReadLine()!);

                for (int o = 0; o < orderCount; o++)
                {
                    Console.Write("Order Id: ");
                    int oid = int.Parse(Console.ReadLine()!);

                    var order = new Order(oid, customer);

                    Console.Write("How many items in this order: ");
                    int itemCount = int.Parse(Console.ReadLine()!);

                    for (int i = 0; i < itemCount; i++)
                    {
                        Console.Write("Product Id: ");
                        int pid = int.Parse(Console.ReadLine()!);
                        Console.Write("Quantity: ");
                        int qty = int.Parse(Console.ReadLine()!);

                        order.AddItem(new OrderItem(products[pid], qty));
                    }

                    ordersPerCustomer[cid].Add(order);
                }
            }
        }

        /// <summary>
        /// Changes order status and triggers delegate notifications.
        /// </summary>
        static void ChangeStatus(
            Order order,
            OrderStatus status,
            Action<Order, OrderStatus, OrderStatus> notifier)
        {
            if (order.ChangeStatus(status, out var oldStatus))
                notifier?.Invoke(order, oldStatus, status);
        }
    }
    #endregion
}