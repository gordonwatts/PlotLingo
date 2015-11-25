using Microsoft.VisualStudio.TestTools.UnitTesting;
using PlotLingoLib;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlotLingoLibTest
{
    [TestClass]
    public class ExtensibilityControlTest
    {
        [TestMethod]
        public void MakeSureInitIsCalled()
        {
            MyFunc.WasCalled = false;
            var rootContext = new RootContext();
            ExtensibilityControl.Get().InitializeFunctionObjects(rootContext);
            Assert.IsTrue(MyFunc.WasCalled, "Initialize method was not called");
        }

    }

    [Export(typeof(IFunctionObject))]
    public class MyFunc : IFunctionObject, IFunctionObjectInitalization
    {
        public static bool WasCalled = false;

        /// <summary>
        /// Dummy object instantiation gets this guy called.
        /// </summary>
        /// <param name="ctx"></param>
        public void Initalize(IScopeContext ctx)
        {
            WasCalled = true;
        }
    }
}
