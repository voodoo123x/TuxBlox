using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using Tuxblox.Model;
using Tuxblox.Model.Entities;

namespace Tuxblox.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// See http://www.mvvmlight.net
    /// </para>
    /// </summary>
    public class TransactionsViewModel : ViewModelBase
    {
        private readonly IDataService _DataService;

        public IList<TransactionEntity> Transactions { get; set; }

        private string _TransactionViewHeaderText;

        /// <summary>
        /// Gets or sets the TransactionViewHeaderText property.
        /// </summary>
        public string TransactionViewHeaderText
        {
            get { return _TransactionViewHeaderText; }

            set
            {
                if (_TransactionViewHeaderText != value)
                {
                    Set(ref _TransactionViewHeaderText, value);
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the TransactionsViewModel class.
        /// </summary>
        public TransactionsViewModel(IDataService dataService)
        {
            Transactions = new List<TransactionEntity>();
            _DataService = dataService;

            WalletManager.Get().RegisterAction(WalletEvent.TransactionUpdated, () =>
            {
                var transactions = WalletManager.Get().Value("Transactions") as IEnumerable<TransactionEntity>;
                UpdateTransactionList(transactions);
            });
        }

        public override void Cleanup()
        {
            base.Cleanup();
        }

        private void UpdateTransactionList(IEnumerable<TransactionEntity> newTxs)
        {
            if (newTxs == null)
            {
                TransactionViewHeaderText = "No Transactions Found";
                return;
            }
            else if (newTxs.Any())
            {
                TransactionViewHeaderText = string.Empty;
            }

            var newTxAdded = false;

            foreach (var tx in newTxs)
            {
                if (!Transactions.Any(oldTx => string.Equals(oldTx.TxId, tx.TxId, StringComparison.Ordinal)))
                {
                    Transactions.Add(tx);
                    newTxAdded = true;
                }
                else 
                {
                    var existingTxs = Transactions.Where(oldTx => string.Equals(oldTx.TxId, tx.TxId, StringComparison.Ordinal));
                    foreach (var existingTx in existingTxs)
                    {
                        existingTx.Confirmations = tx.Confirmations;
                    }
                }
            }

            if (newTxAdded)
            {
                Transactions = Transactions.OrderByDescending(tx => tx.TimeReceived).ToList();
            }

            RaisePropertyChanged("Transactions");
        }
    }
}