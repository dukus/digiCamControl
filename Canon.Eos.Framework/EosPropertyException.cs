using System;

namespace Canon.Eos.Framework
{
    public class EosPropertyException : EosException
    {
        internal EosPropertyException(long eosErrorCode)
            : base(eosErrorCode)
        {            
        }

        internal EosPropertyException(long eosErrorCode, string message)
            : base(eosErrorCode, message) 
        {            
        }

        internal EosPropertyException(long eosErrorCode, string message, Exception innerException)
            : base(eosErrorCode, message, innerException) { }

        /// <summary>
        /// Gets the property id.
        /// </summary>
        public uint PropertyId { get; internal set; }

        /// <summary>
        /// Gets the property value.
        /// </summary>
        public object PropertyValue { get; internal set; }
    }
}
