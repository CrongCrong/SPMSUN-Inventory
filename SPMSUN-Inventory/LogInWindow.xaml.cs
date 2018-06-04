using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using MySql.Data.MySqlClient;
using SPMSUN_Inventory.classes;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace SPMSUN_Inventory
{
    /// <summary>
    /// Interaction logic for LogInWindow.xaml
    /// </summary>
    public partial class LogInWindow : MetroWindow
    {
        public LogInWindow()
        {
            InitializeComponent();
        }

        ConnectionDB conDB;

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void verifyLogIn()
        {
  
            conDB = new ConnectionDB();
            
            string queryString = "SELECT dbpackage.tblusers.ID, username, lastname, firstname, lastname, " +
                "dbpackage.tblusertype.description FROM (dbpackage.tblusertype INNER JOIN dbpackage.tblusers " +
                "ON dbpackage.tblusertype.ID = dbpackage.tblusers.usertype) WHERE dbpackage.tblusers.username = ? AND " +
                "dbpackage.tblusers.password = ? AND dbpackage.tblusers.isDeleted = 0";

            List<string> parameters = new List<string>();
            parameters.Add(txtUsername.Text);
            parameters.Add(txtPassword.Password);
            MySqlDataReader reader = conDB.getSelectConnection(queryString, parameters);
            UserModel.Loginsuccess = false;
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    UserModel.FirstName = reader["firstname"].ToString();
                    UserModel.LastName = reader["lastname"].ToString();
                    UserModel.UserType = reader["description"].ToString();
                    UserModel.Username = reader["username"].ToString();
                    UserModel.Loginsuccess = true;
                }
                if (UserModel.UserType.ToUpper().Equals("admin".ToUpper()))
                {
                    UserModel.TypeOfAccount = AccountType.ADMIN;
                }
                else
                {
                    UserModel.TypeOfAccount = AccountType.SPMUSER;
                }
            }else
            {
                UserModel.Loginsuccess = false;
            }
            
            conDB.closeConnection();
            
           
        }

        private async Task<bool> checkFields()
        {
            bool ifCorrect = false;

            if (string.IsNullOrEmpty(txtUsername.Text))
            {
                await this.ShowMessageAsync("LOG IN", "Incorrect username/password.");
            }else if(string.IsNullOrEmpty(txtPassword.Password))
            {
                await this.ShowMessageAsync("LOG IN", "Incorrect username/password.");
            }else
            {
                ifCorrect = true;
            }

            return ifCorrect;
        }

        private async void btnLogIn_Click(object sender, RoutedEventArgs e)
        {
            bool x = await checkFields();

            if (x)
            {
               verifyLogIn();
                if (UserModel.Loginsuccess)
                {
                    MainWindow mw = new MainWindow(this);
                    mw.Show();
                    this.Hide();
                    txtUsername.Text = "";
                    txtPassword.Password = "";
                }else
                {
                    await this.ShowMessageAsync("LOG IN", "Incorrect username/password.");
                }
            }
        }

        private async void txtPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return && e.Key == Key.Enter)
            {
                bool x = await checkFields();

                if (x)
                {
                    verifyLogIn();
                    if (UserModel.Loginsuccess)
                    {
                        MainWindow mw = new MainWindow(this);
                        mw.Show();
                        this.Hide();
                        txtUsername.Text = "";
                        txtPassword.Password = "";
                    }
                    else
                    {
                        await this.ShowMessageAsync("LOG IN", "Incorrect username/password.");
                    }
                }
            }
        }
    }
}
