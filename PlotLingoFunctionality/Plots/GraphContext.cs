using PlotLingoLib;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using ROOTNET.Interface;

namespace PlotLingoFunctionality.Plots
{
    /// <summary>
    /// Generate plots for various graphs
    /// </summary>
    public class GraphContext : IPlotScriptResult
    {
        NTGraph[] _g;

        /// <summary>
        /// Initialize with a full list of graphs
        /// </summary>
        /// <param name="nTGraph"></param>
        public GraphContext(NTGraph[] nTGraph)
        {
            _g = nTGraph;
        }

        public GraphContext(NTGraph g)
        {
            _g = new NTGraph[] { g };
        }

        /// <summary>
        /// Get the name of the plot for drawing
        /// </summary>
        public string Name
        {
            get { return _g[0].Name; }
        }

        /// <summary>
        /// Save to a file.
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="filenameStub"></param>
        /// <returns></returns>
        public IEnumerable<FileInfo> Save(IScopeContext ctx, string filenameStub)
        {
            // Initialize the canvas
            var c = new ROOTNET.NTCanvas();
            c.Title = _g[0].Title;

            // Plot everything.
            var optS = "";
            foreach (var p in _g)
            {
                p.Draw(optS);
                optS = "SAME";
            }

            // Save it
            string[] formats = null;
            var formatsExpr = ctx.GetVariableValue("plotformats") as object[];
            if (formatsExpr != null)
                formats = formatsExpr.Cast<string>().ToArray();
            if (formats == null)
                formats = new string[] { "png" };

            var finfos = formats
                .Select(fmt =>
                {
                    var fout = new FileInfo(string.Format("{0}.{1}", filenameStub, fmt));
                    if (fout.Exists)
                    {
                        try
                        {
                            fout.Delete();
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Had trouble removing the old file {0}", fout.FullName);
                            Console.WriteLine("  -> {0}", e.Message);
                        }
                    }
                    c.SaveAs(fout.FullName);
                    return fout;
                });
            return finfos.ToArray();
        }
    }
}