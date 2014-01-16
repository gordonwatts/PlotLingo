using Microsoft.VisualStudio.TestTools.UnitTesting;
using PlotLingoLib;
using PlotLingoLib.Expressions.Values;
using System;

namespace PlotLingoLibTest
{
    [TestClass]
    public class RootContextTest
    {
        [TestMethod]
        public void TestGetSet()
        {
            var c = new RootContext();
            c.SetVariableValue("p", 5);
            Assert.AreEqual(5, c.GetVariableValue("p"), "get p");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestBadGet()
        {
            var c = new RootContext();
            c.GetVariableValue("p");
        }

        [TestMethod]
        public void RegisterExprEvalWorks()
        {
            var c = new RootContext();
            object r = null;
            Action<object> saver = o => r = o;
            var es = new PlotLingoLib.Statements.ExpressionStatement(new StringValue("hi"));

            c.AddExpressionStatementEvaluationCallback(saver);
            es.Evaluate(c);
            Assert.AreEqual("hi", r, "result of running");
        }

        [TestMethod]
        public void RegisterAndRemoveExprEvalWorks()
        {
            var c = new RootContext();
            object r = null;
            Action<object> saver = o => r = o;
            var es = new PlotLingoLib.Statements.ExpressionStatement(new StringValue("hi"));

            c.AddExpressionStatementEvaluationCallback(saver);
            c.RemoveExpressionStatementEvaluationCallback(saver);
            es.Evaluate(c);
            Assert.IsNull(r, "result of running");
        }
    }
}
