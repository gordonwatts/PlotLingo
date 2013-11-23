using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PlotLingoLib;

namespace PlotLingoLibTest
{
    [TestClass]
    public class ContextTest
    {
        [TestMethod]
        public void TestGetSet()
        {
            var c = new Context();
            c.SetVariableValue("p", 5);
            Assert.AreEqual(5, c.GetVariableValue("p"), "get p");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestBadGet()
        {
            var c = new Context();
            c.GetVariableValue("p");
        }
    }
}
