using PlotLingoLib;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace PlotLingoFunctionality.Plots
{
    /// <summary>
    /// Items to help with attaching a legend to the plot
    /// </summary>
    [Export(typeof(IFunctionObject))]
    class LegendMaker : IFunctionObject
    {
        class LegendInfo
        {
            /// <summary>
            /// The title in the legend for this color
            /// </summary>
            public string Title;

            /// <summary>
            /// The color for this legend
            /// </summary>
            public int Color;

            /// <summary>
            /// The line style for this legend
            /// </summary>
            public int LineStyle;

            /// <summary>
            /// What marker should be used? 1 for a dot, which is marker at all.
            /// </summary>
            public int MarkerStyle;

            /// <summary>
            /// Set to true if this entry should be removed in the legend.
            /// </summary>
            public bool LeaveOut { get; set; }
        }

        /// <summary>
        /// Track the actual legend items
        /// </summary>
        private static Dictionary<string, LegendInfo> _legendInfo = new Dictionary<string, LegendInfo>();

        /// <summary>
        /// A function to add a few legends for the plot. This remembers them for
        /// later attachment to a plot.
        /// </summary>
        /// <param name="associations"></param>
        public static void Legend(IScopeContext c, IDictionary<object, object> associations)
        {
            // Sign up for all the legends we are going to have to attach.
            c.ExecutionContext.AddPostCallHook("plot", "legend", (obj, result) =>
            {
                (result as DrawingContext).AddPreplotHook(SetLegendColors);
                return result;
            });
            c.ExecutionContext.AddPostCallHook("draw", "legend", (obj, result) =>
            {
                (result as DrawingContext).AddPreplotHook(SetLegendColors);
                return result;
            });

            // Next, record the legend info.
            foreach (var item in associations)
            {
                var s = item.Key as string;
                if (s == null)
                {
                    Console.WriteLine("In legend generation unable to parse as a string {0}.", item.Key.ToString());
                }
                else
                {
                    if (item.Value.GetType() == typeof(int))
                    {
                        _legendInfo[s] = new LegendInfo() { Color = (int)item.Value, Title = s };
                    }
                    else if (item.Value is IDictionary<object, object>)
                    {
                        var linfo = new LegendInfo() { Title = s };
                        var dict = item.Value as IDictionary<object, object>;
                        if (!dict.ContainsKey("Color"))
                        {
                            Console.WriteLine("Dictionary for legend does not contain a Color key!");
                        }
                        else
                        {
                            linfo.Color = (int)dict["Color"];
                        }

                        if (dict.ContainsKey("Title"))
                        {
                            linfo.Title = (string)dict["Title"];
                        }

                        linfo.LeaveOut = false;
                        if (dict.ContainsKey("LeaveOut"))
                        {
                            linfo.LeaveOut = (bool)dict["LeaveOut"];
                        }

                        linfo.LineStyle = 1;
                        if (dict.ContainsKey("LineStyle"))
                        {
                            linfo.LineStyle = (int)dict["LineStyle"];
                        }

                        linfo.MarkerStyle = 1;
                        if (dict.ContainsKey("MarkerStyle"))
                        {
                            linfo.MarkerStyle = (int)dict["MarkerStyle"];
                        }

                        _legendInfo[s] = linfo;
                    }
                    else
                    {
                        Console.WriteLine("In legend generation unable to parse {0} into an integer or a dictionary of values (with Title and Color as members).", item.Value.ToString());
                    }
                }
            }
        }

        private class Options
        {
            public double xmarg = 0.0;
            public double ymarg = 0.0;
            public enum WhereToPlaceLegend
            {
                LowerLeft, LowerRight, UpperLeft, UpperRight
            }
            public WhereToPlaceLegend placement = WhereToPlaceLegend.UpperRight;
            public bool drawBox = true;
            public double scale = 1.0;
            public int fillStyle = 0;
        }

        /// <summary>
        /// Set legend options for a particlar plot.
        /// </summary>
        /// <param name="c"></param>
        /// <param name="associations"></param>
        public static DrawingContext LegendOptions(IScopeContext c, DrawingContext ctx, IDictionary<object, object> associations)
        {
            // If there is no associated legend options with the ctx yet, then create a default one.
            Options opt = ctx.GetProperty("LegendOptions") as Options;
            if (opt == null) {
                opt = new Options();
                ctx.SetProperty("LegendOptions", opt);
            }

            // Parse the list of options.
            foreach (var k in associations.Keys)
	        {
                switch (k as string) {
                    case "xmarg":
                        opt.xmarg = (double) associations[k];
                        break;

                    case "ymarg":
                        opt.ymarg = (double) associations[k];
                        break;

                    case "drawbox":
                        opt.drawBox = bool.Parse(associations[k] as string);
                        break;

                    case "placement":
                        opt.placement = (Options.WhereToPlaceLegend) Enum.Parse(typeof(Options.WhereToPlaceLegend), associations[k] as string);
                        break;

                    case "scale":
                        opt.scale = (double)associations[k];
                        break;

                    default:
                        throw new ArgumentException(string.Format("Unknown legend option '{0}'", k as string));
                }
            }

            return ctx;
        }

        /// <summary>
        /// See if we can't set the legend colors.
        /// </summary>
        /// <param name="ctx"></param>
        private static void SetLegendColors(IScopeContext codeContext, DrawingContext ctx)
        {
            // First pass: determine how many items there are, set the color, etc.
            int count = 0;
            int letterLength = 0;
            foreach (var p in ctx.ObjectsToDraw)
            {
                foreach (var legInfo in _legendInfo)
                {
                    if (p.Title.IndexOf(legInfo.Key) >= 0 || p.hasTag(codeContext, legInfo.Key))
                    {
                        p.LineColor = (short)legInfo.Value.Color;
                        p.LineStyle = (short)legInfo.Value.LineStyle;
                        p.MarkerStyle = (short)legInfo.Value.MarkerStyle;
                        letterLength = Math.Max(letterLength, legInfo.Value.Title.Length);
                        if (!legInfo.Value.LeaveOut)
                        {
                            count++;
                        }
                    }
                }
            }

            // If we have things to make as a legend, then go through and take care of them.
            if (count > 0)
            {
                // Get the options out and calculate basic placement.
                var opt = ctx.GetProperty("LegendOptions") as Options;
                if (opt == null)
                {
                    opt = new Options();
                }

                double xmin, xmax;
                double ymin, ymax;
                if (opt.placement == Options.WhereToPlaceLegend.LowerLeft || opt.placement == Options.WhereToPlaceLegend.UpperLeft)
                {
                    xmin = opt.xmarg;
                    xmax = xmin + (0.20 + 0.01 * letterLength) * opt.scale;
                }
                else
                {
                    xmin = 0.87 - opt.xmarg;
                    xmax = xmin - (0.20 + 0.01 * letterLength) * opt.scale;
                }
                if (opt.placement == Options.WhereToPlaceLegend.LowerLeft || opt.placement == Options.WhereToPlaceLegend.LowerRight)
                {
                    ymin = opt.ymarg;
                    ymax = ymin + 0.06 * count * opt.scale;
                }
                else
                {
                    ymin = 0.86 - opt.ymarg;
                    ymax = ymin - (0.06 * count)*opt.scale;
                }

                // Create the legend box
                var l = new ROOTNET.NTLegend(xmin, ymin, xmax, ymax);
                l.SetBorderSize(0);
                foreach (var p in ctx.ObjectsToDraw)
                {
                    foreach (var legInfo in _legendInfo)
                    {
                        if (p.Title.IndexOf(legInfo.Key) >= 0 || p.hasTag(codeContext, legInfo.Key) && !legInfo.Value.LeaveOut)
                        {
                            l.AddEntry(p.NTObject, legInfo.Value.Title);
                        }
                    }
                }

                // Plot it when we have a canvas to plot it against when the plot is actually drawn.
                ctx.AddPostplotHook((thectx, mctx, c) =>
                {
                    l.Draw();
                    l.SetFillColor(c.FillColor);
                    l.FillStyle = (short) opt.fillStyle;
                    if (!opt.drawBox)
                    {
                        l.LineColor = c.FillColor;
                    }
                });
            }
        }
    }
}
