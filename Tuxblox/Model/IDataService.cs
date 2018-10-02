using System;
using System.Collections.Generic;
using Tuxblox.Model.Entities;

namespace Tuxblox.Model
{
    public interface IDataService
    {
        void GetAppTitle(Action<string, string, Exception> callback);
        void GetNodeStatus(Action<NodeStatusEntity> callback);
        void GetBalance(Action<BalanceEntity> callback);
        void GetTransactions(Action<IEnumerable<TransactionEntity>> callback);
        void GetAddresses(Action<IEnumerable<AddressEntity>> callback);
        void GetPrivateKey(string address, Action<string> callback);
        void CreateTransaction(string address, decimal amount, decimal fee, Action<string> callback);
    }
}
