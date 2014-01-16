using Microsoft.VisualStudio.TestTools.UnitTesting;
using PlotLingoLib;
using PlotLingoLib.Expressions;
using PlotLingoLib.Expressions.Values;

namespace PlotLingoLibTest.Functions
{
    [TestClass]
    public class NumberOperationsTest
    {
        [TestMethod]
        public void TestAddInts()
        {
            var f = new FunctionExpression("+", new IExpression[] { new IntegerValue(4), new IntegerValue(5) });
            var c = new RootContext();
            var r = f.Evaluate(c);
            Assert.AreEqual(9, r, "4+5");
        }

        [TestMethod]
        public void TestSubtractInts()
        {
            var f = new FunctionExpression("-", new IExpression[] { new IntegerValue(4), new IntegerValue(5) });
            var c = new RootContext();
            var r = f.Evaluate(c);
            Assert.AreEqual(-1, r, "4+5");
        }

        [TestMethod]
        public void TestMultiplyInts()
        {
            var f = new FunctionExpression("*", new IExpression[] { new IntegerValue(4), new IntegerValue(5) });
            var c = new RootContext();
            var r = f.Evaluate(c);
            Assert.AreEqual(20, r, "4+5");
        }

        [TestMethod]
        public void TestDivideInts()
        {
            var f = new FunctionExpression("/", new IExpression[] { new IntegerValue(4), new IntegerValue(8) });
            var c = new RootContext();
            var r = f.Evaluate(c);
            Assert.AreEqual(0.5, r, "4+5");
        }

        [TestMethod]
        public void TestAddDoubles()
        {
            var f = new FunctionExpression("+", new IExpression[] { new DoubleValue(7.5), new DoubleValue(1.3) });
            var c = new RootContext();
            var r = f.Evaluate(c);
            Assert.AreEqual(8.8, r, "7.5+1.3");
        }

        [TestMethod]
        public void TestSubtractDoubles()
        {
            var f = new FunctionExpression("-", new IExpression[] { new DoubleValue(7.5), new DoubleValue(1.3) });
            var c = new RootContext();
            var r = f.Evaluate(c);
            Assert.AreEqual(6.2, r, "7.5+1.3");
        }

        [TestMethod]
        public void TestMultiplyDoubles()
        {
            var f = new FunctionExpression("*", new IExpression[] { new DoubleValue(1.1), new DoubleValue(2.0) });
            var c = new RootContext();
            var r = f.Evaluate(c);
            Assert.AreEqual(2.2, r, "7.5+1.3");
        }

        [TestMethod]
        public void TestDivideDoubles()
        {
            var f = new FunctionExpression("/", new IExpression[] { new DoubleValue(2.2), new DoubleValue(2.0) });
            var c = new RootContext();
            var r = f.Evaluate(c);
            Assert.AreEqual(1.1, r, "7.5+1.3");
        }

        [TestMethod]
        public void TestAddDoubleAndInt()
        {
            var f = new FunctionExpression("+", new IExpression[] { new DoubleValue(7.5), new IntegerValue(2) });
            var c = new RootContext();
            var r = f.Evaluate(c);
            Assert.AreEqual(9.5, r, "7.5+2");
        }

        [TestMethod]
        public void TestMultiplyDoubleAndInt()
        {
            var f = new FunctionExpression("*", new IExpression[] { new DoubleValue(7.5), new IntegerValue(2) });
            var c = new RootContext();
            var r = f.Evaluate(c);
            Assert.AreEqual(15.0, r, "7.5+2");
        }

        [TestMethod]
        public void TestSubtractDoubleAndInt1()
        {
            var f = new FunctionExpression("-", new IExpression[] { new DoubleValue(7.5), new IntegerValue(2) });
            var c = new RootContext();
            var r = f.Evaluate(c);
            Assert.AreEqual(5.5, r, "7.5+2");
        }

        [TestMethod]
        public void TestSubtractDoubleAndInt2()
        {
            var f = new FunctionExpression("-", new IExpression[] { new IntegerValue(2), new DoubleValue(7.5) });
            var c = new RootContext();
            var r = f.Evaluate(c);
            Assert.AreEqual(-5.5, r, "7.5+2");
        }

        [TestMethod]
        public void TestDivideDoubleAndInt1()
        {
            var f = new FunctionExpression("/", new IExpression[] { new DoubleValue(8), new IntegerValue(2) });
            var c = new RootContext();
            var r = f.Evaluate(c);
            Assert.AreEqual(4.0, r, "7.5+2");
        }

        [TestMethod]
        public void TestDivideDoubleAndInt2()
        {
            var f = new FunctionExpression("/", new IExpression[] { new IntegerValue(22), new DoubleValue(20) });
            var c = new RootContext();
            var r = f.Evaluate(c);
            Assert.AreEqual(1.1, r, "7.5+2");
        }
    }
}
