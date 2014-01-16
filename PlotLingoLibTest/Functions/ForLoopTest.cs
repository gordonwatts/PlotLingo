using Microsoft.VisualStudio.TestTools.UnitTesting;
using PlotLingoLib;
using PlotLingoLib.Expressions;
using PlotLingoLib.Expressions.Values;
using PlotLingoLib.Statements;
using System.Collections.Generic;

namespace PlotLingoLibTest.Functions
{
    /// <summary>
    /// Test the for loops
    /// </summary>
    [TestClass]
    public class ForLoopTest
    {
        [TestMethod]
        public void DictForEmpty()
        {
            var loopdict = new ObjectValue(new Dictionary<object, object>[] { });

            var statement1 = new ExpressionStatement(new IntegerValue(5));
            var exprStatement = new ObjectValue(new ListOfStatementsExpression(new IStatement[] { statement1 }));

            var forLoop = new FunctionExpression("for", loopdict, exprStatement);

            var c = new RootContext();
            var r = forLoop.Evaluate(c);

            Assert.IsNull(r, "for loop with nothing in it");
        }

        [TestMethod]
        public void DictForOneResult()
        {
            var loop1 = new Dictionary<object, object>() {
                { "a", 10 }
            };
            var loopdict = new ObjectValue(new Dictionary<object, object>[] { loop1 });

            var statement1 = new ExpressionStatement(new IntegerValue(5));
            var exprStatement = new ObjectValue(new ListOfStatementsExpression(new IStatement[] { statement1 }));

            var forLoop = new FunctionExpression("for", loopdict, exprStatement);

            var c = new RootContext();
            var r = forLoop.Evaluate(c);

            Assert.AreEqual(5, r, "for loop with nothing in it");
        }

        [TestMethod]
        public void DictForDefineLocally()
        {
            var loop1 = new Dictionary<object, object>() {
                { "a", 10 }
            };
            var loopdict = new ObjectValue(new Dictionary<object, object>[] { loop1 });

            var statement1 = new ExpressionStatement(new IntegerValue(5));
            var exprStatement = new ObjectValue(new ListOfStatementsExpression(new IStatement[] { statement1 }));

            var forLoop = new FunctionExpression("for", loopdict, exprStatement);

            var c = new RootContext();
            c.SetVariableValue("a", 20);
            var r = forLoop.Evaluate(c);

            Assert.AreEqual(20, c.GetVariableValue("a"), "for loop with nothing in it");
        }

        [TestMethod]
        public void DictForVarSet()
        {
            var loop1 = new Dictionary<object, object>() {
                { "a", 10 }
            };
            var loopdict = new ObjectValue(new Dictionary<object, object>[] { loop1 });

            var statement1 = new ExpressionStatement(new VariableValue("a"));
            var exprStatement = new ObjectValue(new ListOfStatementsExpression(new IStatement[] { statement1 }));

            var forLoop = new FunctionExpression("for", loopdict, exprStatement);

            var c = new RootContext();
            var r = forLoop.Evaluate(c);

            Assert.AreEqual(10, r, "for loop with nothing in it");
        }

        [TestMethod]
        public void DictForVarSet2Values()
        {
            var loop1 = new Dictionary<object, object>() {
                { "a", 10 },
                {"b", 5}
            };
            var loopdict = new ObjectValue(new Dictionary<object, object>[] { loop1 });

            var statement1 = new ExpressionStatement(new FunctionExpression("+", new VariableValue("a"), new VariableValue("b")));
            var exprStatement = new ObjectValue(new ListOfStatementsExpression(new IStatement[] { statement1 }));

            var forLoop = new FunctionExpression("for", loopdict, exprStatement);

            var c = new RootContext();
            var r = forLoop.Evaluate(c);

            Assert.AreEqual(15, r, "for loop with nothing in it");
        }

        [TestMethod]
        public void DictForVar2Iter()
        {
            var loop1 = new Dictionary<object, object>() {
                { "a", 10 }
            };
            var loop2 = new Dictionary<object, object>() {
                { "a", 20 }
            };
            var loopdict = new ObjectValue(new Dictionary<object, object>[] { loop1, loop2 });

            var statement1 = new ExpressionStatement(new VariableValue("a"));
            var exprStatement = new ObjectValue(new ListOfStatementsExpression(new IStatement[] { statement1 }));

            var forLoop = new FunctionExpression("for", loopdict, exprStatement);

            var c = new RootContext();
            var r = forLoop.Evaluate(c);

            Assert.AreEqual(20, r, "for loop with nothing in it");
        }
    }
}
