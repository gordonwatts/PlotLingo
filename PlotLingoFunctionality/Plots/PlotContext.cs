using System;
using System.Collections.Generic;
using System.IO;

namespace PlotLingoFunctionality.Plots
{
    public class PlotContext : IPlotScriptResult
    {
        /// <summary>
        /// The list of plots we are most insterested in. This, basically, preresents a gPad...
        /// </summary>
        private ROOTNET.NTH1[] _plots;

        /// <summary>
        /// Create the plot conext with a list of plots
        /// </summary>
        /// <param name="nTH1"></param>
        public PlotContext(ROOTNET.NTH1[] nTH1)
        {
            this._plots = nTH1;
        }

        /// <summary>
        /// Contains the list of actions to be executed before an actual plot is made.
        /// These are run just before things are dumped out.
        /// </summary>
        private List<Action<PlotContext>> _prePlotHook = new List<Action<PlotContext>>();

        /// <summary>
        /// Keep track of our title.
        /// </summary>
        private string _title = null;

        /// <summary>
        /// Init the title if it hasn't been already.
        /// </summary>
        private void InitTitle()
        {
            if (_title == null)
            {
                if (this._plots == null || _plots.Length == 0)
                {
                    _title = "Plot";
                }
                else
                {
                    _title = _plots[0].Title;
                }
            }
        }

        /// <summary>
        /// Alter the title of the canvas.
        /// </summary>
        /// <param name="title"></param>
        /// <returns></returns>
        public PlotContext title(string title)
        {
            _title = title;
            return this;
        }

        /// <summary>
        /// Return the name of this script.
        /// </summary>
        public string Name
        {
            get { InitTitle(); return _title; }
        }

        /// <summary>
        /// Save the plot to output. This is where we do the heavy lifting of generating a plot.
        /// </summary>
        /// <param name="filenameStub"></param>
        /// <returns></returns>
        public IEnumerable<FileInfo> Save(string filenameStub)
        {
            InitTitle();

            // Now, do the pre-plot hook
            foreach (var act in _prePlotHook)
            {
                act(this);
            }

            // Initialize the canvas
            var c = new ROOTNET.NTCanvas();
            c.Title = _title;
            if (_plots.Length > 0)
                _plots[0].Title = _title;

            // Plot everything.
            var optS = "";
            foreach (var p in _plots)
            {
                p.Draw(optS);
                optS = "SAME";
            }

            // Save it
            var fout = new FileInfo(string.Format("{0}.png", filenameStub));
            c.SaveAs(fout.FullName);
            return new FileInfo[] { fout };
        }
    }
}
