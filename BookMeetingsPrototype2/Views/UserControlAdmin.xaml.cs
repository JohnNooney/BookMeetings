using BookMeetingsPrototype2.ViewModels.ModelClasses;
using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BookMeetingsPrototype2.Views
{
    /// <summary>
    /// Interaction logic for UserControlAdmin.xaml
    /// </summary>
    public partial class UserControlAdmin : UserControl
    {
        //set up connection object
        TayMarkDataDataContext db = new TayMarkDataDataContext(Properties.Settings.Default.sql1803534ConnectionString);

        public UserControlAdmin()
        {
            InitializeComponent();
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //get the combo box value to fill the the data grid with the corresponding category type
            ComboBoxItem typeItem = (ComboBoxItem)comboBox.SelectedItem;
            string selected = typeItem.Content.ToString();

            //get the products with the matching product type in the comboBox
            if (selected == "Rooms")
            {
                roomQuery();
            }
            else if (selected == "Employees")
            {
                empQuery();
            }
        }

        //get the employee records to fill in the datagrid
        private void empQuery()
        {
            var query = from emp in db.TayMarkEmployees
                        select new Employee { ID = emp.empId, Name = emp.name };

            dataGrid.ItemsSource = query;
        }

        //get the room records to fill in the datagrid
        private void roomQuery()
        {
            var query = from rooms in db.TayMarkRooms
                        select new Room { ID = rooms.roomId, Name = rooms.roomName, Capacity = rooms.roomCapacity };

            dataGrid.ItemsSource = query;
        }

        //Based on the row selected and the current table being viewed, delete the record in the table
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            //get the combo box value to choose which table to delete the selected row from
            ComboBoxItem typeItem = (ComboBoxItem)comboBox.SelectedItem;
            string selected = typeItem.Content.ToString();

            if (selected == "Rooms")
            {
                ///****Make sure to delete any records that are associated with rooms
                //get the selected room row
                Room roomRow = dataGrid.SelectedItem as Room;

                if (roomRow != null)
                {
                    if (MessageBox.Show("Are sure you want to delete this record?", "Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                    {
                        //query the database to find the associated selected row
                        var roomRec = (from room in db.TayMarkRooms
                                       where roomRow.ID == room.roomId
                                       select room);

                        if (roomRec.Any()) //if the selected room record exists in the database
                        {
                            //get the booked rooms that are associate with the rooms FK
                            //*** Must delete FK records before the PK record
                            var roomBookings = (from bookings in db.TayMarkBookings
                                                where roomRow.ID == bookings.meetingRoomId
                                                select bookings);

                            if (roomBookings.Any()) //check that there are any records to delete
                            {
                                foreach (var booking in roomBookings) //delete each associated record
                                {


                                    //get the employee booking records. these have a booking FK
                                    //Must delete these records first
                                    var empBookings = (from empsBooked in db.TayMarkBooking_Emps
                                                       where booking.meetingId == empsBooked.meetingId
                                                       select empsBooked);

                                    foreach (var empBookRec in empBookings) //if a booked meeting record exists then so will the employee meeting records
                                    {
                                        db.TayMarkBooking_Emps.DeleteOnSubmit(empBookRec);
                                    }

                                    db.TayMarkBookings.DeleteOnSubmit(booking);
                                }
                            }

                            //get the single record selected and prepare to delete\
                            //delete last
                            db.TayMarkRooms.DeleteOnSubmit(roomRec.Single());

                            try
                            {
                                //submit changes
                                db.SubmitChanges();
                                MessageBox.Show("Deleted room and associated records");

                                //update the datagrid
                                roomQuery();
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.ToString());
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Room selected is not created yet.");
                    }
                }
                else
                {
                    MessageBox.Show("Please select a row to delete.");
                }


                

            }
            else if (selected == "Employees")
            {
                Employee empRow = dataGrid.SelectedItem as Employee;

                if (empRow != null)
                {
                    if (MessageBox.Show("Are sure you want to delete this record?", "Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                    {
                        var empRec = (from emp in db.TayMarkEmployees
                                      where empRow.ID == emp.empId
                                      select emp);

                        if (empRec.Any())
                        {
                            //get the employee booking records. these have a booking FK
                            //Must delete these records first
                            var empBookings = (from empsBooked in db.TayMarkBooking_Emps
                                               where empRow.ID == empsBooked.employeeId
                                               select empsBooked);

                            if (empBookings.Any()) //check that there are any records to delete
                            {
                                foreach (var empBookRec in empBookings) //delete each associated record
                                {
                                    db.TayMarkBooking_Emps.DeleteOnSubmit(empBookRec);
                                }
                            }
                            db.TayMarkEmployees.DeleteOnSubmit(empRec.Single());

                            try
                            {
                                //submit changes
                                db.SubmitChanges();
                                MessageBox.Show("Deleted employee and associated records");

                                //update the datagrid
                                empQuery();
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.ToString());
                            }
                        }
                        else
                        {
                            MessageBox.Show("Employee selected is not yet created.");
                        }
                    }
                        
                }
                else
                {
                    MessageBox.Show("Please select a row to delete.");
                }

                
            }
        }

        //upon saving button click determins which category is in and then saves
        //the entered data either as a new record or updates the existing data
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            //get the combo box value to choose which table to delete the selected row from
            ComboBoxItem typeItem = (ComboBoxItem)comboBox.SelectedItem;
            string selected = typeItem.Content.ToString();

            
            if (selected == "Rooms")
            {
                //get the selected room row
                Room roomRow = dataGrid.SelectedItem as Room;


                //make sure all the attributes are filled in
                if (roomRow.ID != 0 && roomRow.Name != null && roomRow.Name != "" && roomRow.Capacity != 0)
                {
                    //query the database to find the associate row
                    //or create a new record altogether
                    var roomRec = (from room in db.TayMarkRooms
                                   where roomRow.ID == room.roomId
                                   select room);

                    if (roomRec.Any())
                    {
                        //map the new values to the existing record
                        roomRec.SingleOrDefault().roomId = roomRow.ID;
                        roomRec.SingleOrDefault().roomName = roomRow.Name;
                        roomRec.SingleOrDefault().roomCapacity = roomRow.Capacity;

                        try
                        {
                            //submit
                            db.SubmitChanges();
                            MessageBox.Show("Exisiting room has been Updated.");
                            roomQuery(); //update datagrid
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("error: "+ex.ToString());
                        }
                    }
                    else //new room record
                    {
                        TayMarkRoom newRoom = new TayMarkRoom()
                        {
                            roomId = roomRow.ID,
                            roomName = roomRow.Name,
                            roomCapacity = roomRow.Capacity
                        };

                        //place query into queue
                        db.TayMarkRooms.InsertOnSubmit(newRoom);

                        try
                        {
                            db.SubmitChanges();
                            MessageBox.Show("New room has been added.");
                            roomQuery(); //update datagrid
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("error: " + ex.ToString());
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Please fill in all details before saving.");
                }

            }
            else if (selected == "Employees")
            {
                Employee empRow = dataGrid.SelectedItem as Employee;

                if (empRow.ID != 0 && empRow.Name != null && empRow.Name == "")
                {
                    var empRec = (from emp in db.TayMarkEmployees
                                  where empRow.ID == emp.empId
                                  select emp);

                    if (empRec.Any())
                    {

                        //map the new values to the existing record
                        empRec.SingleOrDefault().empId = empRow.ID;
                        empRec.SingleOrDefault().name = empRow.Name;

                        try
                        {
                            //submit
                            db.SubmitChanges();
                            MessageBox.Show("Exisiting employee has been updated.");
                            empQuery();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("error: " + ex.ToString());
                        }
                    }
                    else
                    {
                        TayMarkEmployee newEmp = new TayMarkEmployee()
                        {
                            name = empRow.Name,
                            empId = empRow.ID
                        };

                        db.TayMarkEmployees.InsertOnSubmit(newEmp);

                        try
                        {
                            db.SubmitChanges();
                            MessageBox.Show("New employee has been added.");
                            empQuery();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("error: " + ex.ToString());
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Please fill in all details before saving.");
                }
            }

        }

        //Upon button click query the database for the past six months worth of records (*In this case all of them since everytime app starts up delete old records)
        //and export the data into a csv file that is placed in the same directory as this program
        private void ExportButton_Click(object sender, RoutedEventArgs e)
        {
            //get every single record and store it as a meeting object
            var records = (from booking in db.TayMarkBooking_Emps
                           join meeting in db.TayMarkBookings on booking.meetingId equals meeting.meetingId
                           join room in db.TayMarkRooms on meeting.meetingRoomId equals room.roomId
                           select new Meeting { MeetingId = meeting.meetingId, MeetingTitle = meeting.meetingTitle, RoomName = room.roomName, MeetingDuration = meeting.meetingDuration, MeetingStart = meeting.meetingStart, MeetingEnd = (DateTime)meeting.meetingEnd }).OrderBy(x => x.MeetingStart);

            //setup the csv file
            var csv = new StringBuilder();

            //loop through every record and place the relevant data inside the csv file
            foreach (var meeting in records)
            {
                //query the db for all the participants in the relevant meeting
                //get the participants that are in the meeting
                var meetingParticipants = (from booking in db.TayMarkBooking_Emps
                                           join emps in db.TayMarkEmployees on booking.employeeId equals emps.empId
                                           where meeting.MeetingId == booking.meetingId
                                           select emps);

                //make a string list of every participant in that meeting
                string meetingEmps = "";
                foreach (var emp in meetingParticipants)
                {
                    string temp = ""; //make a placeholder string
                    temp = emp.name + ": " + emp.empId + "\n,,,,,,";

                    meetingEmps += temp; //add the formated employee to the string of all employees in the meeting
                }

                //****
                //Append all meeting information together to placed into the csv
                var meetingId = meeting.MeetingId;
                var title = meeting.MeetingTitle;
                var room = meeting.RoomName;
                var start = meeting.MeetingStart;
                var end = meeting.MeetingEnd;
                var duration = meeting.MeetingDuration;
                var participants = meetingEmps;

                var newLine = string.Format("{0},{1},{2},{3},{4},{5},{6}", meetingId, title, room, start, end, duration, participants);
                
                //add the line to the csv stringbuilder
                csv.AppendLine(newLine);
            }

            //write text to csv file
            try
            {
                File.WriteAllText("Meetings_Past_6_Months.csv", csv.ToString());
                MessageBox.Show("Data exported to same directory as program location.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Data export failed. Error: " + ex);
                throw;
            }
            
            
        }
    }

    /// <summary>
    /// Internal classes exclusive to this view
    /// used for elements in the datagrid
    /// </summary>
    internal class Employee
    {
        public int ID { get; set; }
        public string Name { get; set; }
    }

    internal class Room
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int Capacity { get; set; }
    }
}
