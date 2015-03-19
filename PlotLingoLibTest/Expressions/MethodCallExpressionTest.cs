using Microsoft.VisualStudio.TestTools.UnitTesting;
using PlotLingoLib;
using PlotLingoLib.Expressions;
using PlotLingoLib.Expressions.Values;
using System;
using System.ComponentModel.Composition;

namespace PlotLingoLibTest.Expressions
{
    [TestClass]
    public class MethodCallExpressionTest
    {

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestBadCall()
        {
            var ctx = new RootContext();
            ctx.SetVariableValue("p", new testClass());
            var mc = new MethodCallExpression(new VariableValue("p"), new FunctionExpression("CallNoArgsBogus", new IExpression[] { }));
            var r = mc.Evaluate(ctx);
            Assert.AreEqual(5, r);
        }

        /// <summary>
        /// Simple call with nothing else going on.
        /// </summary>
        [TestMethod]
        public void TestCallNoArgs()
        {
            var ctx = new RootContext();
            ctx.SetVariableValue("p", new testClass());
            var mc = new MethodCallExpression(new VariableValue("p"), new FunctionExpression("CallNoArgs", new IExpression[] { }));
            var r = mc.Evaluate(ctx);
            Assert.AreEqual(5, r);
        }

        [TestMethod]
        public void TestCallOneArg()
        {
            var ctx = new RootContext();
            ctx.SetVariableValue("p", new testClass());
            var s = new StringValue("length");
            var mc = new MethodCallExpression(new VariableValue("p"), new FunctionExpression("CallOneStringArg", new IExpression[] { s }));
            var r = mc.Evaluate(ctx);
            Assert.AreEqual(6, r);
        }

        [TestMethod]
        public void TestMethodOnObjectPostHookCallback()
        {
            var tc = new testClass();
            var ctx = new RootContext();
            int count = 0;
            ctx.ExecutionContext.AddPostCallHook("CallNoArgs", "test", (obj, result) =>
            {
                count++;
                Assert.AreEqual(tc, obj, "object that is passed in as central object");
                Assert.AreEqual(5, result, "Result not passed correctly");
                return result;
            });
            ctx.SetVariableValue("p", tc);
            var mc = new MethodCallExpression(new VariableValue("p"), new FunctionExpression("CallNoArgs", new IExpression[] { }));
            var r = mc.Evaluate(ctx);
            Assert.AreEqual(1, count, "# of times the callback was called");
        }

        [TestMethod]
        public void TestMethodCallbackAlterResult()
        {
            var tc = new testClass();
            var ctx = new RootContext();
            ctx.ExecutionContext.AddPostCallHook("CallNoArgs", "test", (obj, result) =>
            {
                return 33;
            });
            ctx.SetVariableValue("p", tc);
            var mc = new MethodCallExpression(new VariableValue("p"), new FunctionExpression("CallNoArgs", new IExpression[] { }));
            var r = mc.Evaluate(ctx);
            Assert.AreEqual(33, r, "Altered result");
        }

        /// <summary>
        /// Make sure that arguments to an expression are evaluated only once.
        /// </summary>
        [TestMethod]
        public void TestArgumentsEvaluatedOnlyOnce()
        {
            var ctx = new RootContext();
            ctx.SetVariableValue("p", new testClass());
            var s = new MyStringExpression();
            var mc = new MethodCallExpression(new VariableValue("p"), new FunctionExpression("CallOneStringArg", new IExpression[] { s }));
            var r = mc.Evaluate(ctx);
            Assert.AreEqual(1, s._evalCount, "# of times evaluated");
        }

        [TestMethod]
        public void TestPropertyGet()
        {
            var ctx = new RootContext();
            var tc = new testClass();
            tc.MyProp = 10;
            ctx.SetVariableValue("p", tc);
            var mc = new MethodCallExpression(new VariableValue("p"), new FunctionExpression("MyProp"));
            var r = mc.Evaluate(ctx);
            Assert.IsInstanceOfType(r, typeof(int));
            Assert.AreEqual(10, r);
        }

        [TestMethod]
        public void TestPropertySet()
        {
            var ctx = new RootContext();
            var tc = new testClass();
            tc.MyProp = 10;
            ctx.SetVariableValue("p", tc);
            var mc = new MethodCallExpression(new VariableValue("p"), new FunctionExpression("MyProp", new IntegerValue(20)));
            var r = mc.Evaluate(ctx);
            Assert.IsInstanceOfType(r, typeof(testClass));
            Assert.AreEqual(tc, r);
            Assert.AreEqual(20, tc.MyProp);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestPropertySetBadType()
        {
            var ctx = new RootContext();
            var tc = new testClass();
            tc.MyProp = 10;
            ctx.SetVariableValue("p", tc);
            var mc = new MethodCallExpression(new VariableValue("p"), new FunctionExpression("MyProp", new StringValue("hi there")));
            var r = mc.Evaluate(ctx);
            Assert.IsInstanceOfType(r, typeof(testClass));
            Assert.AreEqual(tc, r);
            Assert.AreEqual(20, tc.MyProp);
        }

        [TestMethod]
        public void TestExtensionMethod()
        {
            var ctx = new RootContext();
            ctx.SetVariableValue("p", new testClass());
            var s = new StringValue("hi");
            var mc = new MethodCallExpression(new VariableValue("p"), new FunctionExpression("CallOneStringArgExt", new IExpression[] { s }));
            var r = mc.Evaluate(ctx);
            Assert.AreEqual(4, r, "2*length of string");
        }

        [TestMethod]
        public void TestExtensionMethodWithContext()
        {
            var ctx = new RootContext();
            ctx.SetVariableValue("p", new testClass());
            var s = new StringValue("hi");
            var mc = new MethodCallExpression(new VariableValue("p"), new FunctionExpression("CallOneStringWithCTX", new IExpression[] { s }));
            var r = mc.Evaluate(ctx);
            Assert.AreEqual(6, r, "3*length of string");
        }

        [TestMethod]
        public void TestExtensionMethodOverridesSameObjectGuy()
        {
            var ctx = new RootContext();
            ctx.SetVariableValue("p", new testClass());
            var s = new StringValue("hi");
            var mc = new MethodCallExpression(new VariableValue("p"), new FunctionExpression("CallOneStringToOverride", new IExpression[] { s }));
            var r = mc.Evaluate(ctx);
            Assert.AreEqual(4, r, "2*length of string");
        }

        [TestMethod]
        public void TestPostMethodHookCallInExtensionMethod()
        {
            var ctx = new RootContext();
            var tc = new testClass();
            ctx.SetVariableValue("p", tc);
            int count = 0;
            ctx.ExecutionContext.AddPostCallHook("CallOneStringArgExt", "test", (obj, result) =>
            {
                count++;
                Assert.AreEqual(tc, obj, "Object that is getting the callb ack");
                Assert.AreEqual(4, result, "Result of callback");
                return result;
            });
            var s = new StringValue("hi");
            var mc = new MethodCallExpression(new VariableValue("p"), new FunctionExpression("CallOneStringArgExt", new IExpression[] { s }));
            var r = mc.Evaluate(ctx);
            Assert.AreEqual(1, count, "# of times the hook got called");
        }

        [TestMethod]
        public void TestPostMethod2HookCallInExtensionMethod()
        {
            var ctx = new RootContext();
            ctx.SetVariableValue("p", new testClass());
            int count1 = 0;
            int count2 = 0;
            ctx.ExecutionContext.AddPostCallHook("CallOneStringArgExt", "test1", (o, obj) => count1++);
            ctx.ExecutionContext.AddPostCallHook("CallOneStringArgExt", "test2", (o, obj) => count2++);
            var s = new StringValue("hi");
            var mc = new MethodCallExpression(new VariableValue("p"), new FunctionExpression("CallOneStringArgExt", new IExpression[] { s }));
            var r = mc.Evaluate(ctx);
            Assert.AreEqual(1, count1, "# of times the first hook got called");
            Assert.AreEqual(1, count2, "# of times the second hook got called");
        }

        [TestMethod]
        public void TestExtensionMethodWithDefaultParamFilled()
        {
            var ctx = new RootContext();
            ctx.SetVariableValue("p", new testClass());
            var s = new StringValue("hithere");
            var mc = new MethodCallExpression(new VariableValue("p"), new FunctionExpression("OneWithDefault", s));
            var r = mc.Evaluate(ctx);
            Assert.AreEqual("hithere", r as string);
        }

        [TestMethod]
        public void TestExtensionMethodWithDefaultParam()
        {
            var ctx = new RootContext();
            ctx.SetVariableValue("p", new testClass());
            var s = new StringValue("hithere");
            var mc = new MethodCallExpression(new VariableValue("p"), new FunctionExpression("OneWithDefault"));
            var r = mc.Evaluate(ctx);
            Assert.AreEqual("hi", r as string);
        }

        [TestMethod]
        public void TestExtensionMethodWithMultiDefinition()
        {
            var ctx = new RootContext();
            ctx.SetVariableValue("p", new testClass());
            var s = new IntegerValue(10);
            var mc = new MethodCallExpression(new VariableValue("p"), new FunctionExpression("CommonNameWithDefault", s));
            var r = mc.Evaluate(ctx);
            Assert.AreEqual("10", r as string);
        }

        [TestMethod]
        public void TestExtensionMethodWithMultiDefinitionDefaultArg()
        {
            var ctx = new RootContext();
            ctx.SetVariableValue("p", new testClass());
            var s = new StringValue("noway");
            var mc = new MethodCallExpression(new VariableValue("p"), new FunctionExpression("CommonNameWithDefault", s));
            var r = mc.Evaluate(ctx);
            Assert.AreEqual("noway10", r as string);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestExtensionMethodMissingRequiredParam()
        {
            var ctx = new RootContext();
            ctx.SetVariableValue("p", new testClass());
            var s = new StringValue("noway");
            var mc = new MethodCallExpression(new VariableValue("p"), new FunctionExpression("TwoArgumentMethod", s));
            var r = mc.Evaluate(ctx);
        }

        /// <summary>
        /// Small expression class that will hold onto a string and count the number of times
        /// it is evaluated.
        /// </summary>
        private class MyStringExpression : IExpression
        {
            public int _evalCount = 0;
            public object Evaluate(IScopeContext c)
            {
                _evalCount++;
                return "Length";
            }
        }

        /// <summary>
        /// Test class to help with... testing.
        /// </summary>
        private class testClass
        {
            public int CallNoArgs()
            {
                return 5;
            }

            public int CallOneStringArg(string hi)
            {
                return hi.Length;
            }

            // Will be overridden by below
            public int CallOneStringToOverride(string hi)
            {
                return hi.Length;
            }

            // Simple property
            public int MyProp { get; set; }

            public string OneWithDefault(string value = "hi")
            {
                return value;
            }

            public string CommonNameWithDefault(int i)
            {
                return i.ToString();
            }
            public string CommonNameWithDefault(string arg, int value = 10)
            {
                return arg + value.ToString();
            }

            public string TwoArgumentMethod(string arg, int j)
            {
                return arg + j.ToString();
            }
        }

        [Export(typeof(IFunctionObject))]
        private class testClassOverrides : IFunctionObject
        {
            public static int CallOneStringToOverride(testClass a, string hi)
            {
                return 2 * hi.Length;
            }

            public static int CallOneStringArgExt(testClass a, string hi)
            {
                return 2 * hi.Length;
            }

            public static int CallOneStringWithCTX(RootContext ctx, testClass a, string hi)
            {
                return 3 * hi.Length;
            }
        }
    }
}
