using PlotLingoLib;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlotLingoFunctionality.Plots
{
    /// <summary>
    /// Items to help with attaching a legend to the plot
    /// </summary>
    [Export(typeof(IFunctionObject))]
    class LegendMaker : IFunctionObject
    {
        /// <summary>
        /// Track the actual legend items
        /// </summary>
        private static Dictionary<string, int> _legendInfo = new Dictionary<string, int>();

        /// <summary>
        /// A function to add a few legends for the plot. This remembers them for
        /// later attachment to a plot.
        /// </summary>
        /// <param name="associations"></param>
        public static void Legend(Context c, IDictionary<object, object> associations)
        {
            c.AddPostCallHook("plot", "legend", (obj, result) => {
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
                    if (item.Value.GetType() != typeof(int))
                    {
                        Console.WriteLine("In legend generation unable to parse {0} into an integer.", item.Value.ToString());
                    }
                    else
                    {
                        _legendInfo[s] = (int)item.Value;
                    }
                }
            }
        }

        /// <summary>
        /// See if we can't set the legend colors.
        /// </summary>
        /// <param name="ctx"></param>
        private static void SetLegendColors(PlotContext ctx)
        {
            var l = new ROOTNET.NTLegend(0.2, 0.2, 0.4, 0.4);
            int count = 0;
            int letterLength = 0;
            foreach (var p in ctx.Plots)
            {
                foreach (var legInfo in _legendInfo)
                {
                    if (p.Title.IndexOf(legInfo.Key) >= 0)
                    {
                        p.LineColor = (short) legInfo.Value;
                        l.AddEntry(p, legInfo.Key);
                        letterLength = Math.Max(letterLength, legInfo.Key.Length);
                        count++;
                    }
                }
            }

            // No need to do anything if we couldn't match anything!
            if (count > 0)
            {
                // Resize the box to be about right for the text.
                l.Y2 = l.Y1 + 0.05 * count;
                l.X2 = l.X1 + 0.025 * letterLength;

                // Plot it when we have a canvas to plot it against.
                ctx.AddPostplotHook((mctx, c) => {
                    l.Draw();
                    l.SetFillColor(c.FillColor);
                });
            }
        }
    }
}
