using Microsoft.VisualStudio.TestTools.UnitTesting;
using PlotLingoLib;
using PlotLingoLib.Expressions;
using PlotLingoLib.Expressions.Values;
using PlotLingoLib.Statements;
using System.Collections.Generic;
using System.Linq;

namespace PlotLingoLibTest.Functions
{
    [TestClass]
    public class MapTest
    {
        [TestMethod]
        public void DictMapEmpty()
        {
            var loopdict = new ObjectValue(new Dictionary<object, object>[] { });

            var statement1 = new ExpressionStatement(new IntegerValue(5));
            var exprStatement = new ObjectValue(new ListOfStatementsExpression(new IStatement[] { statement1 }));

            var forLoop = new FunctionExpression("map", loopdict, exprStatement);

            var c = new RootContext();
            var r = forLoop.Evaluate(c);

            Assert.IsNotNull(r, "for loop with nothing in it");
            var lst = r as IEnumerable<object>;
            Assert.IsNotNull(lst, "results isn't in good form!");
            Assert.AreEqual(0, lst.Count());
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

            var forLoop = new FunctionExpression("map", loopdict, exprStatement);

            var c = new RootContext();
            var r = forLoop.Evaluate(c);

            Assert.IsNotNull(r, "for loop with nothing in it");
            var lst = r as IEnumerable<object>;
            Assert.IsNotNull(lst, "results isn't in good form!");
            Assert.AreEqual(1, lst.Count());
            Assert.AreEqual(5, lst.First());
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

            var forLoop = new FunctionExpression("map", loopdict, exprStatement);

            var c = new RootContext();
            var r = forLoop.Evaluate(c);

            Assert.IsNotNull(r, "for loop with nothing in it");
            var lst = r as IEnumerable<object>;
            Assert.IsNotNull(lst, "results isn't in good form!");
            Assert.AreEqual(2, lst.Count());
            Assert.AreEqual(10, lst.First());
            Assert.AreEqual(20, lst.Skip(1).First());
        }
    }
}
