using Microsoft.VisualStudio.TestTools.UnitTesting;
using PlotLingoLib;
using PlotLingoLib.Expressions;
using PlotLingoLib.Expressions.Values;

namespace PlotLingoLibTest.Functions
{
    /// <summary>
    /// Test the code to load an evaluate scripts, and the like.
    /// </summary>
    [TestClass]
    public class ScriptOperationsTest
    {
        [TestMethod]
        public void TestEvalNumber()
        {
            var f = new FunctionExpression("eval", new IExpression[] { new StringValue("5;") });
            var c = new Context();
            var r = f.Evaluate(c);
            Assert.AreEqual(5, r, "simple eval");
        }

        [TestMethod]
        public void TestEvalWithContextUpdate()
        {
            var f = new FunctionExpression("eval", new IExpression[] { new StringValue("i=5;") });
            var c = new Context();
            var r = f.Evaluate(c);
            Assert.AreEqual(5, c.GetVariableValue("i"), "variable i");
        }

        [TestMethod]
        [DeploymentItem("Functions/LoadFileWithSideEffects.plotlingo")]
        public void LoadFileWithSideEffects()
        {
            var f = new FunctionExpression("include", new IExpression[] { new StringValue("LoadFileWithSideEffects.plotlingo") });
            var c = new Context();
            var r = f.Evaluate(c);
            Assert.AreEqual(5, c.GetVariableValue("i"), "variable i");
        }
    }
}
