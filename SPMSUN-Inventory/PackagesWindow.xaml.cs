using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using MySql.Data.MySqlClient;
using SPMSUN_Inventory.classes;
using System.Collections.Generic;
using System.Windows;

namespace SPMSUN_Inventory
{
    /// <summary>
    /// Interaction logic for PackagesWindow.xaml
    /// </summary>
    public partial class PackagesWindow : MetroWindow
    {
        public PackagesWindow()
        {
            InitializeComponent();
        }

        ConnectionDB conDB;

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            dgvPackageList.ItemsSource = loadDataGridDetails();
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            PackageDetails packageDet = new PackageDetails(this);
            packageDet.ShowDialog();

        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            PackageModel pkMod = dgvPackageList.SelectedItem as PackageModel;
            if (pkMod != null)
            {
                PackageDetails pd = new PackageDetails(this, pkMod);
                pd.ShowDialog();
            }
        }

        private async void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            PackageModel pm = dgvPackageList.SelectedItem as PackageModel;
            if (pm != null)
            {
                MessageDialogResult result = await this.ShowMessageAsync("Delete Record", "Are you sure you want to delete record?", MessageDialogStyle.AffirmativeAndNegative);

                if (result.Equals(MessageDialogResult.Affirmative))
                {
                    deletePackageRecord(pm.ID);
                    deleteProductsOnPackage(pm.ID);
                    dgvPackageList.ItemsSource = loadDataGridDetails();
                    await this.ShowMessageAsync("Delete Record", "Record deleted successfully!");
                }
            }
        }

        private void deletePackageRecord(string strRecordID)
        {
            conDB = new ConnectionDB();
            string queryString = "UPDATE dbpackage.tblpackage SET isDeleted = 1 WHERE ID = ?";
            List<string> parameters = new List<string>();
            parameters.Add(strRecordID);

            conDB.AddRecordToDatabase(queryString, parameters);
            conDB.closeConnection();

        }

        private void deleteProductsOnPackage(string strPackageID)
        {
            conDB = new ConnectionDB();
            string queryString = "UPDATE dbpackage.tblpackageproduct SET isDeleted = 1 WHERE packageID = ?";
            List<string> parameters = new List<string>();

            parameters.Add(strPackageID);

            conDB.AddRecordToDatabase(queryString, parameters);
            conDB.closeConnection();

        }

        private List<PackageModel> loadDataGridDetails()
        {
            conDB = new ConnectionDB();
            List<PackageModel> lstLPackageModel = new List<PackageModel>();
            PackageModel pm = new PackageModel();

            string queryString = "SELECT ID, name, nonmemberprice, memberprice, homeprice, megaprice, depotprice, employeeprice, " +
                "MFG, unilevel, lb FROM dbpackage.tblpackage WHERE isDeleted = 0";

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

            return lstLPackageModel;
        }

        private void btnView_Click(object sender, RoutedEventArgs e)
        {
            PackageModel pkMod = dgvPackageList.SelectedItem as PackageModel;
            if (pkMod != null)
            {
                PackageDetails pd = new PackageDetails(this, pkMod);
                pd.ifView = true;
                pd.ShowDialog();
            }
        }
    }
}
