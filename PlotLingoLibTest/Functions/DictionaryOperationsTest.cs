using Microsoft.VisualStudio.TestTools.UnitTesting;
using PlotLingoLib;
using PlotLingoLib.Expressions;
using System;
using System.Collections.Generic;

namespace PlotLingoLibTest.Functions
{
    [TestClass]
    public class DictionaryOperationsTest
    {

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void TestSumEmpyDict()
        {
            var d = new Dictionary<object, object>();
            var fo = new MethodCallExpression(new PlotLingoLib.Expressions.Values.VariableValue("p"),
                new FunctionExpression("Sum", new IExpression[0]));
            var c = new Context();
            c.SetVariableValue("p", d);
            fo.Evaluate(c);
        }

        [TestMethod]
        public void TestSumOneObj()
        {
            var d = new Dictionary<object, object>();
            d["hi"] = 10;
            var fo = new MethodCallExpression(new PlotLingoLib.Expressions.Values.VariableValue("p"),
                new FunctionExpression("Sum", new IExpression[0]));
            var c = new Context();
            c.SetVariableValue("p", d);
            var v = fo.Evaluate(c);
            Assert.AreEqual(10, v, "Value");
        }

        [TestMethod]
        public void TestSumTwoObj()
        {
            var d = new Dictionary<object, object>();
            d["hi"] = 10;
            d[5] = 20;
            var fo = new MethodCallExpression(new PlotLingoLib.Expressions.Values.VariableValue("p"),
                new FunctionExpression("Sum", new IExpression[0]));
            var c = new Context();
            c.SetVariableValue("p", d);
            var v = fo.Evaluate(c);
            Assert.AreEqual(30, v, "Value");
        }
    }
}
