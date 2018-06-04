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
    /// Interaction logic for ProductDetails.xaml
    /// </summary>
    public partial class ProductDetails : MetroWindow
    {
        public ProductDetails()
        {
            InitializeComponent();
        }
        ConnectionDB conDB;
        ProductsWindow productWindow;
        ProductModel productMod;

        public ProductDetails(ProductsWindow pw)
        {
            productWindow = pw;
            InitializeComponent();

        }

        public ProductDetails(ProductsWindow pw, ProductModel pm)
        {
            productWindow = pw;
            productMod = pm;
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            btnUpdate.Visibility = Visibility.Hidden;
            if (productMod != null)
            {
                txtProductName.Text = productMod.Name;
                txtDescription.Text = productMod.Description;
                txtNonMemberPrice.Text = productMod.NonmemberPrice;
                txtMemberPrice.Text = productMod.MemberPrice;
                txtHomePrice.Text = productMod.HomePrice;
                txtMegaPrice.Text = productMod.MegaPrice;
                txtDepotPrice.Text = productMod.DepotPrice;
                txtEmployeePrice.Text = productMod.EmployeePrice;
                txtMFG.Text = productMod.MFG;
                txtUnilevel.Text = productMod.Unilevel;
                txtLB.Text = productMod.LB;
                btnSave.Visibility = Visibility.Hidden;
                btnUpdate.Visibility = Visibility.Visible;
            }
        }

        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            bool x = await checkFields();

            if (x)
            {
                addProduct();
                productWindow.dgvProducts.ItemsSource = loadDataGridDetails();
                this.Close();
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private async Task<bool> checkFields()
        {
            bool ifAllCorrect = false;

            if (string.IsNullOrEmpty(txtProductName.Text))
            {
                await this.ShowMessageAsync("Product Name", "Please provide product name");
            }
            else if (string.IsNullOrEmpty(txtDescription.Text))
            {
                await this.ShowMessageAsync("Description", "Please provide product description");
            }
            else if (string.IsNullOrEmpty(txtNonMemberPrice.Text))
            {
                await this.ShowMessageAsync("Non-Member Price", "Please provide Non-Member price");
            }
            else if (string.IsNullOrEmpty(txtMemberPrice.Text))
            {
                await this.ShowMessageAsync("Member Price", "Please provide Member price");
            }
            else if (string.IsNullOrEmpty(txtHomePrice.Text))
            {
                await this.ShowMessageAsync("Home Price", "Please provide Home stockist price");
            }
            else if (string.IsNullOrEmpty(txtMegaPrice.Text))
            {
                await this.ShowMessageAsync("Mega Price", "Please provide Mega stockist price");
            }
            else if (string.IsNullOrEmpty(txtDepotPrice.Text))
            {
                await this.ShowMessageAsync("Depot Price", "Please provide Depot price");
            }else if (string.IsNullOrEmpty(txtEmployeePrice.Text))
            {
                await this.ShowMessageAsync("Employee Price", "Please provide Employee price");
            }else if (string.IsNullOrEmpty(txtMFG.Text))
            {
                await this.ShowMessageAsync("MFG", "Please provide MFG value");
            }
            else if (string.IsNullOrEmpty(txtUnilevel.Text))
            {
                await this.ShowMessageAsync("Unilevel", "Please provide unilevel value");
            }
            else if (string.IsNullOrEmpty(txtLB.Text))
            {
                await this.ShowMessageAsync("LB", "Please provide LB value");
            }
            else
            {
                ifAllCorrect = true;
            }

            return ifAllCorrect;
        }

        private async void addProduct()
        {
            conDB = new ConnectionDB();

            string queryString = "INSERT INTO dbpackage.tblproducts (name, description, nonmemberprice, memberprice, homeprice, megaprice, " +
                "depotprice, employeeprice, MFG, unilevel, lb, isDeleted) " +
                "VALUES (?,?,?,?,?,?,?,?,?,?,?,0)";

            List<string> parameters = new List<string>();
            parameters.Add(txtProductName.Text);
            parameters.Add(txtDescription.Text);
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

            conDB.closeConnection();

            await this.ShowMessageAsync("Add Product", "Record successfully saved!");
        }

        private List<ProductModel> loadDataGridDetails()
        {
            conDB = new ConnectionDB();
            string queryString = "SELECT ID, name, description, nonmemberprice, memberprice, homeprice, megaprice, depotprice, employeeprice," +
                " MFG, unilevel, lb FROM dbpackage.tblproducts WHERE isDeleted = 0";

            List<ProductModel> lstProducts = new List<ProductModel>();
            ProductModel prod = new ProductModel();

            MySqlDataReader reader = conDB.getSelectConnection(queryString, null);

            while (reader.Read())
            {
                prod.ID = reader["ID"].ToString();
                prod.Name = reader["name"].ToString();
                prod.Description = reader["description"].ToString();
                prod.NonmemberPrice = reader["nonmemberprice"].ToString();
                prod.MemberPrice = reader["memberprice"].ToString();
                prod.HomePrice = reader["homeprice"].ToString();
                prod.MegaPrice = reader["megaprice"].ToString();
                prod.DepotPrice = reader["depotprice"].ToString();
                prod.EmployeePrice = reader["employeeprice"].ToString();
                prod.MFG = reader["MFG"].ToString();
                prod.Unilevel = reader["unilevel"].ToString();
                prod.LB = reader["lb"].ToString();
                
                lstProducts.Add(prod);
                prod = new ProductModel();
            }

            return lstProducts;
        }

        private void updateRecord()
        {

            conDB = new ConnectionDB();
            string queryString = "UPDATE dbpackage.tblproducts SET name = ?, description = ?, nonmemberprice = ?, memberprice = ?, " +
                "homeprice = ?, megaprice = ?, depotprice = ?, employeeprice = ?, MFG = ?, unilevel = ?, lb = ? WHERE ID = ?";
            List<string> parameters = new List<string>();
            parameters.Add(txtProductName.Text);
            parameters.Add(txtDescription.Text);
            parameters.Add(txtNonMemberPrice.Text);
            parameters.Add(txtMemberPrice.Text);
            parameters.Add(txtHomePrice.Text);
            parameters.Add(txtMegaPrice.Text);
            parameters.Add(txtDepotPrice.Text);
            parameters.Add(txtEmployeePrice.Text);
            parameters.Add(txtMFG.Text);
            parameters.Add(txtUnilevel.Text);
            parameters.Add(txtLB.Text);
            parameters.Add(productMod.ID);

            conDB.AddRecordToDatabase(queryString, parameters);


            conDB.closeConnection();
        }

        private async void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            bool x = await checkFields();

            if (x)
            {
                updateRecord();
                await this.ShowMessageAsync("UPDATE RECORD", "Record updated successfully!");

                this.Close();
                productWindow.dgvProducts.ItemsSource = loadDataGridDetails();
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
    }
}
