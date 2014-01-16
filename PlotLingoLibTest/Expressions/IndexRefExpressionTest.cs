using Microsoft.VisualStudio.TestTools.UnitTesting;
using PlotLingoLib;
using PlotLingoLib.Expressions;
using PlotLingoLib.Expressions.Values;
using System.Collections.Generic;

namespace PlotLingoLibTest.Expressions
{
    [TestClass]
    public class IndexRefExpressionTest
    {
        [TestMethod]
        public void TestSimpleIndex()
        {
            var dict = new Dictionary<string, int>();
            dict["hi"] = 5;
            var ir = new IndexerRefExpression(new ObjectValue(dict), new StringValue("hi"));
            var c = new RootContext();
            var r = ir.Evaluate(c);
            Assert.IsNotNull(r, "result");
            Assert.AreEqual(5, r, "the final result");
        }

        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public void TestSimpleIndexForMissingGuy()
        {
            var dict = new Dictionary<string, int>();
            dict["hi"] = 5;
            var ir = new IndexerRefExpression(new ObjectValue(dict), new StringValue("hioi"));
            var c = new RootContext();
            var r = ir.Evaluate(c);
        }

        [TestMethod]
        public void TestToString()
        {
            var dict = new Dictionary<string, int>();
            dict["hi"] = 5;
            var ir = new IndexerRefExpression(new ObjectValue(dict), new StringValue("hi"));
            Assert.AreEqual("PlotLingoLib.Expressions.Values.ObjectValue[\"hi\"]", ir.ToString(), "ToString");
        }
    }
}
