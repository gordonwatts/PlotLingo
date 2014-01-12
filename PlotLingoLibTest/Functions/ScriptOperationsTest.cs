using Microsoft.VisualStudio.TestTools.UnitTesting;
using PlotLingoLib;
using PlotLingoLib.Expressions;
using PlotLingoLib.Expressions.Values;
using System.IO;

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

        /// <summary>
        /// Make sure that the file can be found when it is in the same dir as the script, even if it isn't
        /// the current directory.
        /// </summary>
        [TestMethod]
        [DeploymentItem("Functions/LoadFileWithSideEffects.plotlingo")]
        public void LoadFromMainScriptDirectory()
        {
            if (Directory.Exists("dummy"))
                Directory.Delete("dummy", true);

            var dirinfo = Directory.CreateDirectory("dummy");

            File.Copy("LoadFileWithSideEffects.plotlingo", @"dummy\effects.plotlingo");

            var f = new FunctionExpression("include", new IExpression[] { new StringValue("effects.plotlingo") });
            var c = new Context();
            c.ScriptFileContextPush(new FileInfo(string.Format(@"{0}\bogus.plotlingo", dirinfo.FullName)));
            var r = f.Evaluate(c);
            Assert.AreEqual(5, c.GetVariableValue("i"), "variable i");
        }

        /// <summary>
        /// When we load a file, after done loading, the script directory shouldn't have changed.
        /// </summary>
        [TestMethod]
        public void ScriptFileSameWhenRunning()
        {
            Assert.Inconclusive();
        }

        /// <summary>
        /// When we are running another script, the current script file should be reset.
        /// </summary>
        [TestMethod]
        public void ScriptFileSetCorrectlyDuringLoad()
        {
            Assert.Inconclusive();
        }
    }
}
