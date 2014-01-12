using Microsoft.VisualStudio.TestTools.UnitTesting;
using PlotLingoLib;
using PlotLingoLib.Expressions.Values;
using System.IO;

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

        /// <summary>
        /// Place the file in a directory that is relative to the script, not to where we are running.
        /// </summary>
        [TestMethod]
        [DeploymentItem("Functions/TestReadFile.txt")]
        public void ReadFromFileNearPlotLingoScript()
        {
            if (Directory.Exists("ReadFromScriptFile"))
                Directory.Delete("ReadFromScriptFile", true);

            var dirinfo = Directory.CreateDirectory("ReadFromScriptFile");

            File.Copy("TestReadFile.txt", @"ReadFromScriptFile\read.txt");
            var fs = new PlotLingoLib.Expressions.FunctionExpression("readfile", new StringValue("read.txt"));
            var c = new Context();
            c.ScriptFileContextPush(new FileInfo(string.Format(@"{0}\bogus.plotlingo", dirinfo.FullName)));
            var r = fs.Evaluate(c);

            Assert.AreEqual("hi\r\nthere", r, "content of file");
        }
    }
}
