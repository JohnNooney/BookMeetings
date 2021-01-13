using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace BookMeetingsPrototype2
{
    /// <summary>
    /// Interaction logic for UserLogin.xaml
    /// </summary>
    public partial class UserLogin : Window
    {

        //set up connection object
        TayMarkDataDataContext db = new TayMarkDataDataContext(Properties.Settings.Default.sql1803534ConnectionString);

        public UserLogin()
        {
            InitializeComponent();
        }

        public string UserId
        {
            get {
                if (userId.Text == "")
                {
                    return "0";
                }
                return userId.Text; 
            }
            set { 
                userId.Text = value; 
            }
        }

        //compare employee records ID num to the entered one
        private void BtnSubmit_Click(object sender, RoutedEventArgs e)
        {
            //create db query to look for entered id match
            var user = (from s in db.TayMarkEmployees
                        where s.empId == int.Parse(UserId)
                        select s);
            //where the email matches, check to see if password follows
            if (user.Any())
            {
                //close this window and send user to main store 
                MainWindow app = new MainWindow(user.First().name, user.First().empId);
                this.Close();
                app.Show();
            }
            //produce message if ID does not exist
            else
            {
                MessageBox.Show("ID does not exist.");
            }
        }

        //******************************************************************************
        //**The following is necessary for making the textbox only accept numeric input
        //******************************************************************************
        private void MaskNumericInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !TextIsNumeric(e.Text);
        }

        private void MaskNumericPaste(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(string)))
            {
                string input = (string)e.DataObject.GetData(typeof(string));
                if (!TextIsNumeric(input)) e.CancelCommand();
            }
            else
            {
                e.CancelCommand();
            }
        }

        private bool TextIsNumeric(string input)
        {
            return input.All(c => Char.IsDigit(c) || Char.IsControl(c));
        }
    }
}
