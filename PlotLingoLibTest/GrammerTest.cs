﻿using System;
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
        public void TestMethodNoArgStatement()
        {
            var r = PlotLingoLib.Grammar.ModuleParser.End().Parse("p.plot();");
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
        public void TestEmptyArrayExpressionParse()
        {
            var r = PlotLingoLib.Grammar.ModuleParser.End().Parse("[];");
            Assert.IsNotNull(r);
            Assert.AreEqual(1, r.Length, "# of statements");
            Assert.IsInstanceOfType(r[0], typeof(ExpressionStatement), "expr statement");
            var exprS = r[0] as ExpressionStatement;
            Assert.IsInstanceOfType(exprS.Expression, typeof(ArrayValue), "array method");
            var mc = exprS.Expression as ArrayValue;
            Assert.AreEqual(0, mc.Length, "# of values in the array");
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
            var expr = mc.ObjectExpression as ArrayValue;
            Assert.IsNotNull(expr, "array expression");
            Assert.AreEqual(2, expr.Length, "#of values in array");
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

        [TestMethod]
        public void TestGrouping()
        {
            var r = PlotLingoLib.Grammar.ModuleParser.End().Parse("(p);");
            Assert.IsNotNull(r);
            Assert.AreEqual(1, r.Length, "# of statements");
            Assert.IsInstanceOfType(r[0], typeof(ExpressionStatement), "expr statement");
            var exprS = r[0] as ExpressionStatement;
            Assert.IsInstanceOfType(exprS.Expression, typeof(VariableValue), "Expression method");
            var vv = exprS.Expression as VariableValue;
            Assert.AreEqual("p", vv.VariableName, "var name");
        }

        [TestMethod]
        public void TestAddOperator()
        {
            var r = PlotLingoLib.Grammar.ModuleParser.End().Parse("a+b;");
            Assert.IsNotNull(r);
            Assert.AreEqual(1, r.Length, "# of statements");
            Assert.IsInstanceOfType(r[0], typeof(ExpressionStatement), "expr statement");
            var exprS = r[0] as ExpressionStatement;
            Assert.IsInstanceOfType(exprS.Expression, typeof(FunctionExpression), "add operator");
            var vv = exprS.Expression as FunctionExpression;
            Assert.AreEqual("+", vv.FunctionName, "operator name");
        }


        [TestMethod]
        public void TestSubtractOperator()
        {
            var r = PlotLingoLib.Grammar.ModuleParser.End().Parse("a-b;");
            Assert.IsNotNull(r);
            Assert.AreEqual(1, r.Length, "# of statements");
            Assert.IsInstanceOfType(r[0], typeof(ExpressionStatement), "expr statement");
            var exprS = r[0] as ExpressionStatement;
            Assert.IsInstanceOfType(exprS.Expression, typeof(FunctionExpression), "add operator");
            var vv = exprS.Expression as FunctionExpression;
            Assert.AreEqual("-", vv.FunctionName, "operator name");
        }
        
        [TestMethod]
        public void TestAdd2Operator()
        {
            var r = PlotLingoLib.Grammar.ModuleParser.End().Parse("a+b+c;");
            Assert.IsNotNull(r);
            Assert.AreEqual(1, r.Length, "# of statements");
            Assert.IsInstanceOfType(r[0], typeof(ExpressionStatement), "expr statement");
            var exprS = r[0] as ExpressionStatement;
            Assert.IsInstanceOfType(exprS.Expression, typeof(FunctionExpression), "add operator");
            var vv = exprS.Expression as FunctionExpression;
            Assert.AreEqual("+", vv.FunctionName, "operator name");
            Assert.AreEqual("+(+(a,b),c)", vv.ToString(), "Result of the expression");
        }

        [TestMethod]
        public void TestAddMultPrecedence()
        {
            Assert.Inconclusive();
        }
    }
}
