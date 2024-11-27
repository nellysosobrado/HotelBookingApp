using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBookingApp
{
    public class HotelBookingApp
    {
        private readonly MainMenu _mainMenu;

        public HotelBookingApp(MainMenu mainMenu)
        {
            _mainMenu = mainMenu;
        }
        public void Run()
        {
            Console.WriteLine("Starting hotel booking application..");
            _mainMenu.Run();
        }
    }
}
