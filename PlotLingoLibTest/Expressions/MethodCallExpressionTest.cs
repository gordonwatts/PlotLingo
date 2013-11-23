using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PlotLingoLib;
using PlotLingoLib.Expressions;
using PlotLingoLib.Expressions.Values;

namespace PlotLingoLibTest.Expressions
{
    [TestClass]
    public class MethodCallExpressionTest
    {

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestBadCall()
        {
            var ctx = new Context();
            ctx.SetVariableValue("p", new testClass());
            var mc = new MethodCallExpression("p", new FunctionExpression("CallNoArgsBogus", new IExpression[] { }));
            var r = mc.Evaluate(ctx);
            Assert.AreEqual(5, r);
        }

        /// <summary>
        /// Simple call with nothing else going on.
        /// </summary>
        [TestMethod]
        public void TestCallNoArgs()
        {
            var ctx = new Context();
            ctx.SetVariableValue("p", new testClass());
            var mc = new MethodCallExpression("p", new FunctionExpression("CallNoArgs", new IExpression[] { }));
            var r = mc.Evaluate(ctx);
            Assert.AreEqual(5, r);
        }

        [TestMethod]
        public void TestCallOneArg()
        {
            var ctx = new Context();
            ctx.SetVariableValue("p", new testClass());
            var s = new StringValue("length");
            var mc = new MethodCallExpression("p", new FunctionExpression("CallOneStringArg", new IExpression[] { s }));
            var r = mc.Evaluate(ctx);
            Assert.AreEqual(6, r);
        }

        /// <summary>
        /// Make sure that arguments to an expression are evaluated only once.
        /// </summary>
        [TestMethod]
        public void TestArgumentsEvaluatedOnlyOnce()
        {
            Assert.Inconclusive();
        }

        /// <summary>
        /// Test class to help with... testing.
        /// </summary>
        private class testClass
        {
            public int CallNoArgs()
            {
                return 5;
            }

            public int CallOneStringArg (string hi)
            {
                return hi.Length;
            }
        }
    }
}
