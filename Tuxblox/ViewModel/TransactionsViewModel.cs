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
        private readonly RefreshWorker _RefreshWorker;

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

            _RefreshWorker = new RefreshWorker(50, () =>
            {
                _DataService.GetTransactions((transactions) =>
                {
                    UpdateTransactionList(transactions);
                });
            });
        }

        public override void Cleanup()
        {
            _RefreshWorker.Stop();

            base.Cleanup();
        }

        private void UpdateTransactionList(IEnumerable<TransactionEntity> newTxs)
        {
            if (newTxs.Any())
            {
                TransactionViewHeaderText = string.Empty;
            }
            else
            {
                TransactionViewHeaderText = "No Transactions Found";
            }

            if (newTxs == null)
            {
                return;
            }

            var notifyTxChange = false;

            foreach (var tx in newTxs)
            {
                if (!Transactions.Any(oldTx => string.Equals(oldTx.TxId, tx.TxId, StringComparison.Ordinal)))
                {
                    Transactions.Add(tx);
                    notifyTxChange = true;
                }
                else 
                {
                    var existingTx = Transactions.First(oldTx => string.Equals(oldTx.TxId, tx.TxId, StringComparison.Ordinal));

                    notifyTxChange = existingTx.Confirmations <= 0 && tx.Confirmations > 0;
                    existingTx.Confirmations = tx.Confirmations;
                }
            }

            if (notifyTxChange)
            {
                Transactions = Transactions.OrderByDescending(tx => tx.TimeReceived).ToList();
                RaisePropertyChanged("Transactions");
            }
        }
    }
}