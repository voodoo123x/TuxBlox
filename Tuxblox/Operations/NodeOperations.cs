using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Tuxblox.Model.Entities;

namespace Tuxblox.Operations
{
    public static class NodeOperations
    {
        /// <summary>
        /// Get status and header info of local running node.
        /// </summary>
        /// <returns></returns>
        public static NodeStatusEntity GetNodeStatus()
        {
            var cliResult = MakeCliCall("getnetworkinfo");

            var connections = new int();

            try
            {
                connections = cliResult.connections;
            }
            catch
            {
                connections = 0;
            }

            cliResult = MakeCliCall("getblockchaininfo");

            var blockHeight = new int();
            var headers = new int();

            try
            {
                blockHeight = cliResult.blocks;
                headers = cliResult.headers;
            }
            catch
            {
                blockHeight = 0;
                headers = 0;
            }

            var percentageComplete = string.Format("Updating ({0}%)", headers != 0 ? Math.Floor(((decimal)blockHeight / headers) * 100).ToString() : "0");
            var status = (headers > 0 && headers <= blockHeight) ? "Up to date" : percentageComplete;

            return new NodeStatusEntity
            {
                Status = status,
                BlockHeight = blockHeight,
                Connections = connections
            };
        }

        /// <summary>
        /// Get balance of local wallet.
        /// </summary>
        /// <returns></returns>
        public static BalanceEntity GetWalletBalance()
        {
            var cliResult = MakeCliCall("getwalletinfo");

            var balance = new decimal();

            try
            {
                balance = cliResult.balance;
            }
            catch
            {
                balance = 0;
            }

            return new BalanceEntity
            {
                TotalBalance = balance
            };
        }

        /// <summary>
        /// Get list of transactions for local wallet.
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<TransactionEntity> GetTransactions()
        {
            IList<TransactionEntity> transactions = new List<TransactionEntity>();

            var accountResult = MakeCliCall("listaccounts");
            foreach (var account in accountResult)
            {
                var command = string.Format("listtransactions \"{0}\" 999999", account.Name);
                var txResult = MakeCliCall(command);

                foreach (var tx in txResult)
                {
                    string txid = tx.txid;

                    if (transactions.Any(t => string.Equals(t.TxId, txid, StringComparison.Ordinal)))
                    {
                        var existingTx = transactions.First(t => string.Equals(t.TxId, txid, StringComparison.Ordinal));
                        existingTx.Amount += Convert.ToDecimal(tx.amount);

                        TxCategory category = tx.category;
                        if (existingTx.Category != category)
                        {
                            existingTx.Category = TxCategory.Internal;
                        }
                    }
                    else
                    {
                        var newTx = new TransactionEntity
                        {
                            Category = tx.category,
                            Address = tx.address,
                            Amount = tx.amount,
                            Confirmations = tx.confirmations,
                            BlockHash = tx.blockhash,
                            TxId = tx.txid,
                            TimeReceived = tx.timereceived
                        };

                        if (newTx.Category == TxCategory.Send)
                        {
                            newTx.Amount += Convert.ToDecimal(tx.fee);
                        }

                        transactions.Add(newTx);
                    }
                }
            }

            return transactions;
        }

        /// <summary>
        /// Verifies that specified address is valid for this network.
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public static bool IsAddressValid(string address)
        {
            var command = string.Format("validateaddress \"{0}\"", address);
            var cliResult = MakeCliCall(command);

            return string.Equals(Convert.ToString(cliResult.isvalid), "true", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Sets fee and sends the specified amount to the specified address.
        /// </summary>
        /// <param name="address">Address to send amount to.</param>
        /// <param name="amount">Amount to send to address.</param>
        /// <param name="fee">Fee to be applied when sending.</param>
        /// <returns></returns>
        public static string CreateTransaction(string address, decimal amount, decimal fee)
        {
            string result = string.Empty;

            var command = string.Format("settxfee {0}", fee);
            var cliResult = MakeCliCall(command);

            var setTxSuccess = string.Equals(Convert.ToString(cliResult), "true", StringComparison.OrdinalIgnoreCase);
            var isAddressValue = IsAddressValid(address);
            
            if (setTxSuccess && isAddressValue)
            {
                command = string.Format("sendtoaddress \"{0}\" {1}", address, amount);
                cliResult = MakeCliCall(command);

                try
                {
                    result = cliResult;
                }
                catch
                {
                    result = cliResult.error;
                }
            }
            else
            {
                result = "Error setting tx fee.";
            }

            return result;
        }

        private static dynamic MakeCliCall(string command)
        {
            dynamic returnValue = null;

            Process cliProc = new Process();
            cliProc.StartInfo = new ProcessStartInfo
            {
                FileName = "Daemon\\tuxcoin-cli.exe",
                Arguments = command,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            };

            var returnString = string.Empty;

            cliProc.Start();
            while(!cliProc.StandardOutput.EndOfStream)
            {
                returnString += cliProc.StandardOutput.ReadLine();
            }

            if (returnString.StartsWith("{") || returnString.StartsWith("["))
            {
                returnValue = JsonConvert.DeserializeObject(returnString);
            }
            else
            {
                returnValue = returnString;
            }

            return returnValue;
        }
    }
}
