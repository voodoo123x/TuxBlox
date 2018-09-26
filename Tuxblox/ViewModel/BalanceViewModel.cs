using GalaSoft.MvvmLight;
using Tuxblox.Model;

namespace Tuxblox.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// See http://www.mvvmlight.net
    /// </para>
    /// </summary>
    public class BalanceViewModel : ViewModelBase
    {
        private readonly IDataService _DataService;
        private readonly RefreshWorker _RefreshWorker;

        private decimal _TotalBalance;

        /// <summary>
        /// Gets and set the value of TotalBalance property.
        /// </summary>
        public decimal TotalBalance
        {
            get { return _TotalBalance; }
            set
            {
                if (_TotalBalance != value)
                {
                    Set(ref _TotalBalance, value);
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the BalanceViewModel class.
        /// </summary>
        public BalanceViewModel(IDataService dataService)
        {
            _DataService = dataService;

            _RefreshWorker = new RefreshWorker(50, () =>
            {
                _DataService.GetBalance((balance) =>
                {
                    var totalBalanceString = string.Format("{0:0.0000}", balance?.TotalBalance ?? 0);
                    decimal.TryParse(totalBalanceString, out decimal totalBalance);
                    TotalBalance = totalBalance;
                });
            });
        }

        public override void Cleanup()
        {
            _RefreshWorker.Stop();

            base.Cleanup();
        }
    }
}