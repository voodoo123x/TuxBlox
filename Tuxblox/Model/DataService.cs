using System;
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

        public void GetPrivateKey(string address, Action<string> callback)
        {
            var privateKey = NodeOperations.GetPrivateKey(address);

            callback(privateKey);
        }

        public void CreateTransaction(string address, decimal amount, decimal fee, Action<string> callback)
        {
            var txResult = NodeOperations.CreateTransaction(address, amount, fee);

            callback(txResult);
        }
    }
}