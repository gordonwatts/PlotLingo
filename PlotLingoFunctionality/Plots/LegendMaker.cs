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
        public static void Legend(RootContext c, IDictionary<object, object> associations)
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
        private static void SetLegendColors(RootContext codeContext, PlotContext ctx)
        {
            var l = new ROOTNET.NTLegend(0.2, 0.2, 0.4, 0.4);
            int count = 0;
            int letterLength = 0;
            foreach (var p in ctx.Plots)
            {
                foreach (var legInfo in _legendInfo)
                {
                    if (p.Title.IndexOf(legInfo.Key) >= 0 || Tags.hasTag(codeContext, p, legInfo.Key))
                    {
                        p.LineColor = (short)legInfo.Value.Color;
                        l.AddEntry(p, legInfo.Value.Title);
                        letterLength = Math.Max(letterLength, legInfo.Value.Title.Length);
                        count++;
                    }
                }
            }

            // No need to do anything if we couldn't match anything!
            if (count > 0)
            {
                // Resize the box to be about right for the text.
                l.Y2 = l.Y1 + 0.06 * count;
                l.X2 = l.X1 + 0.20 + 0.01 * letterLength;

                // Plot it when we have a canvas to plot it against.
                ctx.AddPostplotHook((mctx, c) =>
                {
                    l.Draw();
                    l.SetFillColor(c.FillColor);
                });
            }
        }
    }
}
