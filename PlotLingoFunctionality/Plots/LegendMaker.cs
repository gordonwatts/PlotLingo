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
            c.ExecutionContext.AddPostCallHook("plot", "legend", (obj, result) =>
            {
                (result as PlotContext).AddPreplotHook(SetLegendColors);
                return result;
            });
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
                            if (dict.ContainsKey("Title"))
                            {
                                linfo.Title = (string)dict["Title"];
                            }
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

        /// <summary>
        /// See if we can't set the legend colors.
        /// </summary>
        /// <param name="ctx"></param>
        private static void SetLegendColors(IScopeContext codeContext, PlotContext ctx)
        {
            // First pass: determine how many items there are, set the color, etc.
            int count = 0;
            int letterLength = 0;
            foreach (var p in ctx.Plots)
            {
                foreach (var legInfo in _legendInfo)
                {
                    if (p.Title.IndexOf(legInfo.Key) >= 0 || Tags.hasTag(codeContext, p, legInfo.Key))
                    {
                        p.LineColor = (short)legInfo.Value.Color;
                        letterLength = Math.Max(letterLength, legInfo.Value.Title.Length);
                        count++;
                    }
                }
            }

            // If we have things to make as a legend, then go through and take care of them.
            if (count > 0)
            {
                double xmin = 0.2;
                double ymin = 0.2;
                double ymax = ymin + 0.06 * count;
                double xmax = xmin + 0.20 + 0.01 * letterLength;

                var l = new ROOTNET.NTLegend(xmin, ymin, xmax, ymax);
                foreach (var p in ctx.Plots)
                {
                    foreach (var legInfo in _legendInfo)
                    {
                        if (p.Title.IndexOf(legInfo.Key) >= 0 || Tags.hasTag(codeContext, p, legInfo.Key))
                        {
                            l.AddEntry(p, legInfo.Value.Title);
                        }
                    }
                }

                // Plot it when we have a canvas to plot it against.
                ctx.AddPostplotHook((mctx, c) =>
                {
                    l.Draw();
                    l.SetFillColor(c.FillColor);
                });
            }

            // Second pass: create the legend and throw it up there.

        }
    }
}
