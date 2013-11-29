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

        [TestMethod]
        public void TestFunctionCallback()
        {
            var fo = new FunctionExpression("GetMe", new IExpression[] { new StringValue("hi there") });
            var c = new Context();
            int count = 0;
            c.AddPostCallHook("GetMe", "test", (shouldbenull, result) =>
            {
                Assert.IsNull(shouldbenull, "function call should be null obj");
                Assert.AreEqual(8, result, "Result in call back");
                count++;
                return result;
            });
            var r = fo.Evaluate(c);
            Assert.AreEqual(1, count, "# of times the callback was called");
        }

        [TestMethod]
        public void TestFunctionCallWithContextNoArg()
        {
            var fo = new FunctionExpression("GetMeContext", new IExpression[] { });
            var c = new Context();
            var r = fo.Evaluate(c);
            Assert.AreEqual(12, r, "function result");
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

        static public int GetMeContext (Context c)
        {
            if (c == null)
                return 0;
            return 12;
        }
    }
}
