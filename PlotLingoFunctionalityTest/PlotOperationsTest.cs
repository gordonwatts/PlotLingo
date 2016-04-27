using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PlotLingoFunctionality.Plots;
using PlotLingoLib;

namespace PlotLingoFunctionalityTest
{
    [TestClass]
    public class PlotOperationsTest
    {
        [TestMethod]
        public void AsEffOneBinPlot()
        {
            var h = new ROOTNET.NTH2F("hi", "there", 1, 0.0, 10.0, 1, 0.0, 10.0);
            h.SetBinContent(1, 1, 10.0);

            var r = PlotOperations.asEfficiency(new RootContext(), h, true, true);

            Assert.AreEqual(1.0, r.GetBinContent(1, 1));
        }

        [TestMethod]
        public void AsEffOneBinPlotReverse()
        {
            var h = new ROOTNET.NTH2F("hi", "there", 1, 0.0, 10.0, 1, 0.0, 10.0);
            h.SetBinContent(1, 1, 10.0);

            var r = PlotOperations.asEfficiency(new RootContext(), h, false, false);

            Assert.AreEqual(1.0, r.GetBinContent(1, 1));
        }

        [TestMethod]
        public void TwoBinsOneRowOneFilled()
        {
            var h = new ROOTNET.NTH2F("hi", "there", 2, 0.0, 10.0, 1, 0.0, 10.0);
            h.SetBinContent(1, 1, 10.0);
            h.SetBinContent(2, 1, 0.0);

            var r = PlotOperations.asEfficiency(new RootContext(), h, true, true);

            Assert.AreEqual(1.0, r.GetBinContent(1, 1));
            Assert.AreEqual(0.0, r.GetBinContent(2, 1));
        }

        [TestMethod]
        public void TwoBinsOneRowBothFilled()
        {
            var h = new ROOTNET.NTH2F("hi", "there", 2, 0.0, 10.0, 1, 0.0, 10.0);
            h.SetBinContent(1, 1, 10.0);
            h.SetBinContent(2, 1, 10.0);

            var r = PlotOperations.asEfficiency(new RootContext(), h, true, true);

            Assert.AreEqual(1.0, r.GetBinContent(1, 1));
            Assert.AreEqual(0.5, r.GetBinContent(2, 1));
        }

        [TestMethod]
        public void FourBinsOneRowBothFilled()
        {
            var h = new ROOTNET.NTH2F("hi", "there", 4, 0.0, 10.0, 1, 0.0, 10.0);
            h.SetBinContent(1, 1, 10.0);
            h.SetBinContent(2, 1, 10.0);
            h.SetBinContent(3, 1, 10.0);
            h.SetBinContent(4, 1, 10.0);

            var r = PlotOperations.asEfficiency(new RootContext(), h, true, true);

            Assert.AreEqual(1.0, r.GetBinContent(1, 1));
            Assert.AreEqual(0.75, r.GetBinContent(2, 1));
            Assert.AreEqual(0.50, r.GetBinContent(3, 1));
            Assert.AreEqual(0.25, r.GetBinContent(4, 1));
        }

        [TestMethod]
        public void FourBinsOneRowBothFilledReverse()
        {
            var h = new ROOTNET.NTH2F("hi", "there", 4, 0.0, 10.0, 1, 0.0, 10.0);
            h.SetBinContent(1, 1, 10.0);
            h.SetBinContent(2, 1, 10.0);
            h.SetBinContent(3, 1, 10.0);
            h.SetBinContent(4, 1, 10.0);

            var r = PlotOperations.asEfficiency(new RootContext(), h, false, true);

            Assert.AreEqual(0.25, r.GetBinContent(1, 1));
            Assert.AreEqual(0.50, r.GetBinContent(2, 1));
            Assert.AreEqual(0.75, r.GetBinContent(3, 1));
            Assert.AreEqual(1.0, r.GetBinContent(4, 1));
        }
    }
}
