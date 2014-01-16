
using System;
using System.IO;
namespace PlotLingoLib
{
    /// <summary>
    /// The current execution context (things that are more... global).
    /// </summary>
    public interface IExecutionContext
    {

        /// <summary>
        /// Callbacks to execute when a function name of some sort is executed
        /// </summary>
        /// <param name="functionName"></param>
        /// <param name="slotname"></param>
        /// <param name="callback"></param>
        void AddPostCallHook(string functionName, string slotname, Func<object, object, object> callback);

        /// <summary>
        /// Execute the callback after a funciton call back. 
        /// </summary>
        /// <param name="fmName"></param>
        /// <param name="obj"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        object ExecutePostCallHook(string fmName, object obj, object result);

        /// <summary>
        /// Push a new script file onto the stack.
        /// </summary>
        /// <param name="fileInfo"></param>
        void ScriptFileContextPush(FileInfo fileInfo);

        /// <summary>
        /// Pop a script file off the stack.
        /// </summary>
        void ScriptFileContextPop();

        /// <summary>
        /// Name of the script file we are currently executing from
        /// </summary>
        FileInfo CurrentScriptFile { get; }

        /// <summary>
        /// True if we are currently executing commands from a script file.
        /// </summary>
        bool ExecutingScript { get; }
    }
}
