using System;

namespace Tuxblox.Model
{
    public interface IDataService
    {
        void GetAppTitle(Action<string, string, Exception> callback);
        void GetPrivateKey(string address, Action<string> callback);
        void CreateTransaction(string address, decimal amount, decimal fee, Action<string> callback);
    }
}
