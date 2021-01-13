using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace BookMeetingsPrototype2.ViewModels.ModelClasses
{
    class Meeting
    {
        private string _meetingTitle;
        private string _roomName;
        private DateTime _meetingStart;
        private DateTime _meetingEnd;
        private string _meetingDuration;
        private int _meetingId;
        private string _meetingLeader;
        private ObservableCollection<Participant> _meetingParticipants;

        public Meeting()
        {
            MeetingLeader = null;
            MeetingParticipants = null;

            //initialize the view all participants command
            ViewAllCommand = new RelayCommand(ViewAllButtonClick);
        }

        //get/set command
        //private ICommand meetingButtonParam;
        public ICommand ViewAllCommand
        {
            get; set;
        }

        //function that runs code for displaying all relating participants
        private void ViewAllButtonClick()
        {
            string temp = "";
            foreach (var person in MeetingParticipants)
            {
                temp += person.Name + "\n";
            }

            MessageBox.Show(temp, "All Participants in " + MeetingTitle);
        }

        public string MeetingTitle
        {
            get { return _meetingTitle; }
            set { _meetingTitle = value; }
        }
        public string RoomName
        {
            get { return _roomName; }
            set { _roomName = value; }
        }
        public DateTime MeetingStart
        {
            get { return _meetingStart; }
            set { _meetingStart = value; }
        }
        public DateTime MeetingEnd
        {
            get { return _meetingEnd; }
            set { _meetingEnd = value; }
        }
        public string MeetingDuration
        {
            get { return _meetingDuration; }
            set { _meetingDuration = value; }
        }

        public int MeetingId
        {
            get { return _meetingId; }
            set { _meetingId = value; }
        }

        public string MeetingLeader
        {
            get { return _meetingLeader; }
            set { _meetingLeader = value; }
        }

        public ObservableCollection<Participant> MeetingParticipants
        {
            get { return _meetingParticipants; }
            set { _meetingParticipants = value; }
        }
    }
}
