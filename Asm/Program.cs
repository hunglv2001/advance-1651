using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Asm
{
    internal interface IHotelRoom
    {
        int RoomNo { get; set; }
        bool IsAvailable { get; set; }
        double Price { get; set; }
        string Guest { get; set; }
        string GetDescription();
        string DisplayPrice();
    }

    internal class HotelRoom : IHotelRoom
    {
        private int _roomNo;
        private bool _isAvailable;
        private double _price;
        private string _guest = string.Empty;

        public int RoomNo
        {
            get => _roomNo;
            set
            {
                if (value > 0)
                {
                    _roomNo = value;
                }
                else Console.WriteLine("Please input room number again");
            }
        }

        public bool IsAvailable
        {
            get => _isAvailable;
            set => _isAvailable = value;
        }

        public double Price
        {
            get => _price;
            set
            {
                if (value > 0)
                {
                    _price = value;
                }
                else Console.WriteLine("Please input price again");
            }
        }

        public string Guest
        {
            get => string.IsNullOrEmpty(_guest) ? "N/A" : _guest;
            set => _guest = value;
        }

        public string GetDescription()
        {
            StringBuilder desc = new StringBuilder();
            desc.AppendLine($"Room number: {RoomNo}")
                .AppendLine($"Status: {(IsAvailable ? "Available" : "Unavailable")}")
                .AppendLine($"Guest: {Guest}")
                .Append("Description: This is a hotel room");
            return desc.ToString();
        }

        public string DisplayPrice()
        {
            return $"Price: {Price}";
        }

        public HotelRoom(int roomNo, double price)
        {
            this.RoomNo = roomNo;
            this.Price = price;
            this.IsAvailable = true;
        }
    }

    internal abstract class RoomDecorator : IHotelRoom
    {
        protected IHotelRoom wrappee;

        public RoomDecorator(IHotelRoom wrappee)
        {
            this.wrappee = wrappee;
        }

        public int RoomNo
        {
            get => wrappee.RoomNo;
            set => wrappee.RoomNo = value;
        }

        public bool IsAvailable
        {
            get => wrappee.IsAvailable;
            set => wrappee.IsAvailable = value;
        }

        public virtual double Price
        {
            get => wrappee.Price;
            set => wrappee.Price = value;
        }

        public string Guest
        {
            get => wrappee.Guest;
            set => wrappee.Guest = value;
        }

        public virtual string GetDescription() => wrappee.GetDescription();

        public string DisplayPrice()
        {
            return $"Price: {Price}";
        }
    }

    internal class VipDecorator : RoomDecorator
    {
        public VipDecorator(IHotelRoom wrappee) : base(wrappee)
        {

        }

        public override double Price
        {
            get => Math.Round(wrappee.Price * 1.3, 1);
            set => wrappee.Price = value;
        }

        public override string GetDescription() => base.GetDescription() + " and with VIP features";
    }

    internal class BeachSideDecorator : RoomDecorator
    {
        public BeachSideDecorator(IHotelRoom wrappee) : base(wrappee)
        {

        }

        public override double Price
        {
            get => Math.Round(wrappee.Price * 1.2, 1);
            set => wrappee.Price = value;
        }

        public override string GetDescription() => base.GetDescription() + " with beautiful beach view";
    }

    internal class Hotel
    {
        private List<IHotelRoom> hotelRooms;
        private string _name;

        public Hotel()
        {
            this.hotelRooms = new List<IHotelRoom>();
        }

        public Hotel(string name) : this()
        {
            this._name = name;
        }

        public string Name
        {
            get
            {
                if (string.IsNullOrEmpty(_name))
                {
                    return " N/A";
                }
                return _name;
            }
            set => _name = value;
        }

        public string GetEmptyRoomDetails()
        {
            var availableRooms = hotelRooms.FindAll(r => r.IsAvailable);
            StringBuilder allDescription = new StringBuilder();

            if (availableRooms.Count != 0)
            {
                foreach (var room in availableRooms)
                {
                    allDescription.Append(room.GetDescription())
                                   .AppendLine(room.DisplayPrice() + "\n");
                }
            }
            return allDescription.ToString();
        }

        public string GetAllRoomsDetails()
        {
            StringBuilder allDescription = new StringBuilder();

            foreach (var room in hotelRooms)
            {
                allDescription.AppendLine(room.GetDescription())
                               .AppendLine(room.DisplayPrice() + "\n");
            }
            return allDescription.ToString();
        }
        public string GetBookedRoomDetails()
        {
            var bookedRooms = hotelRooms.FindAll(r => !r.IsAvailable);
            StringBuilder allDescription = new StringBuilder();

            if (bookedRooms.Count != 0)
            {
                foreach (var room in bookedRooms)
                {
                    allDescription.Append(room.GetDescription())
                                   .AppendLine(room.DisplayPrice() + "\n");
                }
            }

            return allDescription.ToString();
        }


        public void AddRoom(double price, bool isBeachSide, bool isVip)
        {
            int nextRoomNo;

            if (hotelRooms.Count() == 0)
            {
                nextRoomNo = 1;
            }
            else
            {
                nextRoomNo = hotelRooms.Max(r => r.RoomNo) + 1;
            }

            IHotelRoom newRoom = new HotelRoom(nextRoomNo, price);

            if (isBeachSide)
            {
                newRoom = new BeachSideDecorator(newRoom);
            }

            if (isVip)
            {
                newRoom = new VipDecorator(newRoom);
            }
            hotelRooms.Add(newRoom);
        }

        public void RemoveRoom(int roomNo)
        {
            int index = hotelRooms.FindLastIndex(r => r.RoomNo == roomNo);

            if (index != -1)
            {
                hotelRooms.RemoveAt(index);
                Console.WriteLine("Deleted successfully");
            }
            else
            {
                Console.WriteLine("Room not found!");
            }
        }

        public void BookRoom(int roomNo, string guestName)
        {
            int index = hotelRooms.FindLastIndex(r => r.RoomNo == roomNo);

            if (index != -1)
            {
                hotelRooms[index].IsAvailable = false;
                hotelRooms[index].Guest = guestName;
            }
            else
            {
                Console.WriteLine("Room not found!");
            }
        }

        public void UnbookRoom(int roomNo)
        {
            int index = hotelRooms.FindLastIndex(r => r.RoomNo == roomNo);

            if (index != -1)
            {
                if (!hotelRooms[index].IsAvailable)
                {
                    hotelRooms[index].IsAvailable = true;
                    hotelRooms[index].Guest = string.Empty;
                }
                else
                {
                    Console.WriteLine("The room is not booked");
                }
            }
            else
            {
                Console.WriteLine("Room not found!");
            }
        }
    }

    internal class HotelProgram
    {
        private Hotel _hotel;

        private HotelProgram()
        {
            _hotel = new Hotel();
        }

        public static HotelProgram GetHotelProgram()
        {
            return new HotelProgram();
        }

        public void StartProgram()
        {
            Login();
            Console.WriteLine("Please name your hotel:");
            string name = Console.ReadLine();
            bool isExit = false;
            int option;

            do
            {
                DisplayOption(name);
                option = int.Parse(Console.ReadLine());
                isExit = ChooseOption(option);
            } while (!isExit);
        }

        private void DisplayOption(string name)
        {
            Console.WriteLine($"{name} Hotel Management program");
            Console.WriteLine("Please choose one of the following options");
            Console.WriteLine("1. Create a room");
            Console.WriteLine("2. Remove a room");
            Console.WriteLine("3. Get details of all rooms");
            Console.WriteLine("4. Get details of available rooms");
            Console.WriteLine("5. Book a room");
            Console.WriteLine("6. Unbook a room");
            Console.WriteLine("9. Exit");
            Console.Write("Chosen option: ");
        }

        private void Login()
        {
            string username;
            string password;

            while (true)
            {
                Console.Write("Username: ");
                username = Console.ReadLine();
                Console.Write("Password: ");
                password = Console.ReadLine();

                if (username == "admin" && password == "admin")
                {
                    return;
                }

                Console.WriteLine("Wrong username or password!");
            }
        }

        private bool ChooseOption(int option)
        {
            switch (option)
            {
                case 1:
                    double price;
                    bool isBeachSide = false;
                    bool isVip = false;

                    Console.Write("Please input the price: ");
                    price = double.Parse(Console.ReadLine());

                    Console.Write("Does it have beach view? (Y/N): ");
                    string a = Console.ReadLine();

                    if (a.ToLower() == "y")
                    {
                        isBeachSide = true;
                    }

                    Console.Write("Is it VIP? (Y/N): ");
                    a = Console.ReadLine();

                    if (a.ToLower() == "y")
                    {
                        isVip = true;
                    }

                    _hotel.AddRoom(price, isBeachSide, isVip);
                    break;

                case 2:
                    if (_hotel.GetAllRoomsDetails().Equals(string.Empty))
                    {
                        Console.WriteLine("There is no room.");
                        break;
                    }

                    Console.WriteLine(_hotel.GetAllRoomsDetails());
                    Console.Write("Enter the room number to remove: ");
                    int roomNo = int.Parse(Console.ReadLine());
                    _hotel.RemoveRoom(roomNo);
                    break;

                case 3:
                    if (_hotel.GetAllRoomsDetails().Equals(string.Empty))
                    {
                        Console.WriteLine("There is no room.");
                        break;
                    }

                    Console.WriteLine(_hotel.GetAllRoomsDetails());
                    break;

                case 4:
                    if (_hotel.GetEmptyRoomDetails().Equals(string.Empty))
                    {
                        Console.WriteLine("There are no available rooms.");
                        break;
                    }

                    Console.WriteLine(_hotel.GetEmptyRoomDetails());
                    break;

                case 5:
                    Console.WriteLine("Here are all the details of available rooms.");

                    if (_hotel.GetAllRoomsDetails().Equals(string.Empty))
                    {
                        Console.WriteLine("There are no available rooms.");
                        break;
                    }

                    Console.WriteLine(_hotel.GetAllRoomsDetails());
                    Console.Write("Please choose the room you want to book: ");
                    int temp = int.Parse(Console.ReadLine());
                    Console.Write("Please provide the guest name: ");
                    string guest = Console.ReadLine();
                    _hotel.BookRoom(temp, guest);
                    break;

                case 6:
                    if (_hotel.GetBookedRoomDetails().Equals(string.Empty))
                    {
                        Console.WriteLine("There are no booked rooms.");
                        break;
                    }

                    Console.WriteLine(_hotel.GetBookedRoomDetails());
                    Console.Write("Choose a room to unbook: ");
                    int room = int.Parse(Console.ReadLine());
                    _hotel.UnbookRoom(room);
                    break;

                case 9:
                    return true;

                default:
                    Console.WriteLine("Wrong input!");
                    break;
            }

            return false;
        }
    }

    internal class Program
    {
        public static void Main(string[] args)
        {
            HotelProgram.GetHotelProgram().StartProgram();
        }
    }
}
