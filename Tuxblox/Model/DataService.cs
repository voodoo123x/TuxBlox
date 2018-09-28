using System;
using System.Collections.Generic;
using Tuxblox.Model.Entities;
using Tuxblox.Operations;

namespace Tuxblox.Model
{
    public class DataService : IDataService
    {
        public void GetAppTitle(Action<string, string, Exception> callback)
        {
            var appName = Constants.Application.Name;
            var appVersion = Constants.Application.Version;

            callback(appName, appVersion, null);
        }

        public void GetNodeStatus(Action<NodeStatusEntity> callback)
        {
            var nodeStatus = WalletManager.Get().Value("NodeStatus") as NodeStatusEntity;

            callback(nodeStatus);
        }

        public void GetBalance(Action<BalanceEntity> callback)
        {
            var balance = WalletManager.Get().Value("Balance") as BalanceEntity;

            callback(balance);
        }

        public void GetTransactions(Action<IEnumerable<TransactionEntity>> callback)
        {
            var transactions = WalletManager.Get().Value("Transactions") as IEnumerable<TransactionEntity>;

            callback(transactions);
        }

        public void GetAddresses(Action<IEnumerable<AddressEntity>> callback)
        {
            var addresses = WalletManager.Get().Value("Addresses") as IEnumerable<AddressEntity>;

            callback(addresses);
        }

        public void CreateTransaction(string address, decimal amount, decimal fee, Action<string> callback)
        {
            var txResult = NodeOperations.CreateTransaction(address, amount, fee);

            callback(txResult);
        }
    }
}