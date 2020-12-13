using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace semestralka_pudilova
{
    /// <summary>
    /// Interaction logic for AddMeetingWindow.xaml
    /// </summary>
    public partial class AddMeetingWindow : Window
    {
        private Meeting edited_meeting;
        private Room meetingRoom;
        private DateTime date;
        private List<Meeting> meetings;
        private bool dataChanged;
        public AddMeetingWindow(object room, DateTime datum)
        {
            meetingRoom = room as Room;
            date = datum;
            InitializeComponent();
            meetings = Assistant.SingletoneMeetings();
            dataChanged = Assistant.DataChanged;
            EditButton.Visibility = Visibility.Hidden;
        }

        public AddMeetingWindow(object meeting)
        {
            InitializeComponent();
            edited_meeting = meeting as Meeting;
            meetingRoom = edited_meeting.Room;
            meetings = Assistant.SingletoneMeetings();
            dataChanged = Assistant.DataChanged;
            date = edited_meeting.DateAndTimeFrom.Date;
            AddButton.Visibility = Visibility.Hidden;
            FromHoursTextBox.Text = edited_meeting.DateAndTimeFrom.Hour.ToString();
            FromMinutesTextBox.Text = edited_meeting.DateAndTimeFrom.Minute.ToString();
            ToHoursTextBox.Text = edited_meeting.DateAndTimeTo.Hour.ToString();
            ToMinutesTextBox.Text = edited_meeting.DateAndTimeTo.Minute.ToString();
            ExpectedPersonsTextBox.Text = edited_meeting.ExpectedPersons.ToString();
            CustomerTextBox.Text = edited_meeting.Customer.ToString();
            CheckBoxVideo.IsChecked = edited_meeting.Video;
            NoteTextBox.Text = edited_meeting.Note;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                double hoursFrom = 0;
                if (!Double.TryParse(FromHoursTextBox.Text, out hoursFrom))
                {
                    throw new Exception("Hours From - Nelze převést na double.");
                }
                double minutesFrom = 0;
                if (!Double.TryParse(FromMinutesTextBox.Text, out minutesFrom))
                {
                    throw new Exception("Minutes From - Nelze převést na double.");
                }
                double hoursTo = 0;
                if (!Double.TryParse(ToHoursTextBox.Text, out hoursTo))
                {
                    throw new Exception("Hours To - Nelze převést na double.");
                }
                double minutesTo = 0;
                if (!Double.TryParse(ToMinutesTextBox.Text, out minutesTo))
                {
                    throw new Exception("Minutes To - Nelze převést na double.");
                }
                DateTime dateTimeFrom = date.AddHours(hoursFrom).AddMinutes(minutesFrom);
                DateTime dateTimeTo = date.AddHours(hoursTo).AddMinutes(minutesTo);
                if (!meetingRoom.IsFree(dateTimeFrom, dateTimeTo))
                {
                    throw new Exception("Meeting room " + meetingRoom + " je v požadovaném čase obsazena.");
                }
                if (dateTimeFrom >= dateTimeTo)
                {
                    throw new Exception("Nekorektní doba trvání.");
                }
                int expectedPersons = 0;
                if (!Int32.TryParse(ExpectedPersonsTextBox.Text, out expectedPersons))
                {
                    throw new Exception("Expected Persons - Nelze převést na integer.");
                }
                if (expectedPersons > meetingRoom.Capacity)
                {
                    throw new Exception("Nedostatečná kapacita místnosti.");
                }
                bool video = (CheckBoxVideo.IsChecked.HasValue && CheckBoxVideo.IsChecked.Value == true);
                if (video && (meetingRoom.VideoConference == false))
                {
                    throw new Exception("Zvolená místnost neumožňuje pořádání video konferencí.");
                }
                Meeting meeting = new Meeting(meetingRoom, dateTimeFrom, dateTimeTo, expectedPersons, CustomerTextBox.Text, video, NoteTextBox.Text);
                meetings.Add(meeting);
                meetingRoom.AddMeeting(meeting);
                dataChanged = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            bool time_changed = false;
            try
            {
                double hoursFrom = 0;
                if (!Double.TryParse(FromHoursTextBox.Text, out hoursFrom))
                {
                    throw new Exception("Hours From - Nelze převést na double.");
                }
                double minutesFrom = 0;
                if (!Double.TryParse(FromMinutesTextBox.Text, out minutesFrom))
                {
                    throw new Exception("Minutes From - Nelze převést na double.");
                }
                double hoursTo = 0;
                if (!Double.TryParse(ToHoursTextBox.Text, out hoursTo))
                {
                    throw new Exception("Hours To - Nelze převést na double.");
                }
                double minutesTo = 0;
                if (!Double.TryParse(ToMinutesTextBox.Text, out minutesTo))
                {
                    throw new Exception("Minutes To - Nelze převést na double.");
                }
                DateTime dateTimeFrom = date.AddHours(hoursFrom).AddMinutes(minutesFrom);
                DateTime dateTimeTo = date.AddHours(hoursTo).AddMinutes(minutesTo);
                if (dateTimeFrom != edited_meeting.DateAndTimeFrom || dateTimeTo != edited_meeting.DateAndTimeTo) time_changed = true;
                if (!meetingRoom.IsFree(dateTimeFrom, dateTimeTo, time_changed))
                {
                    throw new Exception("Meeting room " + meetingRoom + " je v požadovaném čase obsazena.");
                }
                if (dateTimeFrom >= dateTimeTo)
                {
                    throw new Exception("Nekorektní doba trvání.");
                }
                int expectedPersons = 0;
                if (!Int32.TryParse(ExpectedPersonsTextBox.Text, out expectedPersons))
                {
                    throw new Exception("Expected Persons - Nelze převést na integer.");
                }
                if (expectedPersons > meetingRoom.Capacity)
                {
                    throw new Exception("Nedostatečná kapacita místnosti.");
                }
                bool video = (CheckBoxVideo.IsChecked.HasValue && CheckBoxVideo.IsChecked.Value == true);
                if (video && (meetingRoom.VideoConference == false))
                {
                    throw new Exception("Zvolená místnost neumožňuje pořádání video konferencí.");
                }
                edited_meeting.DateAndTimeFrom = dateTimeFrom;
                edited_meeting.DateAndTimeTo = dateTimeTo;
                edited_meeting.ExpectedPersons = expectedPersons;
                edited_meeting.Customer = CustomerTextBox.Text;
                edited_meeting.Video = video;
                edited_meeting.Note = NoteTextBox.Text;
                dataChanged = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void StornoButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }


    }
}
