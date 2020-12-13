using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace semestralka_pudilova
{
    class Assistant
    {
        private static Boolean _dataChanged;
        // Zavedení jediné instance listů pro rooms a centres
        private static readonly List<Centre> _meetingCentres = new List<Centre>();
        private static readonly List<Room> _meetingRooms = new List<Room>();
        private static readonly List<Meeting> _meetings = new List<Meeting>();
        public static Boolean DataChanged
        {
            get { return _dataChanged; }
            set { _dataChanged = value; }
        }
        // Getter pro list centres
        public static List<Centre> SingletoneMeetingCentre()
        {
            return _meetingCentres;
        }
        // Getter pro list rooms
        public static List<Room> SingletoneMeetingRooms()
        {
            return _meetingRooms;
        }
        public static List<Meeting> SingletoneMeetings()
        {
            return _meetings;
        }
        public static Boolean IsCodeUnique(string value, string action = "add", bool codeSame = false) //Vrátí true/false dle toho, zda je kód unikátní
        {
            int hits = 0; // Counter hitů 
            foreach (Centre centre in _meetingCentres) // Pro každý prvek cetre porovná code a inkrementuje hits, pokud se shodují
            {
                if (centre.Code == value)
                {
                    hits++;
                }
            }
            foreach (Room room in _meetingRooms) // Pro každý prvek room porovná code a inkrementuje hits, pokud se shodují
            {
                if (room.Code == value)
                {
                    hits++;
                }
            }
            if (action == "add" && hits == 0) // Přidání nového
            {
                return true;
            }
            else if (action == "edit" && codeSame == true && hits == 1) //Editace stávajícího - code zůstává stejný
            {
                return true;
            }
            else if (action == "edit" && codeSame == false && hits == 0) //Editace stávajícho - code je rozdílný
            {
                return true;
            }
            else //Code není unikátní
            {
                return false;
            }
        }
        public static void AssignCentresToRooms() // Metoda která vytvoří vazby mezi centres a rooms (po importu dat)
        {
            foreach (Room room in _meetingRooms)
            {
                string searchCode = room.MeetingCentre.Code;
                foreach (Centre centre in _meetingCentres)
                {
                    if (centre.Code == searchCode)
                    {
                        room.MeetingCentre = centre;
                        centre.AddRoom(room);
                    }
                }
            }
        }
        public static void AssignRoomsToMeetings() // Metoda která vytvoří vazby mezi rooms a centres (po importu dat)
        {
            foreach (Meeting meeting in _meetings)
            {
                string searchCode = meeting.Room.Code;
                foreach (Room room in _meetingRooms)
                {
                    if (room.Code == searchCode)
                    {
                        meeting.Room = room;
                        room.AddMeeting(meeting);
                    }
                }
            }
        }
    }
}
