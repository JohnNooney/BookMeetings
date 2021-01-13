using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookMeetingsPrototype2.ViewModels.ModelClasses
{
    class DailyAgenda : BaseViewModel
    {
        private string _meetingTitle;
        private DateTime _meetingStart;
        private string _meetingAgenda;
        private string _meetingRoom;

        public string MeetingTitle
        {
            get => _meetingTitle;
            set
            {
                _meetingTitle = value;
            }
        }
        public DateTime MeetingStart {
            get => _meetingStart;
            set
            {
               _meetingStart = value;
            }
        }
        public string MeetingRoom
        {
            get => _meetingRoom;
            set
            {
                _meetingRoom = value;
            }
        }
        public string MeetingAgenda
        {
            get {
                _meetingAgenda = MeetingTitle + "\nstarts at " + MeetingStart.ToString("HH:mm") + " in " + MeetingRoom; //construct 
                return _meetingAgenda; 
            }
            set
            {
                _meetingAgenda = MeetingTitle + "\nstarts at " + MeetingStart.ToString("HH:mm") + " in " + MeetingRoom; //construct 
                SetProperty(ref _meetingAgenda, value);
            }
        }
    }
}
