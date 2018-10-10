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
        private bool _IsNodeUpdated;

        /// <summary>
        /// Gets the NodeStatus property.
        /// </summary>
        public NodeStatusEntity NodeStatus { get; } = new NodeStatusEntity();

        /// <summary>
        /// Gets and sets the TuxBloxTitle property.
        /// </summary>
        public string TuxBloxTitle
        {
            get { return _TuxBloxTitle; }

            set
            {
                if (_TuxBloxTitle != value)
                {
                    Set(ref _TuxBloxTitle, value);
                }
            }
        }

        /// <summary>
        /// Gets and sets the IsNodeUpdated property.
        /// </summary>
        public bool IsNodeUpdated
        {
            get { return _IsNodeUpdated; }

            set
            {
                if (_IsNodeUpdated != value)
                {
                    Set(ref _IsNodeUpdated, value);
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
                NodeStatus.Status = nodeStatus.Status;
                NodeStatus.BlockHeight = nodeStatus.BlockHeight;
                NodeStatus.Headers = nodeStatus.Headers;
                NodeStatus.Connections = nodeStatus.Connections;

                IsNodeUpdated = NodeStatus.Connections > 0 && NodeStatus.BlockHeight >= NodeStatus.Headers;
            });
        }

        public override void Cleanup()
        {
            base.Cleanup();
        }
    }
}