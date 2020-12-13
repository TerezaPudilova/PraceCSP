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
    /// Interaction logic for AddWindow.xaml
    /// </summary>
    public partial class AddWindow : Window
    {
        private List<Centre> meetingCentres;
        private List<Room> meetingRooms;
        private bool dataChanged;
        private Centre edited_centre;
        private Room edited_room;
        private string choice;
        public string Choice
        {
            get { return choice; }
            private set
            {
                choice = value;
            }
        }
        public AddWindow(string choice)
        {
            InitializeComponent();
            EditButton.Visibility = Visibility.Hidden; //Schování buttonu EDIT pokud jde o přidání nového prvku
            // Jakou entitu přidáváme - room nebo centre
            Choice = choice;
            if (Choice == "centre")
            {
                // Schování nepotřebných tools
                TextBoxCapacity.Visibility = Visibility.Hidden;
                CheckBoxVideoConference.Visibility = Visibility.Hidden;
                ComboBoxMeetingCentre.Visibility = Visibility.Hidden;
                LabelInfoMeetingRoomCapacity.Visibility = Visibility.Hidden;
                LabelMeetingCentre.Visibility = Visibility.Hidden;
            }
            // Získání jediné instance listů od sekretářky
            meetingCentres = Assistant.SingletoneMeetingCentre();
            meetingRooms = Assistant.SingletoneMeetingRooms();
            dataChanged = Assistant.DataChanged;
            // Naplnění comboboxu meeting centry
            foreach (Centre meetingCentre in meetingCentres)
            {
                ComboBoxMeetingCentre.Items.Add(meetingCentre);
            }
        }

        public AddWindow(string choice, object o) //Kontruktor AddWindow pro editaci stávajícího prvku
        {
            InitializeComponent();
            Button.Visibility = Visibility.Hidden; //Schování buttonu pro přidání nového prvku
            meetingCentres = Assistant.SingletoneMeetingCentre(); //Získání listu meeting centres od sekretářky
            meetingRooms = Assistant.SingletoneMeetingRooms();
            dataChanged = Assistant.DataChanged;
            Choice = choice;
            if (choice == "centre") // Pokud se jedná o editaci centra, schovají se nepotřebné tools a předvyplní se inputy
            {
                edited_centre = o as Centre;
                TextBoxCapacity.Visibility = Visibility.Hidden;
                CheckBoxVideoConference.Visibility = Visibility.Hidden;
                ComboBoxMeetingCentre.Visibility = Visibility.Hidden;
                LabelInfoMeetingRoomCapacity.Visibility = Visibility.Hidden;
                LabelMeetingCentre.Visibility = Visibility.Hidden;
                TextBoxName.Text = edited_centre.Name;
                TextBoxDescription.Text = edited_centre.Description;
                TextBoxCode.Text = edited_centre.Code;
            }
            else if (choice == "room") //Pokud se jedná o editaci room, předvyplní se inputy
            {
                edited_room = o as Room;
                TextBoxName.Text = edited_room.Name;
                TextBoxDescription.Text = edited_room.Description;
                TextBoxCode.Text = edited_room.Code;
                TextBoxCapacity.Text = edited_room.Capacity.ToString();
                CheckBoxVideoConference.IsChecked = edited_room.VideoConference;
                foreach (Centre meetingCentre in meetingCentres)
                {
                    ComboBoxMeetingCentre.Items.Add(meetingCentre);
                }
                ComboBoxMeetingCentre.SelectedItem = edited_room.MeetingCentre;
            }
        }

        private void StornoButton_Click(object sender, RoutedEventArgs e) //Metoda - stisknutí storno buttonu
        {
            this.Close(); //Zavření okna beze změn
        }

        private void Button_Click(object sender, RoutedEventArgs e) //Kliknutí na button pro přidání nového prvku
        {
            try
            {
                if (Assistant.IsCodeUnique(TextBoxCode.Text)) // Zjištění, zda je zadaný code unikátní
                {
                    // Rozhodnutí, zda přidáváme centre nebo room
                    if (choice == "centre")
                    {
                        // Přidání nové instance meeting centre
                        Centre meetingCentre = new Centre(TextBoxName.Text, TextBoxDescription.Text, TextBoxCode.Text);
                        // Přidání instance do listu center
                        meetingCentres.Add(meetingCentre);
                        dataChanged = true; //Změna parametru změny dat
                    }
                    else if (choice == "room")
                    {
                        // Parsování kapacity
                        int capacity = 0;
                        if (!Int32.TryParse(TextBoxCapacity.Text, out capacity))
                        {
                            throw new Exception("Nelze převést na integer.");
                        }
                        Centre selectedCentre = ComboBoxMeetingCentre.SelectedItem as Centre;
                        // Získání stavu checkboxu pro video
                        bool video = (CheckBoxVideoConference.IsChecked.HasValue && CheckBoxVideoConference.IsChecked.Value == true);
                        // Přidání nové instance meeting room
                        Room meetingRoom = new Room(TextBoxName.Text, TextBoxDescription.Text, TextBoxCode.Text, capacity, video, selectedCentre);
                        // Přidání instance do listu rooms
                        meetingRooms.Add(meetingRoom);
                        selectedCentre.AddRoom(meetingRoom);
                        dataChanged = true; //Změna parametru změny dat
                    }
                }
                else
                {
                    throw new Exception("Kód není unikátní."); //Výjimka, pokud kód není unikátní
                }
                this.Close(); //Zavření okna
            }
            catch (Exception ex) // Odchycení výjimky a oznámení uživateli
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void EditButton_Click(object sender, RoutedEventArgs e) //Kliknutí na button pro editaci
        {
            bool codeSame = false; // Parametr, zda je nově zadaný kód stejný

            if (Choice == "centre")
            {
                codeSame = (TextBoxCode.Text == edited_centre.Code.ToString());
            }
            else if (Choice == "room")
            {
                codeSame = (TextBoxCode.Text == edited_room.Code.ToString());
            }

            try
            {
                if (Assistant.IsCodeUnique(TextBoxCode.Text, "edit", codeSame)) //Zjištění unikátnosti kódu
                {
                    // Rozhodnutí, zda přidáváme centre nebo room
                    if (choice == "centre") //Změna dat v daném objektu
                    {
                        edited_centre.Name = TextBoxName.Text;
                        edited_centre.Description = TextBoxDescription.Text;
                        edited_centre.Code = TextBoxCode.Text;
                        dataChanged = true;
                    }
                    else if (choice == "room") //Změna dat v daném objektu
                    {
                        edited_room.Name = TextBoxName.Text;
                        edited_room.Description = TextBoxDescription.Text;
                        edited_room.Code = TextBoxCode.Text;
                        int capacity = 0;
                        if (!Int32.TryParse(TextBoxCapacity.Text, out capacity))
                        {
                            throw new Exception("Nelze převést na integer.");
                        }
                        edited_room.Capacity = capacity;
                        bool video = (CheckBoxVideoConference.IsChecked.HasValue && CheckBoxVideoConference.IsChecked.Value == true);
                        edited_room.VideoConference = video;
                        edited_room.MeetingCentre = ComboBoxMeetingCentre.SelectedItem as Centre;
                        dataChanged = true;
                    }
                }
                else
                {
                    throw new Exception("Kód není unikátní.");
                }
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
