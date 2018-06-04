using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using MySql.Data.MySqlClient;
using SPMSUN_Inventory.classes;
using System.Collections.Generic;
using System.Windows;

namespace SPMSUN_Inventory
{
    /// <summary>
    /// Interaction logic for ProductsWindow.xaml
    /// </summary>
    public partial class ProductsWindow : MetroWindow
    {
        public ProductsWindow()
        {
            InitializeComponent();
        }

        ConnectionDB conDB;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            dgvProducts.ItemsSource = loadDataGridDetails();
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            ProductDetails pd = new ProductDetails(this);
            pd.ShowDialog();
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

        private void DeleteProductRecord(string strProductID)
        {
            string queryString = "UPDATE dbpackage.tblproducts SET isDeleted = 1 WHERE ID = ?";
            List<string> parameters = new List<string>();
            parameters.Add(strProductID);

            conDB.AddRecordToDatabase(queryString, parameters);
            conDB.closeConnection();
        }

        private void DeleteProductOnPackages(string strProductID)
        {
            string queryString = "UPDATE dbpackage.tblpackageproduct SET isDeleted = 1 WHERE productID = ?";
            List<string> parameters = new List<string>();
            parameters.Add(strProductID);

            conDB.AddRecordToDatabase(queryString, parameters);
            conDB.closeConnection();
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            ProductModel p = dgvProducts.SelectedItem as ProductModel;
            if (p != null)
            {
                ProductDetails pd = new ProductDetails(this, p);
                pd.ShowDialog();
            }
        }

        private async void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            ProductModel prodModel = dgvProducts.SelectedItem as ProductModel;
            if (prodModel != null)
            {
                MessageDialogResult result = await this.ShowMessageAsync("Delete Record", "Are you sure you want to delete record?", MessageDialogStyle.AffirmativeAndNegative);

                if (result.Equals(MessageDialogResult.Affirmative))
                {
                    DeleteProductRecord(prodModel.ID);
                    DeleteProductOnPackages(prodModel.ID);
                    dgvProducts.ItemsSource = loadDataGridDetails();
                    await this.ShowMessageAsync("Delete Record", "Record deleted successfully!");
                }
            }
        }
    }
}
