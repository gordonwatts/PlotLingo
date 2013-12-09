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
        /// Get the list of plots
        /// </summary>
        public ROOTNET.NTH1[] Plots { get { return _plots; } }

        /// <summary>
        /// Contains the list of actions to be executed before an actual plot is made.
        /// These are run just before things are dumped out.
        /// </summary>
        private List<Action<PlotContext>> _prePlotHook = new List<Action<PlotContext>>();

        /// <summary>
        /// Track all the things we should call once the plotting is, basically, done.
        /// </summary>
        private List<Action<PlotContext, ROOTNET.Interface.NTCanvas>> _postPlotHook = new List<Action<PlotContext, ROOTNET.Interface.NTCanvas>>();

        /// <summary>
        /// Add a pre-plot hook
        /// </summary>
        /// <param name="act"></param>
        public void AddPreplotHook(Action<PlotContext> act)
        {
            _prePlotHook.Add(act);
        }

        /// <summary>
        /// Add a hook to be called after the basic plotting is done.
        /// </summary>
        /// <param name="act"></param>
        public void AddPostplotHook(Action<PlotContext, ROOTNET.Interface.NTCanvas> act)
        {
            _postPlotHook.Add(act);
        }

        /// <summary>
        /// Keep track of our title.
        /// </summary>
        private string _title = null;

        /// <summary>
        /// Get the filename.
        /// </summary>
        private string _filename;

        /// <summary>
        /// Init the title if it hasn't been already.
        /// </summary>
        private void InitTitleAndFileName()
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

            if (_filename == null)
            {
                _filename = _title;
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
        /// Alter the name of the file.
        /// </summary>
        /// <param name="fname"></param>
        /// <returns></returns>
        public PlotContext filename(string fname)
        {
            _filename = fname;
            return this;
        }

        /// <summary>
        /// Return the name of this script.
        /// </summary>
        public string Name
        {
            get { InitTitleAndFileName(); return _filename; }
        }

        /// <summary>
        /// Save the plot to output. This is where we do the heavy lifting of generating a plot.
        /// </summary>
        /// <param name="filenameStub"></param>
        /// <returns></returns>
        public IEnumerable<FileInfo> Save(string filenameStub)
        {
            InitTitleAndFileName();

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

            // And post-process
            foreach (var a in _postPlotHook)
            {
                a(this, c);
            }

            // Save it
            var fout = new FileInfo(string.Format("{0}.png", filenameStub));
            if (fout.Exists)
                fout.Delete();
            c.SaveAs(fout.FullName);
            return new FileInfo[] { fout };
        }
    }
}
