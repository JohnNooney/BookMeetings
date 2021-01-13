using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookMeetingsPrototype2.ViewModels.ModelClasses
{
    //This is used to fill the combobox with duration times. \
    //gets and set the length when creating a list object
    class Duration
    {
        private TimeSpan _length;
        private string _lengthText;

        public TimeSpan Length
        {
            get { return _length; }
            set { _length = value; }
        }

        public string LengthText
        {
            get {
                _lengthText = _length.ToString("mm") + " min.";
                if (_length == new TimeSpan(1,0,0))
                {
                    _lengthText = _length.ToString("hh") + " hour";
                }
                return _lengthText; 
            }
            set {
                _lengthText = _length.ToString("mm") + " min.";
                if (_length == new TimeSpan(1, 0, 0))
                {
                    _lengthText = _length.ToString("hh") + " hour";
                }
                _lengthText = value; 
            }
        }

        public string DurMsg
        {
            get
            {
                string msg = "Meeting Duration: " + LengthText;
                return msg;
            }
            set
            {
                string msg = "Meeting Duration: " + LengthText;
                msg = value;
            }
        }
    }
}
