using Microsoft.VisualStudio.TestTools.UnitTesting;
using PlotLingoLib;
using PlotLingoLib.Expressions;
using PlotLingoLib.Expressions.Values;
using PlotLingoLib.Functions;
using PlotLingoLib.Statements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlotLingoLibTest.Functions
{
    [TestClass]
    public class ifStatementTest
    {
        [TestMethod]
        public void SimpleTrueStatement()
        {
            var testExpr = new IntegerValue(1);
            var assign = new AssignmentStatement("a", new IntegerValue(10));
            var statement = new ListOfStatementsExpression(new IStatement[] { assign });

            var ctx = new RootContext();
            ctx.SetVariableValue("a", 5);

            var ifStatement = new FunctionExpression("if", testExpr, statement);
            ifStatement.Evaluate(ctx);

            Assert.AreEqual(10, ctx.GetVariableValue("a"));
        }

        [TestMethod]
        public void SimpleFalseStatement()
        {
            var testExpr = new IntegerValue(0);
            var assign = new AssignmentStatement("a", new IntegerValue(10));
            var statement = new ListOfStatementsExpression(new IStatement[] { assign });

            var ctx = new RootContext();
            ctx.SetVariableValue("a", 5);

            var ifStatement = new FunctionExpression("if", testExpr, statement);
            ifStatement.Evaluate(ctx);

            Assert.AreEqual(5, ctx.GetVariableValue("a"));
        }

        [TestMethod]
        public void ExecuteInDifferentContex()
        {
            var testExpr = new IntegerValue(1);
            var assign = new AssignmentStatement("b", new IntegerValue(10));
            var statement = new ListOfStatementsExpression(new IStatement[] { assign });

            var ctx = new RootContext();
            ctx.SetVariableValue("a", 5);

            var ifStatement = new FunctionExpression("if", testExpr, statement);
            ifStatement.Evaluate(ctx);

            Assert.IsFalse(ctx.GetVariableValueOrNull("b").Item1);
        }
    }
}
