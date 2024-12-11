using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBookingApp
{
    public class App
    {
        private readonly MainMenuManager _mainMenu;

        public App(MainMenuManager mainMenu)
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
