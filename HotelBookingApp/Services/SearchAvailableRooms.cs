//using HotelBookingApp.Repositories;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace HotelBookingApp.Services
//{
//    public class SearchAvailableRooms
//    {
//        private readonly BookingRepository _bookingRepository;

//        public SearchAvailableRooms(BookingRepository bookingRepository)
//        {
//            _bookingRepository = bookingRepository;
//        }

//        public void Execute()
//        {
//            Console.Clear();
//            Console.WriteLine("Funktion: Hitta tillgängliga rum");

//            DateTime startDate = GetValidDate("Välj startdatum:");
//            DateTime endDate = GetValidDate("Välj slutdatum:");

//            if (!ValidateDateRange(startDate, endDate)) return;

//            int guestCount = GetValidGuestCount();
//            if (guestCount == -1) return;

//            DisplayAvailableRooms(startDate, endDate, guestCount);
//        }

//        private DateTime GetValidDate(string prompt)
//        {
//            DateTime date;
//            while (true)
//            {
//                Console.WriteLine(prompt);
//                if (DateTime.TryParse(Console.ReadLine(), out date))
//                    break;
//                Console.WriteLine("Ogiltigt datum. Försök igen.");
//            }
//            return date;
//        }

//        private bool ValidateDateRange(DateTime startDate, DateTime endDate)
//        {
//            if (endDate > startDate) return true;

//            Console.WriteLine("Slutdatum måste vara efter startdatum.");
//            Console.ReadKey();
//            return false;
//        }

//        private int GetValidGuestCount()
//        {
//            Console.WriteLine("Ange antalet gäster:");
//            if (int.TryParse(Console.ReadLine(), out int guestCount) && guestCount > 0)
//                return guestCount;

//            Console.WriteLine("Ogiltigt antal gäster.");
//            Console.ReadKey();
//            return -1;
//        }

//        private void DisplayAvailableRooms(DateTime startDate, DateTime endDate, int guestCount)
//        {
//            var availableRooms = _bookingRepository.GetAvailableRooms(startDate, endDate, guestCount);

//            if (!availableRooms.Any())
//            {
//                Console.WriteLine("Inga tillgängliga rum hittades.");
//            }
//            else
//            {
//                Console.WriteLine($"\nTillgängliga rum från {startDate:yyyy-MM-dd} till {endDate:yyyy-MM-dd}:");
//                foreach (var room in availableRooms)
//                {
//                    Console.WriteLine($"Rum {room.RoomId}: {room.Type}, {room.PricePerNight:C} per natt");
//                }
//            }

//            Console.WriteLine("\nTryck på valfri tangent för att återgå till menyn...");
//            Console.ReadKey();
//        }
//    }

//}
