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
        public void TestSumThreeObj()
        {
            var d = new Dictionary<object, object>();
            d["hi"] = 10;
            d[5] = 20;
            d["there"] = 5;
            var fo = new MethodCallExpression(new PlotLingoLib.Expressions.Values.VariableValue("p"),
                new FunctionExpression("sum", new IExpression[0]));
            var c = new Context();
            c.SetVariableValue("p", d);
            var v = fo.Evaluate(c);
            Assert.AreEqual(35, v, "Value");
        }

        [TestMethod]
        public void TestSumSevenObj()
        {
            var d = new Dictionary<object, object>();
            d["hi"] = 10;
            d[5] = 20;
            d["there"] = 5;
            d["also"] = 1;
            d["noway"] = 2;
            d["stuff"] = 4;
            d["ops"] = 5;
            var fo = new MethodCallExpression(new PlotLingoLib.Expressions.Values.VariableValue("p"),
                new FunctionExpression("sum", new IExpression[0]));
            var c = new Context();
            c.SetVariableValue("p", d);
            var v = fo.Evaluate(c);
            Assert.AreEqual(47, v, "Value");
        }

        [TestMethod]
        public void TestAddEmptyMatricies()
        {
            var d1 = new Dictionary<object, object>();
            var d2 = new Dictionary<object, object>();

            var cl = new FunctionExpression("+", new ObjectValue(d1), new ObjectValue(d2));
            var c = new Context();
            var r = cl.Evaluate(c);
            Assert.IsInstanceOfType(r, typeof(Dictionary<object, object>), "type of result");
            var rut = r as Dictionary<object, object>;
            Assert.AreEqual(0, rut.Count, "# of items");
        }

        [TestMethod]
        public void TestAddEmptyAndGoodMatricies()
        {
            var d1 = new Dictionary<object, object>();
            d1["hi"] = 10;
            var d2 = new Dictionary<object, object>();

            var cl = new FunctionExpression("+", new ObjectValue(d1), new ObjectValue(d2));
            var c = new Context();
            var r = cl.Evaluate(c);
            Assert.IsInstanceOfType(r, typeof(Dictionary<object, object>), "type of result");
            var rut = r as Dictionary<object, object>;
            Assert.AreEqual(1, rut.Count, "# of items");
            Assert.AreEqual(10, rut["hi"], "value of hi");
        }

        [TestMethod]
        public void TestAddEmptyAndGoodMatriciesSwitched()
        {
            var d1 = new Dictionary<object, object>();
            d1["hi"] = 10;
            var d2 = new Dictionary<object, object>();

            var cl = new FunctionExpression("+", new ObjectValue(d2), new ObjectValue(d1));
            var c = new Context();
            var r = cl.Evaluate(c);
            Assert.IsInstanceOfType(r, typeof(Dictionary<object, object>), "type of result");
            var rut = r as Dictionary<object, object>;
            Assert.AreEqual(1, rut.Count, "# of items");
            Assert.AreEqual(10, rut["hi"], "value of hi");
        }

        [TestMethod]
        public void TestAddGoodMatchingMatricies()
        {
            var d1 = new Dictionary<object, object>();
            d1["hi"] = 10;
            var d2 = new Dictionary<object, object>();
            d2["hi"] = 7;

            var cl = new FunctionExpression("+", new ObjectValue(d1), new ObjectValue(d2));
            var c = new Context();
            var r = cl.Evaluate(c);
            Assert.IsInstanceOfType(r, typeof(Dictionary<object, object>), "type of result");
            var rut = r as Dictionary<object, object>;
            Assert.AreEqual(1, rut.Count, "# of items");
            Assert.AreEqual(17, rut["hi"], "value of hi");
        }

        [TestMethod]
        public void TestAddDisjointGoodMatricies()
        {
            var d1 = new Dictionary<object, object>();
            d1["hi"] = 10;
            var d2 = new Dictionary<object, object>();
            d2["there"] = 20;

            var cl = new FunctionExpression("+", new ObjectValue(d1), new ObjectValue(d2));
            var c = new Context();
            var r = cl.Evaluate(c);
            Assert.IsInstanceOfType(r, typeof(Dictionary<object, object>), "type of result");
            var rut = r as Dictionary<object, object>;
            Assert.AreEqual(2, rut.Count, "# of items");
            Assert.AreEqual(10, rut["hi"], "value of hi");
            Assert.AreEqual(20, rut["there"], "value of hi");
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

        [TestMethod]
        public void TestMultiplyByEmptyMatrix()
        {
            var d1 = new Dictionary<object, object>();
            d1["hi"] = 5.0;
            var d2 = new Dictionary<object, object>();
            var cl = new FunctionExpression("*", new ObjectValue(d1), new ObjectValue(d2));
            var c = new Context();
            var rut = cl.Evaluate(c);
            Assert.IsNotNull(rut, "Result null");
            Assert.IsInstanceOfType(rut, typeof(Dictionary<object, object>), "Type of result");
            var r = rut as Dictionary<object, object>;
            Assert.AreEqual(0, r.Count, "# of items in resulting dict");
        }

        [TestMethod]
        public void TestMultiplyBySimilarMatrix()
        {
            var d1 = new Dictionary<object, object>();
            d1["hi"] = 5.0;
            var d2 = new Dictionary<object, object>();
            d2["hi"] = 5.0;
            var cl = new FunctionExpression("*", new ObjectValue(d1), new ObjectValue(d2));
            var c = new Context();
            var rut = cl.Evaluate(c);
            Assert.IsNotNull(rut, "Result null");
            Assert.IsInstanceOfType(rut, typeof(Dictionary<object, object>), "Type of result");
            var r = rut as Dictionary<object, object>;
            Assert.AreEqual(25.0, r["hi"], "Resulting value");
        }

        [TestMethod]
        public void TestMultiplyByDisjointMatrix1()
        {
            var d1 = new Dictionary<object, object>();
            d1["hi"] = 5.0;
            var d2 = new Dictionary<object, object>();
            d2["hi"] = 5.0;
            d2["there"] = 10.0;
            var cl = new FunctionExpression("*", new ObjectValue(d1), new ObjectValue(d2));
            var c = new Context();
            var rut = cl.Evaluate(c);
            Assert.IsNotNull(rut, "Result null");
            Assert.IsInstanceOfType(rut, typeof(Dictionary<object, object>), "Type of result");
            var r = rut as Dictionary<object, object>;
            Assert.AreEqual(25.0, r["hi"], "Resulting value");
        }

        [TestMethod]
        public void TestMultiplyByDisjointMatrix2()
        {
            var d1 = new Dictionary<object, object>();
            d1["hi"] = 5.0;
            d1["there"] = 10.0;
            var d2 = new Dictionary<object, object>();
            d2["hi"] = 5.0;
            var cl = new FunctionExpression("*", new ObjectValue(d1), new ObjectValue(d2));
            var c = new Context();
            var rut = cl.Evaluate(c);
            Assert.IsNotNull(rut, "Result null");
            Assert.IsInstanceOfType(rut, typeof(Dictionary<object, object>), "Type of result");
            var r = rut as Dictionary<object, object>;
            Assert.AreEqual(25.0, r["hi"], "Resulting value");
        }

        [TestMethod]
        public void TestDivideByConstant1()
        {
            var d = new Dictionary<object, object>();
            d["hi"] = 5.0;
            var cl = new FunctionExpression("/", new ObjectValue(d), new DoubleValue(1.5));
            var c = new Context();
            var rut = cl.Evaluate(c);
            Assert.IsNotNull(rut, "Result null");
            Assert.IsInstanceOfType(rut, typeof(Dictionary<object, object>), "Type of result");
            var r = rut as Dictionary<object, object>;

            // Make sure the original isn't touched, as well as the final one updated
            Assert.AreEqual(5.0, d["hi"], "original value");
            Assert.AreEqual(5.0 / 1.5, r["hi"], "result");
        }

        [TestMethod]
        public void TestDivideByConstant2()
        {
            var d = new Dictionary<object, object>();
            d["hi"] = 5.0;
            var cl = new FunctionExpression("/", new DoubleValue(1.5), new ObjectValue(d));
            var c = new Context();
            var rut = cl.Evaluate(c);
            Assert.IsNotNull(rut, "Result null");
            Assert.IsInstanceOfType(rut, typeof(Dictionary<object, object>), "Type of result");
            var r = rut as Dictionary<object, object>;

            // Make sure the original isn't touched, as well as the final one updated
            Assert.AreEqual(5.0, d["hi"], "original value");
            Assert.AreEqual(1.5 / 5.0, r["hi"], "result");
        }

        [TestMethod]
        public void TestDivideByEmptyMatrixNum()
        {
            var d1 = new Dictionary<object, object>();
            var d2 = new Dictionary<object, object>();
            d2["hi"] = 5.0;
            var cl = new FunctionExpression("/", new ObjectValue(d1), new ObjectValue(d2));
            var c = new Context();
            var rut = cl.Evaluate(c);
            Assert.IsNotNull(rut, "Result null");
            Assert.IsInstanceOfType(rut, typeof(Dictionary<object, object>), "Type of result");
            var r = rut as Dictionary<object, object>;
            Assert.AreEqual(0, r.Count, "# of items in resulting dict");
        }

        [TestMethod]
        public void TestDivideByEmptyMatrix()
        {
            var d1 = new Dictionary<object, object>();
            var d2 = new Dictionary<object, object>();
            var cl = new FunctionExpression("/", new ObjectValue(d1), new ObjectValue(d2));
            var c = new Context();
            var rut = cl.Evaluate(c);
            Assert.IsNotNull(rut, "Result null");
            Assert.IsInstanceOfType(rut, typeof(Dictionary<object, object>), "Type of result");
            var r = rut as Dictionary<object, object>;
            Assert.AreEqual(0, r.Count, "# of items in resulting dict");
        }

        [TestMethod]
        [ExpectedException(typeof(System.DivideByZeroException))]
        public void TestDivideByEmptyMatrixDenom()
        {
            var d1 = new Dictionary<object, object>();
            d1["hi"] = 5.0;
            var d2 = new Dictionary<object, object>();
            var cl = new FunctionExpression("/", new ObjectValue(d1), new ObjectValue(d2));
            var c = new Context();
            var rut = cl.Evaluate(c);
            Assert.IsNotNull(rut, "Result null");
            Assert.IsInstanceOfType(rut, typeof(Dictionary<object, object>), "Type of result");
            var r = rut as Dictionary<object, object>;
            Assert.AreEqual(0, r.Count, "# of items in resulting dict");
        }

        [TestMethod]
        public void TestDivideBySimilarMatrix()
        {
            var d1 = new Dictionary<object, object>();
            d1["hi"] = 5.0;
            var d2 = new Dictionary<object, object>();
            d2["hi"] = 5.0;
            var cl = new FunctionExpression("/", new ObjectValue(d1), new ObjectValue(d2));
            var c = new Context();
            var rut = cl.Evaluate(c);
            Assert.IsNotNull(rut, "Result null");
            Assert.IsInstanceOfType(rut, typeof(Dictionary<object, object>), "Type of result");
            var r = rut as Dictionary<object, object>;
            Assert.AreEqual(1.0, r["hi"], "Resulting value");
        }

        [TestMethod]
        [ExpectedException(typeof(DivideByZeroException))]
        public void TestDivideByDisjointMatrixNum()
        {
            var d1 = new Dictionary<object, object>();
            d1["hi"] = 5.0;
            var d2 = new Dictionary<object, object>();
            d2["hi"] = 5.0;
            d2["there"] = 10.0;
            var cl = new FunctionExpression("/", new ObjectValue(d2), new ObjectValue(d1));
            var c = new Context();
            var rut = cl.Evaluate(c);
        }

        [TestMethod]
        public void TestDivideByDisjointMatrixDenom()
        {
            var d1 = new Dictionary<object, object>();
            d1["hi"] = 5.0;
            var d2 = new Dictionary<object, object>();
            d2["hi"] = 5.0;
            d2["there"] = 10.0;
            var cl = new FunctionExpression("/", new ObjectValue(d1), new ObjectValue(d2));
            var c = new Context();
            var rut = cl.Evaluate(c);
            Assert.IsNotNull(rut, "Result null");
            Assert.IsInstanceOfType(rut, typeof(Dictionary<object, object>), "Type of result");
            var r = rut as Dictionary<object, object>;
            Assert.AreEqual(1.0, r["hi"], "Resulting value");
        }
    }
}
