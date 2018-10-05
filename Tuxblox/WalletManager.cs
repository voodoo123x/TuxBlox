using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Tuxblox.Model.Entities;
using Tuxblox.Operations;

namespace Tuxblox
{
    public class WalletManager
    {
        private const string DaemonName = "tuxcoind.exe";

        private static WalletManager _Instance;

        private IDictionary<string, object> _Values = new Dictionary<string, object>();
        private IList<Action> _AddressChangeActions = new List<Action>();
        private IList<Action> _BalanceChangeActions = new List<Action>();
        private IList<Action> _StatusChangeActions = new List<Action>();
        private IList<Action> _TransactionsChangeActions = new List<Action>();

        private IList<Thread> _RefreshThreads;
        private int _WaitTime;
        private bool _FirstLoadComplete;
        private Process _DaemonProc;

        /// <summary>
        /// Gets the instance of WalletManager.
        /// </summary>
        /// <returns></returns>
        public static WalletManager Get()
        {
            if (_Instance == null)
            {
                _Instance = new WalletManager();
            }

            return _Instance;
        }

        /// <summary>
        /// Starts local daemon and begins loop getting wallet information.
        /// </summary>
        /// <param name="intervalInMilliseconds">Interval between queries to wallet daemon for updated information.</param>
        public void Start(int intervalInMilliseconds)
        {
            _RefreshThreads = new List<Thread>();

            if (StartDaemon())
            {
                UpdateAll();

                _RefreshThreads.Add(new Thread(new ThreadStart(() => Refresh(UpdateAddresses))));
                _RefreshThreads.Add(new Thread(new ThreadStart(() => Refresh(UpdateStatus))));
                _RefreshThreads.Add(new Thread(new ThreadStart(() => Refresh(UpdateTransactionBalance))));
                _WaitTime = intervalInMilliseconds;

                foreach (var thread in _RefreshThreads)
                {
                    thread.IsBackground = true;
                    thread.Start();
                }
            }
            else
            {
                _FirstLoadComplete = true;
                _Values["NodeStatus"] = new NodeStatusEntity
                {
                    Status = "Failed to start"
                };
            }
        }

        /// <summary>
        /// Stops daemon and all refresh threads.
        /// </summary>
        public void Stop()
        {
            if (_DaemonProc != null && !_DaemonProc.HasExited)
            {
                _DaemonProc?.Kill();
            }

            foreach (var thread in _RefreshThreads)
            {
                thread.Abort();
            }
        }

        /// <summary>
        /// Returns the value of the property name provided.
        /// </summary>
        /// <param name="name">Name of the property to return value for.</param>
        /// <returns></returns>
        public object Value(string name)
        {
            object returnValue = null;

            if (_FirstLoadComplete && _Values.ContainsKey(name))
            {
                returnValue = _Values[name];
            }

            return returnValue;
        }

        /// <summary>
        /// Register an action that will be fired when a specified event occurs.
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="action"></param>
        public void RegisterAction(WalletEvent eventType, Action action)
        {
            switch (eventType)
            {
                case WalletEvent.AddressUpdated:
                    _AddressChangeActions.Add(action);
                    break;

                case WalletEvent.BalanceUpdated:
                    _BalanceChangeActions.Add(action);
                    break;

                case WalletEvent.StatusUpdated:
                    _StatusChangeActions.Add(action);
                    break;

                case WalletEvent.TransactionUpdated:
                    _TransactionsChangeActions.Add(action);
                    break;
            }

            ForceUpdate(eventType);
        }

        #region Private Methods

        private bool StartDaemon()
        {
            var foundProcesses = Process.GetProcessesByName(DaemonName);
            var isRunning = foundProcesses.Length > 0;

            if (!isRunning)
            {
                _DaemonProc = new Process();
                _DaemonProc.StartInfo = new ProcessStartInfo
                {
                    FileName = "Daemon\\" + DaemonName,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                _DaemonProc.Start();
                Thread.Sleep(1000);

                isRunning = Process.GetProcessesByName(DaemonName).Length > 0;
            }
            else
            {
                _DaemonProc = foundProcesses[0];
            }

            return true;
        }

        private void Refresh(Action refreshAction)
        {
            while (true)
            {
                refreshAction.Invoke();
                Thread.Sleep(_WaitTime);
            }
        }

        private void UpdateAll()
        {
            UpdateTransactionBalance();
            UpdateAddresses();
            UpdateStatus();

            _FirstLoadComplete = true;

            FireActions(_BalanceChangeActions);
            FireActions(_TransactionsChangeActions);
            FireActions(_AddressChangeActions);
            FireActions(_StatusChangeActions);
        }

        private void UpdateTransactionBalance()
        {
            var balance = NodeOperations.GetWalletBalance();
            _Values["Balance"] = balance;

            var transactions = NodeOperations.GetTransactions();
            _Values["Transactions"] = transactions;

            FireActions(_BalanceChangeActions);
            FireActions(_TransactionsChangeActions);
        }

        private void UpdateAddresses()
        {
            var addresses = NodeOperations.GetAddresses();
            _Values["Addresses"] = addresses;
            FireActions(_AddressChangeActions);
        }

        private void UpdateStatus()
        {
            var nodeStatus = NodeOperations.GetNodeStatus();
            _Values["NodeStatus"] = nodeStatus;
            FireActions(_StatusChangeActions);
        }

        private void ForceUpdate(WalletEvent eventType)
        {
            switch (eventType)
            {
                case WalletEvent.AddressUpdated:
                    FireActions(_AddressChangeActions);
                    break;

                case WalletEvent.BalanceUpdated:
                    FireActions(_BalanceChangeActions);
                    break;

                case WalletEvent.StatusUpdated:
                    FireActions(_StatusChangeActions);
                    break;

                case WalletEvent.TransactionUpdated:
                    FireActions(_TransactionsChangeActions);
                    break;
            }
        }

        private void FireActions(IEnumerable<Action> actions)
        {
            if (!_FirstLoadComplete)
            {
                return;
            }

            foreach (var action in actions)
            {
                var actionThread = new Thread(new ThreadStart(action));
                actionThread.IsBackground = true;
                actionThread.Start();
            }
        }

        #endregion
    }

    public enum WalletEvent
    {
        AddressUpdated,
        BalanceUpdated,
        StatusUpdated,
        TransactionUpdated
    }
}
