using MahApps.Metro.Controls;
using SPMSUN_Inventory.classes;
using System.Windows;

namespace SPMSUN_Inventory
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        LogInWindow logInWindow;
        public MainWindow(LogInWindow lw)
        {
            logInWindow = lw;

            InitializeComponent();
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {


            if (UserModel.TypeOfAccount.Equals(AccountType.ADMIN))
            {

            }
            else
            {
                menuDepot.IsEnabled = false;
                menuEmployees.IsEnabled = false;
                menuHomeStockist.IsEnabled = false;
                menuMegaStockist.IsEnabled = false;
                pendingBalance.IsEnabled = false;

            }

        }

        private void HamburgerMenuControl_OnItemClick(object sender, ItemClickEventArgs e)
        {
            this.HamburgerControl.Content = e.ClickedItem;
            this.HamburgerControl.IsPaneOpen = false;

        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            logInWindow.Show();
        }
    }
}
