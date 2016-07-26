using PlotLingoLib;
using ROOTNET.Interface;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace PlotLingoFunctionality.Plots
{
    /// <summary>
    /// Language methods to place text on a plot context
    /// </summary>
    [Export(typeof(IFunctionObject))]
    class TextMaker : IFunctionObject
    {
        /// <summary>
        /// Single text info
        /// </summary>
        private class textInfo
        {
            public string _text;

            /// <summary>
            /// Was an absolute location requested for this text?
            /// </summary>
            public bool _absoluteLocation = false;
            public double _absX = 0.0, _absY = 0.0;

            /// <summary>
            /// Configure ourselves from options
            /// </summary>
            /// <param name="options"></param>
            public textInfo(Dictionary<object, object> options = null)
            {
                // Now the options
                if (options != null)
                {
                    foreach (var opt in options)
                    {
                        switch (opt.Key as string)
                        {
                            case "x":
                                _absX = (double)opt.Value;
                                _absoluteLocation = true;
                                break;

                            case "y":
                                _absY = (double)opt.Value;
                                _absoluteLocation = true;
                                break;

                            default:
                                throw new ArgumentException(string.Format("Unknown option for textbox location: '{0}'", opt.Key));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Place a simple text string on the plot
        /// </summary>
        /// <param name="c"></param>
        /// <param name="ctx"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public static DrawingContext Text(IScopeContext c, DrawingContext ctx, string text, Dictionary<object, object> options = null)
        {
            // Get the text we are going to be placing on the plot.
            var plotTextInfo = ctx.GetProperty("TextList") as List<textInfo>;
            if (plotTextInfo == null)
            {
                plotTextInfo = new List<textInfo>();
                ctx.SetProperty("TextList", plotTextInfo);

                // Make sure that the text will get drawn at the appropriate time.
                ctx.AddPostplotHook((sctx, mctx, canv) => DrawText(canv, plotTextInfo));
            }

            // Now, save the new text.
            plotTextInfo.Add(new textInfo(options) { _text = text });

            return ctx;
        }

        /// <summary>
        /// Help us track where we are going to be putting the next text.
        /// </summary>
        private class nextPlotLocation
        {
            public double _x = 0.2, _y = 0.2;

            public void matchToUserRequest(textInfo info)
            {
                if (info._absoluteLocation)
                {
                    _x = info._absX;
                    _y = info._absY;
                }
            }
        }

        /// <summary>
        /// Draw the text as per instructions
        /// </summary>
        /// <param name="canv"></param>
        /// <param name="plotTextInfo"></param>
        private static void DrawText(NTCanvas canv, List<textInfo> plotTextInfo)
        {
            nextPlotLocation loc = new nextPlotLocation();

            foreach (var t in plotTextInfo)
            {
                loc.matchToUserRequest(t);
                var tbox = new ROOTNET.NTText(loc._x, loc._y, t._text);
                tbox.NDC = true;
                tbox.Draw();
                loc._y -= tbox.TextSize;
            }
        }
    }
}
