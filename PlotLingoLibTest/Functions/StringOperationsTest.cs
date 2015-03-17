using Microsoft.VisualStudio.TestTools.UnitTesting;
using PlotLingoLib;
using PlotLingoLib.Expressions;
using PlotLingoLib.Expressions.Values;

namespace PlotLingoLibTest.Functions
{
    [TestClass]
    public class StringOperationsTest
    {
        [TestMethod]
        public void SimpleStringPlus()
        {
            var cl = new FunctionExpression("+", new StringValue("hi"), new StringValue(" there"));
            var c = new RootContext();
            var r = cl.Evaluate(c);
            Assert.IsInstanceOfType(r, typeof(string), "type of result");
            var rut = r as string;
            Assert.AreEqual("hi there", rut);
        }
    }
}
