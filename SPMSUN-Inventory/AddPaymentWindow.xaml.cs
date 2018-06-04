using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using MySql.Data.MySqlClient;
using SPMSUN_Inventory.classes;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace SPMSUN_Inventory
{
    /// <summary>
    /// Interaction logic for AddPaymentWindow.xaml
    /// </summary>
    public partial class AddPaymentWindow : MetroWindow
    {
        public AddPaymentWindow()
        {
            InitializeComponent();
        }

        ConnectionDB conDB;
        HomeStockistModel homeStockModel;
        MegaStockistModel megaStockModel;
        DepotStockistModel depotStockModel;
        EmployeeModel employeesModel;

        OrderHistoryModel orderHistory;
        double dblTotalPaid = 0.0;
        OrderWindow orderWin;

        public AddPaymentWindow(OrderWindow ow, HomeStockistModel hsm, OrderHistoryModel ohm)
        {
            orderWin = ow;
            homeStockModel = hsm;
            orderHistory = ohm;          
            InitializeComponent();
        }

        public AddPaymentWindow(OrderWindow ow, MegaStockistModel msm, OrderHistoryModel ohm)
        {
            orderWin = ow;
            megaStockModel = msm;
            orderHistory = ohm;
            InitializeComponent();
        }

        public AddPaymentWindow(OrderWindow ow, DepotStockistModel dsm, OrderHistoryModel ohm)
        {
            orderWin = ow;
            depotStockModel = dsm;
            orderHistory = ohm;
            InitializeComponent();
        }


        public AddPaymentWindow(OrderWindow ow, EmployeeModel em, OrderHistoryModel ohm)
        {
            orderWin = ow;
            employeesModel = em;
            orderHistory = ohm;
            InitializeComponent();
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (homeStockModel != null)
            {
                dgvPaymentHistory.ItemsSource = getHomeStockistPaymentHistory();
                lblTotalPaid.Content = dblTotalPaid.ToString("N0");

                if (orderHistory != null)
                {
                    if (orderHistory.ifPaid.Equals("YES"))
                    {
                        datePaid.IsEnabled = false;
                        txtAmount.IsEnabled = false;
                        txtNotes.IsEnabled = false;
                        btnAdd.IsEnabled = false;
                    }
                    btnDelete.Visibility = Visibility.Hidden;
                }
            }else if (megaStockModel != null)
            {
                dgvPaymentHistory.ItemsSource = getMegaStockistPaymentHistory();
                lblTotalPaid.Content = dblTotalPaid.ToString("N0");
 
                if (orderHistory != null)
                {
                    if (orderHistory.ifPaid.Equals("YES"))
                    {
                        datePaid.IsEnabled = false;
                        txtAmount.IsEnabled = false;
                        txtNotes.IsEnabled = false;
                        btnAdd.IsEnabled = false;
                    }
                    btnDelete.Visibility = Visibility.Hidden;
                }
            }else if (depotStockModel != null)
            {
                dgvPaymentHistory.ItemsSource = getDepotStockistPaymentHistory();
                lblTotalPaid.Content = dblTotalPaid.ToString("N0");

                if (orderHistory != null)
                {
                    if (orderHistory.ifPaid.Equals("YES"))
                    {
                        datePaid.IsEnabled = false;
                        txtAmount.IsEnabled = false;
                        txtNotes.IsEnabled = false;
                        btnAdd.IsEnabled = false;
                    }
                    btnDelete.Visibility = Visibility.Hidden;
                }
            }else if (employeesModel != null)
            {
                dgvPaymentHistory.ItemsSource = getEmployeePaymentHistory();
                lblTotalPaid.Content = dblTotalPaid.ToString("N0");
 
                if (orderHistory != null)
                {
                    if (orderHistory.ifPaid.Equals("YES"))
                    {
                        datePaid.IsEnabled = false;
                        txtAmount.IsEnabled = false;
                        txtNotes.IsEnabled = false;
                        btnAdd.IsEnabled = false;
                    }
                    btnDelete.Visibility = Visibility.Hidden;
                }
            }           
        }

        private async Task<bool> checkField()
        {
            bool ifAllCorrect = false;

            if (string.IsNullOrEmpty(datePaid.Text))
            {
                await this.ShowMessageAsync("Date", "Please select date");
            }
            else if (string.IsNullOrEmpty(txtAmount.Text))
            {
                await this.ShowMessageAsync("Amount", "Please input amount");
            }
            else
            {
                ifAllCorrect = true;
            }

            return ifAllCorrect;

        }

        private List<PaymentHistoryModel> getHomeStockistPaymentHistory()
        {
            conDB = new ConnectionDB();
            List<PaymentHistoryModel> lstPayments = new List<PaymentHistoryModel>();
            PaymentHistoryModel payment = new PaymentHistoryModel();

            string queryString = "SELECT ID, amountpaid, date, notes FROM dbpackage.tblpaymenthistory WHERE isDeleted = 0 AND clientID = ? AND orderID = ?";
            List<string> parameters = new List<string>();
            parameters.Add(homeStockModel.ID);
            parameters.Add(orderHistory.ID);

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

        private List<PaymentHistoryModel> getMegaStockistPaymentHistory()
        {
            conDB = new ConnectionDB();
            List<PaymentHistoryModel> lstPayments = new List<PaymentHistoryModel>();
            PaymentHistoryModel payment = new PaymentHistoryModel();

            string queryString = "SELECT ID, amountpaid, date, notes FROM dbpackage.tblpaymenthistory WHERE isDeleted = 0 AND clientID = ? AND orderID = ?";
            List<string> parameters = new List<string>();
            parameters.Add(megaStockModel.ID);
            parameters.Add(orderHistory.ID);

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

        private List<PaymentHistoryModel> getDepotStockistPaymentHistory()
        {
            conDB = new ConnectionDB();
            List<PaymentHistoryModel> lstPayments = new List<PaymentHistoryModel>();
            PaymentHistoryModel payment = new PaymentHistoryModel();

            string queryString = "SELECT ID, amountpaid, date, notes FROM dbpackage.tblpaymenthistory WHERE isDeleted = 0 AND clientID = ? AND orderID = ?";
            List<string> parameters = new List<string>();
            parameters.Add(depotStockModel.ID);
            parameters.Add(orderHistory.ID);

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

        private List<PaymentHistoryModel> getEmployeePaymentHistory()
        {
            conDB = new ConnectionDB();
            List<PaymentHistoryModel> lstPayments = new List<PaymentHistoryModel>();
            PaymentHistoryModel payment = new PaymentHistoryModel();

            string queryString = "SELECT ID, amountpaid, date, notes FROM dbpackage.tblpaymenthistory WHERE isDeleted = 0 AND clientID = ? AND orderID = ?";
            List<string> parameters = new List<string>();
            parameters.Add(employeesModel.ID);
            parameters.Add(orderHistory.ID);

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

        private void addHomeStockistPayment()
        {
            conDB = new ConnectionDB();
            string queryString = "INSERT INTO dbpackage.tblpaymenthistory (clientID, orderID, amountpaid, date, notes, isDeleted) VALUES (?,?,?,?,?,0)";
            List<string> parameters = new List<string>();
            parameters.Add(homeStockModel.ID);
            parameters.Add(orderHistory.ID);
            parameters.Add(txtAmount.Text);

            DateTime date = DateTime.Parse(datePaid.Text);
            parameters.Add(date.Year + "-" + date.Month + "-" + date.Day);

            if (string.IsNullOrEmpty(txtNotes.Text))
            {
                parameters.Add("");
            }
            else
            {
                parameters.Add(txtNotes.Text);
            }


            conDB.AddRecordToDatabase(queryString, parameters);

            conDB.closeConnection();
        }

        private void addMegaStockistPayment()
        {
            conDB = new ConnectionDB();
            string queryString = "INSERT INTO dbpackage.tblpaymenthistory (clientID, orderID, amountpaid, date, notes, isDeleted) VALUES (?,?,?,?,?,0)";
            List<string> parameters = new List<string>();
            parameters.Add(megaStockModel.ID);
            parameters.Add(orderHistory.ID);
            parameters.Add(txtAmount.Text);

            DateTime date = DateTime.Parse(datePaid.Text);
            parameters.Add(date.Year + "-" + date.Month + "-" + date.Day);

            if (string.IsNullOrEmpty(txtNotes.Text))
            {
                parameters.Add("");
            }
            else
            {
                parameters.Add(txtNotes.Text);
            }


            conDB.AddRecordToDatabase(queryString, parameters);

            conDB.closeConnection();
        }

        private void addDepotStockistPayment()
        {
            conDB = new ConnectionDB();
            string queryString = "INSERT INTO dbpackage.tblpaymenthistory (clientID, orderID, amountpaid, date, notes, isDeleted) VALUES (?,?,?,?,?,0)";
            List<string> parameters = new List<string>();
            parameters.Add(depotStockModel.ID);
            parameters.Add(orderHistory.ID);
            parameters.Add(txtAmount.Text);

            DateTime date = DateTime.Parse(datePaid.Text);
            parameters.Add(date.Year + "-" + date.Month + "-" + date.Day);

            if (string.IsNullOrEmpty(txtNotes.Text))
            {
                parameters.Add("");
            }
            else
            {
                parameters.Add(txtNotes.Text);
            }


            conDB.AddRecordToDatabase(queryString, parameters);

            conDB.closeConnection();
        }

        private void addEmployeePayment()
        {
            conDB = new ConnectionDB();
            string queryString = "INSERT INTO dbpackage.tblpaymenthistory (clientID, orderID, amountpaid, date, notes, isDeleted) VALUES (?,?,?,?,?,0)";
            List<string> parameters = new List<string>();
            parameters.Add(employeesModel.ID);
            parameters.Add(orderHistory.ID);
            parameters.Add(txtAmount.Text);

            DateTime date = DateTime.Parse(datePaid.Text);
            parameters.Add(date.Year + "-" + date.Month + "-" + date.Day);

            if (string.IsNullOrEmpty(txtNotes.Text))
            {
                parameters.Add("");
            }
            else
            {
                parameters.Add(txtNotes.Text);
            }


            conDB.AddRecordToDatabase(queryString, parameters);

            conDB.closeConnection();
        }

        private async void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            bool x = await checkField();
            
            if(homeStockModel != null)
            {
                if (x)
                {
                    addHomeStockistPayment();
                    await this.ShowMessageAsync("ADD PAYMENT", "Record successfully saved!");
                    double paid = getHomeStockistBalanceOnDR();
                    double oh = Convert.ToDouble(orderHistory.Total);
                    if (paid == oh)
                    {
                        orderWin.chkIfPaid.IsChecked = true;
                        updateRecordToPaid();
                    }
                    else
                    {
                        orderWin.lblBalance.Content = (oh - paid).ToString("N0");
                    }

                    this.Close();
                }
            }else if(megaStockModel != null){
                if (x)
                {
                    addMegaStockistPayment();
                    await this.ShowMessageAsync("ADD PAYMENT", "Record successfully saved!");
                    double paid = getMegaStockistBalanceOnDR();
                    double oh = Convert.ToDouble(orderHistory.Total);
                    if (paid == oh)
                    {
                        orderWin.chkIfPaid.IsChecked = true;
                        updateRecordToPaid();
                    }
                    else
                    {
                        orderWin.lblBalance.Content = (oh - paid).ToString("N0");
                    }

                    this.Close();
                }
            }else if(depotStockModel != null)
            {
                if (x)
                {
                    addDepotStockistPayment();
                    await this.ShowMessageAsync("ADD PAYMENT", "Record successfully saved!");
                    double paid = getDepotBalanceOnDR();
                    double oh = Convert.ToDouble(orderHistory.Total);
                    if (paid == oh)
                    {
                        orderWin.chkIfPaid.IsChecked = true;
                        updateRecordToPaid();
                    }
                    else
                    {
                        orderWin.lblBalance.Content = (oh - paid).ToString("N0");
                    }

                    this.Close();
                }
            }else if(employeesModel != null)
            {
                if (x)
                {
                    addEmployeePayment();
                    await this.ShowMessageAsync("ADD PAYMENT", "Record successfully saved!");
                    double paid = getEmployeeBalanceOnDR();
                    double oh = Convert.ToDouble(orderHistory.Total);
                    if (paid == oh)
                    {
                        orderWin.chkIfPaid.IsChecked = true;
                        updateRecordToPaid();
                    }
                    else
                    {
                        orderWin.lblBalance.Content = (oh - paid).ToString("N0");
                    }
                    this.Close();
                }
            } 
        }

        private void updateRecordToPaid()
        {
            conDB = new ConnectionDB();

            string queryString = "UPDATE dbpackage.tblorderhistory SET isPaid = 1 WHERE ID = ?";
            List<string> parameters = new List<string>();
            parameters.Add(orderHistory.ID);

            conDB.AddRecordToDatabase(queryString, parameters);
            conDB.closeConnection();
        }

        private double getHomeStockistBalanceOnDR()
        {
            double dblBalance = 0.0;

            conDB = new ConnectionDB();
            string queryString = "SELECT sum(amountpaid) as total FROM dbpackage.tblpaymenthistory WHERE " +
                "clientID = ? and orderID = ? AND isDeleted = 0";

            List<string> parameters = new List<string>();
            parameters.Add(homeStockModel.ID);
            parameters.Add(orderHistory.ID);

            MySqlDataReader reader = conDB.getSelectConnection(queryString, parameters);

            while (reader.Read())
            {
                dblBalance = Convert.ToDouble(reader["total"].ToString());
            }

            return dblBalance;
        }

        private double getMegaStockistBalanceOnDR()
        {
            double dblBalance = 0.0;

            conDB = new ConnectionDB();
            string queryString = "SELECT sum(amountpaid) as total FROM dbpackage.tblpaymenthistory WHERE " +
                "clientID = ? and orderID = ? AND isDeleted = 0";

            List<string> parameters = new List<string>();
            parameters.Add(megaStockModel.ID);
            parameters.Add(orderHistory.ID);

            MySqlDataReader reader = conDB.getSelectConnection(queryString, parameters);

            while (reader.Read())
            {
                dblBalance = Convert.ToDouble(reader["total"].ToString());
            }

            return dblBalance;
        }

        private double getDepotBalanceOnDR()
        {
            double dblBalance = 0.0;

            conDB = new ConnectionDB();
            string queryString = "SELECT sum(amountpaid) as total FROM dbpackage.tblpaymenthistory WHERE " +
                "clientID = ? and orderID = ? AND isDeleted = 0";

            List<string> parameters = new List<string>();
            parameters.Add(depotStockModel.ID);
            parameters.Add(orderHistory.ID);

            MySqlDataReader reader = conDB.getSelectConnection(queryString, parameters);

            while (reader.Read())
            {
                dblBalance = Convert.ToDouble(reader["total"].ToString());
            }

            return dblBalance;
        }

        private double getEmployeeBalanceOnDR()
        {
            double dblBalance = 0.0;

            conDB = new ConnectionDB();
            string queryString = "SELECT sum(amountpaid) as total FROM dbpackage.tblpaymenthistory WHERE " +
                "clientID = ? and orderID = ? AND isDeleted = 0";

            List<string> parameters = new List<string>();
            parameters.Add(employeesModel.ID);
            parameters.Add(orderHistory.ID);

            MySqlDataReader reader = conDB.getSelectConnection(queryString, parameters);

            while (reader.Read())
            {
                dblBalance = Convert.ToDouble(reader["total"].ToString());
            }

            return dblBalance;
        }

        private void deletePaymentRecord(string strRecordID)
        {
            conDB = new ConnectionDB();
            string queryString = "UPDATE dbpackage.tblpaymenthistory SET isDeleted = 1 WHERE ID = ?";
            List<string> parameters = new List<string>();
            parameters.Add(strRecordID);

            conDB.AddRecordToDatabase(queryString, parameters);

            conDB.closeConnection();

        }

        private async void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            PaymentHistoryModel payment = dgvPaymentHistory.SelectedItem as PaymentHistoryModel;

            if (payment != null && homeStockModel != null)
            {
                MessageDialogResult result = await this.ShowMessageAsync("Delete Record", "Are you sure you want to delete record?", MessageDialogStyle.AffirmativeAndNegative);
                if (result.Equals(MessageDialogResult.Affirmative))
                {
                    deletePaymentRecord(payment.ID);
                    dgvPaymentHistory.ItemsSource = getHomeStockistPaymentHistory();
                    dgvPaymentHistory.Items.Refresh();
                    lblTotalPaid.Content = (Convert.ToDouble(dblTotalPaid) - Convert.ToDouble(payment.AmountPaid)).ToString();
                    double dblLblBalance = Convert.ToDouble(orderWin.lblBalance.Content);
                    dblLblBalance = dblLblBalance + Convert.ToDouble(payment.AmountPaid);
                    orderWin.lblBalance.Content = dblLblBalance.ToString("N0");

                    await this.ShowMessageAsync("Delete Record", "Record deleted successfully!");
                }
            } else if (payment != null && megaStockModel != null)
            {
                MessageDialogResult result = await this.ShowMessageAsync("Delete Record", "Are you sure you want to delete record?", MessageDialogStyle.AffirmativeAndNegative);
                if (result.Equals(MessageDialogResult.Affirmative))
                {
                    deletePaymentRecord(payment.ID);
                    dgvPaymentHistory.ItemsSource = getMegaStockistPaymentHistory();
                    dgvPaymentHistory.Items.Refresh();
                    lblTotalPaid.Content = (Convert.ToDouble(dblTotalPaid) - Convert.ToDouble(payment.AmountPaid)).ToString();
                    double dblLblBalance = Convert.ToDouble(orderWin.lblBalance.Content);
                    dblLblBalance = dblLblBalance + Convert.ToDouble(payment.AmountPaid);
                    orderWin.lblBalance.Content = dblLblBalance.ToString("N0");

                    await this.ShowMessageAsync("Delete Record", "Record deleted successfully!");
                }
            }
        }

        private void CheckIsNumeric(TextCompositionEventArgs e)
        {
            int result;

            if (!(int.TryParse(e.Text, out result) || e.Text == "."))
            {
                e.Handled = true;
            }
        }

        private void txtAmount_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            CheckIsNumeric(e);
        }
    }
}
