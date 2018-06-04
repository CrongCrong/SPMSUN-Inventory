using MySql.Data.MySqlClient;
using SPMSUN_Inventory.classes;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace SPMSUN_Inventory.views
{
    /// <summary>
    /// Interaction logic for Reports.xaml
    /// </summary>
    public partial class Reports : UserControl
    {
        public Reports()
        {
            InitializeComponent();
        }

        ConnectionDB conDB = new ConnectionDB();
        List<NetworkingSalesModel> lstNetworkingSales = new List<NetworkingSalesModel>();
        List<NetworkingSalesModel> filteredNetworkingSales = new List<NetworkingSalesModel>();
        List<DrNoPayment> lstDrNoPayments = new List<DrNoPayment>();
        List<DrWithPayment> lstDrWithPayments = new List<DrWithPayment>();
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void btnGenerate_Click(object sender, RoutedEventArgs e)
        {
            getDR();
           
            ReportForm report = new ReportForm(filteredNetworkingSales, lstDrNoPayments, lstDrWithPayments);
            report.ShowDialog();
        }

        private List<NetworkingSalesModel> getDR()
        {
            conDB = new ConnectionDB();
            lstNetworkingSales = new List<NetworkingSalesModel>();
            filteredNetworkingSales = new List<NetworkingSalesModel>();
            lstDrNoPayments = new List<DrNoPayment>();
            lstDrWithPayments = new List<DrWithPayment>();
            NetworkingSalesModel networkingSales = new NetworkingSalesModel();

            string queryString = "SELECT tblorderhistory.ID, clientID, concat(tblclients.firstname, ' ', tblclients.lastname) as fName, " +
                "tblorderhistory.date, drNo, total FROM(dbpackage.tblorderhistory INNER JOIN dbpackage.tblclients ON tblorderhistory.clientID = tblclients.ID) " +
                "WHERE (dbpackage.tblorderhistory.date BETWEEN ? AND ? OR dbpackage.tblorderhistory.date BETWEEN ? AND ?) AND dbpackage.tblorderhistory.isDeleted = 0" +
                " order by drNo ASC";
            List<string> parameters = new List<string>();
            DateTime searchDate = DateTime.Parse(dateFrom.Text);
            parameters.Add(searchDate.Year + "-" + searchDate.Month.ToString().PadLeft(2,'0') + "-" + searchDate.Day.ToString().PadLeft(2, '0'));

            searchDate = DateTime.Parse(dateTo.Text);
            parameters.Add(searchDate.Year + "-" + searchDate.Month.ToString().PadLeft(2, '0') + "-" + searchDate.Day.ToString().PadLeft(2, '0'));

            searchDate = DateTime.Parse(dateFrom.Text);
            parameters.Add(searchDate.Year + "-" + searchDate.Month.ToString() + "-" + searchDate.Day.ToString());

            searchDate = DateTime.Parse(dateTo.Text);
            parameters.Add(searchDate.Year + "-" + searchDate.Month.ToString() + "-" + searchDate.Day.ToString());
            
            MySqlDataReader reader = conDB.getSelectConnection(queryString, parameters);

            while (reader.Read())
            {
                networkingSales.ID = reader["ID"].ToString();
                networkingSales.ClientID = reader["clientID"].ToString();
                networkingSales.Name = reader["fName"].ToString();
                networkingSales.Date = reader["date"].ToString();
                networkingSales.DRNo = reader["drNo"].ToString();
                //double z = Convert.ToDouble(reader["total"].ToString());
                networkingSales.TotalAmount = reader["total"].ToString();
                lstNetworkingSales.Add(networkingSales);
                networkingSales = new NetworkingSalesModel();
            }

            conDB.closeConnection();

            getPackageMFG();

            return lstNetworkingSales;
        }

        private void getPackageMFG()
        {
            conDB = new ConnectionDB();
            double netIncome = 0.0;

            List<string> parameters = new List<string>();
            // MFG, UNILEVEL, LB ON PRODUCTS
            string queryString = "SELECT dbpackage.tblorderdetails.ID, packageID, tblproducts.description, qty, price, MFG, unilevel, lb FROM " +
              "(dbpackage.tblorderdetails INNER JOIN dbpackage.tblproducts ON dbpackage.tblorderdetails.productID = " +
              "dbpackage.tblproducts.ID) WHERE tblorderdetails.isDeleted = 0 AND dbpackage.tblproducts.isDeleted = 0 AND orderID = ?";

            foreach (NetworkingSalesModel nsm in lstNetworkingSales)
            {

                parameters.Add(nsm.ID);
                double MFGsum = 0.0;
                double var1 = 0.0;
                double PVSum = 0.0;
                double unilevelVar = 0.0;
                double LBvar = 0.0;
                double qty = 0.0;


                MySqlDataReader reader = conDB.getSelectConnection(queryString, parameters);
                while (reader.Read())
                {
                    qty = Convert.ToDouble(reader["qty"].ToString());
                    var1 = Convert.ToDouble(reader["MFG"].ToString());
                    unilevelVar = Convert.ToDouble(reader["unilevel"].ToString());
                    LBvar = Convert.ToDouble(reader["lb"].ToString());
                    unilevelVar = (qty * unilevelVar);
                    LBvar = (qty * LBvar);
                    PVSum = PVSum + (unilevelVar + LBvar);

                    MFGsum = MFGsum + (qty * var1);
                    unilevelVar = 0;
                    LBvar = 0;
                }
                nsm.MFG = MFGsum.ToString("N0");
                nsm.PV = PVSum.ToString("N0");
                parameters = new List<string>();
                conDB.closeConnection();
            }

            // MFG, UNILEVEL, LB ON PACKAGES
            queryString = "SELECT packageID, dbpackage.tblpackage.name, qty, MFG, price, unilevel, lb FROM " +
                "(dbpackage.tblorderdetails INNER JOIN dbpackage.tblpackage ON dbpackage.tblorderdetails.packageID = dbpackage.tblpackage.ID) " +
                "WHERE productID = 0 AND dbpackage.tblpackage.isDeleted = 0 AND dbpackage.tblorderdetails.isDeleted = 0 AND orderID = ?";

            foreach (NetworkingSalesModel nsm in lstNetworkingSales)
            {

                parameters.Add(nsm.ID);
                double MFGsum = 0.0;
                double var1 = 0.0;
                double PVSum = 0.0;
                double unilevelVar = 0.0;
                double LBvar = 0.0;
                double qty = 0.0;

                MySqlDataReader reader = conDB.getSelectConnection(queryString, parameters);
                while (reader.Read())
                {
                    qty = Convert.ToDouble(reader["qty"].ToString());
                    var1 = Convert.ToDouble(reader["MFG"].ToString());
                    unilevelVar = Convert.ToDouble(reader["unilevel"].ToString());
                    LBvar = Convert.ToDouble(reader["lb"].ToString());
                    unilevelVar = qty * unilevelVar;
                    LBvar = qty * LBvar;
                    PVSum = PVSum + (unilevelVar + LBvar);

                    MFGsum = MFGsum + (qty * var1);
                    unilevelVar = 0;
                    LBvar = 0;
                }

                nsm.MFG = (Convert.ToDouble(nsm.MFG) + MFGsum).ToString("N0");
                nsm.PV = (Convert.ToDouble(nsm.PV) + PVSum).ToString("N0");
                netIncome = (Convert.ToDouble(nsm.TotalAmount) - Convert.ToDouble(nsm.MFG)) - Convert.ToDouble(nsm.PV);
                nsm.NetIncome = netIncome.ToString("N0");
                parameters = new List<string>();
                conDB.closeConnection();
            }


            DrNoPayment drNoPayment = new DrNoPayment();

            //DR WITHOUT PAYMENTS
            List<DrNoPayment> tempDrNoPayment = new List<DrNoPayment>();

            queryString = "SELECT tblclients.ID, concat(tblclients.firstname, ' ', tblclients.lastname) as fName, drNo, total FROM " +
                "(dbpackage.tblorderhistory INNER JOIN dbpackage.tblclients ON " +
                "dbpackage.tblorderhistory.clientID = dbpackage.tblclients.ID) WHERE " +
                "(dbpackage.tblorderhistory.isDeleted = 0) AND (dbpackage.tblclients.isDeleted = 0) AND dbpackage.tblorderhistory.date BETWEEN ? AND ? AND isPaid = 0";

            parameters = new List<string>();
            DateTime searchDate = DateTime.Parse(dateFrom.Text);
            parameters.Add(searchDate.Year + "-" + searchDate.Month.ToString().PadLeft(2, '0') + "-" + searchDate.Day.ToString().PadLeft(2, '0'));

            searchDate = DateTime.Parse(dateTo.Text);
            parameters.Add(searchDate.Year + "-" + searchDate.Month.ToString().PadLeft(2, '0') + "-" + searchDate.Day.ToString().PadLeft(2, '0'));

            MySqlDataReader reader1 = conDB.getSelectConnection(queryString, parameters);

            while (reader1.Read())
            {
                drNoPayment.ClientID = reader1["ID"].ToString();
                drNoPayment.DRNo = reader1["drNo"].ToString();
                drNoPayment.TotalAmount = reader1["total"].ToString();
                drNoPayment.Name = reader1["fName"].ToString();
                drNoPayment.ARBalance = reader1["total"].ToString();
                lstDrNoPayments.Add(drNoPayment);
                drNoPayment = new DrNoPayment();
            }

            //Removing dr without payments on list and add it to new list of no payments
            if(lstDrNoPayments.Count > 0)
            {
                foreach (NetworkingSalesModel nsm in lstNetworkingSales)
                {
                    filteredNetworkingSales.Add(nsm);
                    foreach (DrNoPayment noPay in lstDrNoPayments)
                    {
                        if ((noPay.ClientID.Equals(nsm.ClientID)) && (noPay.DRNo.Equals(nsm.DRNo)))
                        {
                            noPay.ID = nsm.ID;
                            noPay.DRNo = nsm.DRNo;
                            noPay.Date = nsm.Date;
                            noPay.MFG = nsm.MFG;
                            noPay.PV = nsm.PV;
                            noPay.TotalAmount = nsm.TotalAmount;
                            noPay.NetIncome = nsm.NetIncome;
                            double x = Convert.ToDouble(noPay.ARBalance);
                            noPay.ARBalance = x.ToString("N0");
                            filteredNetworkingSales.Remove(nsm);
                            tempDrNoPayment.Add(noPay);
                        }

                    }
                }
                lstDrNoPayments = new List<DrNoPayment>();
                foreach (DrNoPayment d in tempDrNoPayment)
                {
                    lstDrNoPayments.Add(d);
                }
            }else
            {
                foreach(NetworkingSalesModel ns in lstNetworkingSales)
                {
                    ns.TotalAmount = Convert.ToDouble(ns.TotalAmount).ToString("N0");
                    filteredNetworkingSales.Add(ns);
                }
            }
            

            //DR WITH PARTIAL PAYMENTS

            DrWithPayment drWithPayment = new DrWithPayment();

            List<DrWithPayment> tempDrWithPayment = new List<DrWithPayment>();

            queryString = "SELECT dbpackage.tblclients.ID, concat(tblclients.firstname, ' ', tblclients.lastname) as fName, orderID, " +
                "total - sum(amountpaid) as pending, isPaid, drNo FROM((dbpackage.tblpaymenthistory " +
                "INNER JOIN dbpackage.tblorderhistory ON dbpackage.tblpaymenthistory.orderID = dbpackage.tblorderhistory.ID) " +
                "INNER JOIN dbpackage.tblclients ON dbpackage.tblpaymenthistory.clientID = dbpackage.tblclients.ID) " +
                "WHERE (dbpackage.tblorderhistory.isDeleted = 0 and dbpackage.tblpaymenthistory.isDeleted = 0) " +
                "and (dbpackage.tblorderhistory.date BETWEEN ? AND ? OR dbpackage.tblorderhistory.date BETWEEN ? AND ?) and isPaid = 0";

            parameters = new List<string>();
            searchDate = DateTime.Parse(dateFrom.Text);
            parameters.Add(searchDate.Year + "-" + searchDate.Month.ToString().PadLeft(2, '0') + "-" + searchDate.Day.ToString().PadLeft(2, '0'));

            searchDate = DateTime.Parse(dateTo.Text);
            parameters.Add(searchDate.Year + "-" + searchDate.Month.ToString().PadLeft(2, '0') + "-" + searchDate.Day.ToString().PadLeft(2, '0'));

            searchDate = DateTime.Parse(dateFrom.Text);
            parameters.Add(searchDate.Year + "-" + searchDate.Month.ToString().PadLeft(2, '0') + "-" + searchDate.Day.ToString().PadLeft(2, '0'));

            searchDate = DateTime.Parse(dateTo.Text);
            parameters.Add(searchDate.Year + "-" + searchDate.Month.ToString() + "-" + searchDate.Day.ToString());



            reader1 = conDB.getSelectConnection(queryString, parameters);

            while (reader1.Read())
            {
                drWithPayment.Name = reader1["fName"].ToString();
                drWithPayment.ClientID = reader1["ID"].ToString();
                drWithPayment.ARBalance = reader1["pending"].ToString();
                drWithPayment.DRNo = reader1["drNo"].ToString();
                lstDrWithPayments.Add(drWithPayment);
                drWithPayment = new DrWithPayment();
            }

            conDB.closeConnection();

            foreach (NetworkingSalesModel nsm in lstNetworkingSales)
            {
                foreach(DrWithPayment dw in lstDrWithPayments)
                {
                    if(dw.ClientID.Equals(nsm.ClientID) && (dw.DRNo.Equals(nsm.DRNo)))
                    {
                        dw.ID = nsm.ID;
                        dw.Date = nsm.Date;
                        dw.MFG = nsm.MFG;
                        dw.PV = nsm.PV;
                        dw.TotalAmount = nsm.TotalAmount;
                        dw.NetIncome = nsm.NetIncome;
                        tempDrWithPayment.Add(dw);
                        
                    }
                }
            }

            lstDrWithPayments = new List<DrWithPayment>();
            foreach(DrWithPayment dwp in tempDrWithPayment)
            {
                lstDrWithPayments.Add(dwp);
            }
        }
    }
}
