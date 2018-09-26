using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Tuxblox.Operations;

namespace Tuxblox.Tests
{
    [TestClass]
    public class NodeOperationsTests
    {
        [TestMethod]
        public void TestGetNetworkInfo()
        {
            var result = NodeOperations.GetNodeStatus();
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void TestGetWalletBalance()
        {
            var result = NodeOperations.GetWalletBalance();
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void TestGetTransactions()
        {
            var result = NodeOperations.GetTransactions();
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void TestIsAddressValid()
        {
            var result = NodeOperations.IsAddressValid("GaCiQorXTGTCLCqgQv8KiBgWPpe85VrErq");
            Assert.IsFalse(result);

            result = NodeOperations.IsAddressValid("TJhH9bV2agSEzxAGaarqpoEXzBFvcFNMAz");
            Assert.IsTrue(result);
        }
    }
}
