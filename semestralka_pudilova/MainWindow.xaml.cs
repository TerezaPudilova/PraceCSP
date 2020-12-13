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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using Newtonsoft.Json;
using System.Web.Script.Serialization;

namespace semestralka_pudilova
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<Centre> meetingCentres;
        private List<Room> meetingRooms;
        private List<Meeting> meetings;
        private bool dataChanged;

        public MainWindow()
        {
            meetingCentres = Assistant.SingletoneMeetingCentre();
            meetingRooms = Assistant.SingletoneMeetingRooms();
            meetings = Assistant.SingletoneMeetings();
            dataChanged = Assistant.DataChanged;
            InitializeComponent();
            string loadDataFileCentres = AppDomain.CurrentDomain.BaseDirectory + "export_centres.json";
            string loadDataFileRooms = AppDomain.CurrentDomain.BaseDirectory + "export_rooms.json";
            if (File.Exists(loadDataFileCentres) && File.Exists(loadDataFileRooms))
            {
                InitLoadSavedData();
            }
            FillMeetingCentreComboBox();
        }

        private void ButtonAddMeetingCentre_Click(object sender, RoutedEventArgs e)
        {
            //Otevření okna pro přidání ve stavu pro přidání meeting centre
            AddWindow addWindow = new AddWindow("centre");
            addWindow.Show();
        }

        private void ButtonAddMeetingRoom_Click(object sender, RoutedEventArgs e)
        {
            //Otevření okna pro přidání ve stavu pro přidání meeting room
            AddWindow addWindow = new AddWindow("room");
            addWindow.Show();
        }

        private void MeetingAddButton_Click(object sender, RoutedEventArgs e) //Vyvolání formuláře pro přidání meetingu
        {
            Room room = MeetingRoomComboBox.SelectedItem as Room;
            if (room != null && DatePicker.SelectedDate.Value.Date != null)
            {
                DateTime date = DatePicker.SelectedDate.Value.Date;
                AddMeetingWindow addMeetingWindow = new AddMeetingWindow(room, date);
                addMeetingWindow.Show();
            }
        }

        private void ButtonEditMeetingCentre_Click(object sender, RoutedEventArgs e) //Kliknutí na editaci meeting centra
        {
            Centre selected_centre = MeetingCentresListView.SelectedItem as Centre; //Uložení vybraného centra do proměnné
            if (selected_centre != null)
            {
                AddWindow addWindow = new AddWindow("centre", selected_centre); //Inicializace nového okna pro přidání a editaci - předává zda jde o centre nebo room a referenci na daný objekt
                addWindow.Show(); // Ukázání okna
            }
        }

        private void ButtonEditMeetingRoom_Click(object sender, RoutedEventArgs e) //Kliknutí na editace meeting room
        {
            Room selected_room = MeetingRoomsListView.SelectedItem as Room; //Uložení vybrané room do proměnné
            if (selected_room != null)
            {
                AddWindow addWindow = new AddWindow("room", selected_room); ////Inicializace nového okna pro přidání a editaci - předává zda jde o centre nebo room a referenci na daný objekt
                addWindow.Show(); // Ukázání okna
            }
        }

        private void MeetingEditButton_Click(object sender, RoutedEventArgs e) //Vyvolání formuláře pro editaci meetingu
        {
            Meeting selected_meeting = MeetingsListView.SelectedItem as Meeting;
            if (selected_meeting != null)
            {
                AddMeetingWindow addMeetingWindow = new AddMeetingWindow(selected_meeting);
                addMeetingWindow.Show();
            }
        }

        private void ButtonDeleteMeetingCentre_Click(object sender, RoutedEventArgs e) //Odstranění meeting centre
        {
            if (MeetingCentresListView.SelectedItem != null)
            {
                Centre selectedCentre = MeetingCentresListView.SelectedItem as Centre;
                MessageBoxResult result = MessageBox.Show("Pokud má meeting centre přiřazeny meeting rooms, budou také smazány. Přejete si pokračovat?", "Remove", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    List<Room> rooms = selectedCentre.HasRooms();
                    foreach (Room room in rooms)
                    {
                        meetingRooms.Remove(room); //Odstranění meeting room
                    }
                    meetingCentres.Remove(selectedCentre); //Odstranění meeting centre
                    dataChanged = true; // Změna parametru pro dotaz na uložení změn
                }
                this.Window_Activated(null, null); // Překreslení listviews
            }
        }

        private void ButtonDeleteMeetingRoom_Click(object sender, RoutedEventArgs e) //Odstranění meeting room
        {
            if (MeetingRoomsListView.SelectedItem != null)
            {
                Room selectedRoom = MeetingRoomsListView.SelectedItem as Room;
                Centre roomsCentre = selectedRoom.MeetingCentre;
                MessageBoxResult result = MessageBox.Show("Opravdu si přejdete smazat zvolenou meeting room?", "Remove", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    List<Meeting> room_meetings = selectedRoom.HasMeetings();
                    foreach (Meeting meeting in room_meetings)
                    {
                        meetings.Remove(meeting);
                    }
                    roomsCentre.RemoveRoom(selectedRoom);
                    meetingRooms.Remove(selectedRoom);
                    dataChanged = true;
                }
            }
            this.Window_Activated(null, null); // Překreslení listviews
        }

        private void MeetingDeleteButton_Click(object sender, RoutedEventArgs e) //Vymazání meetingu
        {
            if (MeetingsListView.SelectedItem != null)
            {
                Meeting selected_meeting = MeetingsListView.SelectedItem as Meeting;
                Room meeting_room = selected_meeting.Room;
                MessageBoxResult result = MessageBox.Show("Opravdu si přejdete smazat zvolený meeting?", "Remove", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    meeting_room.RemoveMeeting(selected_meeting);
                    meetings.Remove(selected_meeting);
                    dataChanged = true;
                    this.Window_Activated(null, null);
                }
            }
        }

        private void Exit(object sender, RoutedEventArgs e) // Kliknutí na Exit v Menu
        {
            if (dataChanged == true) //Podmínka zda se změnila data
            {
                MessageBoxResult result = MessageBox.Show("Chcete uložit změny?", "Save", MessageBoxButton.YesNo); //Ukázání okna s dotazem, zda se mají změny uložit
                if (result == MessageBoxResult.Yes) //Pokud uživatel klikne na Ano
                {
                    this.Save(null, null); // Zavolání metody pro uložení
                }
            }
            this.Close(); //Zavření aplikace
        }

        private void Import(object sender, RoutedEventArgs e) //Metoda pro import dat z poskytnutého .csv
        {
            try
            {
                Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog(); //Inicializace openfile dialogu
                dialog.DefaultExt = ".csv"; // Výchozí formát v dialogu
                dialog.Filter = "Comma Separated Values (.csv)|*.csv"; // Nastavení filtru v dialogu
                Nullable<bool> result = dialog.ShowDialog(); //Uložení výsledku dialogu
                if (result == true)
                {
                    string path = dialog.FileName; //Cesta k souboru
                    StreamReader sr = new StreamReader(path); //Inicializace streamreaderu
                    dataChanged = true; //Změna parametru pro změnu dat
                    bool meeting_centres = false; //Pomocné proměnné pro import
                    bool meeting_rooms = false;
                    string current_line; //Indikuje aktuální line streamreaderu
                    while ((current_line = sr.ReadLine()) != null) // čti dokud nenačteš null
                    {
                        if (current_line.Contains("MEETING_CENTRES")) // Pokud se v aktuální řádce vyskutuje daný text, změní se pomocné proměnné
                        {
                            meeting_centres = true;
                            meeting_rooms = false;
                        }
                        else if (current_line.Contains("MEETING_ROOMS")) // Pokud se v aktuální řádce vyskutuje daný text, změní se pomocné proměnné
                        {
                            meeting_rooms = true;
                            meeting_centres = false;
                        }
                        else // Ostatní řádky
                        {
                            string[] values = current_line.Split(','); // Rozdělení řetězce podle ','
                            if (meeting_centres == true && meeting_rooms == false) // Pokud pomocné proměnné splňují podmínky pro čtení center vytvoří se nové centrum
                            {
                                Centre meetingCentre = new Centre(values[0], values[2], values[1]); // Volání konstruktoru centra
                                meetingCentres.Add(meetingCentre); // Přidání do listu center
                            }
                            else if (meeting_rooms == true && meeting_centres == false) // Pokud pomocné proměnné splňují podmínky pro čtení room vytvoří se nová room
                            {
                                int capacity = 0;
                                if (!Int32.TryParse(values[3], out capacity)) // Parsování na integer
                                {
                                    throw new Exception("Nelze převést na integer.");
                                }
                                bool video = false;
                                if (values[4] == "YES") //Získání stavu videa
                                {
                                    video = true;
                                }
                                if (values[4] == "NO")
                                {
                                    video = false;
                                }

                                Centre inMeetingCentre = null;

                                foreach (Centre mc in meetingCentres) //Získání meeting centra, ke kterému room patří
                                {
                                    if (mc.Code == values[5])
                                    {
                                        inMeetingCentre = mc;
                                    }
                                }
                                Room meetingRoom = new Room(values[0], values[2], values[1], capacity, video, inMeetingCentre); // Volání konstruktoru pro room
                                meetingRooms.Add(meetingRoom); // Přidání do listu rooms
                            }
                        }
                    }
                    Assistant.AssignCentresToRooms();
                }
                MessageBox.Show("Data naimportována."); //Oznámení uživateli o úspěšném importu
                this.Window_Activated(null, null); // Aktualizace listviews
            }
            catch (Exception ex) //Odchycení výjimky
            {
                MessageBox.Show(ex.Message + " Doposud naimportovaná data budou ponechány v aplikaci.");
            }
        }

        private void ExportToJSON(object sender, RoutedEventArgs e)
        {
            ExportHelper data = new ExportHelper(meetingRooms);
            string json = JsonConvert.SerializeObject(data, Formatting.Indented);
            string path = AppDomain.CurrentDomain.BaseDirectory + "export_meetings.json"; //Cesta pro uložení
            File.WriteAllText(path, json);
            MessageBox.Show("Vyexportováno"); // Oznámení uživateli
        }

        private void Save(object sender, RoutedEventArgs e) //Metoda pro uložení dat
        {
            JavaScriptSerializer jsonSerializer = new JavaScriptSerializer(); //Inicializace JSSerializeru
            var json = jsonSerializer.Serialize(meetingCentres); // Serializace meeting centres
            string path = AppDomain.CurrentDomain.BaseDirectory + "export_centres.json"; //Cesta pro uložení
            File.WriteAllText(path, json); //Vypsání textu do souboru
            json = jsonSerializer.Serialize(meetingRooms); // Serializace meeting rooms
            path = AppDomain.CurrentDomain.BaseDirectory + "export_rooms.json"; // Cesta pro uložení
            File.WriteAllText(path, json); // Vypsání textu do souboru
            System.Xml.Serialization.XmlSerializer writer = new System.Xml.Serialization.XmlSerializer(typeof(List<Meeting>));
            var pathXML = AppDomain.CurrentDomain.BaseDirectory + "export_meetingXML.xml";
            System.IO.FileStream file = System.IO.File.Create(pathXML);
            writer.Serialize(file, meetings);
            file.Close();
            dataChanged = false; // Změna parametru pro změnu dat
            MessageBox.Show("Uloženo"); // Oznámení uživateli
        }

        private void LoadSavedData() //Metoda pro načtení uložených dat
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "export_centres.json"; //Cesta k souboru s centry
            string json_data = File.ReadAllText(path); // Načtení textu ze souboru
            JavaScriptSerializer jsonSerializer = new JavaScriptSerializer(); //Inicializace JSSerializeru
            List<Centre> importCentres = new List<Centre>(); //Inicializace listu pro importovaná centra
            importCentres = jsonSerializer.Deserialize<List<Centre>>(json_data); //Deserializace do listu pro importovaná centra
            foreach (Centre centre in importCentres) // Pro každé centrum v listu pro importovaná centra se provede řádné volání konstruktoru a přidání do listu center
            {
                Centre meetingCentre = new Centre(centre.Name, centre.Description, centre.Code);
                meetingCentres.Add(meetingCentre);
            }
            path = AppDomain.CurrentDomain.BaseDirectory + "export_rooms.json"; //Cesta k souboru s rooms
            json_data = File.ReadAllText(path); // Načtení textu ze souboru
            List<Room> importRooms = new List<Room>(); //Inicializace listu pro importované rooms
            importRooms = jsonSerializer.Deserialize<List<Room>>(json_data); //Deserializace do listu pro importované rooms
            foreach (Room room in importRooms) // Pro každou room v listu pro importované rooms se provede řádné volání konstruktoru a přidání do listu rooms
            {
                Room meetingRoom = new Room(room.Name, room.Description, room.Code, room.Capacity, room.VideoConference, room.MeetingCentre as Centre);
                meetingRooms.Add(meetingRoom);
            }
            Assistant.AssignCentresToRooms(); //Metoda pro přiřazení rooms k centrům (získání vazby)
            if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "export_meetingXML.xml"))
            {
                List<Meeting> imported_meetings = new List<Meeting>();
                System.Xml.Serialization.XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(List<Meeting>));
                using (var sr = new StreamReader(AppDomain.CurrentDomain.BaseDirectory + "export_meetingXML.xml"))
                {
                    imported_meetings = (List<Meeting>)reader.Deserialize(sr);
                }
                foreach (Meeting meeting in imported_meetings)
                {
                    meetings.Add(meeting);
                }
                Assistant.AssignRoomsToMeetings(); //Metoda pro přiřazení meetingů k roomům (získání vazby)
            }
        }

        private void MeetingCentresListView_SelectionChanged(object sender, SelectionChangedEventArgs e) // Změna vybraného prvku v listview pro centra
        {
            Centre selected_centre = MeetingCentresListView.SelectedItem as Centre; //Uložení vybraného itemu
            if (selected_centre != null) // Naplnění daty pokud je vybraný item
            {
                LabelMeetingCentreName.Content = selected_centre.Name;
                LabelMeetingCentreDescription.Content = selected_centre.Description;
                LabelMeetingCentreCode.Text = selected_centre.Code;

                MeetingRoomsListView.Items.Clear(); //Vyčistění listview pro rooms
                List<Room> rooms = selected_centre.HasRooms();
                foreach (Room room in rooms) // Vypsání všech souvisejících rooms do listview
                {
                    MeetingRoomsListView.Items.Add(room);
                }
            }
        }

        private void MeetingRoomsListView_SelectionChanged(object sender, SelectionChangedEventArgs e) // Změna vybraného prvku v listview pro rooms
        {
            Room selected_room = MeetingRoomsListView.SelectedItem as Room; //Uložení vybraného itemu do proměnné
            if (selected_room != null) // Překreslení informací pro daný room
            {
                LabelMeetingRoomName.Content = selected_room.Name;
                LabelMeetingRoomDescription.Content = selected_room.Description;
                LabelMeetingRoomCode.Text = selected_room.Code;
                LabelMeetingRoomCapacity.Content = selected_room.Capacity.ToString();
                LabelMeetingRoomVideo.Content = selected_room.VideoConference.ToString();
                LabelMeetingRoomCentre.Content = selected_room.MeetingCentre.ToString();
            }
        }

        private void MeetingsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Meeting selected_meeting = MeetingsListView.SelectedItem as Meeting; //Uložení vybraného itemu do proměnné
            if (selected_meeting != null) // Překreslení informací pro daný room
            {
                FromHoursTextBox.Text = selected_meeting.DateAndTimeFrom.Hour.ToString();
                FromMinutesTextBox.Text = selected_meeting.DateAndTimeFrom.Minute.ToString();
                ToHoursTextBox.Text = selected_meeting.DateAndTimeTo.Hour.ToString();
                ToMinutesTextBox.Text = selected_meeting.DateAndTimeTo.Minute.ToString();
                ExpectedPersonsTextBox.Text = selected_meeting.ExpectedPersons.ToString();
                CustomerTextBox.Text = selected_meeting.Customer;
                NoteTextBox.Text = selected_meeting.Note;
                VideoConferenceLabel.Content = selected_meeting.Video.ToString();
            }
        }

        private void MeetingCentreComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) //Změna výběru v comboboxu meeting center
        {
            MeetingRoomComboBox.Items.Clear();
            Centre centre = MeetingCentreComboBox.SelectedItem as Centre;
            if (centre != null)
            {
                List<Room> rooms = centre.HasRooms();
                foreach (Room room in rooms)
                {
                    MeetingRoomComboBox.Items.Add(room);
                }
            }
        }

        private void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e) //Změna zvoleného data v date pickeru
        {
            MeetingsListView.Items.Clear();
            if ((MeetingCentreComboBox.SelectedItem != null) && (MeetingRoomComboBox.SelectedItem != null) && (DatePicker.SelectedDate != null))
            {
                Centre centre = MeetingCentreComboBox.SelectedItem as Centre;
                Room room = MeetingRoomComboBox.SelectedItem as Room;
                DateTime date = DatePicker.SelectedDate.Value.Date;
                List<Meeting> meetings = room.HasMeetings(date);
                List<Meeting> sorted_meetings = meetings.OrderBy(o => o.DateAndTimeFrom).ToList();

                foreach (Meeting meeting in sorted_meetings)
                {
                    MeetingsListView.Items.Add(meeting);
                }
            }
        }

        private void FillMeetingCentreComboBox() //Naplnění combobxu pro meeting centra
        {
            MeetingCentreComboBox.Items.Clear();
            foreach (Centre centre in meetingCentres)
            {
                MeetingCentreComboBox.Items.Add(centre);
            }
        }

        private void Window_Activated(object sender, EventArgs e) // Aktualizace listviews
        {
            MeetingCentresListView.Items.Clear();
            foreach (Centre centre in meetingCentres)
            {
                MeetingCentresListView.Items.Add(centre);
            }
            MeetingRoomsListView.Items.Clear();
            DatePicker_SelectedDateChanged(null, null);
            MeetingCentreComboBox.Items.Clear();
            foreach (Centre centre in meetingCentres)
            {
                MeetingCentreComboBox.Items.Add(centre);
            }
        }

        private void InitLoadSavedData() //Metoda pro načtení dat při spuštění
        {
            LoadSavedData();
            this.Window_Activated(null, null); // Aktualizace listviews
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (dataChanged == true) //Změna parametru pro uložení dat
            {
                MessageBoxResult result = MessageBox.Show("Chcete uložit změny?", "Save", MessageBoxButton.YesNo); //Dotaz uživateli
                if (result == MessageBoxResult.Yes)
                {
                    this.Save(null, null); //Volání metody pro uložení dat
                }
            }
        }
    }
}
