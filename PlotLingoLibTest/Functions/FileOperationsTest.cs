using Microsoft.VisualStudio.TestTools.UnitTesting;
using PlotLingoLib;
using PlotLingoLib.Expressions.Values;

namespace PlotLingoLibTest.Functions
{
    [TestClass]
    public class FileOperationsTest
    {
        [TestMethod]
        [DeploymentItem("Functions/TestReadFile.txt")]
        public void TestReadFile()
        {
            var fs = new PlotLingoLib.Expressions.FunctionExpression("readfile", new StringValue("TestReadFile.txt"));
            var c = new Context();
            var r = fs.Evaluate(c);

            Assert.AreEqual("hi\r\nthere", r, "content of file");
        }
    }
}
