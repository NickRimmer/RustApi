using System;

namespace Oxide.Ext.RustApi.Primitives.Exceptions
{
    /// <summary>
    /// Route exception.
    /// </summary>
    internal class ApiCommandNotFoundException : Exception
    {
        public ApiCommandNotFoundException(string message) : base(message)
        {
            
        }
    }
}
