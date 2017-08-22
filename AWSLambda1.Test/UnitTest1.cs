using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AWSLambda1.Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            Function function = new Function();

            var res = function.GetCurrentBillHandler(null, String.Empty);
             
            Assert.IsNotNull(res);
        }
    }
}
