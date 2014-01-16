using System;
using System.Collections.Generic;
using System.IO;

namespace PlotLingoLib
{
    /// <summary>
    /// Tracks the current execution context flying around.
    /// </summary>
    public class ExecutionContext : IExecutionContext
    {
        /// <summary>
        /// This is the stack of scripts that are being executed right now.
        /// </summary>
        private Stack<FileInfo> _scriptStack = new Stack<FileInfo>();

        /// <summary>
        /// Returns the current file that current statements are being executed from. Returns a FileInfo that
        /// points to a file called "in memory" if no actual script is being executed.
        /// </summary>
        public FileInfo CurrentScriptFile
        {
            get
            {
                if (_scriptStack.Count == 0)
                {
                    return new FileInfo("in memory");
                }
                else
                {
                    return _scriptStack.Peek();
                }
            }
        }

        /// <summary>
        /// Start executing a new script
        /// </summary>
        /// <param name="fileInfo"></param>
        public void ScriptFileContextPush(FileInfo fileInfo)
        {
            _scriptStack.Push(fileInfo);
        }

        /// <summary>
        /// Finish up executing a script - so the context is no longer valid.
        /// </summary>
        public void ScriptFileContextPop()
        {
            _scriptStack.Pop();
        }

        /// <summary>
        /// True if we are currently executing a script
        /// </summary>
        public bool ExecutingScript { get { return _scriptStack.Count != 0; } }

        /// <summary>
        /// Maintain a list of post-function and method call hooks.
        /// </summary>
        private Dictionary<string, Dictionary<string, Func<object, object, object>>> _postCallHooks = new Dictionary<string, Dictionary<string, Func<object, object, object>>>();

        /// <summary>
        /// Add a call back that is called after each time the method is called.
        /// </summary>
        /// <param name="functionName">Function to call. First argument is the object this was called against (or null if a function) and the second is the return value from the function or method. And returns the new value of the result object (which may be the same as the old one).</param>
        /// <param name="callback">Name of the method or function that should trigger this callback</param>
        /// <param name="slotname">The slot where the callback is called. One function per slot, and overwrite anything that was there.</param>
        public void AddPostCallHook(string functionName, string slotname, Func<object, object, object> callback)
        {
            if (!_postCallHooks.ContainsKey(functionName))
                _postCallHooks[functionName] = new Dictionary<string, Func<object, object, object>>();

            _postCallHooks[functionName][slotname] = callback;
        }

        /// <summary>
        /// Given a method or function name, find all applicable hooks and call them. Returning
        /// the altered object.
        /// </summary>
        /// <param name="fmName"></param>
        /// <param name="obj"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public object ExecutePostCallHook(string fmName, object obj, object result)
        {
            Dictionary<string, Func<object, object, object>> callbacks;
            if (_postCallHooks.TryGetValue(fmName, out callbacks))
            {
                foreach (var cb in callbacks)
                {
                    result = cb.Value(obj, result);
                }
            }
            return result;
        }
    }
}
