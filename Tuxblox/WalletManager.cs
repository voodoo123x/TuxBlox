using System;
using System.Collections.Generic;
using System.Threading;
using Tuxblox.Operations;

namespace Tuxblox
{
    public class WalletManager
    {
        private static WalletManager _Instance;

        private IDictionary<string, object> _Values = new Dictionary<string, object>();
        private IList<Thread> _RefreshThreads;
        private int _WaitTime;
        private bool _FirstLoadComplete;

        public static WalletManager Get()
        {
            if (_Instance == null)
            {
                _Instance = new WalletManager();
            }

            return _Instance;
        }

        public void Start(int intervalInMilliseconds)
        {
            _RefreshThreads = new List<Thread>();

            _RefreshThreads.Add(new Thread(new ThreadStart(() => Refresh(UpdateWalletBalance))));
            _RefreshThreads.Add(new Thread(new ThreadStart(() => Refresh(UpdateTransactions))));
            _RefreshThreads.Add(new Thread(new ThreadStart(() => Refresh(UpdateStatus))));
            _WaitTime = intervalInMilliseconds;

            FirstRun();

            foreach (var thread in _RefreshThreads)
            {
                thread.Start();
            }
        }

        public void Stop()
        {
            foreach (var thread in _RefreshThreads)
            {
                thread.Abort();
            }
        }

        public object Value(string name, bool useLock = true)
        {
            object returnValue = null;

            if (_FirstLoadComplete && _Values.ContainsKey(name))
            {
                returnValue = _Values[name];
            }

            return returnValue;
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
