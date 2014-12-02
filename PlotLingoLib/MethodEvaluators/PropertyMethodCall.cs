using System;

namespace PlotLingoLib.MethodEvaluators
{
    /// <summary>
    /// If a property name is given as a method call, set or get the value.
    /// 
    /// If MyObject::MyIntProp is a get/set integer property, then
    ///
    ///     obj.MyIntProp() - returns the value
    ///     obj.MyIntProp(10) - sets the value and returns the original object
    /// 
    /// </summary>
    /// <remarks>
    /// This modifies an object in-place. It is *not* functional. No idea how to clone everything, after all!
    /// </remarks>
    class PropertyMethodCall : IMethodEvaluator
    {
        /// <summary>
        /// Is this a property name we know about, and can it do what we need it to do?
        /// </summary>
        /// <param name="c"></param>
        /// <param name="obj"></param>
        /// <param name="methodName"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public Tuple<bool, object> Evaluate(IScopeContext c, object obj, string methodName, object[] args)
        {
            // If we have multiple arguments, then we aren't doing anything
            if (args.Length > 1)
                return new Tuple<bool, object>(false, null);

            // Get the type of the object
            var t = obj.GetType();

            // Now, see if we can't get the property name from there.
            var property = t.GetProperty(methodName);
            if (property == null)
                return new Tuple<bool, object>(false, null);

            // Next, this is a get or set.
            if (args.Length == 0 && property.CanRead)
            {
                return new Tuple<bool, object>(true, property.GetValue(obj));
            }

            if (args.Length == 1 && property.CanWrite)
            {
                property.SetValue(obj, args[0]);
                return new Tuple<bool, object>(true, obj);
            }

            // Failed if we got here!
            return new Tuple<bool, object>(false, null);
        }
    }
}
