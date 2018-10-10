using System;

namespace Tuxblox.Model.Entities
{
    public class TransactionEntity : BaseEntity
    {
        private const int MinedMaturity = 101;
        private const int TransactionMaturity = 1;

        private ulong _Confirmations = 0;

        public TxCategory Category { get; set; }
        public string Address { get; set; }
        public decimal Amount { get; set; }
        public string BlockHash { get; set; }
        public string TxId { get; set; }
        public ulong TimeReceived { get; set; }

        public ulong Confirmations
        {
            get { return _Confirmations; }

            set
            {
                SetValue(ref _Confirmations, value);
                NotifyPropertyChanged("IsPending");
            }
        }

        /// <summary>
        /// Gets the TransactionSummary.
        /// </summary>
        public string TransactionSummary
        {
            get
            {
                string summary = string.Empty;

                switch (Category)
                {
                    case TxCategory.Internal:
                        summary = "Internal transaction";
                        break;

                    case TxCategory.Receive:
                        summary = "Received by " + Address;
                        break;

                    case TxCategory.Send:
                        summary = "Sent to " + Address;
                        break;

                    case TxCategory.Generate:
                    case TxCategory.Immature:
                        summary = "Mined by " + Address;
                        break;

                    default:
                        summary = "Undefined transaction";
                        break;
                }

                return summary;
            }
        }

        /// <summary>
        /// Get the TransactionDate.
        /// </summary>
        public DateTime TransactionDate
        {
            get { return (new DateTime(1970, 1, 1)).AddSeconds(Convert.ToDouble(TimeReceived)).ToLocalTime(); }
        }

        /// <summary>
        /// Get the IsPending.
        /// </summary>
        public bool IsPending
        {
            get
            {
                var isPending = false;

                switch (Category)
                {
                    case TxCategory.Generate:
                    case TxCategory.Immature:
                        isPending = Confirmations < MinedMaturity;
                        break;

                    default:
                        isPending = Confirmations < TransactionMaturity;
                        break;
                }

                return isPending;
            }
        }
    }

    public enum TxCategory
    {
        Generate,
        Immature,
        Internal,
        Orphan,
        Receive,
        Send,
        Undefined
    }
}
