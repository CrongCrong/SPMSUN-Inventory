using MySql.Data.MySqlClient;
using SPMSUN_Inventory.classes;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace SPMSUN_Inventory.views
{
    /// <summary>
    /// Interaction logic for PendingBalance.xaml
    /// </summary>
    public partial class PendingBalance : UserControl
    {
        public PendingBalance()
        {
            InitializeComponent();
        }

        ConnectionDB conDB;

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            dgvPendingBalance.ItemsSource = loadPendingBalances();
        }

        private List<PendingBalanceModel> loadPendingBalances()
        {
            List<PendingBalanceModel> unpaidDR = loadUnpaidDR();
            List<PendingBalanceModel> incDR = loadIncompletePaidDR();
            List<PendingBalanceModel> finalUnpaidDR = new List<PendingBalanceModel>();
            PendingBalanceModel finalPBM = new PendingBalanceModel();
            foreach (PendingBalanceModel pbm in unpaidDR)
            {
                finalPBM.ID = pbm.ID;
                finalPBM.Firstname = pbm.Firstname;
                finalPBM.Lastname = pbm.Lastname;
                finalPBM.FullName = finalPBM.Firstname + " " + finalPBM.Lastname;
                finalPBM.Total = pbm.Total;
                finalPBM.DRNo = pbm.DRNo;
                finalPBM.UnpaidAmount = pbm.Total;
                foreach (PendingBalanceModel incpbm in incDR)
                {
                    if (pbm.ID.Equals(incpbm.ID))
                    {
                        double x = Convert.ToDouble(pbm.Total);
                        double y = Convert.ToDouble(incpbm.UnpaidAmount);

                        double sum = x - y;
                        if(sum == 0)
                        {
                            finalPBM.UnpaidAmount = Convert.ToDouble(pbm.Total).ToString("N0");
                        }else
                        {
                            finalPBM.UnpaidAmount = Convert.ToDouble(sum).ToString("N0");
                        }
                        
                    }
                    //else
                    //{
                    //    finalPBM.UnpaidAmount = pbm.Total;
                    //}
                }
                finalUnpaidDR.Add(finalPBM);
                finalPBM = new PendingBalanceModel();
            }
            return finalUnpaidDR;
        }

        private List<PendingBalanceModel> loadUnpaidDR()
        {
            conDB = new ConnectionDB();
            List<PendingBalanceModel> lstPending = new List<PendingBalanceModel>();
            PendingBalanceModel pending = new PendingBalanceModel();

            string queryString = "SELECT tblclients.ID, firstname, lastname, drNo, total FROM " +
                "(dbpackage.tblorderhistory INNER JOIN dbpackage.tblclients ON " +
                "dbpackage.tblorderhistory.clientID = dbpackage.tblclients.ID) " +
                "WHERE(dbpackage.tblorderhistory.isDeleted = 0) AND (dbpackage.tblclients.isDeleted = 0) AND isPaid = 0 AND dbpackage.tblorderhistory.isCancelled = 0";

            MySqlDataReader reader = conDB.getSelectConnection(queryString, null);

            while (reader.Read())
            {
                pending.ID = reader["ID"].ToString();
                pending.Firstname = reader["firstname"].ToString();
                pending.Lastname = reader["lastname"].ToString();
                pending.DRNo = reader["drNo"].ToString();
                double z = Convert.ToDouble(reader["total"].ToString());
                pending.Total = z.ToString("N0");
                lstPending.Add(pending);
                pending = new PendingBalanceModel();
            }

            conDB.closeConnection();
            return lstPending;
        }

        private List<PendingBalanceModel> loadIncompletePaidDR()
        {
            conDB = new ConnectionDB();

            List<PendingBalanceModel> lstPending = new List<PendingBalanceModel>();
            PendingBalanceModel pending = new PendingBalanceModel();

            string queryString = "SELECT dbpackage.tblclients.ID, firstname, lastname, orderID, " +
                "total - sum(amountpaid) as pending, total, isPaid FROM((dbpackage.tblpaymenthistory " +
                "INNER JOIN dbpackage.tblorderhistory ON dbpackage.tblpaymenthistory.orderID = dbpackage.tblorderhistory.ID) " +
                "INNER JOIN dbpackage.tblclients ON dbpackage.tblpaymenthistory.clientID = dbpackage.tblclients.ID) " +
                "WHERE dbpackage.tblorderhistory.isDeleted = 0 and dbpackage.tblpaymenthistory.isDeleted = 0 " +
                "and isPaid = 0 group by orderID";

            MySqlDataReader reader = conDB.getSelectConnection(queryString, null);

            while (reader.Read())
            {
                pending.ID = reader["ID"].ToString();
                pending.Firstname = reader["firstname"].ToString();
                pending.Lastname = reader["lastname"].ToString();
                //pending.DRNo = reader["drNo"].ToString();
                pending.Total = reader["total"].ToString();
                pending.UnpaidAmount = reader["pending"].ToString();
                lstPending.Add(pending);
                pending = new PendingBalanceModel();
            }

            conDB.closeConnection();
            return lstPending;
        }
    }
}
