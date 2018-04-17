using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Popflex.Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void PrintSalesOrder()
        {
            var order = AllfleXML.FlexOrder.Parser.Import(@"TestData\sample2.xml").OrderHeaders.SingleOrDefault();
            var tmp = Popflex.Print.SalesOrder(order);

            Assert.IsNotNull(tmp);
            System.Diagnostics.Process.Start(tmp);
        }
    }
}
