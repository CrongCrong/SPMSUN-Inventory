using SPMSUN_Inventory.classes;
using System.Windows;
using System.Windows.Controls;

namespace SPMSUN_Inventory.views
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : UserControl
    {
        public Settings()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (!UserModel.TypeOfAccount.Equals(AccountType.ADMIN))
            {
                btnAddPackages.IsEnabled = false;
                btnAddProducts.IsEnabled = false;
            }
        }

        private void btnAddPackages_Click(object sender, RoutedEventArgs e)
        {
            PackagesWindow pw = new PackagesWindow();
            pw.ShowDialog();
        }

        private void btnAddProducts_Click(object sender, RoutedEventArgs e)
        {
            ProductsWindow ppw = new ProductsWindow();
            ppw.ShowDialog();
        }
    }
}
