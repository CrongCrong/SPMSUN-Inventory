using MahApps.Metro.Controls.Dialogs;
using MySql.Data.MySqlClient;
using SPMSUN_Inventory.classes;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace SPMSUN_Inventory.views
{
    /// <summary>
    /// Interaction logic for SPMSUN_Employees.xaml
    /// </summary>
    public partial class SPMSUN_Employees : UserControl
    {
        public SPMSUN_Employees()
        {
            InitializeComponent();
        }
        ConnectionDB conDB;
        double dblTotalView = 0.0;
        double dblTotal = 0.0;
        double dblTotalPaid = 0.0;

        EmployeeModel viewEmployee;
        OrderHistoryModel ohm = new OrderHistoryModel();
        List<string> parameters;
        string queryString = "";

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            dgvclientemp.ItemsSource = loadClientEmployees();
            flyout.IsOpen = false;
        }

        private List<EmployeeModel> loadClientEmployees()
        {
            conDB = new ConnectionDB();
            EmployeeModel empStockist = new EmployeeModel();
            List<EmployeeModel> lstEmpStockist = new List<EmployeeModel>();

            queryString = "SELECT ID, firstname, lastname, phonenumber, address, clienttype FROM dbpackage.tblclients where " +
                "isDeleted = 0 AND clienttype = 4 ORDER BY lastname asc";

            MySqlDataReader reader = conDB.getSelectConnection(queryString, null);

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

        private List<EmployeeModel> searchStockist()
        {
            conDB = new ConnectionDB();
            EmployeeModel depotStockist = new EmployeeModel();
            List<EmployeeModel> lstDepotStockist = new List<EmployeeModel>();

            queryString = "SELECT ID, firstname, lastname, phonenumber, address, clienttype FROM dbpackage.tblclients where " +
                "(isDeleted = 0) AND (clienttype = 4)";

            queryString += " AND (dbpackage.tblclients.lastname like '%" + searchName.Text + "%' OR dbpackage.tblclients.firstname " +
                "like '%" + searchName.Text + "%')";

            MySqlDataReader reader = conDB.getSelectConnection(queryString, null);

            while (reader.Read())
            {
                depotStockist.ID = reader["ID"].ToString();
                depotStockist.FirstName = reader["firstname"].ToString();
                depotStockist.LastName = reader["lastname"].ToString();
                depotStockist.ContactNo = reader["phonenumber"].ToString();
                depotStockist.Address = reader["address"].ToString();
                depotStockist.Fullname = depotStockist.LastName + ", " + depotStockist.FirstName;

                lstDepotStockist.Add(depotStockist);
                depotStockist = new EmployeeModel();
            }

            conDB.closeConnection();

            return lstDepotStockist;
        }

        private async void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            MahApps.Metro.Controls.MetroWindow window = Window.GetWindow(this) as MahApps.Metro.Controls.MetroWindow;

            if (string.IsNullOrEmpty(searchName.Text))
            {
                await window.ShowMessageAsync("Search Name", "Please provide name");
            }
            else
            {
                dgvclientemp.ItemsSource = searchStockist();
            }
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            searchName.Text = "";
            dgvclientemp.ItemsSource = loadClientEmployees();
        }

        private List<OrderHistoryModel> loadOrdersHistory(string strClientID)
        {
            conDB = new ConnectionDB();

            OrderHistoryModel order = new OrderHistoryModel();
            List<OrderHistoryModel> lstOrderHistory = new List<OrderHistoryModel>();

            queryString = "SELECT tblorderhistory.ID, clientID, drNo, total, date, IF(isPaid, 'YES', 'NO') as 'ifPaid', IF(isCancelled, 'YES', 'NO') as 'ifCancelled'" +
                "FROM dbpackage.tblorderhistory " +
                " WHERE tblorderhistory.isDeleted = 0 AND clientID = ? order by drNo";

            parameters = new List<string>();

            parameters.Add(strClientID);

            MySqlDataReader reader = conDB.getSelectConnection(queryString, parameters);

            while (reader.Read())
            {
                order.ID = reader["ID"].ToString();

                order.DRDate = Convert.ToDateTime(reader["date"].ToString()).ToShortDateString();

                order.ClientID = reader["clientID"].ToString();
                order.DRNumber = reader["drNo"].ToString();
                int x = Convert.ToInt32(reader["total"].ToString());
                order.Total = x.ToString("N0");
                order.ifPaid = reader["ifPaid"].ToString();
                order.ifCancelled = reader["ifCancelled"].ToString();
                lstOrderHistory.Add(order);
                order = new OrderHistoryModel();
            }

            conDB.closeConnection();

            return lstOrderHistory;
        }

        private List<ProductModel> getProductsOnDR(string strOrderID)
        {
            conDB = new ConnectionDB();
            List<ProductModel> lstProducts = new List<ProductModel>();
            ProductModel product = new ProductModel();

            queryString = "SELECT dbpackage.tblorderdetails.ID, packageID, tblproducts.description, qty, price FROM " +
                "(dbpackage.tblorderdetails INNER JOIN dbpackage.tblproducts ON dbpackage.tblorderdetails.productID = " +
                "dbpackage.tblproducts.ID) WHERE tblorderdetails.isDeleted = 0 AND orderID = ?";

            parameters = new List<string>();
            parameters.Add(strOrderID);

            MySqlDataReader reader = conDB.getSelectConnection(queryString, parameters);
            while (reader.Read())
            {
                product.Description = reader["description"].ToString();
                product.ID = reader["packageID"].ToString();
                product.Qty = reader["qty"].ToString();
                product.Total = reader["price"].ToString();

                double tempTotal = Convert.ToDouble(lblTotal.Content);
                dblTotalView = tempTotal + Convert.ToDouble(product.Total);
                lblTotal.Content = dblTotalView.ToString();
                lstProducts.Add(product);
                product = new ProductModel();
            }
            conDB.closeConnection();
            lblTotal.Content = dblTotalView.ToString("N0");
            return lstProducts;
        }

        private List<PackageModel> getPackagesOnDR(string strOrderID)
        {
            conDB = new ConnectionDB();

            List<PackageModel> lstPackages = new List<PackageModel>();
            PackageModel package = new PackageModel();

            queryString = "SELECT packageID, dbpackage.tblpackage.name, qty, price FROM " +
                "(dbpackage.tblorderdetails INNER JOIN dbpackage.tblpackage ON dbpackage.tblorderdetails.packageID = dbpackage.tblpackage.ID) " +
                "WHERE productID = 0 AND dbpackage.tblorderdetails.isDeleted = 0 AND orderID = ?";

            parameters = new List<string>();
            parameters.Add(strOrderID);

            MySqlDataReader reader = conDB.getSelectConnection(queryString, parameters);
            while (reader.Read())
            {
                package.Name = reader["name"].ToString();
                package.ID = reader["packageID"].ToString();
                package.Qty = reader["qty"].ToString();
                package.Total = reader["price"].ToString();

                double tempTotal = Convert.ToDouble(lblTotal.Content);
                dblTotalView = tempTotal + Convert.ToDouble(package.Total);
                lblTotal.Content = dblTotalView.ToString();
                lstPackages.Add(package);
                package = new PackageModel();
            }
            conDB.closeConnection();
            lblTotal.Content = dblTotalView.ToString("N0");
            return lstPackages;
        }

        private List<PaymentHistoryModel> getPaymentHistory()
        {
            conDB = new ConnectionDB();
            List<PaymentHistoryModel> lstPayments = new List<PaymentHistoryModel>();
            PaymentHistoryModel payment = new PaymentHistoryModel();

            queryString = "SELECT ID, amountpaid, date, notes FROM dbpackage.tblpaymenthistory WHERE isDeleted = 0 AND clientID = ? AND orderID = ?";
            parameters = new List<string>();
            parameters.Add(viewEmployee.ID);
            parameters.Add(ohm.ID);

            MySqlDataReader reader = conDB.getSelectConnection(queryString, parameters);

            while (reader.Read())
            {
                payment.ID = reader["ID"].ToString();
                payment.AmountPaid = reader["amountpaid"].ToString();
                double amtPaid = Convert.ToDouble(payment.AmountPaid);
                dblTotalPaid = dblTotalPaid + amtPaid;

                payment.Date = Convert.ToDateTime(reader["date"].ToString()).ToShortDateString();
                if (string.IsNullOrEmpty(reader["notes"].ToString()))
                {
                    payment.Notes = "";
                }
                else
                {
                    payment.Notes = reader["notes"].ToString();
                }

                lstPayments.Add(payment);
                payment = new PaymentHistoryModel();

            }

            conDB.closeConnection();

            return lstPayments;

        }

        private void disableEnableControls(bool bl)
        {
            dgvclientemp.IsEnabled = bl;
            btnAddClient.IsEnabled = bl;
            btnEditClient.IsEnabled = bl;
            btnDelete.IsEnabled = bl;
        }

        private void btnViewDR_Click(object sender, RoutedEventArgs e)
        {
            viewEmployee = dgvclientemp.SelectedItem as EmployeeModel;

            if (viewEmployee != null)
            {
                dgvdr.ItemsSource = loadOrdersHistory(viewEmployee.ID);
                lblCientNameDisplay.Content = "D.R. records for " + viewEmployee.Fullname;

                disableEnableControls(false);
                btnDRDetails.IsEnabled = true;
                btnAddDR.IsEnabled = true;
                btnEditDR.IsEnabled = true;
                btnDeleteDR.IsEnabled = true;
            }
        }

        private void btnDRDetails_Click(object sender, RoutedEventArgs e)
        {
            ohm = dgvdr.SelectedItem as OrderHistoryModel;

            if (ohm != null)
            {
                lblDrNumber.Content = "DR NUMBER: " + ohm.DRNumber;
                lblDate.Content = "DATE: " + ohm.DRDate;
                lblClientName.Content = "CLIENT NAME: " + viewEmployee.Fullname;

                dgvPackages.ItemsSource = getPackagesOnDR(ohm.ID);
                dgvProducts.ItemsSource = getProductsOnDR(ohm.ID);
                dgvPaymentHistory.ItemsSource = getPaymentHistory();
                if (ohm.ifPaid.Equals("YES"))
                {
                    lblBalance.Content = "0.0";
                }
                else
                {
                    lblBalance.Content = (dblTotalView - getBalanceOnDR()).ToString("N0");
                }
                lblCancelled.Content = "CANCELLED :" + ohm.ifCancelled;
                flyout.IsOpen = true;

            }
        }

        private void flyout_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!flyout.IsOpen)
            {
                dblTotal = 0.0;
                dblTotalView = 0.0;
                lblTotal.Content = 0.0;
                //btnNotifs.Visibility = Visibility.Visible;
            }
        }

        private double getBalanceOnDR()
        {
            double dblBalance = 0.0;

            conDB = new ConnectionDB();
            queryString = "SELECT coalesce(sum(amountpaid),0) as total FROM dbpackage.tblpaymenthistory WHERE " +
                "clientID = ? and orderID = ? AND isDeleted = 0";

            parameters = new List<string>();
            parameters.Add(viewEmployee.ID);
            parameters.Add(ohm.ID);

            MySqlDataReader reader = conDB.getSelectConnection(queryString, parameters);

            while (reader.Read())
            {
                dblBalance = Convert.ToDouble(reader["total"].ToString());
            }

            conDB.closeConnection();
            return dblBalance;
        }

        private void btnAddClient_Click(object sender, RoutedEventArgs e)
        {
            AddClientWindow acw = new AddClientWindow(this);
            acw.ShowDialog();
        }

        private void btnEditClient_Click(object sender, RoutedEventArgs e)
        {
            EmployeeModel empS = dgvclientemp.SelectedItem as EmployeeModel;
            if (empS != null)
            {
                AddClientWindow acw = new AddClientWindow(this, empS);
                acw.ShowDialog();
            }
            
        }

        private async void btnAddDR_Click(object sender, RoutedEventArgs e)
        {
            MahApps.Metro.Controls.MetroWindow window = Window.GetWindow(this) as MahApps.Metro.Controls.MetroWindow;

            if (viewEmployee != null)
            {
                OrderWindow ow = new OrderWindow(viewEmployee, null);
                ow.ShowDialog();
            }
            else
            {
                await window.ShowMessageAsync("ADDING DR", "Please click View DR for selected client.");
            }


        }

        private void btnEditDR_Click(object sender, RoutedEventArgs e)
        {
            OrderHistoryModel orderHistMod = dgvdr.SelectedItem as OrderHistoryModel;

            if (orderHistMod != null)
            {
                OrderWindow orderWin = new OrderWindow(viewEmployee, orderHistMod);
                orderWin.ShowDialog();
            }
        }

        private async void btnDeleteDR_Click(object sender, RoutedEventArgs e)
        {
            OrderHistoryModel orderHist = dgvdr.SelectedItem as OrderHistoryModel;

            MahApps.Metro.Controls.MetroWindow window = Window.GetWindow(this) as MahApps.Metro.Controls.MetroWindow;

            if (orderHist != null)
            {
                 MessageDialogResult result = await window.ShowMessageAsync("Delete D.R.", "Are you sure you want to Delete D.R. No: " + orderHist.DRNumber, MessageDialogStyle.AffirmativeAndNegative);

                if (result == MessageDialogResult.Affirmative)
                {
                    deleteDR(orderHist.ID);
                    dgvdr.ItemsSource = loadOrdersHistory(viewEmployee.ID);
                    dgvdr.Items.Refresh();
                }
            }
          
        }

        private void deleteEmployee(string empID)
        {
            conDB = new ConnectionDB();
            queryString = "UPDATE dbpackage.tblclients SET isDeleted = 1 WHERE ID = ?";
            parameters = new List<string>();

            parameters.Add(empID);

            conDB.AddRecordToDatabase(queryString, parameters);
            conDB.closeConnection();
        }

        private void deleteDR(string recID)
        {
            conDB = new ConnectionDB();

            queryString = "UPDATE dbpackage.tblorderhistory SET isDeleted = 1 WHERE ID = ?";
            parameters = new List<string>();
            parameters.Add(recID);

            conDB.AddRecordToDatabase(queryString, parameters);
            conDB.closeConnection();

        }

        private async void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            EmployeeModel empm = dgvclientemp.SelectedItem as EmployeeModel;

            if (empm != null)
            {
                MahApps.Metro.Controls.MetroWindow window = Window.GetWindow(this) as MahApps.Metro.Controls.MetroWindow;

                MessageDialogResult result = await window.ShowMessageAsync("Delete Client", "Are you sure you want to delete client: " + empm.Fullname, MessageDialogStyle.AffirmativeAndNegative);

                if (result == MessageDialogResult.Affirmative)
                {
                    deleteEmployee(empm.ID);
                    dgvclientemp.ItemsSource = loadClientEmployees();
                    dgvclientemp.Items.Refresh();
                }

            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            disableEnableControls(true);
            btnDRDetails.IsEnabled = false;
            btnAddDR.IsEnabled = false;
            btnEditDR.IsEnabled = false;
            btnDeleteDR.IsEnabled = false;

            List<OrderHistoryModel> lstOrderHistory = new List<OrderHistoryModel>();
            dgvdr.ItemsSource = lstOrderHistory;
            dgvdr.Items.Refresh();
            lblCientNameDisplay.Content = "";
        }
    }
}
