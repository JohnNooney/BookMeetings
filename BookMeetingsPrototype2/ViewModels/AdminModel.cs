using BookMeetingsPrototype2.ViewModels.ModelClasses;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookMeetingsPrototype2.ViewModels
{
    class AdminModel : BaseViewModel
    {
        //store the user information
        private Participant _user;

        //category items that can be selected in the combo box
        //for data grid display
        //private ObservableCollection<string> _comboItems;
        //private string _selectedComboItem;

        ////collection that will be displayed in the data grid
        //private ObservableCollection<Room> _categoryDataGridRooms;
        //private ObservableCollection<Participant> _categoryDataGridParticipants;

        public AdminModel(Participant loggedUser)
        {
            User = loggedUser;

            //loadTestData();
        }

        public Participant User
        {
            get { return _user; }
            set { SetProperty(ref _user, value); }
        }

        //    private void loadTestData()
        //    {
        //        ComboItems = new ObservableCollection<string> { 
        //            "Rooms", "Employees"
        //        };

        //        CategoryDataGridRooms = new ObservableCollection<Room>
        //        {
        //             new Room(){RoomName="Far Far Away", Capacity=6}
        //            ,new Room(){RoomName="HogWarts", Capacity=4}
        //            ,new Room(){RoomName="Hundred Acrew Wood", Capacity=10}
        //            ,new Room(){RoomName="Narnia", Capacity=15}
        //            ,new Room(){RoomName="Neverland", Capacity=6}
        //        };

        //        CategoryDataGridParticipants = new ObservableCollection<Participant>
        //        {
        //            new Participant(){Name="Bill"}
        //            ,new Participant(){Name="Jeremy", EmpId=12345}
        //            ,new Participant(){Name="Maxell", EmpId=54321}
        //            ,new Participant(){Name="Lydia", EmpId=98765}
        //            ,new Participant(){Name="Daniela", EmpId=56789}
        //        };
        //    }

        //    public String SelectedComboItem
        //    {
        //        get { return _selectedComboItem; }
        //        set { SetProperty(ref _selectedComboItem, value); }
        //    }

        //    

        //    public ObservableCollection<string> ComboItems
        //    {
        //        get { return _comboItems; }
        //        set { SetProperty(ref _comboItems, value); }
        //    }
        //    public ObservableCollection<Room> CategoryDataGridRooms
        //    {
        //        get { return _categoryDataGridRooms; }
        //        set { SetProperty(ref _categoryDataGridRooms, value); }
        //    }

        //    public ObservableCollection<Participant> CategoryDataGridParticipants
        //    {
        //        get { return _categoryDataGridParticipants; }
        //        set { SetProperty(ref _categoryDataGridParticipants, value); }
        //    }
    }
}
