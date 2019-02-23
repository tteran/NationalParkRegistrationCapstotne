﻿using Capstone.DAL;
using Capstone.Models;
using ProjectOrganizer;
using System;
using System.Collections.Generic;
using System.Text;

namespace Capstone
{
    public class MainMenuCLI
    {
        private IParkDAO parkDAO;
        private ICampGroundDAO campGroundDAO;
        private ICampSiteDAO campSiteDAO;
        private IReservationDAO reservationDAO;

        private int parkId;

        public MainMenuCLI(IParkDAO parkDAO, ICampGroundDAO campGroundDAO, ICampSiteDAO campSiteDAO, IReservationDAO reservationDAO)
        {
            this.parkDAO = parkDAO;
            this.campGroundDAO = campGroundDAO;
            this.campSiteDAO = campSiteDAO;
            this.reservationDAO = reservationDAO;
        }

        public void RunMenuCLI()
        {
            while (true)
            {
            PrintHeader();
            Menu();
                const string Command_ListAvailableParks = "1";
                const string Command_Quit = "q";

                string command = Console.ReadLine();

                Console.Clear();

                switch (command.ToLower())
                {
                    case Command_ListAvailableParks:
                        ListAvailableParks();                      
                        GetParkDetail();                 
                        break;

                    case Command_Quit:
                        Console.WriteLine("Thank you for using the park registry program.");
                        PrintHeader();
                        return;

                    default:
                        Console.WriteLine("The command provided was not a valid command, please try again.");
                        break;
                }
            }
        }

        private void ListAvailableParks()
        {
            IList<Park> parks = parkDAO.ListAvailableParks();

            Console.WriteLine();
            Console.WriteLine("Printing all parks in the registry");

            foreach (Park park in parks)
            {
                Console.WriteLine($"({park.ParkId.ToString()}) - {park.Name.PadLeft(5)}");
            }

            Console.WriteLine("(Q) - Quit");
            
        }
      
        private void GetParkDetail()
        {
            this.parkId = CLIHelper.GetInteger("Please choose a park for more details: ");
            IList<Park> parks = parkDAO.GetParkDetail(this.parkId);

            Console.Clear();

            foreach (Park detail in parks)
            {
                Console.WriteLine($"{detail.Name} National Park");
                Console.WriteLine($"Location: {detail.Location}");
                Console.WriteLine($"Established: {detail.EstablishDate}");
                Console.WriteLine($"Area: {detail.Area}");
                Console.WriteLine($"Annual Visitors: {detail.Visitors}");
                Console.WriteLine();
                Console.WriteLine($"{detail.Description}");
                Console.WriteLine();
                Console.WriteLine();

                CampgroundCommandMenu();
            }
        }

        private void CampgroundCommandMenu()
        {
            while (true)
            { 
                Console.WriteLine("Select a Command");
                Console.WriteLine("(1) - View Campgrounds");
                Console.WriteLine("(2) - Search for Reservation");
                Console.WriteLine("(3) - Return to Previous Screen");
                string choice = Console.ReadLine();

                switch (choice.ToString())
                {
                    case ("1"):
                        Console.Clear();
                        ViewCampground();
                        break;

                    case ("2"):
                        SearchReservationRun();
                        break;

                    case ("3"):
                        Console.Clear();
                        return;
                        
                    default:
                        Console.WriteLine("Invalid entry. Please try again.");
                        break;
                }
            }
        }

        private void ViewCampground()
        {           
            IList<CampGround> campGrounds = campGroundDAO.ViewCampgrounds(this.parkId);

            //Console.WriteLine($"{this.parkId}");//TODO figure out how to get name of park
            Console.WriteLine("             Name                   Open               Close           Daily Fee");
            Console.WriteLine();

            foreach (CampGround camp in campGrounds)
            {  
                Console.WriteLine($"#{camp.CampgroundId}\t{camp.Name.PadRight(20)}{new DateTime(2001, camp.OpenFrom, 1).ToString("MMMM")}\t\t{new DateTime(2001, camp.OpenTo, 1).ToString("MMMM")}\t\t{camp.DailyFee:C2}");
            }
<<<<<<< HEAD
        }
=======

            //CampgroundCommandMenu();

        }

        //private void CheckSiteAvailabilty()
        //{

        //}
>>>>>>> 7a58de86f628a952b1d0d5b58b70cc4b86f57a40

        private void SearchReservationRun()
        {
            while (true)
            {
                Console.WriteLine("Search for Available Campground Sites");

                Console.WriteLine("Which campground (enter 0 to cancel)?:");
                int choice = int.Parse(Console.ReadLine());
                int campgroundId;

                if (choice == 0)
                {
                    Console.Clear();
                    return;
                }

                else
                {
                    campgroundId = choice;

                    DateTime arrivalDate = CLIHelper.GetDateTime("What is the arrival date?:");
                    DateTime departureDate = CLIHelper.GetDateTime("What is the departure date?:");

                    IList<CampSite> campSites = campSiteDAO.SearchReservationRun(campgroundId, arrivalDate, departureDate);   //TODO: check if campground is null

                    if (campSites == null)
                    {
                        Console.WriteLine("There are no available campsites for those dates, Please try again.");
                        return;
                    }

                    else
                    {
                        CampGround campGround = campGroundDAO.ViewCampground(campgroundId);

                        decimal cost = campGround.DailyFee * (decimal)(departureDate - arrivalDate).TotalDays;

                        Console.WriteLine();
                        Console.WriteLine("Site NO.  MAX OCCUPANCY  ACCESSIBLE   MAX RV LENGTH    UTILITY  COST");
                        Console.WriteLine();

                        foreach (CampSite campSite in campSites)
                        {
                            Console.WriteLine($"#{campSite.SiteId}\t{campSite.MaxOccupancy}\t\t\t{campSite.IsAccessible}\t{campSite.MaxRvLength}\t{campSite.HasUtilties}\t{cost}"); //todo: add daily fee
                        }

                        CreateReservation(arrivalDate, departureDate);
                    }
                }
            }
        }

        private void CreateReservation(DateTime arrivalDate, DateTime departureDate)
        {
            int siteIdChoice = CLIHelper.GetInteger("Which site should be reserved (enter 0 to cancel)?:");
            string reservationName = CLIHelper.GetString("What name should the reservation be made under?:");

            Reservation newReservation = new Reservation
            {
                SiteId = siteIdChoice,
                Name = reservationName,
                FromDate = arrivalDate,
                ToDate = departureDate
            };

            int reservationId = reservationDAO.CreateReservation(newReservation);

        }

        private void PrintHeader()
        {
            Console.WriteLine(@"                       __,--'\");
            Console.WriteLine(@"                 __,--'    :. \.");
            Console.WriteLine(@"            _,--'              \`.");
            Console.WriteLine(@"          / | \        `:        \  `/");
            Console.WriteLine(@"         / '|  \        `:.       \ ");
            Console.WriteLine(@"        / , |   \                  \           ");
            Console.WriteLine(@"       /    |:   \              `:. \              ");
            Console.WriteLine(@"      /| '  |     \ :.           _,-'`.");
            Console.WriteLine(@"    \' |,  / \   ` \ `:.     _,-'_|    `/");
            Console.WriteLine(@"       '._;   \ .   \   `_,-'_,-'");
            Console.WriteLine(@"     \'    `- .\_   |\,-'_,-'");
            Console.WriteLine(@"                 `--|_,`'");
            Console.WriteLine(@"                         `/");        
            Console.WriteLine();
            Console.WriteLine();
        }

        public void Menu()
        {
            Console.WriteLine("    WELCOME TO USA Camp Registry!!!");
            Console.WriteLine("(1) - Show list of all PARKS.");
            Console.WriteLine("(Q) - QUIT");
        }
    }
}
