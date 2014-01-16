using Microsoft.VisualStudio.TestTools.UnitTesting;
using PlotLingoLib;
using PlotLingoLib.Expressions.Values;
using System;

namespace PlotLingoLibTest
{
    [TestClass]
    public class ScopeContextTest
    {
        [TestMethod]
        public void TestGetSetNotAffectParent()
        {
            var c = new ScopeContext(new RootContext());
            c.SetVariableValue("p", 5);
            Assert.AreEqual(5, c.GetVariableValue("p"), "get p");
            Assert.IsFalse(c.Parent.GetVariableValueOrNull("p").Item1, "parent has the variable");
        }

        [TestMethod]
        public void TestGetSetAffectsParent()
        {
            var c = new ScopeContext(new RootContext());
            c.Parent.SetVariableValue("p", 0);
            c.SetVariableValue("p", 5);
            Assert.AreEqual(5, c.GetVariableValue("p"), "get p");
            Assert.IsTrue(c.Parent.GetVariableValueOrNull("p").Item1, "parent has the variable");
            Assert.AreEqual(5, c.Parent.GetVariableValueOrNull("p").Item2, "parent variable");
        }

        [TestMethod]
        public void TestGetFromParent()
        {
            var c = new ScopeContext(new RootContext());
            c.Parent.SetVariableValue("p", 5);
            Assert.AreEqual(5, c.GetVariableValue("p"), "get p");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestBadGet()
        {
            var c = new ScopeContext(new RootContext());
            c.GetVariableValue("p");
        }

        [TestMethod]
        public void RegisterExprEvalWorksInScope()
        {
            var c = new ScopeContext(new RootContext());
            object r = null;
            Action<object> saver = o => r = o;
            var es = new PlotLingoLib.Statements.ExpressionStatement(new StringValue("hi"));

            c.AddExpressionStatementEvaluationCallback(saver);
            es.Evaluate(c);
            Assert.AreEqual("hi", r, "result of running");
        }

        [TestMethod]
        public void RegisterExprEvalIgnoredByParent()
        {
            var c = new ScopeContext(new RootContext());
            object r = null;
            Action<object> saver = o => r = o;
            var es = new PlotLingoLib.Statements.ExpressionStatement(new StringValue("hi"));

            c.AddExpressionStatementEvaluationCallback(saver);
            es.Evaluate(c.Parent);
            Assert.IsNull(r, "result of running");
        }

        [TestMethod]
        public void RegisterExprEvalHiddenInScope()
        {
            var c = new ScopeContext(new RootContext());
            object r = null;
            Action<object> saver = o => r = o;
            var es = new PlotLingoLib.Statements.ExpressionStatement(new StringValue("hi"));

            c.Parent.AddExpressionStatementEvaluationCallback(saver);
            es.Evaluate(c);
            Assert.AreEqual("hi", r, "result of running");
        }

        [TestMethod]
        public void RegisterAndRemoveExprEvalWorks()
        {
            var c = new ScopeContext(new RootContext());
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
