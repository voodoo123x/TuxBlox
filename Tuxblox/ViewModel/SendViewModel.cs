using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using System;
using System.ComponentModel;
using System.Text.RegularExpressions;
using Tuxblox.Model;
using Tuxblox.Model.Entities;

namespace Tuxblox.ViewModel
{
    public class SendViewModel : ViewModelBase, IDataErrorInfo
    {
        private const string DefaultSendAmount = "0";
        private const string DefaultSendFee = "0.00001";

        private readonly IDataService _DataService;

        private string _SendAddress;
        private string _SendAmount;
        private string _SendFee;
        private decimal _MaxAmount;
        private bool _AddressIsValid;

        /// <summary>
        /// Gets or sets the SendAddress property.
        /// </summary>
        public string SendAddress
        {
            get { return _SendAddress; }
            set
            {
                if (_SendAddress != value)
                {
                    Set(ref _SendAddress, value);
                }
            }
        }

        /// <summary>
        /// Gets or sets the SendAmount property.
        /// </summary>
        public string SendAmount
        {
            get { return _SendAmount; }
            set
            {
                if (_SendAmount != value)
                {
                    Set(ref _SendAmount, value);
                }
            }
        }

        /// <summary>
        /// Gets or sets the SendFee property.
        /// </summary>
        public string SendFee
        {
            get { return _SendFee; }
            set
            {
                if (_SendFee != value)
                {
                    Set(ref _SendFee, value);
                }
            }
        }

        /// <summary>
        /// Gets or sets the MaxAmount property.
        /// </summary>
        public decimal MaxAmount
        {
            get { return _MaxAmount; }
            set
            {
                if (_MaxAmount != value)
                {
                    Set(ref _MaxAmount, value);
                }
            }
        }

        /// <summary>
        /// Gets or sets the AddressIsValid property.
        /// </summary>
        public bool AddressIsValid
        {
            get { return _AddressIsValid; }
            set
            {
                if (_AddressIsValid != value)
                {
                    Set(ref _AddressIsValid, value);
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the SendViewModel class.
        /// </summary>
        public SendViewModel(IDataService dataService)
        {
            _DataService = dataService;
            SendAmount = DefaultSendAmount;
            SendFee = DefaultSendFee;
        }

        public override void Cleanup()
        {
            base.Cleanup();
        }

        #region IDataErrorInfo Implementation

        private string _Error;
        public string Error
        {
            get { return _Error; }
            set
            {
                if (_Error != value)
                {
                    Set(ref _Error, value);
                }
            }
        }

        public string this[string columnName]
        {
            get
            {
                Error = string.Empty;

                switch (columnName)
                {
                    case "SendAddress":
                        Error = ValidateAddressString(SendAddress);
                        break;

                    case "SendFee":
                        Error = ValidateDecimal(SendFee);
                        break;

                    case "SendAmount":
                        Error = ValidateDecimal(SendAmount);
                        break;
                }

                return Error;
            }
        }

        private string ValidateAddressString(string addressString)
        {
            string error = string.Empty;

            if (addressString?.Length > 0 && addressString?.Length != 34)
            {
                error = "Address is invalid length.";
            }

            return error;
        }

        private string ValidateDecimal(string decimalString)
        {
            string error = string.Empty;

            var regex = new Regex(@"^\d*\.?\d{1,8}$");
            if (!regex.IsMatch(decimalString))
            {
                error = "Invalid decimal format.";
            }

            return error;
        }

        #endregion

        #region Commands

        private RelayCommand _FillMaxCommand;
        public RelayCommand FillMaxCommand
        {
            get
            {
                if (_FillMaxCommand == null)
                {
                    _FillMaxCommand = new RelayCommand(OnFillMax);
                }

                return _FillMaxCommand;
            }
        }

        private void OnFillMax()
        {
            var balance = WalletManager.Get().Value("Balance", false) as BalanceEntity;
            SendAmount = Convert.ToString(balance.TotalBalance);
        }

        private RelayCommand _ResetCommand;
        public RelayCommand ResetCommand
        {
            get
            {
                if (_ResetCommand == null)
                {
                    _ResetCommand = new RelayCommand(OnReset);
                }

                return _ResetCommand;
            }
        }

        private void OnReset()
        {
            SendAddress = string.Empty;
            SendAmount = DefaultSendAmount;
            SendFee = DefaultSendFee;
        }

        private RelayCommand _SendCommand;
        public RelayCommand SendCommand
        {
            get
            {
                if (_SendCommand == null)
                {
                    _SendCommand = new RelayCommand(OnSend, CanSend);
                }

                return _SendCommand;
            }
        }

        private bool CanSend()
        {
            return string.IsNullOrEmpty(Error) && !string.IsNullOrEmpty(SendAddress);
        }

        private void OnSend()
        {
            var amount = Convert.ToDecimal(SendAmount);
            var fee = Convert.ToDecimal(SendFee);

            _DataService.CreateTransaction(SendAddress, amount, fee, (txResult) =>
            {
                Error = txResult;
            });
        }

        #endregion
    }
}
