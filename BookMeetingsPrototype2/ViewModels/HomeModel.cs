using BookMeetingsPrototype2.ViewModels.ModelClasses;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BookMeetingsPrototype2.ViewModels
{
    class HomeModel : BaseViewModel
    {

        private string _greeting; //used in get/set of greeting msg
        private Participant _user; // " user creds
        private ObservableCollection<DailyAgenda> _agenda;// " agenda items
        private DailyAgenda _agendaItem; // " a single agenda item

        //set up connection object
        TayMarkDataDataContext db = new TayMarkDataDataContext(Properties.Settings.Default.sql1803534ConnectionString);

        public HomeModel(Participant loggedUser)
        {
            //set Object definitions
            User = loggedUser;
            Greeting = "Hello " + User.Name; //***Later have dynamic day greetings(ie: good morning, good afternoon)

            //load Data 
            loadDBData();

            //******
            //Black Box Testing
            //******
            //testBookingData();
        }

        public void loadDBData()
        {
            //variables used to get the current datetime in order to display the meetings for today only
            DateTime today = DateTime.Now;
            DateTime endToday = today + new TimeSpan(18, 0, 0);

            try
            {
                //get a list of the meetings that the user is in 
                var usersMeetings = (from booking in db.TayMarkBooking_Emps
                                     join meeting in db.TayMarkBookings on booking.meetingId equals meeting.meetingId
                                     join room in db.TayMarkRooms on meeting.meetingRoomId equals room.roomId
                                     where booking.employeeId == User.EmpId &&
                                     meeting.meetingEnd >= today && //make sure to list meetings that have not ended yet
                                     endToday >= meeting.meetingEnd
                                     select new DailyAgenda { MeetingTitle = meeting.meetingTitle, MeetingStart = meeting.meetingStart, MeetingRoom = room.roomName}).OrderBy(x => x.MeetingStart);

                //MessageBox.Show(usersMeetings.First().MeetingTitle);

                //initalize an object list of the meetings
                Agenda = new ObservableCollection<DailyAgenda>();
                foreach (var aMeeting in usersMeetings)
                {
                    Agenda.Add(aMeeting); //add meetings to list of user's meetings
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
                throw;
            }
        }

        //Integration Test
        //Tests that the test meeting in the booking menu was retrieved correctly
        public void testBookingData()
        {
            //test meeting title
            Debug.Assert(Agenda[0].MeetingTitle == "TestMeeting", "Meeting title does not match.");
            //room name
            Debug.Assert(Agenda[0].MeetingRoom == "Far Far Away", "Meeting room does not match.");
            //meeting start
            Debug.Assert(Agenda[0].MeetingStart == new DateTime(2020, 12, 12, 17, 30, 0), "Meeting time does not match.");

            MessageBox.Show("All Tests Pass. Data Matches.");
        }

        public Participant User
        {
            get => _user;
            set => SetProperty(ref _user, value); //updates value if changed
        }

        public string Greeting
        {
            get => _greeting;
            set => SetProperty(ref _greeting, value); //updates value if changed
        }

        public ObservableCollection<DailyAgenda> Agenda
        {
            get => _agenda;
            set => SetProperty(ref _agenda, value);
        }

        public DailyAgenda AgendaItem
        {
            get => _agendaItem;
            set => SetProperty(ref _agendaItem, value);
        }
    }
}
