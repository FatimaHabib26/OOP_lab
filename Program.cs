using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransportManagementSystem1
{
  
        // ==========================================
        // 1. MODEL LAYER (Entities & Data Structures)
        // ==========================================

        // Represents a single passenger booking (Aggregation: A Bus contains many Bookings)
        public class Booking
        {
            private string passengerName;
            private string bookingDate;

            public Booking(string passengerName, string bookingDate)
            {
                this.passengerName = passengerName;
                this.bookingDate = bookingDate;
            }

            public string GetPassengerName() { return passengerName; }
            public string GetBookingDate() { return bookingDate; }
        }

        public class Bus
        {
            // Private Data Fields (Encapsulation)
            private int busNo;
            private string route;
            private string time;
            private int totalSeats;
            private int bookedSeats;

            // Aggregation: 1 Bus has a list of 1-to-Many Bookings
            private List<Booking> bookings;

            // Static member shared across the class
            private static string agencyName = "Daewoo Express & Transport";

            public Bus()
            {
                busNo = 0;
                route = "";
                time = "";
                totalSeats = 0;
                bookedSeats = 0;
                bookings = new List<Booking>();
            }

            public Bus(int busNo, string route, string time, int totalSeats, int bookedSeats)
            {
                SetBusNo(busNo);
                SetRoute(route);
                SetTime(time);
                SetTotalSeats(totalSeats);
                this.bookedSeats = bookedSeats;
                bookings = new List<Booking>();
            }

            // Static Method
            public static string GetAgencyName()
            {
                return agencyName;
            }

            // Getters and Setters with Validation (Encapsulation)
            public int GetBusNo() { return busNo; }
            public void SetBusNo(int busNo)
            {
                if (busNo > 0) this.busNo = busNo;
            }

            public string GetRoute() { return route; }
            public void SetRoute(string route)
            {
                if (!string.IsNullOrEmpty(route)) this.route = route;
            }

            public string GetTime() { return time; }
            public void SetTime(string time)
            {
                if (!string.IsNullOrEmpty(time)) this.time = time;
            }

            public int GetTotalSeats() { return totalSeats; }
            public void SetTotalSeats(int totalSeats)
            {
                if (totalSeats > 0) this.totalSeats = totalSeats;
            }

            public int GetBookedSeats() { return bookedSeats; }
            public List<Booking> GetBookings() { return bookings; }

            public int GetAvailableSeats()
            {
                return totalSeats - bookedSeats;
            }

            public void AddBooking(Booking booking)
            {
                if (bookedSeats < totalSeats)
                {
                    bookings.Add(booking);
                    bookedSeats++;
                }
            }

            public void RemoveBooking()
            {
                if (bookedSeats > 0 && bookings.Count > 0)
                {
                    bookings.RemoveAt(bookings.Count - 1);
                    bookedSeats--;
                }
            }

            // ORM / File Serialization Format (Converts object to text line)
            public override string ToString()
            {
                return $"{busNo},{route},{time},{totalSeats},{bookedSeats}";
            }
        }

        // ==========================================
        // 2. REPOSITORY / DATA ACCESS LAYER (DAL)
        // Handles reading/writing to physical text files
        // ==========================================
        public class BusRepository
        {
            private readonly string filePath = "buses.txt";

            public List<Bus> GetAllBusesFromFile()
            {
                List<Bus> buses = new List<Bus>();
                if (File.Exists(filePath))
                {
                    using (StreamReader reader = new StreamReader(filePath))
                    {
                        string line;
                        while ((line = reader.ReadLine()) != null)
                        {
                            string[] parts = line.Split(',');
                            if (parts.Length == 5)
                            {
                                int bNo = int.Parse(parts[0]);
                                string rt = parts[1];
                                string tm = parts[2];
                                int tSeats = int.Parse(parts[3]);
                                int bSeats = int.Parse(parts[4]);

                                Bus bus = new Bus(bNo, rt, tm, tSeats, bSeats);
                                buses.Add(bus);
                            }
                        }
                    }
                }
                else
                {
                    // Seed default records if the file doesn't exist yet
                    buses.Add(new Bus(101, "Lahore-Islamabad", "08:00AM", 40, 0));
                    buses.Add(new Bus(102, "Karachi-Lahore", "02:00PM", 35, 0));
                    buses.Add(new Bus(103, "Peshawar-Islamabad", "06:00PM", 45, 0));
                    SaveAllBusesToFile(buses);
                }
                return buses;
            }

            public void SaveAllBusesToFile(List<Bus> buses)
            {
                // Flushes and overwrites file with updated list state (as taught in lecture)
                using (StreamWriter writer = new StreamWriter(filePath, false))
                {
                    foreach (var bus in buses)
                    {
                        writer.WriteLine(bus.ToString());
                    }
                }
            }
        }

        // ==========================================
        // 3. BUSINESS LOGIC LAYER (SERVICE / BLL)
        // Handles searching, validation rules, operations 
        // (Strictly NO Console I/O here)
        // ==========================================
        public class BusService
        {
            private BusRepository repository;

            public BusService()
            {
                repository = new BusRepository();
            }

            public List<Bus> GetAllBuses()
            {
                return repository.GetAllBusesFromFile();
            }

            public bool AddBus(int busNo, string route, string time, int totalSeats)
            {
                List<Bus> buses = repository.GetAllBusesFromFile();
                foreach (var b in buses)
                {
                    if (b.GetBusNo() == busNo)
                        return false; // Bus already exists
                }

                Bus newBus = new Bus(busNo, route, time, totalSeats, 0);
                buses.Add(newBus);
                repository.SaveAllBusesToFile(buses);
                return true;
            }

            public Bus SearchBus(int busNo)
            {
                List<Bus> buses = repository.GetAllBusesFromFile();
                foreach (var b in buses)
                {
                    if (b.GetBusNo() == busNo)
                        return b;
                }
                return null;
            }

            public bool UpdateBus(int busNo, string newRoute, string newTime, int newTotalSeats)
            {
                List<Bus> buses = repository.GetAllBusesFromFile();
                bool found = false;
                foreach (var b in buses)
                {
                    if (b.GetBusNo() == busNo)
                    {
                        b.SetRoute(newRoute);
                        b.SetTime(newTime);
                        b.SetTotalSeats(newTotalSeats);
                        found = true;
                        break;
                    }
                }

                if (found)
                {
                    repository.SaveAllBusesToFile(buses);
                    return true;
                }
                return false;
            }

            public bool DeleteBus(int busNo)
            {
                List<Bus> buses = repository.GetAllBusesFromFile();
                Bus busToRemove = null;
                foreach (var b in buses)
                {
                    if (b.GetBusNo() == busNo)
                    {
                        busToRemove = b;
                        break;
                    }
                }

                if (busToRemove != null)
                {
                    buses.Remove(busToRemove);
                    repository.SaveAllBusesToFile(buses);
                    return true;
                }
                return false;
            }

            public bool BookSeat(int busNo, string passengerName)
            {
                List<Bus> buses = repository.GetAllBusesFromFile();
                bool success = false;
                foreach (var b in buses)
                {
                    if (b.GetBusNo() == busNo)
                    {
                        if (b.GetAvailableSeats() > 0)
                        {
                            b.AddBooking(new Booking(passengerName, DateTime.Now.ToString("yyyy-MM-dd")));
                            success = true;
                        }
                        break;
                    }
                }

                if (success)
                {
                    repository.SaveAllBusesToFile(buses);
                    return true;
                }
                return false;
            }

            public bool CancelBooking(int busNo)
            {
                List<Bus> buses = repository.GetAllBusesFromFile();
                bool success = false;
                foreach (var b in buses)
                {
                    if (b.GetBusNo() == busNo)
                    {
                        b.RemoveBooking();
                        success = true;
                        break;
                    }
                }

                if (success)
                {
                    repository.SaveAllBusesToFile(buses);
                    return true;
                }
                return false;
            }
        }

        // ==========================================
        // 4. PRESENTATION LAYER (UI) & DRIVER
        // Handles menus, console inputs, and outputs
        // ==========================================
        public class Program
        {
            static BusService busService = new BusService();
            static List<string> history = new List<string>();

            public static void Main(string[] args)
            {
                while (true)
                {
                    Console.Clear();
                    Console.WriteLine("============================================================");
                    Console.WriteLine($" ======= {Bus.GetAgencyName().ToUpper()} ======= ");
                    Console.WriteLine("============================================================");
                    Console.WriteLine("------USER MENU-----");
                    Console.WriteLine("1. Admin");
                    Console.WriteLine("2. Passenger");
                    Console.WriteLine("3. Exit");
                    Console.Write("Choose option: ");

                    string option = Console.ReadLine();
                    if (option == "1")
                    {
                        AdminLoginMenu();
                    }
                    else if (option == "2")
                    {
                        PassengerMenu();
                    }
                    else if (option == "3")
                    {
                        break;
                    }
                    else
                    {
                        Console.WriteLine("Wrong option! Press any key to continue...");
                        Console.ReadKey();
                    }
                }
                Console.WriteLine("Thanks for using this software!");
            }

            static void AdminLoginMenu()
            {
                Console.Clear();
                Console.WriteLine("====ADMIN LOGIN====");
                Console.Write("Enter username: ");
                string username = Console.ReadLine();
                Console.Write("Enter password: ");
                string password = Console.ReadLine();

                if (username == "admin" && password == "123")
                {
                    Console.WriteLine("Login Successfully!");
                    Console.ReadKey();
                    AdminMenu();
                }
                else
                {
                    Console.WriteLine("Invalid username or password");
                    Console.ReadKey();
                }
            }

            static void AdminMenu()
            {
                while (true)
                {
                    Console.Clear();
                    Console.WriteLine("=======ADMIN MENU======");
                    Console.WriteLine("1. Add Bus");
                    Console.WriteLine("2. View Buses");
                    Console.WriteLine("3. Search Bus");
                    Console.WriteLine("4. Update Bus");
                    Console.WriteLine("5. Delete Bus");
                    Console.WriteLine("6. Logout");
                    Console.Write("Enter your choice: ");

                    string choice = Console.ReadLine();
                    if (choice == "1")
                    {
                        Console.Write("Enter Bus Number: ");
                        int bNo = int.Parse(Console.ReadLine());
                        Console.Write("Enter Route: ");
                        string route = Console.ReadLine();
                        Console.Write("Enter Departure time: ");
                        string time = Console.ReadLine();
                        Console.Write("Enter Total Seats: ");
                        int seats = int.Parse(Console.ReadLine());

                        bool added = busService.AddBus(bNo, route, time, seats);
                        if (added)
                            Console.WriteLine("Bus Added Successfully");
                        else
                            Console.WriteLine("Bus number already exists!");

                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey();
                    }
                    else if (choice == "2")
                    {
                        List<Bus> buses = busService.GetAllBuses();
                        Console.WriteLine("\nBusNo\tRoute\t\t\tTime\t\tSeats\tBooked");
                        foreach (var b in buses)
                        {
                            Console.WriteLine($"{b.GetBusNo()}\t{b.GetRoute()}\t{b.GetTime()}\t\t{b.GetTotalSeats()}\t{b.GetBookedSeats()}");
                        }
                        Console.WriteLine("\nPress any key to continue...");
                        Console.ReadKey();
                    }
                    else if (choice == "3")
                    {
                        Console.Write("Enter bus number: ");
                        int num = int.Parse(Console.ReadLine());
                        Bus b = busService.SearchBus(num);
                        if (b != null)
                        {
                            Console.WriteLine("Bus Found!");
                            Console.WriteLine("Route : " + b.GetRoute());
                            Console.WriteLine("Time : " + b.GetTime());
                            Console.WriteLine("Available Seats: " + b.GetAvailableSeats());
                        }
                        else
                        {
                            Console.WriteLine("Bus not Found");
                        }
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey();
                    }
                    else if (choice == "4")
                    {
                        Console.Write("Enter bus number to update: ");
                        int num = int.Parse(Console.ReadLine());
                        Console.Write("Enter new route: ");
                        string route = Console.ReadLine();
                        Console.Write("Enter new time: ");
                        string time = Console.ReadLine();
                        Console.Write("Enter new seats: ");
                        int seats = int.Parse(Console.ReadLine());

                        bool updated = busService.UpdateBus(num, route, time, seats);
                        if (updated)
                            Console.WriteLine("Bus Updated Successfully");
                        else
                            Console.WriteLine("Bus not found");

                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey();
                    }
                    else if (choice == "5")
                    {
                        Console.Write("Enter bus number to delete: ");
                        int num = int.Parse(Console.ReadLine());
                        bool deleted = busService.DeleteBus(num);
                        if (deleted)
                            Console.WriteLine("Bus Deleted Successfully");
                        else
                            Console.WriteLine("Bus not found");

                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey();
                    }
                    else if (choice == "6")
                    {
                        break;
                    }
                }
            }

            static void PassengerMenu()
            {
                int bookedBusNo = -1;

                while (true)
                {
                    Console.Clear();
                    Console.WriteLine("=====Passenger Menu======");
                    Console.WriteLine("1. View Buses");
                    Console.WriteLine("2. Book Seat");
                    Console.WriteLine("3. Cancel Booking");
                    Console.WriteLine("4. My Booking");
                    Console.WriteLine("5. History");
                    Console.WriteLine("6. Exit");
                    Console.Write("Enter your choice: ");

                    string choice = Console.ReadLine();
                    if (choice == "1")
                    {
                        List<Bus> buses = busService.GetAllBuses();
                        Console.WriteLine("BusNo\tRoute\t\t\tTime\t\tAvailable Seats");
                        foreach (var b in buses)
                        {
                            Console.WriteLine($"{b.GetBusNo()}\t{b.GetRoute()}\t{b.GetTime()}\t\t{b.GetAvailableSeats()}");
                        }
                        Console.WriteLine("\nPress any key to continue...");
                        Console.ReadKey();
                    }
                    else if (choice == "2")
                    {
                        Console.Write("Enter bus number: ");
                        int num = int.Parse(Console.ReadLine());
                        Console.Write("Enter Passenger Name: ");
                        string pName = Console.ReadLine();

                        Bus b = busService.SearchBus(num);
                        if (b != null)
                        {
                            if (b.GetAvailableSeats() > 0)
                            {
                                busService.BookSeat(num, pName);
                                bookedBusNo = num;
                                Console.WriteLine("Seat booked successfully!");
                                history.Add($"Booked Bus {num} for {pName} at {b.GetTime()}");
                            }
                            else
                            {
                                Console.WriteLine("Bus is full");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Bus not found");
                        }
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey();
                    }
                    else if (choice == "3")
                    {
                        if (bookedBusNo != -1)
                        {
                            busService.CancelBooking(bookedBusNo);
                            Console.WriteLine("Booking cancelled");
                            history.Add($"Cancelled Booking for Bus {bookedBusNo}");
                            bookedBusNo = -1;
                        }
                        else
                        {
                            Console.WriteLine("No booking yet");
                        }
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey();
                    }
                    else if (choice == "4")
                    {
                        if (bookedBusNo != -1)
                        {
                            Bus b = busService.SearchBus(bookedBusNo);
                            if (b != null)
                            {
                                Console.WriteLine($"Your Bus: {b.GetBusNo()} on Route {b.GetRoute()} at {b.GetTime()}");
                            }
                        }
                        else
                        {
                            Console.WriteLine("No booking");
                        }
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey();
                    }
                    else if (choice == "5")
                    {
                        Console.WriteLine("History:");
                        if (history.Count == 0)
                        {
                            Console.WriteLine("No history yet");
                        }
                        else
                        {
                            foreach (var h in history)
                            {
                                Console.WriteLine(h);
                            }
                        }
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey();
                    }
                    else if (choice == "6")
                    {
                        break;
                    }
                }
            }
        }
    }

