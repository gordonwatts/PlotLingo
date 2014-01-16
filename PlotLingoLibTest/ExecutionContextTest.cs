using Microsoft.VisualStudio.TestTools.UnitTesting;
using PlotLingoLib;
using System.IO;

namespace PlotLingoLibTest
{
    [TestClass]
    public class ExecutionContextTest
    {
        [TestMethod]
        public void DefaultScriptIsInMemory()
        {
            var c = new ExecutionContext();
            Assert.AreEqual("in memory", c.CurrentScriptFile.Name, "default script name");
        }

        [TestMethod]
        public void AfterEmptyStackDefaultScriptStillThere()
        {
            var c = new ExecutionContext();
            c.ScriptFileContextPush(new FileInfo("dude.txt"));
            c.ScriptFileContextPop();
            Assert.AreEqual("in memory", c.CurrentScriptFile.Name, "default script name");
        }

        [TestMethod]
        public void ScriptFileContextChangesAfterUse()
        {
            var c = new ExecutionContext();
            c.ScriptFileContextPush(new FileInfo("dude.txt"));
            Assert.AreEqual("dude.txt", c.CurrentScriptFile.Name, "filename");
        }

        [TestMethod]
        public void ExecutingScriptFalse()
        {
            var c = new ExecutionContext();
            Assert.IsFalse(c.ExecutingScript, "false");
        }

        [TestMethod]
        public void ExecutingScriptTrue()
        {
            var c = new ExecutionContext();
            c.ScriptFileContextPush(new FileInfo("dude.txt"));
            Assert.IsTrue(c.ExecutingScript, "false");
        }
    }
}
