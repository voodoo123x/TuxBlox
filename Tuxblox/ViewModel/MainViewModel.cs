using GalaSoft.MvvmLight;
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
    public class MainViewModel : ViewModelBase
    {
        private readonly IDataService _DataService;

        private string _TuxBloxTitle = string.Empty;
        private string _NodeStatus;
        private int _BlockHeight;
        private int _Connections;

        /// <summary>
        /// Gets and sets the TuxBloxTitle property.
        /// </summary>
        public string TuxBloxTitle
        {
            get { return _TuxBloxTitle; }
            set { Set(ref _TuxBloxTitle, value); }
        }

        /// <summary>
        /// Gets and sets the NodeStatus property.
        /// </summary>
        public string NodeStatus
        {
            get { return _NodeStatus; }
            set
            {
                if (_NodeStatus != value)
                {
                    Set(ref _NodeStatus, value);
                }
            }
        }

        /// <summary>
        /// Gets and sets the BlockHeight property.
        /// </summary>
        public int BlockHeight
        {
            get { return _BlockHeight; }
            set
            {
                if (_BlockHeight != value)
                {
                    Set(ref _BlockHeight, value);
                }
            }
        }

        /// <summary>
        /// Gets and sets the Connections property.
        /// </summary>
        public int Connections
        {
            get { return _Connections; }
            set
            {
                if (_Connections != value)
                {
                    Set(ref _Connections, value);
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel(IDataService dataService)
        {
            _DataService = dataService;
            _DataService.GetAppTitle((name, version, error) =>
            {
                if (error != null)
                {
                    // Report error here
                    return;
                }

                TuxBloxTitle = string.Format($"{name} {version}");
            });

            WalletManager.Get().RegisterAction(WalletEvent.StatusUpdated, () =>
            {
                var nodeStatus = WalletManager.Get().Value("NodeStatus") as NodeStatusEntity;
                NodeStatus = nodeStatus?.Status;
                BlockHeight = nodeStatus?.BlockHeight ?? 0;
                Connections = nodeStatus?.Connections ?? 0;
            });
        }

        public override void Cleanup()
        {
            base.Cleanup();
        }
    }
}