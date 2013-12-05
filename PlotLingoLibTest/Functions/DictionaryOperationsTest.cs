using Microsoft.VisualStudio.TestTools.UnitTesting;
using PlotLingoLib;
using PlotLingoLib.Expressions;
using PlotLingoLib.Expressions.Values;
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
                new FunctionExpression("sum", new IExpression[0]));
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
                new FunctionExpression("sum", new IExpression[0]));
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
                new FunctionExpression("sum", new IExpression[0]));
            var c = new Context();
            c.SetVariableValue("p", d);
            var v = fo.Evaluate(c);
            Assert.AreEqual(30, v, "Value");
        }

        [TestMethod]
        public void TestMultiplyByConstant()
        {
            var d = new Dictionary<object, object>();
            d["hi"] = 5.0;
            var cl = new FunctionExpression("*", new ObjectValue(d), new DoubleValue(1.5));
            var c = new Context();
            var rut = cl.Evaluate(c);
            Assert.IsNotNull(rut, "Result null");
            Assert.IsInstanceOfType(rut, typeof(Dictionary<object, object>), "Type of result");
            var r = rut as Dictionary<object, object>;

            // Make sure the original isn't touched, as well as the final one updated
            Assert.AreEqual(5.0, d["hi"], "original value");
            Assert.AreEqual(5.0 * 1.5, r["hi"], "result");
        }
    }
}
