using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace semestralka_pudilova
{
    public class Centre : Place
    {
        private List<Room> _meetingRooms = new List<Room>();

        public Centre() : base() { } //Konstruktor bez parametrů pro užití v deserializaci z jsonu
        public Centre(string name, string description, string code) : base(name, description, code) { }

        public override string ToString() //Přetížení metody pro vypsání
        {
            string meeting_centre = Name + " - " + Code;
            return meeting_centre;
        }

        public List<Room> HasRooms()
        {
            return _meetingRooms;
        }

        public void AddRoom(Room room)
        {
            _meetingRooms.Add(room);
        }

        public void RemoveRoom(Room room)
        {
            _meetingRooms.Remove(room);
        }
    }
}
