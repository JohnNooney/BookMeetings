using BookMeetingsPrototype2.ViewModels.ModelClasses;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.IO.Packaging;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Duration = BookMeetingsPrototype2.ViewModels.ModelClasses.Duration;

/// <summary>
/// A base view model that fires Property Changes events as needed
/// </summary>

namespace BookMeetingsPrototype2.ViewModels
{
    //view model that implements the booking view into the main window
    class BookingModel : BaseViewModel
    {
        //*** For Meeting Title Box
        private string _title;

        //** For storing user
        private Participant _user;

        //*** For Room Box
        private ObservableCollection<Room> _rooms; //these are the rooms 
        private Room _sroom; //this is the room selected 

        //*** For Time Selector Box
        private DateTime _selectedDateTime = DateTime.Now; // get the current time of the client, this will change based on the user's time selection
        private DateTime _realDateTime = DateTime.Now;
        private TimeSpan _minTime = new TimeSpan(08, 0, 0); //will be used to modify the current time
        private TimeSpan _maxTime = new TimeSpan(18, 0, 0); //will be used to modify the current time

        //*** For Participants Box
        private ObservableCollection<Participant> _participants; //these are the participants 
        private ObservableCollection<Participant> _sparticipants = new ObservableCollection<Participant>(); //these are the selected participants

        //*** For Duration Box
        private ObservableCollection<Duration> _durations; //these are the participants
        private Duration _sduration;

        //** For Feedback message (Real Time Validation)
        private string _capacityFeedback;
        private string _timeFeedback;
        private DateTime _bookingEnd;

        //*** For filling out the box of users that may be in a meeting during the booked times
        private ObservableCollection<Participant> _busyParticipants;

        //set up connection object
        TayMarkDataDataContext db = new TayMarkDataDataContext(Properties.Settings.Default.sql1803534ConnectionString);

        public BookingModel(Participant loggedUser)
        {
            //load DB data
            //testData();
            loadDBData();

            //initialize user credidentials
            _user = loggedUser;
            //remove user from selectable participants list
            foreach (var emp in Participants)
            {
                if (_user.EmpId == emp.EmpId)
                {
                    Participants.Remove(emp);
                    break;
                }
            }

            //initialize the users Data...
            //create the selected participants list with the first being the user
            SParticipants = new ObservableCollection<Participant>();
            SParticipants.Add(User); //add the user

            //used to store which user's are in a meeting
            BusyParticipants = new ObservableCollection<Participant>();

            //create a list of meeting durations to populate the listbox
            Durations = new ObservableCollection<Duration>();
            for (int i = 5; i < 65; i += 5)
            {
                Duration x = new Duration() { Length = new TimeSpan(0, i, 0) };
                Durations.Add(x);
            }

            //initialize SelectedDateTime in table to set intial time selected
            SelectedDateTime = _selectedDateTime;
            RealDateTime = _realDateTime;

            //initialize the verify booking command
            VerifyBookingCommand = new RelayCommand(VerifyButtonClick);
            //initialize the verify booking command
            ClearAllCommand = new RelayCommand(ClearAllButtonClick);

            //dynamically update capacity message when the capacity has changed
            SParticipants.CollectionChanged += CapacityChanged;

            //*****
            //Black Box Testing
            //*****
            //autoSelectData();
            //falseDataTest();
        }

        //function that retrives data from the database
        private void loadDBData()
        {
            //*************************************
            //load the employees from the database
            //*************************************
            var dbEmps = (from emps in db.TayMarkEmployees
                            select emps);

            //init Participants and create a Participant object for each employee 
            Participants = new ObservableCollection<Participant>();
            foreach (var item in dbEmps)
            {
                //allocate each employee as a Participant object
                Participant employee = new Participant() { Name = item.name, EmpId = item.empId }; //make sure the Participants box is always filled with team leader
                Participants.Add(employee);
            }

            //*************************************
            // load the employees from the database
            //*************************************
            var dbRooms = (from rooms in db.TayMarkRooms
                        select rooms);

            //init Participants and create a Participant object for each employee 
            Rooms = new ObservableCollection<Room>();
            foreach (var item in dbRooms)
            {
                Room newRoom = new Room() { RoomName = item.roomName, Capacity = item.roomCapacity, RoomId = item.roomId }; //make sure the Participants box is always filled with team leader
                Rooms.Add(newRoom);
            }

        }

        //function for automating the testing of making a booking confirmation
        private void autoSelectData()
        {
            //write a room title
            MeetingTitle = "TestMeeting";
            //select a room
            SRoom = Rooms.First(); //first room on the list
            //select a Duration
            SDuration = Durations.ElementAt(1); //selects second element: 10 min
            //select a Time
            SelectedDateTime = new DateTime(2020,12,12,17,30,00);
            //add selected participants
            SParticipants.Add(Participants.ElementAt(0));
            SParticipants.Add(Participants.ElementAt(1));
            SParticipants.Add(Participants.ElementAt(2));

            //submit test data for test booking
            VerifyButtonClick();
            
        }

        //funtion to test data that should not pass the booking confirmation
        private void falseDataTest()
        {
            //Test when room is already booked at that time
            Debug.Assert(!verifyRoomTime(), "Room should not be booked.");

            //test when booking time is out of bounds
            //min
            SelectedDateTime = new DateTime(2020, 12, 12, 02, 30, 00);
            Debug.Assert(!bookedTimeTest(), "Selected time should be out of min bounds");
            //max
            SelectedDateTime = new DateTime(2020, 12, 12, 20, 30, 00);
            Debug.Assert(bookedTimeTest(), "Selected time should be out of max bounds"); //should pass because does not allow setting time beyond 18:00

            //test when too many participants are booked for the room's capacity
            SParticipants.Add(Participants.ElementAt(3));
            SParticipants.Add(Participants.ElementAt(4));

            Debug.Assert(capacityTest(), "Participant selection should be exceeded");

            //test when no data is entered
            ClearAllButtonClick(); //used to clear all fields instantly
            //title
            Debug.Assert(MeetingTitle == null, "Meeting title should be empty");

            //if selected participants is greater than 2 (team leader will always selected)
            Debug.Assert(SParticipants.Count > 2, "Meeting title should be empty");

            //duration
            Debug.Assert(SDuration == null, "Meeting duration should be empty");
        }

        //function to populate the information with test data
        private void testData()
        {
            //create a list of rooms to populate the listbox
            Rooms = new ObservableCollection<Room>()
             {
                  new Room(){RoomName="Far Far Away", Capacity=6}
                    ,new Room(){RoomName="HogWarts", Capacity=4}
                    ,new Room(){RoomName="Hundred Acrew Wood", Capacity=10}
                    ,new Room(){RoomName="Narnia", Capacity=15}
                    ,new Room(){RoomName="Neverland", Capacity=6}
            };

            //create a list of test participants to populate the listbox
            Participants = new ObservableCollection<Participant>()
            {
                 new Participant(){Name="Bill"}
                    ,new Participant(){Name="Jeremy"}
                    ,new Participant(){Name="Maxell"}
                    ,new Participant(){Name="Lydia"}
                    ,new Participant(){Name="Daniela"}
            };  

            //create the selected participants list with the first being
            //the Employee creating the booking
            SParticipants = new ObservableCollection<Participant>()
            {
                new Participant() { Name = "Derek" }
            };
        }

        public Participant User
        {
            get { return _user; }
            set => SetProperty(ref _user, value);
        }

        public string MeetingTitle
        {
            get { return _title; }
            set => SetProperty(ref _title, value);
        }
        public ObservableCollection<Room> Rooms
        {
            get { return _rooms; }
            set { _rooms = value; }
        }

        //get/set for selected room
        public Room SRoom
        {
            get { return _sroom; }
            set {
                CapacityFeedback = null; //reset feedback each time room is selected
                SetProperty(ref _sroom, value);
                verifyParticipants();
                TimeFeedback = verifyRoomTimeMsg(); //set the message if room can be selected at this time or not
            }
        }

        public DateTime minTime
        {
            get
            {
                //change the time to the minimum time of the day
                //for booking a meeting
                DateTime temp = _selectedDateTime;
                temp = temp.Date + _minTime;
                return temp;
            }
            set
            {
                DateTime temp = _selectedDateTime;
                temp = temp.Date + _minTime;
                temp = value;
            }
        }

        public DateTime maxTime
        {
            get
            {
                //change the time to the maximum time of the day
                //for booking a meeting
                DateTime temp = _selectedDateTime;
                temp = temp.Date + _maxTime;
                return temp;
            }
            set
            {
                DateTime temp = _selectedDateTime;
                temp = temp.Date + _maxTime;
                temp = value;
            }
        }

        public DateTime SelectedDateTime
        {
            get { return _selectedDateTime; }
            set {
                SetProperty(ref _selectedDateTime, value);
                verifyParticipants(); //remove any participants from list that are already in a room at this time
                TimeFeedback = verifyRoomTimeMsg(); //set the message if room can be selected at this time or not
            }
        }

        //used in storing the real date time in order to compare to the selected date time
        public DateTime RealDateTime
        {
            get { return _realDateTime; }
            set{SetProperty(ref _realDateTime, value);}
        }

        /// <summary>
        /// used in creating the booking error conflict message
        /// stores when the selected room ends
        /// </summary>
        private DateTime BookingEnd
        {
            get { return _bookingEnd; }
            set { SetProperty(ref _bookingEnd, value); }
        }
        public string TimeFeedback
        {
            get
            {
                return _timeFeedback;
            }
            set
            {
                SetProperty(ref _timeFeedback, value);
            }
        }

        /// <summary>
        /// used to get/set the feedback msg *************** fix
        /// </summary>
        public string CapacityFeedback
        {
            get
            {
                return _capacityFeedback;
            }
            set {
                SetProperty(ref _capacityFeedback, value);
            }
        }

        //used to update capacity message dynamically
        public void CapacityChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if(SRoom != null)
            {
                //provide a message based on the participant to 
                if (SParticipants.Count > SRoom.Capacity){
                    CapacityFeedback = SParticipants.Count.ToString() + "/" + SRoom.Capacity.ToString() + " Exceeded Capacity. Unable to Book";
                }
                else if(SParticipants.Count == SRoom.Capacity){
                    CapacityFeedback = SParticipants.Count.ToString() + "/" + SRoom.Capacity.ToString() +" Maximum Capacity Reached";
                }
                else
                {
                    CapacityFeedback = SParticipants.Count.ToString() + "/" + SRoom.Capacity.ToString();
                }

            }
            //MessageBox.Show(SParticipants.Count.ToString());
        }

        //acquire collection of particpants to populate the combobox
        public ObservableCollection<Participant> Participants
        {
            get { return _participants; }
            set { SetProperty(ref _participants, value); }
        }

        public ObservableCollection<Participant> SParticipants
        {
            get {
                
                return _sparticipants; 
            }
            set
            {
                SetProperty(ref _sparticipants, value);
            }
        }

        public ObservableCollection<Participant> BusyParticipants
        {
            get { return _busyParticipants; }
            set { SetProperty(ref _busyParticipants, value); }
        }

        //get/set room durations for combobox
        public ObservableCollection<Duration> Durations
        {
            get { return _durations; }
            set { _durations = value; }
        }

        //get/set when a duration is selected
        public Duration SDuration
        {
            get { return _sduration; }
            set { _sduration = value; }
        }

        //used to get/set the operation of the buttons
        public ICommand VerifyBookingCommand { get; set; }
        public ICommand ClearAllCommand { get; set; }

        private void ClearAllButtonClick()
        {
            //reset values
            CapacityFeedback = null;
            TimeFeedback = null;
            SRoom = null;
            SDuration = null;
            MeetingTitle = null;
            
            SParticipants.Clear();
            //make sure the Participants box is always filled with team leader
            SParticipants.Add(User);

            //make these null again because adding a participant triggers them
            CapacityFeedback = null;
            TimeFeedback = null;
        }

        //verify booking button function
        private void VerifyButtonClick()
        {
            //perform basic validations
            if (SRoom == null || SDuration == null || MeetingTitle == null)
            {
                MessageBox.Show("Please fill all boxes before submitting.");
            }
            else if (SParticipants.Count < 2)
            {
                MessageBox.Show("Meeting must have more than 1 Participant.");
            }
            //check if booking is eligable
            else if (SParticipants.Count > SRoom.Capacity)
            {
                MessageBox.Show("Participant selection exceeds Room Capacity.");
            }
            else if(bookedTimeTest() && capacityTest()) //if the time selected pass and capacity isn't exceeded
            {
                if (verifyBusyEmployees()) //if there are no employees selected that are booked then proceed
                {
                    //test the inputs******** remove later
                    DateTime testEndTime = SelectedDateTime.Add(SDuration.Length);
                    MessageBox.Show("Room: " + SRoom.RoomName + "\nTime: " + SelectedDateTime + "\nEnd Time: " + testEndTime);


                    //perform last check with database values
                    if (verifyRoomTime() == true)
                    {
                        //submit values into database
                        submitBooking();

                        //reset values
                        ClearAllButtonClick();

                        MessageBox.Show("Booking confirmed.");
                    }
                    else
                    {
                        MessageBox.Show("There is a meeting booked in that room at that time");
                    }
                }
            }


        }

        private bool bookedTimeTest()
        {
            if (RealDateTime > SelectedDateTime) //if the selected datetime is 
            {
                MessageBox.Show("Can't book a meeting in the past. Please choose the current time or later.");
                return false;
            }
            else if (RealDateTime > maxTime) //if the selected datetime is 
            {
                MessageBox.Show("Working hours are over, wait until tomorrow to book.");
                return false;
            }
            else
            {
                return true;
            }
        }

        private bool capacityTest()
        {
            if (SParticipants.Count > SRoom.Capacity)
            {
                MessageBox.Show("Participant selection exceeds Room Capacity.");
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// used to check if any of the participants selected will be in a meeting
        /// during the selected time
        /// </summary>
        private void verifyParticipants()
        {
            //everytime this method is run restart list of busy employees if there are any already in it
            if (BusyParticipants != null)
            {
                try
                {
                    BusyParticipants.Clear();
                }
                catch (Exception e)
                {
                    MessageBox.Show("Error: " + e.ToString());
                    throw;
                }
            }

            bool flag = false; //used if to signy if any participants have been removed from the list
            if (SRoom != null)
            {
                try
                {
                    //empoloyees that are in a meeting at the selected time 
                    var employeeMatch = (from booking in db.TayMarkBookings
                                         join empsBooked in db.TayMarkBooking_Emps on booking.meetingId equals empsBooked.meetingId
                                         join emps in db.TayMarkEmployees on empsBooked.employeeId equals emps.empId
                                         where booking.meetingStart <= SelectedDateTime && //match the time
                                         SelectedDateTime <= booking.meetingEnd
                                         select emps);

                    if (employeeMatch.Count() > 0)
                    {
                        foreach (var bookedEmp in employeeMatch)
                        {
                            foreach (var busyEmployee in Participants.Where(i => i.EmpId == bookedEmp.empId).ToList())
                            {
                                Participant busy = new Participant() { Name = busyEmployee.Name, EmpId = busyEmployee.EmpId};
                                BusyParticipants.Add(busy);
                            }
                            flag = true;
                        }
                    }
                    
                }
                catch (Exception e)
                {
                    MessageBox.Show("Error: " + e);
                    throw;
                }
            }
            
            

            if (flag)
            {
                CapacityFeedback = "\nOne or more employees may be busy during this time";
            }
            else
            {
                CapacityFeedback = "\nAll employees are availabe at this time";
            }
        }

        //returns if there are any selected employees that are busy and shows a dialoge box of these
        //participants
        private bool verifyBusyEmployees()
        {
            bool flag = true;
            string bookedEmpsMatch = "";

            //check each selected participant to see if they are busy
            //if so then prevent booking with that selected user
            foreach (var busyEmp in BusyParticipants)
            {
                foreach (var selectedEmp in SParticipants)
                {
                    if (busyEmp.EmpId == selectedEmp.EmpId)
                    {
                        bookedEmpsMatch += busyEmp.Name + "\n";
                        flag = false;
                    }
                }
            }
            if (bookedEmpsMatch != "")
            {
                MessageBox.Show(bookedEmpsMatch, "These employees are already booked at this time.");
            }
            return flag;
        }

        //test if room is already booked at the time selected
        private bool verifyRoomTime()
        {
            bool bookingClash = true;

            if (SRoom != null)
            {
                //get the booking where the times intersect
                try
                {

                    var bookingMatch = (from booking in db.TayMarkBookings 
                                            where booking.meetingRoomId == SRoom.RoomId &&
                                            booking.meetingStart <= SelectedDateTime &&
                                            SelectedDateTime <= booking.meetingEnd
                                            select booking);

                    //test if selected booking time for room exists/ or clashes
                    if (bookingMatch.Any())
                    {
                        BookingEnd = (DateTime)bookingMatch.Single().meetingEnd; //get the time the selected meeting will end
                        bookingClash = false;
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show("Error: "+e.ToString());
                    throw;
                }
            }
            return bookingClash;
        }

        //the message that displays when a room is not available at this time
        private string verifyRoomTimeMsg()
        {
            if (verifyRoomTime()) //room passes availability test
            {
                return "";
            }
            else
            {
                return SRoom.RoomName + " is not available until " + BookingEnd.ToString("HH:mm");
            }
        }

        private void submitBooking()
        {
            //create query to add the booking details to the booking table
            TayMarkBooking booking = new TayMarkBooking
            {
                meetingTitle = MeetingTitle,
                meetingRoomId = SRoom.RoomId,
                meetingDuration = SDuration.LengthText,
                meetingStart = SelectedDateTime,
                meetingEnd = SelectedDateTime.Add(SDuration.Length)

            };
            //place query into queue
            db.TayMarkBookings.InsertOnSubmit(booking);

            //submit new meeting
            try
            {
                db.SubmitChanges();  //once a new meeting is submitted then link the employees

                //get the id of the new meeting
                var thisMeeting = (from meeting in db.TayMarkBookings
                                   select meeting).OrderByDescending(x => x.meetingId);

                //*****Remove this later when user is logged in
                //create query to link the employees to the meeting
                foreach (var emp in SParticipants)
                {
                    //create a new insert
                    TayMarkBooking_Emp employee = new TayMarkBooking_Emp
                    {
                        meetingId = thisMeeting.First().meetingId,
                        employeeId = emp.EmpId
                    };

                    //queue new insert
                    db.TayMarkBooking_Emps.InsertOnSubmit(employee);

                    // Submit the change to the database.
                    try
                    {
                        db.SubmitChanges();
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show("Error linking Employee: "+ employee.employeeId +" to Meeting: " + e);
                    }
                }

            }
            catch (Exception e)
            {
                MessageBox.Show("Error submitting new Meeting: " + e);
            }
        }
    }
}

////create a string of the participants selected 
//if (value is IEnumerable)
//{
//    StringBuilder sb = new StringBuilder();
//    foreach (var s in value as IEnumerable)
//    {
//        sb.AppendLine(s.ToString());
//    }
//    SParticipantsTxt = sb.ToString();
//}
//SParticipantsTxt = string.Empty;
