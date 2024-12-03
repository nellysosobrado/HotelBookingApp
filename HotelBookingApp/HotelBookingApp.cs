using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBookingApp
{
    public class HotelBookingApp
    {
        private readonly MainMenuManager _mainMenu;

        public HotelBookingApp(MainMenuManager mainMenu)
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
