using System;
using System.Windows;
using System.Windows.Controls;
using Tuxblox.View;
using Tuxblox.ViewModel;

namespace Tuxblox
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private BalanceView _BalancePage;
        private TransactionsView _TransactionsPage;
        private SendView _SendView;

        /// <summary>
        /// Initializes a new instance of the MainWindow class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            Closing += (s, e) => ViewModelLocator.Cleanup();

            WalletManager.Get().Start(250);

            LoadPageIntoFrame(_BalanceFrame, _BalancePage, typeof(BalanceView));
            LoadPageIntoFrame(_ContentFrame, _TransactionsPage, typeof(TransactionsView));
        }

        private void Transactions_Button_Click(object sender, RoutedEventArgs e)
        {
            LoadPageIntoFrame(_ContentFrame, _TransactionsPage, typeof(TransactionsView));
        }

        private void Send_Button_Click(object sender, RoutedEventArgs e)
        {
            LoadPageIntoFrame(_ContentFrame, _SendView, typeof(SendView));
        }

        private void LoadPageIntoFrame(Frame frame, object pageObject, Type pageType)
        {
            if (pageObject == null)
            {
                pageObject = Activator.CreateInstance(pageType); 
            }

            frame.Content = (Page)pageObject;
        }
    }
}