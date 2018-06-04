using MahApps.Metro.Controls;
using Microsoft.Reporting.WinForms;
using SPMSUN_Inventory.classes;
using System.Collections.Generic;
using System.Windows;

namespace SPMSUN_Inventory
{
    /// <summary>
    /// Interaction logic for ReportForm.xaml
    /// </summary>
    public partial class ReportForm : MetroWindow
    {
        public ReportForm()
        {
            InitializeComponent();
        }

        List<NetworkingSalesModel> lstNetworkingSales;
        List<DrNoPayment> lstDRNoPayment;
        List<DrWithPayment> lstDrWithPayment;
        public ReportForm(List<NetworkingSalesModel> nsm, List<DrNoPayment> dnp, List<DrWithPayment> dwp)
        {
            lstNetworkingSales = nsm;
            lstDRNoPayment = dnp;
            lstDrWithPayment = dwp;
            InitializeComponent();
        }

      

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            ReportDataSource rds = new ReportDataSource();
            ReportDataSource dsNoDR = new ReportDataSource();
            ReportDataSource dsWithDR = new ReportDataSource();

            if (lstNetworkingSales != null)
            {
               
                rds = new ReportDataSource("DataSet1", lstNetworkingSales);
                dsNoDR = new ReportDataSource("DataSet2", lstDRNoPayment);
                dsWithDR = new ReportDataSource("DataSet3", lstDrWithPayment);
               
                reportViewer.ProcessingMode = ProcessingMode.Local;
                LocalReport localReport = reportViewer.LocalReport;

                localReport.ReportPath = "reports/NetworkingSales.rdlc";
                reportViewer.RefreshReport();

                System.Drawing.Printing.PageSettings ps = new System.Drawing.Printing.PageSettings();
                ps.Landscape = true;

                ps.PaperSize = new System.Drawing.Printing.PaperSize("A4", 827, 1170);
                ps.PaperSize.RawKind = (int)System.Drawing.Printing.PaperKind.A4;
                reportViewer.SetPageSettings(ps);

                reportViewer.LocalReport.DataSources.Add(rds);
                reportViewer.LocalReport.DataSources.Add(dsNoDR);
                reportViewer.LocalReport.DataSources.Add(dsWithDR);

                // Refresh the report  
                reportViewer.RefreshReport();
            }
        }
    }
}
