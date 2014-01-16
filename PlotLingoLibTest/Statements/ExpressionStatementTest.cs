using Microsoft.VisualStudio.TestTools.UnitTesting;
using PlotLingoLib;
using PlotLingoLib.Expressions;
using PlotLingoLib.Statements;
using System.Linq;

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

        /// <summary>
        /// If the expression returns an IEnumerable, then we need to make sure we evaluate it
        /// otherwise it may never be used!
        /// </summary>
        [TestMethod]
        public void TestIEnumerableResult()
        {
            var exp = new IEnumExprForTest();
            var c = new RootContext();
            var expS = new ExpressionStatement(exp);

            expS.Evaluate(c);

            Assert.AreEqual(4, exp.NumberOfTimesCalled, "# of times the expression was evaluated");
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

        /// <summary>
        /// Test expression that counts.
        /// </summary>
        class IEnumExprForTest : IExpression
        {
            public IEnumExprForTest()
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
                return new int[] { 1, 2, 3, 4 }
                    .Select(i =>
                    {
                        NumberOfTimesCalled++;
                        return i * 10;
                    })
                    .Cast<object>();
            }

            public int NumberOfTimesCalled { get; set; }
        }
    }
}
