using Microsoft.VisualStudio.TestTools.UnitTesting;
using PlotLingoLib;
using PlotLingoLib.Expressions;
using PlotLingoLib.Expressions.Values;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PlotLingoLibTest.Functions
{
    [TestClass]
    public class ListOperationsTest
    {
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void SumEmptyObjectArray()
        {
            var ar = new object[] { };
            var f = new FunctionExpression("sum", new ObjectValue(ar));

            var c = new RootContext();
            var r = f.Evaluate(c);
        }

        [TestMethod]
        public void SumSingleObjectArray()
        {
            var ar = new object[] { 5 };
            var f = new FunctionExpression("sum", new ObjectValue(ar));

            var c = new RootContext();
            var r = f.Evaluate(c);

            Assert.AreEqual(5, r);
        }

        [TestMethod]
        public void SumDoubleObjectArray()
        {
            var ar = new object[] { 5, 6 };
            var f = new FunctionExpression("sum", new ObjectValue(ar));

            var c = new RootContext();
            var r = f.Evaluate(c);

            Assert.AreEqual(11, r);
        }

        [TestMethod]
        public void DivideObjectArray()
        {
            var ar = new object[] { 4.0, 6.0 };
            var f = new FunctionExpression("/", new ObjectValue(ar), new DoubleValue(2.0));
            var c = new RootContext();
            var r = f.Evaluate(c);

            Assert.IsInstanceOfType(r, typeof(IEnumerable<object>));
            var lst = r as IEnumerable<object>;
            var list = lst.ToArray();

            Assert.AreEqual(2, list.Length, "# of items");
            Assert.AreEqual(2.0, list[0]);
            Assert.AreEqual(3.0, list[1]);
        }
    }
}
