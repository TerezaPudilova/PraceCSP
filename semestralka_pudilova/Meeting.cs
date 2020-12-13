using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace semestralka_pudilova
{
    public class Meeting
    {
        private DateTime _datetimeFrom;
        private DateTime _datetimeTo;
        private int _expectedPersons;
        private string _customer;
        private bool _video;
        private string _note;
        private Room _meetingRoom;

        [JsonIgnore]
        public Room Room
        {
            get { return _meetingRoom; }
            set
            {
                if (value != null)
                {
                    _meetingRoom = value;
                }
                else
                {
                    throw new Exception("Meeting musí být přiřazený k meeting room.");
                }
            }
        }


        [JsonProperty("note", Order = 5)]
        public string Note
        {
            get { return _note; }
            set
            {
                if ((value.Length >= 0) && (value.Length <= 300)) // Podmínka pro nastavení názvu
                {
                    _note = value;
                }
                else
                {
                    throw new Exception("Poznámka musí být dlouhá maximálně 300 znaků.");
                }
            }
        }

        [JsonProperty("videoConference", Order = 4)]
        public bool Video
        {
            get { return _video; }
            set { _video = value; }
        }

        [JsonProperty("customer", Order = 3)]
        public string Customer
        {
            get { return _customer; }
            set
            {
                if ((value.Length >= 2) && (value.Length <= 100)) // Podmínka pro nastavení názvu zákazníka
                {
                    _customer = value;
                }
                else
                {
                    throw new Exception("Název zákazníka musí být dlouhý minimálně 2-100 znaků.");
                }
            }
        }

        [JsonProperty("expectedPersonsCount", Order = 2)]
        public int ExpectedPersons
        {
            get { return _expectedPersons; }
            set { _expectedPersons = value; }
        }

        [JsonIgnore]
        public DateTime DateAndTimeFrom
        {
            get { return _datetimeFrom; }
            set { _datetimeFrom = value; }
        }

        [JsonIgnore]
        public DateTime DateAndTimeTo
        {
            get { return _datetimeTo; }
            set { _datetimeTo = value; }
        }

        [JsonProperty(Order = 0)]
        public string from
        {
            get { return _datetimeFrom.ToShortTimeString(); }
        }

        [JsonProperty(Order = 1)]
        public string to
        {
            get { return _datetimeTo.ToShortTimeString(); }
        }

        public Meeting(Room room, DateTime datetimeFrom, DateTime datetimeTo, int expectedPersons, string customer, bool video, string note)
        {
            Room = room;
            DateAndTimeFrom = datetimeFrom;
            DateAndTimeTo = datetimeTo;
            ExpectedPersons = expectedPersons;
            Customer = customer;
            Video = video;
            Note = note;
        }

        public Meeting() { }

        public override string ToString()
        {
            return DateAndTimeFrom.ToShortTimeString() + " - " + DateAndTimeTo.ToShortTimeString();
        }
    }
}
