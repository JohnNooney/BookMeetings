using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookMeetingsPrototype2.ViewModels.ModelClasses
{
    //This is used to fill the combo box with rooms. \
    //gets and set the name when creating a list object
    class Room
    {
        private string _name;
        private int _capacity;
        private int _roomId;
        public string RoomName
        {
            get { return _name; }
            set { _name = value; }
        }
        public int Capacity
        {
            get { return _capacity; }
            set { _capacity = value; }
        }

        public int RoomId
        {
            get { return _roomId; }
            set { _roomId = value; }
        }

        public string RoomMsg
        {
            get
            {
                string msg = "Meeting Room: " + _name + "\nCapacity: " + _capacity;
                return msg;
            }
            set
            {
                string msg = "Meeting Room: " + _name + "\nCapacity: " + _capacity;
                msg = value;
            }
        }

    }
}
