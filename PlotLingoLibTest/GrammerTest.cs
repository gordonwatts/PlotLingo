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
        public void TestAssignmentWS1()
        {
            var r = PlotLingoLib.Grammar.ModuleParser.End().Parse("a = p1;");
            Assert.IsNotNull(r);
            Assert.AreEqual(1, r.Length, "# of statements");
            Assert.IsInstanceOfType(r[0], typeof(AssignmentStatement));
            var a = (AssignmentStatement)r[0];
        }

        [TestMethod]
        public void TestAssignmentWS2()
        {
            var r = PlotLingoLib.Grammar.ModuleParser.End().Parse("a = [p1];");
            Assert.IsNotNull(r);
            Assert.AreEqual(1, r.Length, "# of statements");
            Assert.IsInstanceOfType(r[0], typeof(AssignmentStatement));
            var a = (AssignmentStatement)r[0];
        }

        [TestMethod]
        public void TestAssignmentNoWS2()
        {
            var r = PlotLingoLib.Grammar.ModuleParser.End().Parse("a=[p1];");
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
        public void Test2ExpressionStatement()
        {
            var r = PlotLingoLib.Grammar.ModuleParser.End().Parse("\"hi\";  p ;");
            Assert.IsNotNull(r);
            Assert.AreEqual(2, r.Length, "# of statements");
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
        public void TestMethodChainStatement()
        {
            var r = PlotLingoLib.Grammar.ModuleParser.End().Parse("p.plot(\"hi\").Title();");
            Assert.IsNotNull(r);
            Assert.AreEqual(1, r.Length, "# of statements");
            Assert.IsInstanceOfType(r[0], typeof(ExpressionStatement), "expr statement");
            Assert.AreEqual("p.plot(\"hi\").Title();", r[0].ToString(), "Parsed item");
        }

        [TestMethod]
        public void TestMethodAddStatement()
        {
            var r = PlotLingoLib.Grammar.ModuleParser.End().Parse("p.plot() + p.plot();");
            Assert.IsNotNull(r);
            Assert.AreEqual(1, r.Length, "# of statements");
            Assert.IsInstanceOfType(r[0], typeof(ExpressionStatement), "expr statement");
            Assert.AreEqual("+(p.plot(),p.plot());", r[0].ToString(), "Parsed item");
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
        public void TestMethodDotWSStatement()
        {
            var r = PlotLingoLib.Grammar.ModuleParser.End().Parse("p . plot();");
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
        public void TestValueArray()
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
        public void TestValueArrayEmpty()
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
        public void TestExpressionWithPreceedingWhitespace()
        {
            var r = PlotLingoLib.Grammar.ModuleParser.End().Parse("  [\"hi\", \"there\"].plot();");
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
        
        /// <summary>
        /// Seen in the wild - variable names seem to cause problems..
        /// </summary>
        [TestMethod]
        public void TestArrayMethodCallWithVars()
        {
            var r = PlotLingoLib.Grammar.ModuleParser.End().Parse("[p1,p2].plot();");
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

        /// <summary>
        /// Seen in the wild - array after somethign else seems to cause problems..
        /// </summary>
        [TestMethod]
        public void TestArrayMethodCallAfterStatementWhitespace()
        {
            var r = PlotLingoLib.Grammar.ModuleParser.End().Parse("p1 = \"hi\"; [p1,p2].plot();");
            Assert.IsNotNull(r);
            Assert.AreEqual(2, r.Length, "# of statements");
            Assert.IsInstanceOfType(r[1], typeof(ExpressionStatement), "expr statement");
            var exprS = r[1] as ExpressionStatement;
            Assert.IsInstanceOfType(exprS.Expression, typeof(MethodCallExpression), "Expression method");
            var mc = exprS.Expression as MethodCallExpression;
            var expr = mc.ObjectExpression as ArrayValue;
            Assert.IsNotNull(expr, "array expression");
            Assert.AreEqual(2, expr.Length, "#of values in array");
        }

        /// <summary>
        /// Seen in the wild - array after somethign else seems to cause problems..
        /// </summary>
        [TestMethod]
        public void TestArrayMethodCallAfterStatementNoWhitespace()
        {
            var r = PlotLingoLib.Grammar.ModuleParser.End().Parse("p1 = \"hi\";[p1,p2].plot();");
            Assert.IsNotNull(r);
            Assert.AreEqual(2, r.Length, "# of statements");
            Assert.IsInstanceOfType(r[1], typeof(ExpressionStatement), "expr statement");
            var exprS = r[1] as ExpressionStatement;
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
        public void TestValueWithUnderscoreAsExpression()
        {
            var r = PlotLingoLib.Grammar.ModuleParser.End().Parse("_p;");
            Assert.IsNotNull(r);
            Assert.AreEqual(1, r.Length, "# of statements");
            Assert.IsInstanceOfType(r[0], typeof(ExpressionStatement), "expr statement");
            var exprS = r[0] as ExpressionStatement;
            Assert.IsInstanceOfType(exprS.Expression, typeof(VariableValue), "Expression method");
            var vv = exprS.Expression as VariableValue;
            Assert.AreEqual("_p", vv.VariableName, "var name");
        }

        [TestMethod]
        [ExpectedException(typeof(Sprache.ParseException))]
        public void TestValueIllegalVariableName()
        {
            var r = PlotLingoLib.Grammar.ModuleParser.End().Parse("5p;");
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
        public void TestMultOperator()
        {
            var r = PlotLingoLib.Grammar.ModuleParser.End().Parse("a*b;");
            Assert.IsNotNull(r);
            Assert.AreEqual(1, r.Length, "# of statements");
            Assert.IsInstanceOfType(r[0], typeof(ExpressionStatement), "expr statement");
            Assert.AreEqual("*(a,b);", r[0].ToString(), "actual expression");
        }

        [TestMethod]
        public void TestDivideOperator()
        {
            var r = PlotLingoLib.Grammar.ModuleParser.End().Parse("a/b;");
            Assert.IsNotNull(r);
            Assert.AreEqual(1, r.Length, "# of statements");
            Assert.IsInstanceOfType(r[0], typeof(ExpressionStatement), "expr statement");
            Assert.AreEqual("/(a,b);", r[0].ToString(), "actual expression");
        }

        [TestMethod]
        public void TestAdd2Operator()
        {
            var r = PlotLingoLib.Grammar.ModuleParser.End().Parse("a+b+c;");
            Assert.IsNotNull(r);
            Assert.AreEqual(1, r.Length, "# of statements");
            Assert.IsInstanceOfType(r[0], typeof(ExpressionStatement), "expr statement");
            Assert.AreEqual("+(+(a,b),c);", r[0].ToString(), "Result of the expression");
        }

        [TestMethod]
        public void TestAddMultPrecedence1()
        {
            var r = PlotLingoLib.Grammar.ModuleParser.End().Parse("a+b*c;");
            Assert.IsNotNull(r);
            Assert.AreEqual(1, r.Length, "# of statements");
            Assert.IsInstanceOfType(r[0], typeof(ExpressionStatement), "expr statement");
            Assert.AreEqual("+(a,*(b,c));", r[0].ToString(), "Result of the expression");
        }

        [TestMethod]
        public void TestAddMultPrecedence2()
        {
            var r = PlotLingoLib.Grammar.ModuleParser.End().Parse("a*b+c;");
            Assert.IsNotNull(r);
            Assert.AreEqual(1, r.Length, "# of statements");
            Assert.IsInstanceOfType(r[0], typeof(ExpressionStatement), "expr statement");
            Assert.AreEqual("+(*(a,b),c);", r[0].ToString(), "Result of the expression");
        }

        [TestMethod]
        public void TestAddMultPrecedence3()
        {
            var r = PlotLingoLib.Grammar.ModuleParser.End().Parse("a+b*c*d;");
            Assert.IsNotNull(r);
            Assert.AreEqual(1, r.Length, "# of statements");
            Assert.IsInstanceOfType(r[0], typeof(ExpressionStatement), "expr statement");
            Assert.AreEqual("+(a,*(*(b,c),d));", r[0].ToString(), "Result of the expression");
        }

        [TestMethod]
        public void TestValueInteger()
        {
            var r = PlotLingoLib.Grammar.ModuleParser.End().Parse("5;");
            Assert.IsNotNull(r);
            Assert.AreEqual(1, r.Length, "# of statements");
            Assert.IsInstanceOfType(r[0], typeof(ExpressionStatement), "expr statement");
            Assert.AreEqual("5;", r[0].ToString(), "Result of the expression");
        }

        [TestMethod]
        public void TestValueDouble()
        {
            var r = PlotLingoLib.Grammar.ModuleParser.End().Parse("5.5;");
            Assert.IsNotNull(r);
            Assert.AreEqual(1, r.Length, "# of statements");
            Assert.IsInstanceOfType(r[0], typeof(ExpressionStatement), "expr statement");
            Assert.AreEqual("5.5;", r[0].ToString(), "Result of the expression");
        }

        [TestMethod]
        public void TestValueDoubleNothingBehind()
        {
            var r = PlotLingoLib.Grammar.ModuleParser.End().Parse("5.;");
            Assert.IsNotNull(r);
            Assert.AreEqual(1, r.Length, "# of statements");
            Assert.IsInstanceOfType(r[0], typeof(ExpressionStatement), "expr statement");
            Assert.AreEqual("5;", r[0].ToString(), "Result of the expression");
        }

        [TestMethod]
        public void TestValueDoubleNothingAhead()
        {
            var r = PlotLingoLib.Grammar.ModuleParser.End().Parse(".5;");
            Assert.IsNotNull(r);
            Assert.AreEqual(1, r.Length, "# of statements");
            Assert.IsInstanceOfType(r[0], typeof(ExpressionStatement), "expr statement");
            Assert.AreEqual("0.5;", r[0].ToString(), "Result of the expression");
        }

        [TestMethod]
        [ExpectedException(typeof(Sprache.ParseException))]
        public void TestValueDoubleDotBad()
        {
            var r = PlotLingoLib.Grammar.ModuleParser.End().Parse(".;");
        }

        [TestMethod]
        public void TestValueString()
        {
            var r = PlotLingoLib.Grammar.ModuleParser.End().Parse("\"hi\";");
            Assert.IsNotNull(r);
            Assert.AreEqual(1, r.Length, "# of statements");
            Assert.IsInstanceOfType(r[0], typeof(ExpressionStatement), "expr statement");
            Assert.AreEqual("\"hi\";", r[0].ToString(), "Result of the expression");
        }

        [TestMethod]
        public void TestValueDictionary()
        {
            var r = PlotLingoLib.Grammar.ModuleParser.End().Parse("{5 => 10, 6 => 11};");
            Assert.IsNotNull(r);
            Assert.AreEqual(1, r.Length, "# of statements");
            Assert.IsInstanceOfType(r[0], typeof(ExpressionStatement), "expr statement");
            Assert.AreEqual("{5 => 10, 6 => 11};", r[0].ToString(), "Result of the expression");
        }

        [TestMethod]
        public void TestValueDictionaryColon()
        {
            var r = PlotLingoLib.Grammar.ModuleParser.End().Parse("{5 : 10, 6 : 11};");
            Assert.IsNotNull(r);
            Assert.AreEqual(1, r.Length, "# of statements");
            Assert.IsInstanceOfType(r[0], typeof(ExpressionStatement), "expr statement");
            Assert.AreEqual("{5 => 10, 6 => 11};", r[0].ToString(), "Result of the expression");
        }

        [TestMethod]
        public void TestValueDictionaryEmpty()
        {
            var r = PlotLingoLib.Grammar.ModuleParser.End().Parse("{};");
            Assert.IsNotNull(r);
            Assert.AreEqual(1, r.Length, "# of statements");
            Assert.IsInstanceOfType(r[0], typeof(ExpressionStatement), "expr statement");
            Assert.AreEqual("{};", r[0].ToString(), "Result of the expression");
        }

        [TestMethod]
        public void TestValueDictionarySingleValue()
        {
            var r = PlotLingoLib.Grammar.ModuleParser.End().Parse("5 => 10;");
            Assert.IsNotNull(r);
            Assert.AreEqual(1, r.Length, "# of statements");
            Assert.IsInstanceOfType(r[0], typeof(ExpressionStatement), "expr statement");
            Assert.AreEqual("{5 => 10};", r[0].ToString(), "Result of the expression");
        }

        [TestMethod]
        public void TestValueDictionaryCascadeValue()
        {
            var r = PlotLingoLib.Grammar.ModuleParser.End().Parse("5 => 10 => 15;");
            Assert.IsNotNull(r);
            Assert.AreEqual(1, r.Length, "# of statements");
            Assert.IsInstanceOfType(r[0], typeof(ExpressionStatement), "expr statement");
            Assert.AreEqual("{{5 => 10} => 15};", r[0].ToString(), "Result of the expression");
        }

        [TestMethod]
        public void TestValueDictionaryCascadeValueWithB()
        {
            var r = PlotLingoLib.Grammar.ModuleParser.End().Parse("5 => {10 => 15};");
            Assert.IsNotNull(r);
            Assert.AreEqual(1, r.Length, "# of statements");
            Assert.IsInstanceOfType(r[0], typeof(ExpressionStatement), "expr statement");
            Assert.AreEqual("{5 => {10 => 15}};", r[0].ToString(), "Result of the expression");
        }

        [TestMethod]
        public void TestValueDictionaryStringKey()
        {
            var r = PlotLingoLib.Grammar.ModuleParser.End().Parse("5 => {\"quark\" => 15};");
            Assert.IsNotNull(r);
            Assert.AreEqual(1, r.Length, "# of statements");
            Assert.IsInstanceOfType(r[0], typeof(ExpressionStatement), "expr statement");
            Assert.AreEqual("{5 => {\"quark\" => 15}};", r[0].ToString(), "Result of the expression");
        }
    }
}
