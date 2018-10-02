using System.Windows;
using System.Windows.Controls;
using Tuxblox.Model.Entities;
using Tuxblox.ViewModel;

namespace Tuxblox.View
{
    /// <summary>
    /// Interaction logic for AddressesView.xaml
    /// </summary>
    public partial class AddressesView : Page
    {
        public AddressesView()
        {
            InitializeComponent();
        }

        private void MenuItem_GetPrivateKey_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is AddressesViewModel modelContext)
            {
                var item = sender as MenuItem;
                var itemContext = item.DataContext as AddressEntity;

                if (itemContext is AddressEntity)
                {
                    modelContext.GetPrivateKey(itemContext.Address);

                    var message = "Private key has been copied to clipboard!";
                    var dialog = new BloxDialogView(300, 400, "Private Key", message);
                    dialog.ShowDialog();
                }
            }
        }

        private void MenuItem_Copy_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is AddressesViewModel modelContext)
            {
                var item = sender as MenuItem;
                var itemContext = item.DataContext as AddressEntity;

                if (itemContext is AddressEntity)
                {
                    Clipboard.SetText(itemContext.Address);
                }
            }
        }
    }
}
