using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookMeetingsPrototype2.ViewModels.ModelClasses
{
    //This is used to fill the combobox with participants. \
    //gets and set the name when creating a list object
    class Participant
    {
        private string _name;
        private int _empId;
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        public int EmpId
        {
            get { return _empId; }
            set { _empId = value; }
        }
        public string PartMsg
        {
            get
            {
                string msg = "Meeting Participants: " + _name;
                return msg;
            }
            set
            {
                string msg = "Meeting Participants: " + _name;
                msg = value;
            }
        }
    }
}
