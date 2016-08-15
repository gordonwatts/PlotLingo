using Microsoft.VisualStudio.TestTools.UnitTesting;
using PlotLingoLib;
using PlotLingoLib.Expressions;
using PlotLingoLib.Expressions.Values;
using System;
using System.Collections.Generic;
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
        public void CallWithConversion()
        {
            var ctx = new RootContext();
            ctx.SetVariableValue("p", new testClass());
            var s = new IntegerValue(5);
            var mc = new MethodCallExpression(new VariableValue("p"), new FunctionExpression("CallWithShort", s));
            var r = mc.Evaluate(ctx);
            Assert.AreEqual("5", r);
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
            var mc = new MethodCallExpression(new VariableValue("p"), new FunctionExpression("OneWithDefaultStatic", s));
            var r = mc.Evaluate(ctx);
            Assert.AreEqual("hithere", r as string);
        }

        [TestMethod]
        public void TestExtensionMethodWithDefaultParam()
        {
            var ctx = new RootContext();
            ctx.SetVariableValue("p", new testClass());
            var s = new StringValue("hithere");
            var mc = new MethodCallExpression(new VariableValue("p"), new FunctionExpression("OneWithDefaultStatic"));
            var r = mc.Evaluate(ctx);
            Assert.AreEqual("hi", r as string);
        }

        [TestMethod]
        public void TestExtensionMethodWithMultiDefinition()
        {
            var ctx = new RootContext();
            ctx.SetVariableValue("p", new testClass());
            var s = new IntegerValue(10);
            var mc = new MethodCallExpression(new VariableValue("p"), new FunctionExpression("CommonNameWithDefaultStatic", s));
            var r = mc.Evaluate(ctx);
            Assert.AreEqual("10", r as string);
        }

        [TestMethod]
        public void TestExtensionMethodWithMultiDefinitionDefaultArg()
        {
            var ctx = new RootContext();
            ctx.SetVariableValue("p", new testClass());
            var s = new StringValue("noway");
            var mc = new MethodCallExpression(new VariableValue("p"), new FunctionExpression("CommonNameWithDefaultStatic", s));
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
            var mc = new MethodCallExpression(new VariableValue("p"), new FunctionExpression("TwoArgumentMethodStatic", s));
            var r = mc.Evaluate(ctx);
        }

        [TestMethod]
        public void TestMethodWithDefaultParamFilled()
        {
            var ctx = new RootContext();
            ctx.SetVariableValue("p", new testClass());
            var s = new StringValue("hithere");
            var mc = new MethodCallExpression(new VariableValue("p"), new FunctionExpression("OneWithDefault", s));
            var r = mc.Evaluate(ctx);
            Assert.AreEqual("hithere", r as string);
        }

        [TestMethod]
        public void TestMethodWithDefaultParam()
        {
            var ctx = new RootContext();
            ctx.SetVariableValue("p", new testClass());
            var s = new StringValue("hithere");
            var mc = new MethodCallExpression(new VariableValue("p"), new FunctionExpression("OneWithDefault"));
            var r = mc.Evaluate(ctx);
            Assert.AreEqual("hi", r as string);
        }

        [TestMethod]
        public void TestMethodWithMultiDefinition()
        {
            var ctx = new RootContext();
            ctx.SetVariableValue("p", new testClass());
            var s = new IntegerValue(10);
            var mc = new MethodCallExpression(new VariableValue("p"), new FunctionExpression("CommonNameWithDefault", s));
            var r = mc.Evaluate(ctx);
            Assert.AreEqual("10", r as string);
        }

        [TestMethod]
        public void TestMethodWithMultiDefinitionDefaultArg()
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
        public void TestMethodMissingRequiredParam()
        {
            var ctx = new RootContext();
            ctx.SetVariableValue("p", new testClass());
            var s = new StringValue("noway");
            var mc = new MethodCallExpression(new VariableValue("p"), new FunctionExpression("TwoArgumentMethod", s));
            var r = mc.Evaluate(ctx);
        }

        [TestMethod]
        public void TestMethodWithMultiSignatureLowerClass()
        {
            var ctx = new RootContext();
            ctx.SetVariableValue("p", new testClass());
            var s = new ObjectValue(new t1() { i = 10 });
            var mc = new MethodCallExpression(new VariableValue("p"), new FunctionExpression("MultiSignatureHierarchy", s));
            var r = mc.Evaluate(ctx);
            Assert.AreEqual(10, (int)r);
        }

        [TestMethod]
        public void TestMethodWithMultiSignatureHigherClass()
        {
            var ctx = new RootContext();
            ctx.SetVariableValue("p", new testClass());
            var s = new ObjectValue(new t2() { i = 10, i2 = 20 });
            var mc = new MethodCallExpression(new VariableValue("p"), new FunctionExpression("MultiSignatureHierarchy", s));
            var r = mc.Evaluate(ctx);
            Assert.AreEqual(20, (int)r);
        }

        [TestMethod]
        public void TestExtensionMethodWithMultiSignatureLowerClass()
        {
            var ctx = new RootContext();
            ctx.SetVariableValue("p", new testClass());
            var s = new ObjectValue(new t1() { i = 10 });
            var mc = new MethodCallExpression(new VariableValue("p"), new FunctionExpression("MultiSignatureHierarchyStatic", s));
            var r = mc.Evaluate(ctx);
            Assert.AreEqual(10, (int)r);
        }

        [TestMethod]
        public void TestExtensionMethodWithMultiSignatureHigherClass()
        {
            var ctx = new RootContext();
            ctx.SetVariableValue("p", new testClass());
            var s = new ObjectValue(new t2() { i = 10, i2 = 20 });
            var mc = new MethodCallExpression(new VariableValue("p"), new FunctionExpression("MultiSignatureHierarchyStatic", s));
            var r = mc.Evaluate(ctx);
            Assert.AreEqual(20, (int)r);
        }

        [TestMethod]
        public void TestMethodWithMultiSignatureObjectInheritanceLowerClass()
        {
            var ctx = new RootContext();
            ctx.SetVariableValue("p", new testClass());
            var s = new ObjectValue(new object[10] as IEnumerable<object>);
            var mc = new MethodCallExpression(new VariableValue("p"), new FunctionExpression("MultiSignatureHierarchyWithSig", s));
            var r = mc.Evaluate(ctx);
            Assert.AreEqual(10, (int)r);
        }

        [TestMethod]
        public void TestMethodWithMultiSignatureObjectInheritanceHigherClass()
        {
            var ctx = new RootContext();
            ctx.SetVariableValue("p", new testClass());
            var s = new ObjectValue(new Dictionary<object, object>() as IDictionary<object, object>);
            var mc = new MethodCallExpression(new VariableValue("p"), new FunctionExpression("MultiSignatureHierarchyWithSig", s));
            var r = mc.Evaluate(ctx);
            Assert.AreEqual(20, (int)r);
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

        private class t1
        {
            public int i;
        }

        private class t2 : t1
        {
            public int i2;
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

            // Test out two guys, who are sub-classes to make sure the most specific is called.
            public int MultiSignatureHierarchy(t1 c)
            {
                return c.i;
            }

            public int MultiSignatureHierarchy(t2 c)
            {
                return c.i2;
            }

            public int MultiSignatureHierarchyWithSig(IEnumerable<object> obj)
            {
                return 10;
            }

            public int MultiSignatureHierarchyWithSig(IDictionary<object, object> obj)
            {
                return 20;
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

            public string CallWithShort(short i)
            {
                return i.ToString();
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

            public static string OneWithDefaultStatic(testClass a, string value = "hi")
            {
                return value;
            }

            public static string CommonNameWithDefaultStatic(testClass a, int i)
            {
                return i.ToString();
            }
            public static string CommonNameWithDefaultStatic(testClass a, string arg, int value = 10)
            {
                return arg + value.ToString();
            }

            public static string TwoArgumentMethodStatic(testClass a, string arg, int j)
            {
                return arg + j.ToString();
            }

            // Test out two guys, who are sub-classes to make sure the most specific is called.
            public static int MultiSignatureHierarchyStatic(testClass a, t1 c)
            {
                return c.i;
            }

            public static int MultiSignatureHierarchyStatic(testClass a, t2 c)
            {
                return c.i2;
            }
        }
    }
}
