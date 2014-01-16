using Microsoft.VisualStudio.TestTools.UnitTesting;
using PlotLingoLib;
using PlotLingoLib.Expressions;
using PlotLingoLib.Expressions.Values;
using System;
using System.ComponentModel.Composition;

namespace PlotLingoLibTest.Expressions
{
    [TestClass]
    public class FunctionExpressionTest
    {
        [TestMethod]
        public void TestBasicCalling()
        {
            var fo = new FunctionExpression("GetMe", new IExpression[] { new StringValue("hi there") });
            var c = new RootContext();
            var r = fo.Evaluate(c);
            Assert.AreEqual(8, r, "function result");
        }

        [TestMethod]
        public void TestFunctionCallback()
        {
            var fo = new FunctionExpression("GetMe", new IExpression[] { new StringValue("hi there") });
            var c = new RootContext();
            int count = 0;
            c.ExecutionContext.AddPostCallHook("GetMe", "test", (shouldbenull, result) =>
            {
                Assert.IsNull(shouldbenull, "function call should be null obj");
                Assert.AreEqual(8, result, "Result in call back");
                count++;
                return result;
            });
            var r = fo.Evaluate(c);
            Assert.AreEqual(1, count, "# of times the callback was called");
        }

        [TestMethod]
        public void TestFunctionCallWithContextNoArg()
        {
            var fo = new FunctionExpression("GetMeContext", new IExpression[] { });
            var c = new RootContext();
            var r = fo.Evaluate(c);
            Assert.AreEqual(12, r, "function result");
        }

        [TestMethod]
        public void TestAddCommutation1()
        {
            var fo = new FunctionExpression("+", new IExpression[] { new VariableValue("p"), new IntegerValue(7) });
            var c = new RootContext();
            c.SetVariableValue("p", new OperatorTestObjects());
            var r = fo.Evaluate(c);
            Assert.AreEqual(9, r, "Operator result");
        }

        [TestMethod]
        public void TestAddCommutation2()
        {
            var fo = new FunctionExpression("+", new IExpression[] { new IntegerValue(7), new VariableValue("p") });
            var c = new RootContext();
            c.SetVariableValue("p", new OperatorTestObjects());
            var r = fo.Evaluate(c);
            Assert.AreEqual(9, r, "Operator result");
        }

        [TestMethod]
        public void TestMultiplyCommutation1()
        {
            var fo = new FunctionExpression("*", new IExpression[] { new VariableValue("p"), new IntegerValue(7) });
            var c = new RootContext();
            c.SetVariableValue("p", new OperatorTestObjects());
            var r = fo.Evaluate(c);
            Assert.AreEqual(14, r, "Operator result");
        }

        [TestMethod]
        public void TestMultiplyCommutation2()
        {
            var fo = new FunctionExpression("*", new IExpression[] { new IntegerValue(7), new VariableValue("p") });
            var c = new RootContext();
            c.SetVariableValue("p", new OperatorTestObjects());
            var r = fo.Evaluate(c);
            Assert.AreEqual(14, r, "Operator result");
        }

        [TestMethod]
        public void TestSubtractCommutation1()
        {
            var fo = new FunctionExpression("-", new IExpression[] { new VariableValue("p"), new IntegerValue(7) });
            var c = new RootContext();
            c.SetVariableValue("p", new OperatorTestObjects());
            var r = fo.Evaluate(c);
            Assert.AreEqual(5, r, "Operator result");
        }

        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void TestSubtractCommutation2()
        {
            var fo = new FunctionExpression("-", new IExpression[] { new IntegerValue(7), new VariableValue("p") });
            var c = new RootContext();
            c.SetVariableValue("p", new OperatorTestObjects());
            var r = fo.Evaluate(c);
        }

        [TestMethod]
        public void TestDivideCommutation1()
        {
            var fo = new FunctionExpression("/", new IExpression[] { new VariableValue("p"), new IntegerValue(8) });
            var c = new RootContext();
            c.SetVariableValue("p", new OperatorTestObjects());
            var r = fo.Evaluate(c);
            Assert.AreEqual(4, r, "Operator result");
        }

        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void TestDivideCommutation2()
        {
            var fo = new FunctionExpression("/", new IExpression[] { new IntegerValue(7), new VariableValue("p") });
            var c = new RootContext();
            c.SetVariableValue("p", new OperatorTestObjects());
            var r = fo.Evaluate(c);
        }

        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void TestPlusNotDefined()
        {
            var fo = new FunctionExpression("/", new IExpression[] { new IntegerValue(7), new VariableValue("p") });
            var c = new RootContext();
            c.SetVariableValue("p", new OperatorTestNotDefined());
            var r = fo.Evaluate(c);
        }
    }

    /// <summary>
    /// Some test objects for the function above.
    /// </summary>
    [Export(typeof(IFunctionObject))]
    public class FunctionObjectTests : IFunctionObject
    {
        static public int GetMe(string arg)
        {
            return arg.Length;
        }

        static public int GetMeContext(RootContext c)
        {
            if (c == null)
                return 0;
            return 12;
        }
    }

    /// <summary>
    /// Objects used to test operators and how we can permute the operands.
    /// </summary>
    [Export(typeof(IFunctionObject))]
    public class OperatorTestObjects : IFunctionObject
    {
        static public int OperatorPlus(OperatorTestObjects obj, int val)
        {
            return val + 2;
        }

        static public int OperatorMultiply(OperatorTestObjects obj, int val)
        {
            return val * 2;
        }

        static public int OperatorMinus(OperatorTestObjects obj, int val)
        {
            return val - 2;
        }

        static public int OperatorDivide(OperatorTestObjects obj, int val)
        {
            return val / 2;
        }
    }

    public class OperatorTestNotDefined
    {

    }
}
