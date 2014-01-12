using Microsoft.VisualStudio.TestTools.UnitTesting;
using PlotLingoLib;
using System.IO;
using System.Text;

namespace PlotLingoLibTest
{
    [TestClass]
    public class RunPlotUnitTests
    {
        [TestMethod]
        public void LineReadOK()
        {
            var r = ReadThroughString("i = 5;");
            Assert.AreEqual("i = 5;", r);
        }

        [TestMethod]
        public void FullCommentDoubleSlash()
        {
            var r = ReadThroughString("//i = 5;");
            Assert.AreEqual("", r);
        }

        [TestMethod]
        public void FullCommandPound()
        {
            var r = ReadThroughString("#i = 5;");
            Assert.AreEqual("", r);
        }

        [TestMethod]
        public void PartialCommentPound()
        {
            var r = ReadThroughString("i = 5; # dude");
            Assert.AreEqual("i = 5;", r);
        }

        [TestMethod]
        public void PartialCommentDoubleSlash()
        {
            var r = ReadThroughString("i = 5; // dude");
            Assert.AreEqual("i = 5;", r);
        }

        [TestMethod]
        public void DoubleSlashInQuotes()
        {
            var r = ReadThroughString("i = \"//\";");
            Assert.AreEqual("i = \"//\";", r);
        }

        [TestMethod]
        public void PoundInQuotes()
        {
            var r = ReadThroughString("i = \"#\";");
            Assert.AreEqual("i = \"#\";", r);
        }

        [TestMethod]
        public void DoubleSlashInQuotesWithComment()
        {
            var r = ReadThroughString("i = \"//\"; // not going to work");
            Assert.AreEqual("i = \"//\";", r);
        }

        [TestMethod]
        public void PoundInQuotesWithComment()
        {
            var r = ReadThroughString("i = \"#\"; # Not going to work");
            Assert.AreEqual("i = \"#\";", r);
        }

        /// <summary>
        /// Simulate the read of a string.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private string ReadThroughString(string input)
        {
            using (var rdr = new StringReader(input))
            {
                var sb = new StringBuilder();
                foreach (var l in rdr.ReadFromReader())
                {
                    sb.Append(l);
                }
                return sb.ToString();
            }
        }
    }
}
