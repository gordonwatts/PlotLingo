using Microsoft.VisualStudio.TestTools.UnitTesting;
using PlotLingoLib;
using PlotLingoLib.Expressions;
using PlotLingoLib.Expressions.Values;

namespace PlotLingoLibTest.Functions
{
    [TestClass]
    public class ListOperationsTest
    {
        [TestMethod]
        public void SumEmptyObjectArray()
        {
            var ar = new object[] { };
            var f = new FunctionExpression("sum", new ObjectValue(ar));

            var c = new RootContext();
            var r = f.Evaluate(c);

            Assert.IsNull(r);
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
    }
}
