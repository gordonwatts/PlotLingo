using System;
using Sprache;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PlotLingoLibTest
{
    [TestClass]
    public class GrammerTest
    {
        [TestMethod]
        public void TestFunctionCall()
        {
            var r = PlotLingoLib.Grammar.ModuleParser.End().Parse("a = file(\"hi\")");
            Assert.IsNotNull(r);
            Assert.AreEqual(1, r.Length, "# of statements");
            Assert.IsInstanceOfType(r[0], typeof(PlotLingoLib.Grammar.AssignmentStatement));
            var a = (PlotLingoLib.Grammar.AssignmentStatement)r[0];
        }
    }
}
