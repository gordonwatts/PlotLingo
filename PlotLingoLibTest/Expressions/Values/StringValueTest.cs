using Microsoft.VisualStudio.TestTools.UnitTesting;
using PlotLingoLib;
using PlotLingoLib.Expressions.Values;

namespace PlotLingoLibTest.Expressions.Values
{
    [TestClass]
    public class StringValueTest
    {
        [TestMethod]
        public void StringPlain()
        {
            var s = new StringValue("hi");
            var mys = s.Evaluate(new Context());
            Assert.AreEqual("hi", mys, "Plain string value");
        }

        [TestMethod]
        public void StringWithOpenB()
        {
            var s = new StringValue("hi{there");
            var mys = s.Evaluate(new Context());
            Assert.AreEqual("hi{there", mys, "string value");
        }

        [TestMethod]
        public void StringWithOpenBAndValue()
        {
            var s = new StringValue("hi{there");
            var c = new Context();
            c.SetVariableValue("there", "dude");
            var mys = s.Evaluate(c);
            Assert.AreEqual("hi{there", mys, "string value");
        }

        [TestMethod]
        public void StringWithCloseB()
        {
            var s = new StringValue("hi there}");
            var mys = s.Evaluate(new Context());
            Assert.AreEqual("hi there}", mys, "string value");
        }

        [TestMethod]
        public void StringWithVarNoTrans()
        {
            var s = new StringValue("hi {there}");
            var mys = s.Evaluate(new Context());
            Assert.AreEqual("hi {there}", mys, "string value");
        }

        [TestMethod]
        public void StringWithVar()
        {
            var s = new StringValue("hi {there}");
            var c = new Context();
            c.SetVariableValue("there", "dude");
            var mys = s.Evaluate(c);
            Assert.AreEqual("hi dude", mys, "string value");
        }

        [TestMethod]
        public void StringWithVarAndNumbers()
        {
            var s = new StringValue("hi {there53}");
            var c = new Context();
            c.SetVariableValue("there53", "dude");
            var mys = s.Evaluate(c);
            Assert.AreEqual("hi dude", mys, "string value");
        }

        [TestMethod]
        public void StringWithVarTwice()
        {
            var s = new StringValue("hi {there}{there}");
            var c = new Context();
            c.SetVariableValue("there", "dude");
            var mys = s.Evaluate(c);
            Assert.AreEqual("hi dudedude", mys, "string value");
        }

        [TestMethod]
        public void StringWith2Vars()
        {
            var s = new StringValue("hi {there} {are}");
            var c = new Context();
            c.SetVariableValue("there", "dude");
            c.SetVariableValue("are", "medude");
            var mys = s.Evaluate(c);
            Assert.AreEqual("hi dude medude", mys, "string value");
        }
    }
}
