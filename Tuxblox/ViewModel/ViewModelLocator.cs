/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocatorTemplate xmlns:vm="clr-namespace:Tuxblox.ViewModel"
                                   x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"
*/

using CommonServiceLocator;
using GalaSoft.MvvmLight.Ioc;
using Tuxblox.Model;

namespace Tuxblox.ViewModel
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// <para>
    /// See http://www.mvvmlight.net
    /// </para>
    /// </summary>
    public class ViewModelLocator
    {
        static ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);
            SimpleIoc.Default.Register<IDataService, DataService>();
            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<BalanceViewModel>();
            SimpleIoc.Default.Register<TransactionsViewModel>();
            SimpleIoc.Default.Register<SendViewModel>();
            SimpleIoc.Default.Register<AddressesViewModel>();
            SimpleIoc.Default.Register<BloxDialogViewModel>();
        }

        /// <summary>
        /// Gets the Main property.
        /// </summary>
        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
        //    "CA1822:MarkMembersAsStatic",
        //    Justification = "This non-static member is needed for data binding purposes.")]
        public MainViewModel Main
        {
            get { return ServiceLocator.Current.GetInstance<MainViewModel>(); }
        }

        /// <summary>
        /// Gets the Balance property.
        /// </summary>
        public BalanceViewModel Balance
        {
            get { return ServiceLocator.Current.GetInstance<BalanceViewModel>(); }
        }

        /// <summary>
        /// Gets the Transactions property.
        /// </summary>
        public TransactionsViewModel Transactions
        {
            get { return ServiceLocator.Current.GetInstance<TransactionsViewModel>(); }
        }

        /// <summary>
        /// Gets the Send property.
        /// </summary>
        public SendViewModel Send
        {
            get { return ServiceLocator.Current.GetInstance<SendViewModel>(); }
        }

        /// <summary>
        /// Gets the Addresses property.
        /// </summary>
        public AddressesViewModel Addresses
        {
            get { return ServiceLocator.Current.GetInstance<AddressesViewModel>(); }
        }

        /// <summary>
        /// Gets the BloxDialog property.
        /// </summary>
        public BloxDialogViewModel BloxDialog
        {
            get { return ServiceLocator.Current.GetInstance<BloxDialogViewModel>(); }
        }

        /// <summary>
        /// Cleans up all the resources.
        /// </summary>
        public static void Cleanup()
        {
            ServiceLocator.Current.GetInstance<MainViewModel>().Cleanup();
            ServiceLocator.Current.GetInstance<BalanceViewModel>().Cleanup();
            ServiceLocator.Current.GetInstance<TransactionsViewModel>().Cleanup();
            ServiceLocator.Current.GetInstance<SendViewModel>().Cleanup();
            ServiceLocator.Current.GetInstance<AddressesViewModel>().Cleanup();

            WalletManager.Get().Stop();
        }
    }
}