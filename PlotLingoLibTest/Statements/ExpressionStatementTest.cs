﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using PlotLingoLib;
using PlotLingoLib.Expressions;
using PlotLingoLib.Statements;

namespace PlotLingoLibTest.Statements
{
    [TestClass]
    public class ExpressionStatementTest
    {
        [TestMethod]
        public void TestBasicExecution()
        {
            var exp = new simpleExprForTest();
            var c = new RootContext();
            var expS = new ExpressionStatement(exp);

            expS.Evaluate(c);

            Assert.AreEqual(1, exp.NumberOfTimesCalled, "# of times the expression was evaluated");
        }

        [TestMethod]
        public void TestEvaluationCallback()
        {
            var exp = new simpleExprForTest();
            int count = 0;
            var c = new RootContext();
            c.AddExpressionStatementEvaluationCallback(o =>
            {
                count++;
                Assert.IsNull(o, "callback object");
            });
            var expS = new ExpressionStatement(exp);
            expS.Evaluate(c);

            Assert.AreEqual(1, count, "# of times callback was called");
        }

        /// <summary>
        /// Test expression that counts.
        /// </summary>
        class simpleExprForTest : IExpression
        {
            public simpleExprForTest()
            {
                NumberOfTimesCalled = 0;
            }

            /// <summary>
            /// Dummy evaluator.
            /// </summary>
            /// <param name="c"></param>
            /// <returns></returns>
            public object Evaluate(IScopeContext c)
            {
                NumberOfTimesCalled++;
                return null;
            }

            public int NumberOfTimesCalled { get; set; }
        }

    }
}
