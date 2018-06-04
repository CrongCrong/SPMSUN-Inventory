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
    /// Interaction logic for PackageDetails.xaml
    /// </summary>
    public partial class PackageDetails : MetroWindow
    {
        public PackageDetails()
        {
            InitializeComponent();
        }

        ConnectionDB conDB;
        int packageID = 0;
        PackagesWindow packageWindow;
        PackageModel packageMod;
        public bool ifView = false;

        public PackageDetails(PackagesWindow pw, PackageModel pm)
        {
            packageWindow = pw;
            packageMod = pm;
            InitializeComponent();
        }

        public PackageDetails(PackagesWindow pw)
        {
            packageWindow = pw;
            InitializeComponent();
        }


        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {

            btnUpdate.Visibility = Visibility.Hidden;
            loadProductsOnCombo();

            if (packageMod != null)
            {
                btnUpdate.Visibility = Visibility.Visible;
                btnSave.Visibility = Visibility.Hidden;

                txtPackageName.Text = packageMod.Name;
                txtNonMemberPrice.Text = packageMod.NonMemberPrice;
                txtMemberPrice.Text = packageMod.MemberPrice;
                txtHomePrice.Text = packageMod.HomePrice;
                txtMegaPrice.Text = packageMod.MegaPrice;
                txtDepotPrice.Text = packageMod.DepotPrice;
                txtEmployeePrice.Text = packageMod.EmployeePrice;
                txtMFG.Text = packageMod.MFG;
                txtUnilevel.Text = packageMod.Unilevel;
                txtLB.Text = packageMod.LB;
                txtPackageName.IsEnabled = false;
                dgvProductLink.ItemsSource = getProductsForPackage(packageMod.ID);

            }

            if (ifView)
            {
                txtPackageName.IsEnabled = false;
                txtNonMemberPrice.IsEnabled = false;
                txtMemberPrice.IsEnabled = false;
                txtHomePrice.IsEnabled = false;
                txtMegaPrice.IsEnabled = false;
                txtDepotPrice.IsEnabled = false;
                cmbProducts.IsEnabled = false;
                txtQty.IsEnabled = false;
                btnAdd.IsEnabled = false;
                btnUpdate.IsEnabled = false;
                btnRemove.IsEnabled = false;
                cmbProducts.IsEnabled = false;
                txtQty.IsEnabled = false;
                btnAdd.IsEnabled = false;
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private async void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            updateProductPackage(packageMod.ID);
            await this.ShowMessageAsync("UPDATE RECORD", "Record updated successfully!");
            packageWindow.dgvPackageList.ItemsSource = loadDataGridDetails();
        }

        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            bool x = await checkField();
            if (x)
            {
                packageID = AddPackageName();
                await this.ShowMessageAsync("Add Package", "Package added successfully!");
                cmbProducts.IsEnabled = true;
                txtQty.IsEnabled = true;
                btnAdd.IsEnabled = true;
            }

        }

        private List<PackageModel> loadDataGridDetails()
        {
            conDB = new ConnectionDB();
            List<PackageModel> lstLPackageModel = new List<PackageModel>();
            PackageModel pm = new PackageModel();

            string queryString = "SELECT ID, name, nonmemberprice, memberprice, homeprice, megaprice, depotprice, employeeprice," +
                " MFG, unilevel, lb FROM dbpackage.tblpackage WHERE isDeleted = 0";

            MySqlDataReader reader = conDB.getSelectConnection(queryString, null);

            while (reader.Read())
            {
                pm.ID = reader["ID"].ToString();
                pm.Name = reader["name"].ToString();
                pm.NonMemberPrice = reader["nonmemberprice"].ToString();
                pm.MemberPrice = reader["memberprice"].ToString();
                pm.HomePrice = reader["homeprice"].ToString();
                pm.MegaPrice = reader["megaprice"].ToString();
                pm.DepotPrice = reader["depotprice"].ToString();
                pm.EmployeePrice = reader["employeeprice"].ToString();
                pm.MFG = reader["MFG"].ToString();
                pm.Unilevel = reader["unilevel"].ToString();
                pm.LB = reader["lb"].ToString();
                lstLPackageModel.Add(pm);
                pm = new PackageModel();
            }
            conDB.closeConnection();

            return lstLPackageModel;
        }

        private void loadProductsOnCombo()
        {
            conDB = new ConnectionDB();

            ProductModel prodMod = new ProductModel();
            string queryString = "SELECT ID, name, description FROM dbpackage.tblproducts WHERE isDeleted  = 0";

            MySqlDataReader reader = conDB.getSelectConnection(queryString, null);

            while (reader.Read())
            {
                prodMod.ID = reader["ID"].ToString();
                prodMod.Name = reader["name"].ToString();
                prodMod.Description = reader["description"].ToString();

                cmbProducts.Items.Add(prodMod);

                prodMod = new ProductModel();
            }

            conDB.closeConnection();
        }

        private int AddPackageName()
        {
            conDB = new ConnectionDB();

            string queryString = "INSERT INTO dbpackage.tblpackage (name, nonmemberprice, memberprice, homeprice, megaprice, depotprice, employeeprice," +
                " MFG, unilevel, lb, isDeleted)  " +
                "VALUES (?,?,?,?,?,?,?,?,?,0)";
            List<string> parameters = new List<string>();
            parameters.Add(txtPackageName.Text);
            parameters.Add(txtNonMemberPrice.Text);
            parameters.Add(txtMemberPrice.Text);
            parameters.Add(txtHomePrice.Text);
            parameters.Add(txtMegaPrice.Text);
            parameters.Add(txtDepotPrice.Text);
            parameters.Add(txtEmployeePrice.Text);
            parameters.Add(txtMFG.Text);
            parameters.Add(txtUnilevel.Text);
            parameters.Add(txtLB.Text);
            conDB.AddRecordToDatabase(queryString, parameters);

            MySqlDataReader reader = conDB.getSelectConnection("select ID from dbpackage.tblpackage order by ID desc limit 1", null);
            int x = 0;
            while (reader.Read())
            {
                x = Convert.ToInt32(reader["ID"].ToString());
            }


            conDB.closeConnection();

            return x;

        }

        private void AddProductToPackage(int packageID)
        {
            conDB = new ConnectionDB();
            string queryString = "INSERT INTO dbpackage.tblpackageproduct (productID, packageID, qty, isDeleted) VALUES (?,?,?,0)";
            List<string> parameters = new List<string>();
            parameters.Add(cmbProducts.SelectedValue.ToString());
            parameters.Add(packageID.ToString());
            parameters.Add(txtQty.Text);

            conDB.AddRecordToDatabase(queryString, parameters);
            conDB.closeConnection();
        }

        private void updateProductPackage(string strRecordID)
        {
            conDB = new ConnectionDB();
            string queryString = "UPDATE dbpackage.tblpackage SET nonmemberprice = ?, memberprice = ?, homeprice = ?, " +
                "megaprice = ?, depotprice = ?, employeeprice = ?, MFG = ?, unilevel = ?, lb = ? WHERE ID = ?";

            List<string> parameters = new List<string>();
            parameters.Add(txtNonMemberPrice.Text);
            parameters.Add(txtMemberPrice.Text);
            parameters.Add(txtHomePrice.Text);
            parameters.Add(txtMegaPrice.Text);
            parameters.Add(txtDepotPrice.Text);
            parameters.Add(txtEmployeePrice.Text);
            parameters.Add(txtMFG.Text);
            parameters.Add(txtUnilevel.Text);
            parameters.Add(txtLB.Text);
            parameters.Add(strRecordID);

            conDB.AddRecordToDatabase(queryString, parameters);
            conDB.closeConnection();
        }

        private List<ProductModel> getProductsForPackage(string strPackageID)
        {
            conDB = new ConnectionDB();
            List<ProductModel> lstProdModel = new List<ProductModel>();
            ProductModel pMod = new ProductModel();
            string queryString = "SELECT productID, dbpackage.tblproducts.name, qty FROM (dbpackage.tblpackageproduct INNER JOIN " +
                "dbpackage.tblproducts ON dbpackage.tblpackageproduct.productID = dbpackage.tblproducts.ID) WHERE " +
                "dbpackage.tblpackageproduct.isDeleted = 0 AND dbpackage.tblpackageproduct.packageID = ? ";
            List<string> parameters = new List<string>();
            parameters.Add(strPackageID);

            MySqlDataReader reader = conDB.getSelectConnection(queryString, parameters);

            while (reader.Read())
            {
                pMod.ID = reader["productID"].ToString();
                pMod.Name = reader["name"].ToString();
                pMod.Qty = reader["qty"].ToString();
                lstProdModel.Add(pMod);
                pMod = new ProductModel();
            }

            conDB.closeConnection();

            return lstProdModel;
        }

        private void removeProductFromPackage(string strPackageID, string strProductID)
        {
            conDB = new ConnectionDB();

            string queryString = "UPDATE dbpackage.tblpackageproduct SET isDeleted = 1 WHERE productID =? AND packageID = ?";
            List<string> parameters = new List<string>();
            parameters.Add(strProductID);
            parameters.Add(strPackageID);

            conDB.AddRecordToDatabase(queryString, parameters);
            conDB.closeConnection();
        }

        private async Task<bool> checkField()
        {
            bool ifAllCorrect = false;

            if (string.IsNullOrEmpty(txtPackageName.Text))
            {
                await this.ShowMessageAsync("Package Name", "Please provide package name");
            }
            else if (string.IsNullOrEmpty(txtNonMemberPrice.Text))
            {
                await this.ShowMessageAsync("Non-Member Price", "Please provide Non-member price");
            }
            else if (string.IsNullOrEmpty(txtMemberPrice.Text))
            {
                await this.ShowMessageAsync("Member Price", "Please provide Member price");
            }
            else if (string.IsNullOrEmpty(txtHomePrice.Text))
            {
                await this.ShowMessageAsync("Home Stockist Price", "Please provide Home stockist price");
            }
            else if (string.IsNullOrEmpty(txtMegaPrice.Text))
            {
                await this.ShowMessageAsync("Mega Stockist Price", "Please provide Mega stockist price");
            }
            else if (string.IsNullOrEmpty(txtDepotPrice.Text))
            {
                await this.ShowMessageAsync("Depot Price", "Please provide Depot price");
            }
            else if (string.IsNullOrEmpty(txtEmployeePrice.Text))
            {
                await this.ShowMessageAsync("Employee Price", "Please provide Employee price");
            }
            else if (string.IsNullOrEmpty(txtMFG.Text))
            {
                await this.ShowMessageAsync("MFG", "Please provide MFG value");
            }
            else if (string.IsNullOrEmpty(txtUnilevel.Text))
            {
                await this.ShowMessageAsync("Unilevel", "Please provide unilevel value");
            }
            else if (string.IsNullOrEmpty(txtUnilevel.Text))
            {
                await this.ShowMessageAsync("LB", "Please provide LB value");
            }
            else
            {
                ifAllCorrect = true;
            }

            return ifAllCorrect;
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (packageMod == null)
            {
                AddProductToPackage(packageID);
                dgvProductLink.ItemsSource = getProductsForPackage(packageID.ToString());
                packageMod = new PackageModel();
                packageMod.ID = packageID.ToString();
            }
            else
            {
                AddProductToPackage(Convert.ToInt32(packageMod.ID));
                dgvProductLink.ItemsSource = getProductsForPackage(packageMod.ID);
            }


            packageWindow.dgvPackageList.ItemsSource = loadDataGridDetails();
        }

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            ProductModel p = dgvProductLink.SelectedItem as ProductModel;
            if (p != null)
            {
                removeProductFromPackage(packageMod.ID, p.ID);
                dgvProductLink.ItemsSource = getProductsForPackage(packageMod.ID);
            }
        }

        private void txtNonMemberPrice_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            CheckIsNumeric(e);
        }

        private void txtMemberPrice_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            CheckIsNumeric(e);
        }

        private void txtHomePrice_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            CheckIsNumeric(e);
        }

        private void txtMegaPrice_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            CheckIsNumeric(e);
        }

        private void CheckIsNumeric(TextCompositionEventArgs e)
        {
            int result;

            if (!(int.TryParse(e.Text, out result) || e.Text == "."))
            {
                e.Handled = true;
            }
        }

        private void txtDepotPrice_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            CheckIsNumeric(e);
        }

        private void txtEmployeePrice_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            CheckIsNumeric(e);
        }

        private void txtUnilevel_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            CheckIsNumeric(e);
        }

        private void txtPV_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            CheckIsNumeric(e);
        }

        private void txtLB_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            CheckIsNumeric(e);
        }

    }
}
