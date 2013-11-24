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
            var mc = new MethodCallExpression(new VariableValue("p"), new FunctionExpression("CallNoArgsBogus", new IExpression[] { }));
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
            var mc = new MethodCallExpression(new VariableValue("p"), new FunctionExpression("CallNoArgs", new IExpression[] { }));
            var r = mc.Evaluate(ctx);
            Assert.AreEqual(5, r);
        }

        [TestMethod]
        public void TestCallOneArg()
        {
            var ctx = new Context();
            ctx.SetVariableValue("p", new testClass());
            var s = new StringValue("length");
            var mc = new MethodCallExpression(new VariableValue("p"), new FunctionExpression("CallOneStringArg", new IExpression[] { s }));
            var r = mc.Evaluate(ctx);
            Assert.AreEqual(6, r);
        }

        /// <summary>
        /// Make sure that arguments to an expression are evaluated only once.
        /// </summary>
        [TestMethod]
        public void TestArgumentsEvaluatedOnlyOnce()
        {
            var ctx = new Context();
            ctx.SetVariableValue("p", new testClass());
            var s = new MyStringExpression();
            var mc = new MethodCallExpression(new VariableValue("p"), new FunctionExpression("CallOneStringArg", new IExpression[] { s }));
            var r = mc.Evaluate(ctx);
            Assert.AreEqual(1, s._evalCount, "# of times evaluated");
        }
        
        /// <summary>
        /// Small expression class that will hold onto a string and count the number of times
        /// it is evaluated.
        /// </summary>
        private class MyStringExpression : IExpression
        {
            public int _evalCount = 0;
            public object Evaluate(Context c)
            {
                _evalCount++;
                return "Length";
            }
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
