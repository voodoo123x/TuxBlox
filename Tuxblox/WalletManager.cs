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

            _RefreshThreads.Add(new Thread(new ThreadStart(() => Refresh(UpdateWalletBalance))));
            _RefreshThreads.Add(new Thread(new ThreadStart(() => Refresh(UpdateTransactions))));
            _RefreshThreads.Add(new Thread(new ThreadStart(() => Refresh(UpdateStatus))));
            _WaitTime = intervalInMilliseconds;

            if (StartDaemon())
            {
                FirstRun();

                foreach (var thread in _RefreshThreads)
                {
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

        private void FirstRun()
        {
            UpdateWalletBalance();
            UpdateTransactions();
            UpdateStatus();
            _FirstLoadComplete = true;
        }

        private void Refresh(Action refreshAction)
        {
            while (true)
            {
                refreshAction.Invoke();
                Thread.Sleep(_WaitTime);
            }
        }

        private void UpdateWalletBalance()
        {
            var balance = NodeOperations.GetWalletBalance();
            _Values["Balance"] = balance;
        }

        private void UpdateTransactions()
        {
            var transactions = NodeOperations.GetTransactions();
            _Values["Transactions"] = transactions;
        }

        private void UpdateStatus()
        {
            var nodeStatus = NodeOperations.GetNodeStatus();
            _Values["NodeStatus"] = nodeStatus;
        }
    }
}
