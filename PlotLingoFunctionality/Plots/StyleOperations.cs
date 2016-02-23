using PlotLingoLib;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlotLingoFunctionality.Plots
{
    [Export(typeof(IFunctionObject))]
    public class StyleOperations : IFunctionObject
    {
        /// <summary>
        /// Put a nice smooth gradient as the central style.
        /// </summary>
        public static void UseSmoothGradient()
        {
            ROOTNET.Globals.gStyle.Value.SetPalette(1);

            const int NCont = 255;

            double[] stops = { 0.00, 0.34, 0.61, 0.84, 1.00 };
            double[] red = { 0.00, 0.00, 0.87, 1.00, 0.51 };
            double[] green = { 0.00, 0.81, 1.00, 0.20, 0.00 };
            double[] blue = { 0.51, 1.00, 0.12, 0.00, 0.00 };

            ROOTNET.NTColor.CreateGradientColorTable((uint) stops.Length, stops, red, green, blue, NCont);
            ROOTNET.Globals.gStyle.Value.SetNumberContours(NCont);
        }
    }
}
