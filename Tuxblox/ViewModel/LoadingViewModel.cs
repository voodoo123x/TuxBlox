using GalaSoft.MvvmLight;
using System.Threading;
using Tuxblox.Model;

namespace Tuxblox.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// See http://www.mvvmlight.net
    /// </para>
    /// </summary>
    public class LoadingViewModel : ViewModelBase
    {
        private const string UpdatingText = "Updating";

        private IDataService _DataService;
        private string _UpdatingString;
        private int _UpdatingCount;

        /// <summary>
        /// Get and sets the value of LoadingText property.
        /// </summary>
        public string UpdatingString
        {
            get { return _UpdatingString; }

            set
            {
                if (_UpdatingString != value)
                {
                    Set(ref _UpdatingString, value);
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the LoadingViewModel class.
        /// </summary>
        public LoadingViewModel(IDataService dataService)
        {
            _DataService = dataService;
            UpdatingString = UpdatingText;

            var animateThread = new Thread(new ThreadStart(AnimateLoadingText));
            animateThread.IsBackground = true;
            animateThread.Start();
        }

        /// <summary>
        /// Cleanup view model.
        /// </summary>
        public override void Cleanup()
        {
            base.Cleanup();
        }

        private void AnimateLoadingText()
        {
            _UpdatingCount = 0;

            while (true)
            {
                if (_UpdatingCount > 3)
                {
                    _UpdatingCount = 0;
                }

                var localUpdatingString = UpdatingText;

                for (int i = 0; i < _UpdatingCount; i++)
                {
                    localUpdatingString += " .";
                }

                UpdatingString = localUpdatingString;
                _UpdatingCount++;

                Thread.Sleep(1000);
            }
        }
    }
}