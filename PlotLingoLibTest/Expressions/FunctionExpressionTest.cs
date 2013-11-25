using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel.Composition;
using PlotLingoLib;
using PlotLingoLib.Expressions;
using PlotLingoLib.Expressions.Values;

namespace PlotLingoLibTest.Expressions
{
    [TestClass]
    public class FunctionExpressionTest
    {
        [TestMethod]
        public void TestBasicCalling()
        {
            var fo = new FunctionExpression("GetMe", new IExpression[] { new StringValue("hi there") });
            var c = new Context();
            var r = fo.Evaluate(c);
            Assert.AreEqual(8, r, "function result");
        }
    }

    /// <summary>
    /// Some test objects for the function.
    /// </summary>
    [Export(typeof(IFunctionObject))]
    public class FunctionObjectTests : IFunctionObject
    {
        static public int GetMe (string arg)
        {
            return arg.Length;
        }
    }
}
