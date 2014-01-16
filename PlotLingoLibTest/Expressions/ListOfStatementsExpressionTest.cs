using Microsoft.VisualStudio.TestTools.UnitTesting;
using PlotLingoLib;
using PlotLingoLib.Expressions;
using PlotLingoLib.Expressions.Values;
using PlotLingoLib.Statements;
using System;

namespace PlotLingoLibTest.Expressions
{
    /// <summary>
    /// Test out the list of statement explorers.
    /// </summary>
    [TestClass]
    public class ListOfStatementsExpressionTest
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CreateNoSttatements()
        {
            var l = new ListOfStatementsExpression(null);
        }

        [TestMethod]
        public void TestNoSttatements()
        {
            var l = new ListOfStatementsExpression(new IStatement[] { });
            RootContext c = new RootContext();
            var r = l.Evaluate(c);
            Assert.IsNull(r, "result when no statements");
        }

        [TestMethod]
        public void ExecuteSingleExpression()
        {
            var l = new ListOfStatementsExpression(new IStatement[] { new ExpressionStatement(new IntegerValue(5)) });
            RootContext c = new RootContext();
            var r = l.Evaluate(c);
            Assert.AreEqual(5, r, "Result of eval");
        }

        [TestMethod]
        public void ExecuteSingleExpressionNoSideEffects()
        {
            var l = new ListOfStatementsExpression(new IStatement[] { new AssignmentStatement("a", new IntegerValue(5)) });
            RootContext c = new RootContext();
            var r = l.Evaluate(c);
            Assert.IsFalse(c.GetVariableValueOrNull("a").Item1, "variable part fo context");
        }

        [TestMethod]
        public void ExecuteContextUpdatedWhileRunning()
        {
            var l = new ListOfStatementsExpression(new IStatement[] {
                new AssignmentStatement("a", new IntegerValue(5)), 
                new ExpressionStatement(new FunctionExpression("+", new VariableValue("a"), new IntegerValue(5)))
            });
            RootContext c = new RootContext();
            var r = l.Evaluate(c);
            Assert.AreEqual(10, r, "value of result");
        }

        [TestMethod]
        public void UpdateExistingVariables()
        {
            var l = new ListOfStatementsExpression(new IStatement[] { 
                new AssignmentStatement("a", new IntegerValue(5)),
            });
            RootContext c = new RootContext();
            c.SetVariableValue("a", 6);
            l.Evaluate(c);
            var r = c.GetVariableValue("a");
            Assert.AreEqual(5, r, "value of result");
        }

        [TestMethod]
        public void CanExecuteStatemetnsTwice()
        {
            var l = new ListOfStatementsExpression(new IStatement[] { 
                new AssignmentStatement("a", new FunctionExpression("+", new VariableValue("a"), new IntegerValue(5))),
            });
            RootContext c = new RootContext();
            c.SetVariableValue("a", 0);
            l.Evaluate(c);
            l.Evaluate(c);
            var r = c.GetVariableValue("a");
            Assert.AreEqual(10, r, "value of result");
        }
    }
}
