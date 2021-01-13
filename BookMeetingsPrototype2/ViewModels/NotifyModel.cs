using BookMeetingsPrototype2.ViewModels.ModelClasses;
//using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace BookMeetingsPrototype2.ViewModels
{
    class NotifyModel : BaseViewModel
    {
        //set up connection object
        TayMarkDataDataContext db = new TayMarkDataDataContext(Properties.Settings.Default.sql1803534ConnectionString);

        private Participant _user;
        private ObservableCollection<Meeting> _meetings;
        public ObservableCollection<Meeting> Meetings {
            get { return _meetings; } 
            set { _meetings = value; }
        }
        public NotifyModel(Participant loggedUser)
        {
            //load harcoded test data
            //testNotifications();
            

            //set the user credidentials
            User = loggedUser;

            //load DB associated data
            meetingData();

            //perfom tests on meeting created in the booking menu
            //testBookingData();
        }

        public Participant User
        {
            get { return _user; }
            set { _user = value; }
        }

        //test that hardcoded data can work before adding data via database connection
        public void testNotifications()
        {
            ObservableCollection<Participant> Participants = new ObservableCollection<Participant>()
            {
                 new Participant(){Name="Bill"}
                    ,new Participant(){Name="Jeremy"}
                    ,new Participant(){Name="Maxell"}
                    ,new Participant(){Name="Lydia"}
                    ,new Participant(){Name="Daniela"}
            };

            Meetings = new ObservableCollection<Meeting>();
            for (int i = 1; i < 8; i++)
            {
                Meeting x = new Meeting { MeetingTitle = ("Meeting" + i), RoomName = "Far Far Away", MeetingStart = DateTime.Now, MeetingDuration = "10 min.", MeetingLeader = Participants[0].Name, MeetingParticipants = Participants };
                Meetings.Add(x);
            }
        }

        //Integration Test
        //compare the test data that was submitted in the booking menu to make sure it matches correctly
        public void testBookingData()
        {
            //test meeting title
            Debug.Assert(Meetings[0].MeetingTitle == "TestMeeting", "Meeting title does not match.");
            //room name
            Debug.Assert(Meetings[0].RoomName == "Far Far Away", "Meeting room does not match.");
            //meeting start
            Debug.Assert(Meetings[0].MeetingStart == new DateTime(2020, 12, 12, 17, 30, 0), "Meeting time does not match.");
            //meeting duration
            Debug.Assert(Meetings[0].MeetingDuration == "10 min.", "Meeting duration does not match.");
            //meeting leader
            Debug.Assert(Meetings[0].MeetingLeader == "Elsa Duncan", "Meeting team leader does not match.");
            //meeting participants
            Debug.Assert(Meetings[0].MeetingParticipants[0].Name == "Elsa Duncan");
            Debug.Assert(Meetings[0].MeetingParticipants[1].Name == "Sandy Grant");
            Debug.Assert(Meetings[0].MeetingParticipants[2].Name == "Allan Johnstone");
            Debug.Assert(Meetings[0].MeetingParticipants[3].Name == "Chloe Walker");

            MessageBox.Show("All Tests Pass. Data Matches.");
        }

        //selects the meetings that the logged in user is included in for the day
        public void meetingData()
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
                                     select new Meeting { MeetingId = meeting.meetingId, MeetingTitle = meeting.meetingTitle, RoomName = room.roomName, MeetingDuration = meeting.meetingDuration, MeetingStart = meeting.meetingStart, MeetingEnd = (DateTime)meeting.meetingEnd}).OrderBy(x => x.MeetingStart);

                //MessageBox.Show(usersMeetings.First().MeetingTitle);

                //initalize an object list of the meetings
                Meetings = new ObservableCollection<Meeting>();
                foreach (var aMeeting in usersMeetings)
                {
                    //get the participants that are in the meeting
                    var meetingParticipants = (from booking in db.TayMarkBooking_Emps
                                               join emps in db.TayMarkEmployees on booking.employeeId equals emps.empId
                                               where aMeeting.MeetingId == booking.meetingId
                                               select emps);

                    //create a list of the participants
                    ObservableCollection<Participant> participants = new ObservableCollection<Participant>();
                    foreach (var emp in meetingParticipants)
                    {
                        Participant tempEmp = new Participant() { Name = emp.name, EmpId = emp.empId }; //create Participant object of the indexed employee 
                        participants.Add(tempEmp);
                    }

                    aMeeting.MeetingParticipants = participants;
                    aMeeting.MeetingLeader = participants.First().Name; //the first person in the list will always be the meeting creator
                    Meetings.Add(aMeeting); //add meetings to list of user's meetings
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
                throw;
            }
        }

    }

    //internal class TimeLineEvent : NotifyModel
    //{
    //    private TimeSpan _start;
    //    public TimeSpan Start
    //    {
    //        get
    //        {
    //            return _start;
    //        }
    //        set
    //        {
    //            SetProperty(ref _start, value);
    //        }
    //    }


    //    private TimeSpan _duration;
    //    public TimeSpan Duration
    //    {
    //        get
    //        {
    //            return _duration;
    //        }
    //        set
    //        {
    //            SetProperty(ref _duration, value);
    //        }
    //    }

    //}


    //internal class TimeLine : NotifyModel
    //{
    //    private TimeSpan _duration;
    //    public TimeSpan Duration
    //    {
    //        get
    //        {
    //            return _duration;
    //        }
    //        set
    //        {
    //            SetProperty(ref _duration, value);
    //        }
    //    }


    //    private ObservableCollection<TimeLineEvent> _events = new ObservableCollection<TimeLineEvent>();
    //    public ObservableCollection<TimeLineEvent> Events
    //    {
    //        get
    //        {
    //            return _events;
    //        }
    //        set
    //        {
    //            SetProperty(ref _events, value);
    //        }
    //    }
    //}

    //////Coonstuctor***************

    //TimeLine first = new TimeLine();
    //first.Duration = new TimeSpan(1, 0, 0);
    //first.Events.Add(new TimeLineEvent() { Start = new TimeSpan(0, 15, 0), Duration = new TimeSpan(0, 15, 0) });
    //first.Events.Add(new TimeLineEvent() { Start = new TimeSpan(0, 40, 0), Duration = new TimeSpan(0, 10, 0) });
    //this.TimeLines.Add(first);

    //TimeLine second = new TimeLine();
    //second.Duration = new TimeSpan(1, 0, 0);
    //second.Events.Add(new TimeLineEvent() { Start = new TimeSpan(0, 0, 0), Duration = new TimeSpan(0, 25, 0) });
    //second.Events.Add(new TimeLineEvent() { Start = new TimeSpan(0, 30, 0), Duration = new TimeSpan(0, 15, 0) });
    //second.Events.Add(new TimeLineEvent() { Start = new TimeSpan(0, 50, 0), Duration = new TimeSpan(0, 10, 0) });
    //this.TimeLines.Add(second);
    ////*******************************
    ///

    //private ObservableCollection<TimeLine> _timeLines = new ObservableCollection<TimeLine>();
    //public ObservableCollection<TimeLine> TimeLines
    //{
    //    get
    //    {
    //        return _timeLines;
    //    }
    //    set
    //    {

    //        SetProperty(ref _timeLines, value);
    //    }
    //}
}
