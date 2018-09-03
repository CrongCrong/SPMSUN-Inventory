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
    /// Interaction logic for OrderWindow.xaml
    /// </summary>
    public partial class OrderWindow : MetroWindow
    {
        public OrderWindow()
        {
            InitializeComponent();
        }

        ConnectionDB conDB;
        HomeStockistModel homeStockistMod;
        MegaStockistModel megaStockistMod;
        DepotStockistModel depotStockistMod;
        EmployeeModel employeeStockistMod;
        MembersModel membersMod;

        OrderHistoryModel orderHistory;
        List<PackageModel> lstPackages = new List<PackageModel>();
        List<ProductModel> lstProducts = new List<ProductModel>();

        double dblTotalView = 0.0;
        double dblTotal = 0.0;

        public OrderWindow(HomeStockistModel hsm)
        {
            homeStockistMod = hsm;
            InitializeComponent();
        }

        public OrderWindow(MegaStockistModel msm)
        {
            megaStockistMod = msm;
            InitializeComponent();
        }

        public OrderWindow(DepotStockistModel dsm)
        {
            depotStockistMod = dsm;
            InitializeComponent();
        }

        public OrderWindow(EmployeeModel esm)
        {
            employeeStockistMod = esm;
            InitializeComponent();
        }

        public OrderWindow(MembersModel memM)
        {
            membersMod = memM;
            InitializeComponent();
        }

        public OrderWindow(HomeStockistModel hsm, OrderHistoryModel ohm)
        {

            orderHistory = ohm;
            homeStockistMod = hsm;
            InitializeComponent();
        }

        public OrderWindow(MegaStockistModel msm, OrderHistoryModel ohm)
        {

            orderHistory = ohm;
            megaStockistMod = msm;
            InitializeComponent();
        }

        public OrderWindow(DepotStockistModel dsm, OrderHistoryModel ohm)
        {
            orderHistory = ohm;
            depotStockistMod = dsm;
            InitializeComponent();
        }

        public OrderWindow(EmployeeModel dsm, OrderHistoryModel ohm)
        {

            orderHistory = ohm;
            employeeStockistMod = dsm;
            InitializeComponent();
        }

        public OrderWindow(MembersModel memM, OrderHistoryModel ohm)
        {

            orderHistory = ohm;
            membersMod = memM;
            InitializeComponent();
        }


        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            dblTotalView = 0.0;
            loadPackagesForCombo();
            loadProductsOnCombo();
            txtName.Text = homeStockistMod != null ? homeStockistMod.Fullname :
                (megaStockistMod != null ? megaStockistMod.Fullname :
                (employeeStockistMod != null ? employeeStockistMod.Fullname :
                (depotStockistMod != null ? depotStockistMod.Fullname :
                (membersMod != null ? membersMod.Fullname : ""))));

            txtName.IsEnabled = false;
            btnPayment.IsEnabled = false;
            btnUpdate.Visibility = Visibility.Hidden;
            //btnPaymentHistory.Visibility = Visibility.Hidden;

            if (orderHistory != null)
            {
                txtDR.Text = orderHistory.DRNumber;
                txtDR.IsEnabled = true;
                dateDR.IsEnabled = false;
                dateDR.Text = orderHistory.DRDate;

                btnSave.Visibility = Visibility.Hidden;
                btnUpdate.Visibility = Visibility.Visible;
                btnUpdate.IsEnabled = true;

                getDRDetails();
                if (orderHistory.ifPaid.Equals("YES"))
                {
                    chkIfPaid.IsChecked = true;
                    chkIfPaid.IsEnabled = false;
                }
                else
                {
                    chkIfPaid.IsEnabled = false;
                    lblBalance.Content = (dblTotalView - getBalanceOnDR()).ToString("N0");
                    btnPayment.IsEnabled = true;
                }
                btnSave.Visibility = Visibility.Hidden;

                cmbPackage.IsEnabled = false;
                cmbProducts.IsEnabled = false;
                txtQtyPackage.IsEnabled = false;
                txtQtyProduct.IsEnabled = false;
                btnQtyPackage.IsEnabled = false;
                btnQtyProduct.IsEnabled = false;

                btnQtyPackage.IsEnabled = false;
                btnQtyProduct.IsEnabled = false;
                btnRemovePackage.IsEnabled = false;
                btnRemoveProduct.IsEnabled = false;
                //btnPaymentHistory.Visibility = Visibility.Visible;
                //ENABLE PAYMENT BUTTON FOR ADMIN ACCT
                //if (user.TypeOfAccount.Equals(AccountType.ADMIN))
                //{
                //    btnPayment.IsEnabled = true;
                //}

            }
        }

        private void getDRDetails()
        {
            dgvPackages.ItemsSource = getPackagesOnDR(orderHistory.ID);
            dgvProducts.ItemsSource = getProductsOnDR(orderHistory.ID);

        }

        private double getBalanceOnDR()
        {
            double dblBalance = 0.0;

            conDB = new ConnectionDB();
            string queryString = "SELECT coalesce(sum(amountpaid),0) as total FROM dbpackage.tblpaymenthistory WHERE " +
                "clientID = ? and orderID = ? AND isDeleted = 0";

            List<string> parameters = new List<string>();
            parameters.Add(homeStockistMod != null ? homeStockistMod.ID :
                (megaStockistMod != null ? megaStockistMod.ID :
                (employeeStockistMod != null ? employeeStockistMod.ID :
                (depotStockistMod != null ? depotStockistMod.ID :
                (membersMod != null ? membersMod.ID : "0")))));

            parameters.Add(orderHistory.ID);

            MySqlDataReader reader = conDB.getSelectConnection(queryString, parameters);

            while (reader.Read())
            {
                dblBalance = Convert.ToDouble(reader["total"].ToString());
            }

            conDB.closeConnection();

            return dblBalance;
        }

        private List<ProductModel> getProductsOnDR(string strOrderID)
        {
            conDB = new ConnectionDB();
            List<ProductModel> lstProducts = new List<ProductModel>();
            ProductModel product = new ProductModel();

            string queryString = "SELECT dbpackage.tblorderdetails.ID, packageID, tblproducts.description, qty, price FROM " +
                "(dbpackage.tblorderdetails INNER JOIN dbpackage.tblproducts ON dbpackage.tblorderdetails.productID = " +
                "dbpackage.tblproducts.ID) WHERE tblorderdetails.isDeleted = 0 AND orderID = ?";

            List<string> parameters = new List<string>();
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

            string queryString = "SELECT packageID, dbpackage.tblpackage.name, qty, price FROM " +
                "(dbpackage.tblorderdetails INNER JOIN dbpackage.tblpackage ON dbpackage.tblorderdetails.packageID = dbpackage.tblpackage.ID) " +
                "WHERE productID = 0 AND dbpackage.tblorderdetails.isDeleted = 0 AND orderID = ?";

            List<string> parameters = new List<string>();
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

        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            bool x = await checkField();
            orderHistory = new OrderHistoryModel();
            int iID = 0;
            if (x)
            {
                if (chkCancelled.IsChecked == true)
                {
                    iID = saveTransaction();
                    btnPayment.IsEnabled = false;
                }
                else
                {
                    iID = saveTransaction();
                    if (lstPackages.Count != 0 && lstProducts.Count != 0)
                    {
                        saveProductsAndPackages(iID.ToString());
                    }
                    btnPayment.IsEnabled = true;
                }

                if (chkIfPaid.IsChecked == true)
                {
                    addPayment(iID.ToString());
                }

                await this.ShowMessageAsync("ADD RECORD", "Record successfully saved!");
                txtDR.IsEnabled = false;
                dateDR.IsEnabled = false;
                btnSave.IsEnabled = false;
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void loadProductsOnCombo()
        {
            conDB = new ConnectionDB();

            ProductModel prodMod = new ProductModel();
            string queryString = "SELECT ID, name, description, nonmemberprice, memberprice, homeprice, megaprice, depotprice, employeeprice FROM dbpackage.tblproducts WHERE isDeleted = 0";

            MySqlDataReader reader = conDB.getSelectConnection(queryString, null);

            while (reader.Read())
            {
                prodMod.ID = reader["ID"].ToString();
                prodMod.Name = reader["name"].ToString();
                prodMod.Description = reader["description"].ToString();
                prodMod.NonmemberPrice = reader["nonmemberprice"].ToString();
                prodMod.MemberPrice = reader["memberprice"].ToString();
                prodMod.HomePrice = reader["homeprice"].ToString();
                prodMod.MegaPrice = reader["megaprice"].ToString();
                prodMod.DepotPrice = reader["depotprice"].ToString();
                prodMod.EmployeePrice = reader["employeeprice"].ToString();

                cmbProducts.Items.Add(prodMod);
                prodMod = new ProductModel();
            }
            conDB.closeConnection();

        }

        private void loadPackagesForCombo()
        {
            conDB = new ConnectionDB();
            List<PackageModel> lstPackageModels = new List<PackageModel>();
            PackageModel packMod = new PackageModel();

            string queryString = "SELECT ID, name, nonmemberprice, memberprice, homeprice, megaprice, depotprice, employeeprice FROM dbpackage.tblpackage WHERE isDeleted = 0";

            MySqlDataReader reader = conDB.getSelectConnection(queryString, null);

            while (reader.Read())
            {
                packMod.ID = reader["ID"].ToString();
                packMod.Name = reader["name"].ToString();
                packMod.NonMemberPrice = reader["nonmemberprice"].ToString();
                packMod.MemberPrice = reader["memberprice"].ToString();
                packMod.HomePrice = reader["homeprice"].ToString();
                packMod.MegaPrice = reader["megaprice"].ToString();
                packMod.DepotPrice = reader["depotprice"].ToString();
                packMod.EmployeePrice = reader["employeeprice"].ToString();

                cmbPackage.Items.Add(packMod);
                packMod = new PackageModel();

            }

            conDB.closeConnection();
        }

        private int saveTransaction()
        {
            conDB = new ConnectionDB();

            string queryString = "INSERT INTO dbpackage.tblorderhistory (clientID, date, drNo, total, isPaid, isCancelled, isDeleted) VALUES " +
                "(?,?,?,?,?,?,0)";

            List<string> parameters = new List<string>();
            parameters.Add(homeStockistMod != null ? homeStockistMod.ID :
                (megaStockistMod != null ? megaStockistMod.ID :
                (employeeStockistMod != null ? employeeStockistMod.ID :
                (depotStockistMod != null ? depotStockistMod.ID :
                (membersMod != null ? membersMod.ID : "0")))));

            DateTime date = DateTime.Parse(dateDR.Text);
            parameters.Add(date.Year + "-" + date.Month + "-" + date.Day);

            parameters.Add(txtDR.Text);
            parameters.Add(dblTotal.ToString());

            if (chkIfPaid.IsChecked.Value)
            {
                parameters.Add("1");
                orderHistory.ifPaid = "YES";
            }
            else
            {
                parameters.Add("0");
                orderHistory.ifPaid = "NO";
            }

            if (chkCancelled.IsChecked.Value)
            {
                parameters.Add("1");
            }
            else
            {
                parameters.Add("0");
            }

            orderHistory.ClientID = homeStockistMod != null ? homeStockistMod.ID : 
                (megaStockistMod != null ? megaStockistMod.ID : 
                (employeeStockistMod != null ? employeeStockistMod.ID : 
                (depotStockistMod != null ? depotStockistMod.ID :
                (membersMod != null ? membersMod.ID : "0"))));

            orderHistory.DRDate = dateDR.Text;
            orderHistory.DRNumber = txtDR.Text;
            orderHistory.Total = dblTotal.ToString();

            conDB.AddRecordToDatabase(queryString, parameters);
            MySqlDataReader reader = conDB.getSelectConnection("SELECT ID from dbpackage.tblorderhistory order by ID desc limit 1", null);

            int x = 0;
            while (reader.Read())
            {
                x = Convert.ToInt32(reader["ID"].ToString());
            }
            orderHistory.ID = x.ToString();
            conDB.closeConnection();

            return x;

        }

        private void updateTransaction(string strRecordID)
        {
            conDB = new ConnectionDB();

            string queryString = "UPDATE dbpackage.tblorderhistory SET drNo = ?, isCancelled = ? WHERE ID = ?";
            List<string> parameters = new List<string>();
            parameters.Add(txtDR.Text);

            if (chkCancelled.IsChecked == true)
            {
                parameters.Add("1");
            }
            else
            {
                parameters.Add("0");
            }

            parameters.Add(strRecordID);

            conDB.AddRecordToDatabase(queryString, parameters);
            conDB.closeConnection();

        }

        private void saveProductsAndPackages(string strRecordID)
        {
            conDB = new ConnectionDB();
            string queryString = "INSERT INTO dbpackage.tblorderdetails (orderID, packageID, productID, qty, price, isDeleted) VALUES (?,?,?,?,?,0)";


            foreach (PackageModel pMod in lstPackages)
            {
                List<string> parameters = new List<string>();
                parameters.Add(strRecordID);
                parameters.Add(pMod.ID);
                parameters.Add("0");
                parameters.Add(pMod.Qty);
                parameters.Add(pMod.Total);
                conDB.AddRecordToDatabase(queryString, parameters);
            }

            queryString = "INSERT INTO dbpackage.tblorderdetails (orderID, packageID, productID, qty, price, isDeleted) VALUES (?,?,?,?,?,0)";

            foreach (ProductModel prodMod in lstProducts)
            {
                List<string> parameters = new List<string>();
                parameters.Add(strRecordID);
                parameters.Add("0");
                parameters.Add(prodMod.ID);
                parameters.Add(prodMod.Qty);
                parameters.Add(prodMod.Total);
                conDB.AddRecordToDatabase(queryString, parameters);
            }

            conDB.closeConnection();

        }

        private void addPayment(string strRecordID)
        {
            conDB = new ConnectionDB();
            string queryString = "INSERT INTO dbpackage.tblpaymenthistory (clientID, orderID, amountpaid, date, isDeleted) VALUES (?,?,?,?,0)";
            List<string> parameters = new List<string>();
            parameters.Add(homeStockistMod != null ? homeStockistMod.ID :
                (megaStockistMod != null ? megaStockistMod.ID :
                (employeeStockistMod != null ? employeeStockistMod.ID :
                (depotStockistMod != null ? depotStockistMod.ID :
                (membersMod != null ? membersMod.ID : "0")))));

            parameters.Add(strRecordID);
            parameters.Add(dblTotal.ToString());

            DateTime date = DateTime.Parse(dateDR.Text);
            parameters.Add(date.Year + "-" + date.Month + "-" + date.Day);

            conDB.AddRecordToDatabase(queryString, parameters);

            conDB.closeConnection();
        }

        private void btnQtyPackage_Click(object sender, RoutedEventArgs e)
        {
            PackageModel buyPackage = new PackageModel();
            PackageModel selectedPackage = cmbPackage.SelectedItem as PackageModel;

            if (selectedPackage != null)
            {
                if (megaStockistMod != null)
                {
                    buyPackage.ID = selectedPackage.ID;
                    buyPackage.Name = selectedPackage.Name;
                    buyPackage.Total = (Convert.ToDouble(selectedPackage.MegaPrice) * Convert.ToDouble(txtQtyPackage.Text)).ToString();
                    buyPackage.Qty = txtQtyPackage.Text;
                    double tempTotal = Convert.ToDouble(lblTotal.Content);
                    dblTotal = tempTotal + Convert.ToDouble(buyPackage.Total);

                    lblTotal.Content = dblTotal.ToString("N0");
                    lblBalance.Content = dblTotal.ToString("N0");
                }


                else if (homeStockistMod != null)
                {
                    buyPackage.ID = selectedPackage.ID;
                    buyPackage.Name = selectedPackage.Name;
                    buyPackage.Total = (Convert.ToDouble(selectedPackage.HomePrice) * Convert.ToDouble(txtQtyPackage.Text)).ToString();
                    buyPackage.Qty = txtQtyPackage.Text;
                    double tempTotal = Convert.ToDouble(lblTotal.Content);
                    dblTotal = tempTotal + Convert.ToDouble(buyPackage.Total);

                    lblTotal.Content = dblTotal.ToString("N0");
                    lblBalance.Content = dblTotal.ToString("N0");
                }

                else if (depotStockistMod != null)
                {
                    buyPackage.ID = selectedPackage.ID;
                    buyPackage.Name = selectedPackage.Name;
                    buyPackage.Total = (Convert.ToDouble(selectedPackage.DepotPrice) * Convert.ToDouble(txtQtyPackage.Text)).ToString();
                    buyPackage.Qty = txtQtyPackage.Text;
                    double tempTotal = Convert.ToDouble(lblTotal.Content);
                    dblTotal = tempTotal + Convert.ToDouble(buyPackage.Total);

                    lblTotal.Content = dblTotal.ToString("N0");
                    lblBalance.Content = dblTotal.ToString("N0");
                }

                else if (employeeStockistMod != null)
                {
                    buyPackage.ID = selectedPackage.ID;
                    buyPackage.Name = selectedPackage.Name;
                    buyPackage.Total = (Convert.ToDouble(selectedPackage.EmployeePrice) * Convert.ToDouble(txtQtyPackage.Text)).ToString();
                    buyPackage.Qty = txtQtyPackage.Text;
                    double tempTotal = Convert.ToDouble(lblTotal.Content);
                    dblTotal = tempTotal + Convert.ToDouble(buyPackage.Total);

                    lblTotal.Content = dblTotal.ToString("N0");
                    lblBalance.Content = dblTotal.ToString("N0");
                }
                else if (membersMod != null)
                {
                    buyPackage.ID = selectedPackage.ID;
                    buyPackage.Name = selectedPackage.Name;
                    buyPackage.Total = (Convert.ToDouble(selectedPackage.MemberPrice) * Convert.ToDouble(txtQtyPackage.Text)).ToString();
                    buyPackage.Qty = txtQtyPackage.Text;
                    double tempTotal = Convert.ToDouble(lblTotal.Content);
                    dblTotal = tempTotal + Convert.ToDouble(buyPackage.Total);

                    lblTotal.Content = dblTotal.ToString("N0");
                    lblBalance.Content = dblTotal.ToString("N0");
                }

                lstPackages.Add(buyPackage);
                dgvPackages.ItemsSource = lstPackages;
                dgvPackages.Items.Refresh();
            }
        }

        private void btnQtyProduct_Click(object sender, RoutedEventArgs e)
        {
            ProductModel buyProduct = new ProductModel();
            ProductModel selectedProduct = cmbProducts.SelectedItem as ProductModel;

            if (selectedProduct != null)
            {

                if (megaStockistMod != null)
                {
                    buyProduct.ID = selectedProduct.ID;
                    buyProduct.Description = selectedProduct.Description;
                    buyProduct.Total = (Convert.ToDouble(selectedProduct.MegaPrice) * Convert.ToDouble(txtQtyProduct.Text)).ToString();
                    buyProduct.Qty = txtQtyProduct.Text;

                    double tempTotal = Convert.ToDouble(lblTotal.Content);
                    dblTotal = tempTotal + Convert.ToDouble(buyProduct.Total);

                    lblTotal.Content = dblTotal.ToString("N0");
                    lblBalance.Content = dblTotal.ToString("N0");
                }
                else if (homeStockistMod != null)
                {
                    buyProduct.ID = selectedProduct.ID;
                    buyProduct.Description = selectedProduct.Description;
                    buyProduct.Total = (Convert.ToDouble(selectedProduct.HomePrice) * Convert.ToDouble(txtQtyProduct.Text)).ToString();
                    buyProduct.Qty = txtQtyProduct.Text;

                    double tempTotal = Convert.ToDouble(lblTotal.Content);
                    dblTotal = tempTotal + Convert.ToDouble(buyProduct.Total);

                    lblTotal.Content = dblTotal.ToString("N0");
                    lblBalance.Content = dblTotal.ToString("N0");
                }
                else if (depotStockistMod != null)
                {
                    buyProduct.ID = selectedProduct.ID;
                    buyProduct.Description = selectedProduct.Description;
                    buyProduct.Total = (Convert.ToDouble(selectedProduct.DepotPrice) * Convert.ToDouble(txtQtyProduct.Text)).ToString();
                    buyProduct.Qty = txtQtyProduct.Text;

                    double tempTotal = Convert.ToDouble(lblTotal.Content);
                    dblTotal = tempTotal + Convert.ToDouble(buyProduct.Total);

                    lblTotal.Content = dblTotal.ToString("N0");
                    lblBalance.Content = dblTotal.ToString("N0");
                }

                else if (employeeStockistMod != null)
                {
                    buyProduct.ID = selectedProduct.ID;
                    buyProduct.Description = selectedProduct.Description;
                    buyProduct.Total = (Convert.ToDouble(selectedProduct.EmployeePrice) * Convert.ToDouble(txtQtyProduct.Text)).ToString();
                    buyProduct.Qty = txtQtyProduct.Text;

                    double tempTotal = Convert.ToDouble(lblTotal.Content);
                    dblTotal = tempTotal + Convert.ToDouble(buyProduct.Total);

                    lblTotal.Content = dblTotal.ToString("N0");
                    lblBalance.Content = dblTotal.ToString("N0");

                }
                else if (membersMod != null)
                {
                    buyProduct.ID = selectedProduct.ID;
                    buyProduct.Description = selectedProduct.Description;
                    buyProduct.Total = (Convert.ToDouble(selectedProduct.MemberPrice) * Convert.ToDouble(txtQtyProduct.Text)).ToString();
                    buyProduct.Qty = txtQtyProduct.Text;

                    double tempTotal = Convert.ToDouble(lblTotal.Content);
                    dblTotal = tempTotal + Convert.ToDouble(buyProduct.Total);

                    lblTotal.Content = dblTotal.ToString("N0");
                    lblBalance.Content = dblTotal.ToString("N0");

                }
                lstProducts.Add(buyProduct);
                dgvProducts.ItemsSource = lstProducts;
                dgvProducts.Items.Refresh();
            }
        }

        private async Task<bool> checkField()
        {
            bool ifAllCorrect = false;

            if (string.IsNullOrEmpty(txtDR.Text))
            {
                await this.ShowMessageAsync("D.R No.", "Please provide D.R No.");
            }
            else if (string.IsNullOrEmpty(dateDR.Text))
            {
                await this.ShowMessageAsync("DATE", "Please select Date");
            }
            else
            {
                ifAllCorrect = true;
            }

            return ifAllCorrect;
        }

        private void btnPayment_Click(object sender, RoutedEventArgs e)
        {
            if (homeStockistMod != null)
            {
                AddPaymentWindow addPay = new AddPaymentWindow(this, homeStockistMod, orderHistory);
                addPay.ShowDialog();

            }
            else if (megaStockistMod != null)
            {
                AddPaymentWindow addPay = new AddPaymentWindow(this, megaStockistMod, orderHistory);
                addPay.ShowDialog();
            }
            else if (depotStockistMod != null)
            {
                AddPaymentWindow addPay = new AddPaymentWindow(this, depotStockistMod, orderHistory);
                addPay.ShowDialog();
            }
            else if (employeeStockistMod != null)
            {
                AddPaymentWindow addPay = new AddPaymentWindow(this, employeeStockistMod, orderHistory);
                addPay.ShowDialog();

            }
            else if(membersMod != null)
            {
                AddPaymentWindow addPay = new AddPaymentWindow(this, membersMod, orderHistory);
                addPay.ShowDialog();
            }
        }

        private void chkIfPaid_Checked(object sender, RoutedEventArgs e)
        {
            lblBalance.Content = "0.0";
            btnPayment.IsEnabled = false;
        }

        private void chkIfPaid_Unchecked(object sender, RoutedEventArgs e)
        {
            lblBalance.Content = lblTotal.Content;
            btnPayment.IsEnabled = true;
        }

        private void viewDetails()
        {
            //PackageModel pa = dgvPackages.SelectedItem as PackageModel;
            //DetailsWindow details = new DetailsWindow(pa, null);
            //details.ShowDialog();
        }

        private void menuClickItem_Click(object sender, RoutedEventArgs e)
        {
            //PackageModel pa = dgvPackages.SelectedItem as PackageModel;
            //DetailsWindow details = new DetailsWindow(pa, null);
            //details.ShowDialog();
        }

        private void CheckIsNumeric(TextCompositionEventArgs e)
        {
            int result;

            if (!(int.TryParse(e.Text, out result) || e.Text == "."))
            {
                e.Handled = true;
            }
        }

        private void txtDR_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            CheckIsNumeric(e);
        }

        private void txtQtyPackage_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            CheckIsNumeric(e);
        }

        private void txtQtyProduct_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            CheckIsNumeric(e);
        }

        private async void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            updateTransaction(orderHistory.ID);
            await this.ShowMessageAsync("UPDATE RECORD", "Record updated successfully!");
        }

        private void btnRemovePackage_Click(object sender, RoutedEventArgs e)
        {
            double tempTotal = Convert.ToDouble(lblTotal.Content);
            PackageModel selectedPackage = dgvPackages.SelectedItem as PackageModel;

            if (selectedPackage != null)
            {
                double dblSelectedValue = Convert.ToDouble(selectedPackage.Total);
                tempTotal = tempTotal - dblSelectedValue;


                lblTotal.Content = tempTotal.ToString("N0");
                lblBalance.Content = tempTotal.ToString("N0");

                lstPackages.Remove(selectedPackage);
                dgvPackages.ItemsSource = lstPackages;
                dgvPackages.Items.Refresh();
            }
        }

        private void btnRemoveProduct_Click(object sender, RoutedEventArgs e)
        {
            double tempTotal = Convert.ToDouble(lblTotal.Content);
            ProductModel selectedProduct = dgvProducts.SelectedItem as ProductModel;

            if (selectedProduct != null)
            {
                double dblSelectedValue = Convert.ToDouble(selectedProduct.Total);
                tempTotal = tempTotal - dblSelectedValue;

                lblTotal.Content = tempTotal.ToString("N0");
                lblBalance.Content = tempTotal.ToString("N0");

                lstProducts.Remove(selectedProduct);
                dgvProducts.ItemsSource = lstProducts;
                dgvProducts.Items.Refresh();
            }
        }

        private void chkCancelled_Checked(object sender, RoutedEventArgs e)
        {
            chkIfPaid.IsEnabled = false;
            btnPayment.IsEnabled = false;

        }
    }
}
