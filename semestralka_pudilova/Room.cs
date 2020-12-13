using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Xml.Serialization;

namespace semestralka_pudilova
{
    public class Room : Place
    {
        private int _capacity; //Parametr kapacita
        private Boolean _videoConference; //Parametr pro video konferenci
        private Centre _meetingCentre; //Parametr meeting centra
        private List<Meeting> _meetings = new List<Meeting>();

        [JsonIgnore]
        public int Capacity
        {
            get { return _capacity; }
            set
            {
                if ((value >= 1) && (value <= 100)) //Podmínka pro kapacitu
                {
                    _capacity = value;
                }
                else
                {
                    throw new Exception("Kapacita musí být v rozsahu 1-100.");
                }
            }
        }

        [JsonIgnore]
        public Boolean VideoConference
        {
            get { return _videoConference; }
            set { _videoConference = value; }
        }

        [JsonIgnore]
        public Centre MeetingCentre
        {
            get { return _meetingCentre; }
            set
            {
                if (value != null)
                {
                    _meetingCentre = value;
                }
                else
                {
                    throw new Exception("Meeting room musí být přiřazena k meeting cetre.");
                }
            }
        }

        [JsonProperty("meetingCentre", Order = 0)]
        public string MeetingCentreCodeGetter
        {
            get { return _meetingCentre.Code; }
        }

        public Room(string name, string description, string code, int capacity, Boolean videoConference, Centre meetingCentre) : base(name, description, code) //Konstruktor
        {
            Capacity = capacity;
            VideoConference = videoConference;
            MeetingCentre = meetingCentre;
        }

        public Room() : base() { } //Konstruktor bez parametrů pro užití v deserializaci z jsonu

        public override string ToString() //Přetížení pro výpis
        {
            return Name + " - " + Code;
        }

        [JsonProperty("reservations", Order = 2)]
        [XmlIgnoreAttribute]
        public Dictionary<MyDateTime, List<Meeting>> mymeeting
        {
            get
            {
                Dictionary<MyDateTime, List<Meeting>> collection = new Dictionary<MyDateTime, List<Meeting>>();
                foreach (var reservation in this.Reservations)
                {
                    MyDateTime mdt = new MyDateTime(reservation.DateAndTimeFrom);
                    if (!collection.ContainsKey(mdt))
                    {
                        collection[mdt] = new List<Meeting>();
                    }
                    collection[mdt].Add(reservation);
                }
                return collection;
            }
        }

        [JsonIgnore] 
        public List<Meeting> Reservations
        {
            get { return _meetings; }
        }

        public List<Meeting> HasMeetings()
        {
            return _meetings;
        }

        public List<Meeting> HasMeetings(DateTime date)
        {
            List<Meeting> meetingsOnDate = new List<Meeting>();
            foreach (Meeting meeting in _meetings)
            {
                if (meeting.DateAndTimeFrom.Date == date)
                {
                    meetingsOnDate.Add(meeting);
                }
            }

            return meetingsOnDate;
        }

        public void AddMeeting(Meeting meeting)
        {
            _meetings.Add(meeting);
        }

        public void RemoveMeeting(Meeting meeting)
        {
            _meetings.Remove(meeting);
        }

        public bool IsFree(DateTime From, DateTime To, bool time_changed = false)
        {
            foreach (Meeting meeting in _meetings)
            {
                if (From > meeting.DateAndTimeFrom && From < meeting.DateAndTimeTo) return false; //Meeting začíná v existujícím meetingu
                if (To > meeting.DateAndTimeFrom && To < meeting.DateAndTimeTo) return false; // Meeting končí v existujícím meetingu
                if (From < meeting.DateAndTimeFrom && To > meeting.DateAndTimeTo) return false; // Meeting překrývá celý existující meeting
                if (From == meeting.DateAndTimeFrom && To == meeting.DateAndTimeTo && time_changed) return false;
            }
            return true;
        }
    }
}
