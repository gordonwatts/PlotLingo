using System;
using Sprache;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PlotLingoLib.Statements;
using PlotLingoLib.Expressions;
using PlotLingoLib.Expressions.Values;

namespace PlotLingoLibTest
{
    [TestClass]
    public class GrammerTest
    {
        [TestMethod]
        public void TestFunctionCall()
        {
            var r = PlotLingoLib.Grammar.ModuleParser.End().Parse("a = file(\"hi\");");
            Assert.IsNotNull(r);
            Assert.AreEqual(1, r.Length, "# of statements");
            Assert.IsInstanceOfType(r[0], typeof(AssignmentStatement));
            var a = (AssignmentStatement)r[0];
        }

        [TestMethod]
        public void TestAssignmentByStringNoWS()
        {
            var r = PlotLingoLib.Grammar.ModuleParser.End().Parse("a =\"hi\";");
            Assert.IsNotNull(r);
            Assert.AreEqual(1, r.Length, "# of statements");
            Assert.IsInstanceOfType(r[0], typeof(AssignmentStatement));
            var a = (AssignmentStatement)r[0];
        }

        /// <summary>
        /// Make sure we deal with whitespace correctly. Because we are looking for characters,
        /// this means WS is important.
        /// </summary>
        [TestMethod]
        public void TestAssignmentByStringWS()
        {
            var r = PlotLingoLib.Grammar.ModuleParser.End().Parse("a = \"hi\";");
            Assert.IsNotNull(r);
            Assert.AreEqual(1, r.Length, "# of statements");
            Assert.IsInstanceOfType(r[0], typeof(AssignmentStatement));
            var a = (AssignmentStatement)r[0];
        }

        [TestMethod]
        public void TestExpressionStatement()
        {
            var r = PlotLingoLib.Grammar.ModuleParser.End().Parse("\"hi\";");
            Assert.IsNotNull(r);
            Assert.AreEqual(1, r.Length, "# of statements");
            Assert.IsInstanceOfType(r[0], typeof(ExpressionStatement));
        }

        [TestMethod]
        public void TestMethodStatement()
        {
            var r = PlotLingoLib.Grammar.ModuleParser.End().Parse("p.plot(\"hi\");");
            Assert.IsNotNull(r);
            Assert.AreEqual(1, r.Length, "# of statements");
            Assert.IsInstanceOfType(r[0], typeof(ExpressionStatement), "expr statement");
            var exprS = r[0] as ExpressionStatement;
            Assert.IsInstanceOfType(exprS.Expression, typeof(MethodCallExpression), "Expression method");
            var mc = exprS.Expression as MethodCallExpression;
            Assert.IsInstanceOfType(mc.ObjectExpression, typeof(VariableValue), "object name");
            var ve = mc.ObjectExpression as VariableValue;
            Assert.AreEqual("p", ve.VariableName, "object name");
        }

        [TestMethod]
        public void TestArrayExpressionParse()
        {
            var r = PlotLingoLib.Grammar.ModuleParser.End().Parse("[\"hi\", \"there\"];");
            Assert.IsNotNull(r);
            Assert.AreEqual(1, r.Length, "# of statements");
            Assert.IsInstanceOfType(r[0], typeof(ExpressionStatement), "expr statement");
            var exprS = r[0] as ExpressionStatement;
            Assert.IsInstanceOfType(exprS.Expression, typeof(ArrayValue), "array method");
            var mc = exprS.Expression as ArrayValue;
            Assert.AreEqual(2, mc.Length, "# of values in the array");
        }

        [TestMethod]
        public void TestArrayMethodCall()
        {
            var r = PlotLingoLib.Grammar.ModuleParser.End().Parse("[\"hi\", \"there\"].plot();");
            Assert.IsNotNull(r);
            Assert.AreEqual(1, r.Length, "# of statements");
            Assert.IsInstanceOfType(r[0], typeof(ExpressionStatement), "expr statement");
            var exprS = r[0] as ExpressionStatement;
            Assert.IsInstanceOfType(exprS.Expression, typeof(MethodCallExpression), "Expression method");
            var mc = exprS.Expression as MethodCallExpression;
            Assert.Inconclusive();
        }

        [TestMethod]
        public void TestValueAsExpression()
        {
            var r = PlotLingoLib.Grammar.ModuleParser.End().Parse("p;");
            Assert.IsNotNull(r);
            Assert.AreEqual(1, r.Length, "# of statements");
            Assert.IsInstanceOfType(r[0], typeof(ExpressionStatement), "expr statement");
            var exprS = r[0] as ExpressionStatement;
            Assert.IsInstanceOfType(exprS.Expression, typeof(VariableValue), "Expression method");
            var vv = exprS.Expression as VariableValue;
            Assert.AreEqual("p", vv.VariableName, "var name");
        }
    }
}
