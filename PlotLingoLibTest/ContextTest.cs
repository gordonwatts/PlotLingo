using Microsoft.VisualStudio.TestTools.UnitTesting;
using PlotLingoLib;
using PlotLingoLib.Expressions.Values;
using System;
using System.IO;

namespace PlotLingoLibTest
{
    [TestClass]
    public class ContextTest
    {
        [TestMethod]
        public void TestGetSet()
        {
            var c = new Context();
            c.SetVariableValue("p", 5);
            Assert.AreEqual(5, c.GetVariableValue("p"), "get p");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestBadGet()
        {
            var c = new Context();
            c.GetVariableValue("p");
        }

        [TestMethod]
        public void RegisterExprEvalWorks()
        {
            var c = new Context();
            object r = null;
            Action<object> saver = o => r = o;
            var es = new PlotLingoLib.Statements.ExpressionStatement(new StringValue("hi"));

            c.AddExpressionStatementEvaluationCallback(saver);
            es.Evaluate(c);
            Assert.AreEqual("hi", r, "result of running");
        }

        [TestMethod]
        public void RegisterAndRemoveExprEvalWorks()
        {
            var c = new Context();
            object r = null;
            Action<object> saver = o => r = o;
            var es = new PlotLingoLib.Statements.ExpressionStatement(new StringValue("hi"));

            c.AddExpressionStatementEvaluationCallback(saver);
            c.RemoveExpressionStatementEvaluationCallback(saver);
            es.Evaluate(c);
            Assert.IsNull(r, "result of running");
        }

        [TestMethod]
        public void DefaultScriptIsInMemory()
        {
            var c = new Context();
            Assert.AreEqual("in memory", c.CurrentScriptFile.Name, "default script name");
        }

        [TestMethod]
        public void AfterEmptyStackDefaultScriptStillThere()
        {
            var c = new Context();
            c.ScriptFileContextPush(new FileInfo("dude.txt"));
            c.ScriptFileContextPop();
            Assert.AreEqual("in memory", c.CurrentScriptFile.Name, "default script name");
        }

        [TestMethod]
        public void ScriptFileContextChangesAfterUse()
        {
            var c = new Context();
            c.ScriptFileContextPush(new FileInfo("dude.txt"));
            Assert.AreEqual("dude.txt", c.CurrentScriptFile.Name, "filename");
        }

        [TestMethod]
        public void ExecutingScriptFalse()
        {
            var c = new Context();
            Assert.IsFalse(c.ExecutingScript, "false");
        }

        [TestMethod]
        public void ExecutingScriptTrue()
        {
            var c = new Context();
            c.ScriptFileContextPush(new FileInfo("dude.txt"));
            Assert.IsTrue(c.ExecutingScript, "false");
        }
    }
}
