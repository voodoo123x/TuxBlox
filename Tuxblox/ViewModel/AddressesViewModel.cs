using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using Tuxblox.Model;
using Tuxblox.Model.Entities;
using Tuxblox.Operations;

namespace Tuxblox.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// See http://www.mvvmlight.net
    /// </para>
    /// </summary>
    public class AddressesViewModel : ViewModelBase
    {
        private readonly IDataService _DataService;
        private readonly RefreshWorker _RefreshWorker;
        private readonly object _LockObject = new object();

        public ObservableCollection<AddressEntity> Addresses { get; set; }

        private string _AddressesViewHeaderText;
        private string _NewAddressLabel;
        private string _SelectedAddress;

        /// <summary>
        /// Gets or sets the AddressesViewHeaderText property.
        /// </summary>
        public string AddressesViewHeaderText
        {
            get { return _AddressesViewHeaderText; }

            set
            {
                if (_AddressesViewHeaderText != value)
                {
                    Set(ref _AddressesViewHeaderText, value);
                }
            }
        }

        /// <summary>
        /// Gets or sets the NewAddressLabel property.
        /// </summary>
        public string NewAddressLabel
        {
            get { return _NewAddressLabel; }

            set
            {
                if (_NewAddressLabel != value)
                {
                    Set(ref _NewAddressLabel, value);
                }
            }
        }

        /// <summary>
        /// Gets or sets the SelectedAddress property.
        /// </summary>
        public string SelectedAddress
        {
            get { return _SelectedAddress; }

            set
            {
                if (_SelectedAddress != value)
                {
                    Set(ref _SelectedAddress, value);
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the AddressesViewModel class.
        /// </summary>
        public AddressesViewModel(IDataService dataService)
        {
            Addresses = new ObservableCollection<AddressEntity>();
            _DataService = dataService;

            _RefreshWorker = new RefreshWorker(50, () =>
            {
                _DataService.GetAddresses((addresses) =>
                {
                    lock (_LockObject)
                    {
                        UpdateAddressList(addresses);
                    }
                });
            });
        }

        /// <summary>
        /// Cleanup view model.
        /// </summary>
        public override void Cleanup()
        {
            _RefreshWorker.Stop();

            base.Cleanup();
        }

        /// <summary>
        /// Gets the private for a specified address.
        /// </summary>
        /// <param name="address">Address to get private key.</param>
        public void GetPrivateKey(string address)
        {
            var privateKey = string.Empty;

            _DataService.GetPrivateKey(address, (pk) =>
            {
                privateKey = pk;
            });

            Clipboard.SetText(privateKey);
        }

        private void UpdateAddressList(IEnumerable<AddressEntity> newAddresses)
        {
            if (newAddresses == null)
            {
                return;
            }

            foreach (var addr in newAddresses)
            {
                if (!Addresses.Any(oldAddr => string.Equals(oldAddr.Address, addr.Address, StringComparison.Ordinal)))
                {
                    Addresses.Add(addr);
                }
            }
        }

        #region Commands

        private RelayCommand _GetNewAddressCommand;
        public RelayCommand GetNewAddressCommand
        {
            get
            {
                if (_GetNewAddressCommand == null)
                {
                    _GetNewAddressCommand = new RelayCommand(OnGetNewAddress);
                }

                return _GetNewAddressCommand;
            }
        }

        private void OnGetNewAddress()
        {
            var newAddress = NodeOperations.GetNewAddress(NewAddressLabel);
            lock (_LockObject)
            {
                if (!Addresses.Any(a => string.Equals(a.Address, newAddress.Address, StringComparison.Ordinal)))
                {
                    Addresses.Add(newAddress);
                }
            }

            NewAddressLabel = string.Empty;
        }

        private RelayCommand<string> _CopyAddressCommand;
        public RelayCommand<string> CopyAddressCommand
        {
            get
            {
                if (_CopyAddressCommand == null)
                {
                    _CopyAddressCommand = new RelayCommand<string>(OnCopyAddress);
                }

                return _CopyAddressCommand;
            }
        }

        private void OnCopyAddress(string param)
        {
            Clipboard.SetText(param);
        }

        #endregion
    }
}