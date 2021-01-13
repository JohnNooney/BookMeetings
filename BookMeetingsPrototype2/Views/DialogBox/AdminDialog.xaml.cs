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

namespace BookMeetingsPrototype2.Views.DialogBox
{
    /// <summary>
    /// Interaction logic for AdminDialog.xaml
    /// </summary>
    public partial class AdminDialog : Window
    {
        //set up connection object
        TayMarkDataDataContext db = new TayMarkDataDataContext(Properties.Settings.Default.sql1803534ConnectionString);

        public AdminDialog()
        {
            InitializeComponent();
        }

        public string Password
        {
            get { return PasswordTextBox.Password; }
            set { PasswordTextBox.Password = value; }
        }

        private void Cancel_Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this.Close();
        }

        private void Enter_Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            //query the password table to see if the password exists
            var pass = (from password in db.TayMarkAdminPasses
                        select password).Single();

            //check the password entered is correct
            //***Make sure to use table from database to store password
            if (pass.password == Password)
            {
                DialogResult = true;
            }
            else
            {
                MessageBox.Show("Invalid Password Entered");
            }
        }
    }
}
