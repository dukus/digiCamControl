using System;
using Canon.Eos.Framework.Helper;
using Canon.Eos.Framework.Internal;

namespace Canon.Eos.Framework
{
    public class EosException : Exception
    {
        internal EosException(long eosErrorCode)
        {
            this.EosErrorCode = (EosErrorCode)eosErrorCode;
        }

        internal EosException(long eosErrorCode, string message)
            : base(message) 
        {
            this.EosErrorCode = (EosErrorCode)eosErrorCode;
        }

        internal EosException(long eosErrorCode, string message, Exception innerException)
            : base(message, innerException) 
        {
            this.EosErrorCode = (EosErrorCode)eosErrorCode;
        }

        public EosErrorCode EosErrorCode { get; private set; }

        public string EosErrorCodeMessage
        {
            get { return Util.ConvertCamelCasedStringToFriendlyString(this.EosErrorCode.ToString()) + "."; }
        }

        public override string Message
        {
            get { return string.Format("{0}{1}{2}", this.EosErrorCodeMessage, Environment.NewLine, base.Message); }
        }
    }
}
