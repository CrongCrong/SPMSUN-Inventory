using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using MySql.Data.MySqlClient;
using SPMSUN_Inventory.classes;
using SPMSUN_Inventory.views;
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

namespace SPMSUN_Inventory
{
    /// <summary>
    /// Interaction logic for AddClientWindow.xaml
    /// </summary>
    public partial class AddClientWindow : MetroWindow
    {
        public AddClientWindow()
        {
            InitializeComponent();
        }
     
        ConnectionDB conDB;
        HomeStockistModel homeStockist;
        MegaStockistModel megaStockist;
        DepotStockistModel depotStockist;
        EmployeeModel employeeStockist;

        SPMSUN_Stockist spmsun_home;
        SPMSUN_MegaStockist spmsun_mega;
        SPMSUN_DepotStockist spmsun_depot;
        SPMSUN_Employees spmsun_employees;

        public AddClientWindow(SPMSUN_Stockist ss)
        {
            spmsun_home = ss;
            InitializeComponent();
        }

        public AddClientWindow(SPMSUN_MegaStockist ms)
        {
            spmsun_mega = ms;
            InitializeComponent();
        }

        public AddClientWindow(SPMSUN_DepotStockist ds)
        {
            spmsun_depot = ds;
            InitializeComponent();
        }

        public AddClientWindow(SPMSUN_Employees es)
        {
            spmsun_employees = es;
            InitializeComponent();
        }

        public AddClientWindow(SPMSUN_Stockist ss, HomeStockistModel hsm)
        {
            homeStockist = hsm;
            spmsun_home = ss;
            InitializeComponent();
        }

        public AddClientWindow(SPMSUN_MegaStockist ms, MegaStockistModel msm)
        {
            megaStockist = msm;
            spmsun_mega = ms; ; 
            InitializeComponent();
        }

        public AddClientWindow(SPMSUN_DepotStockist ds, DepotStockistModel dsm)
        {
            spmsun_depot = ds;
            depotStockist = dsm;
            InitializeComponent();
        }

        public AddClientWindow(SPMSUN_Employees es, EmployeeModel esm)
        {
            spmsun_employees = es;
            employeeStockist = esm;
            InitializeComponent();
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {        
            btnUpdate.Visibility = Visibility.Hidden;   

            if(homeStockist != null)
            {
                txtFirstName.Text = homeStockist.FirstName;
                txtLastName.Text = homeStockist.LastName;
                txtContactNo.Text = homeStockist.ContactNo;
                txtAddress.Text = homeStockist.Address;

                btnUpdate.Visibility = Visibility.Visible;
                btnSave.Visibility = Visibility.Hidden;
            }else if( megaStockist != null)
            {
                txtFirstName.Text = megaStockist.FirstName;
                txtLastName.Text = megaStockist.LastName;
                txtContactNo.Text = megaStockist.ContactNo;
                txtAddress.Text = megaStockist.Address;

                btnUpdate.Visibility = Visibility.Visible;
                btnSave.Visibility = Visibility.Hidden;
            }else if(depotStockist != null)
            {
                txtFirstName.Text = depotStockist.FirstName;
                txtLastName.Text = depotStockist.LastName;
                txtContactNo.Text = depotStockist.ContactNo;
                txtAddress.Text = depotStockist.Address;

                btnUpdate.Visibility = Visibility.Visible;
                btnSave.Visibility = Visibility.Hidden;
            }else if(employeeStockist != null)
            {
                txtFirstName.Text = employeeStockist.FirstName;
                txtLastName.Text = employeeStockist.LastName;
                txtContactNo.Text = employeeStockist.ContactNo;
                txtAddress.Text = employeeStockist.Address;

                btnUpdate.Visibility = Visibility.Visible;
                btnSave.Visibility = Visibility.Hidden;
            }
        }

        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            bool x = await checkFields();
            if(spmsun_home != null)
            {
                if (x)
                {
                    AddClientRecord("2");
                    spmsun_home.dgvclient.ItemsSource = loadClientStockist("2");
                    await this.ShowMessageAsync("ADD CLIENT", "Record successfully saved!");
                    this.Close();
                }
            }else if(spmsun_mega != null)
            {
                if (x)
                {
                    AddClientRecord("1");
                    spmsun_mega.dgvclientmega.ItemsSource = loadMegaClientStockist("1");
                    await this.ShowMessageAsync("ADD CLIENT", "Record successfully saved!");
                    this.Close();
                }
            }else if (spmsun_depot != null)
            {
                if (x)
                {
                    AddClientRecord("3");
                    spmsun_depot.dgvclientmega.ItemsSource = loadDepotClientStockist("3");
                    await this.ShowMessageAsync("ADD CLIENT", "Record successfully saved!");
                    this.Close();
                }
            }else if (spmsun_employees != null)
            {
                if (x)
                {
                    AddClientRecord("4");
                    spmsun_employees.dgvclientemp.ItemsSource = loadEmployees("4");
                    spmsun_employees.dgvclientemp.Items.Refresh();
                    await this.ShowMessageAsync("ADD CLIENT", "Record successfully saved!");
                    this.Close();
                }
            }
            
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
      
        private void AddClientRecord(string strClientType)
        {
            conDB = new ConnectionDB();
            string queryString = "INSERT INTO dbpackage.tblclients (firstname, lastname, phonenumber, address, clienttype, isDeleted) " +
                " VALUES(?,?,?,?,?,0)";

            List<string> parameters = new List<string>();
            parameters.Add(txtFirstName.Text);
            parameters.Add(txtLastName.Text);
            parameters.Add(txtContactNo.Text);
            parameters.Add(txtAddress.Text);
            parameters.Add(strClientType);

            conDB.AddRecordToDatabase(queryString, parameters);
            conDB.closeConnection();
        }

        private List<HomeStockistModel> loadClientStockist(string strClientType)
        {
            conDB = new ConnectionDB();
            HomeStockistModel hmeStockist = new HomeStockistModel();
            List<HomeStockistModel> lstHomeStockist = new List<HomeStockistModel>();

            string queryString = "SELECT ID, firstname, lastname, phonenumber, address, clienttype FROM dbpackage.tblclients where " +
                "isDeleted = 0 AND clienttype = ? ORDER BY lastname asc";

            List<string> parameters = new List<string>();
            parameters.Add(strClientType);

            MySqlDataReader reader = conDB.getSelectConnection(queryString, parameters);

            while (reader.Read())
            {
                hmeStockist.ID = reader["ID"].ToString();
                hmeStockist.FirstName = reader["firstname"].ToString();
                hmeStockist.LastName = reader["lastname"].ToString();
                hmeStockist.ContactNo = reader["phonenumber"].ToString();
                hmeStockist.Address = reader["address"].ToString();
                hmeStockist.Fullname = hmeStockist.LastName + ", " + hmeStockist.FirstName;

                lstHomeStockist.Add(hmeStockist);
                hmeStockist = new HomeStockistModel();
            }

            conDB.closeConnection();

            return lstHomeStockist;
        }


        private List<MegaStockistModel> loadMegaClientStockist(string strClientType)
        {
            conDB = new ConnectionDB();
            MegaStockistModel mgaStockist = new MegaStockistModel();
            List<MegaStockistModel> lstMegaStockist = new List<MegaStockistModel>();

            string queryString = "SELECT ID, firstname, lastname, phonenumber, address, clienttype FROM dbpackage.tblclients where " +
                "isDeleted = 0 AND clienttype = ? ORDER BY lastname asc";

            List<string> parameters = new List<string>();
            parameters.Add(strClientType);

            MySqlDataReader reader = conDB.getSelectConnection(queryString, parameters);

            while (reader.Read())
            {
                mgaStockist.ID = reader["ID"].ToString();
                mgaStockist.FirstName = reader["firstname"].ToString();
                mgaStockist.LastName = reader["lastname"].ToString();
                mgaStockist.ContactNo = reader["phonenumber"].ToString();
                mgaStockist.Address = reader["address"].ToString();
                mgaStockist.Fullname = mgaStockist.LastName + ", " + mgaStockist.FirstName;

                lstMegaStockist.Add(mgaStockist);
                mgaStockist = new MegaStockistModel();
            }

            conDB.closeConnection();

            return lstMegaStockist;
        }


        private List<DepotStockistModel> loadDepotClientStockist(string strClientType)
        {
            conDB = new ConnectionDB();
            DepotStockistModel dpotStockist = new DepotStockistModel();
            List<DepotStockistModel> lstDepotStockist = new List<DepotStockistModel>();

            string queryString = "SELECT ID, firstname, lastname, phonenumber, address, clienttype FROM dbpackage.tblclients where " +
                "isDeleted = 0 AND clienttype = ? ORDER BY lastname asc";

            List<string> parameters = new List<string>();
            parameters.Add(strClientType);

            MySqlDataReader reader = conDB.getSelectConnection(queryString, parameters);

            while (reader.Read())
            {
                dpotStockist.ID = reader["ID"].ToString();
                dpotStockist.FirstName = reader["firstname"].ToString();
                dpotStockist.LastName = reader["lastname"].ToString();
                dpotStockist.ContactNo = reader["phonenumber"].ToString();
                dpotStockist.Address = reader["address"].ToString();
                dpotStockist.Fullname = dpotStockist.LastName + ", " + dpotStockist.FirstName;

                lstDepotStockist.Add(dpotStockist);
                dpotStockist = new DepotStockistModel();
            }

            conDB.closeConnection();

            return lstDepotStockist;
        }

        private List<EmployeeModel> loadEmployees(string strClientType)
        {
            EmployeeModel empStockist = new EmployeeModel();
            List<EmployeeModel> lstEmpStockist = new List<EmployeeModel>();

            string queryString = "SELECT ID, firstname, lastname, phonenumber, address, clienttype FROM dbpackage.tblclients where " +
                "isDeleted = 0 AND clienttype = ? ORDER BY lastname asc";

            List<string> parameters = new List<string>();
            parameters.Add(strClientType);

            MySqlDataReader reader = conDB.getSelectConnection(queryString, parameters);

            while (reader.Read())
            {
                empStockist.ID = reader["ID"].ToString();
                empStockist.FirstName = reader["firstname"].ToString();
                empStockist.LastName = reader["lastname"].ToString();
                empStockist.ContactNo = reader["phonenumber"].ToString();
                empStockist.Address = reader["address"].ToString();
                empStockist.Fullname = empStockist.LastName + ", " + empStockist.FirstName;

                lstEmpStockist.Add(empStockist);
                empStockist = new EmployeeModel();
            }

            conDB.closeConnection();

            return lstEmpStockist;

        }


        private void UpdateClientRecord(string strClientType, string stockistID)
        {
            conDB = new ConnectionDB();

            string queryString = "UPDATE dbpackage.tblclients SET firstname = ?, lastname = ?, phonenumber = ?, address = ?, clienttype = ?"
                + " WHERE ID = ?";
            List<string> parameters = new List<string>();
            parameters.Add(txtFirstName.Text);
            parameters.Add(txtLastName.Text);
            parameters.Add(txtContactNo.Text);
            parameters.Add(txtAddress.Text);
            parameters.Add(strClientType);
            parameters.Add(stockistID);

            conDB.AddRecordToDatabase(queryString, parameters);
            conDB.closeConnection();
        }

        private async Task<bool> checkFields()
        {
            bool ifAllCorrect = false;

            if (string.IsNullOrEmpty(txtFirstName.Text))
            {
                await this.ShowMessageAsync("First Name", "Please provide first name");
            }
            else if (string.IsNullOrEmpty(txtLastName.Text))
            {
                await this.ShowMessageAsync("Last Name", "Please provide last name");
            }
            else if (string.IsNullOrEmpty(txtAddress.Text))
            {
                await this.ShowMessageAsync("Address", "Please provide address");
            }
            else if (string.IsNullOrEmpty(txtContactNo.Text))
            {
                await this.ShowMessageAsync("Contact Number", "Please provide contact number");
            }
            else
            {
                ifAllCorrect = true;
            }

            return ifAllCorrect;
        }

        private async void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            bool x = await checkFields();
            if(homeStockist != null)
            {
                if (x)
                {
                    UpdateClientRecord("2", homeStockist.ID);
                    spmsun_home.dgvclient.ItemsSource = loadClientStockist("2");
                    await this.ShowMessageAsync("RECORD UPDATE", "Record updated successfully!");
                    this.Close();
                }
            }else if(megaStockist != null)
            {
                if (x)
                {
                    UpdateClientRecord("1", megaStockist.ID);
                    spmsun_home.dgvclient.ItemsSource = loadClientStockist("1");
                    await this.ShowMessageAsync("RECORD UPDATE", "Record updated successfully!");
                    this.Close();
                }
            }else if(depotStockist != null)
            {
                if (x)
                {
                    UpdateClientRecord("3", depotStockist.ID);
                    spmsun_home.dgvclient.ItemsSource = loadClientStockist("3");
                    await this.ShowMessageAsync("RECORD UPDATE", "Record updated successfully!");
                    this.Close();
                }
            }
           
        }

    }
}
