using System;

namespace Oxide.Ext.RustApi.Exceptions
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
