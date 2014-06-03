using System;
using System.Text;
using System.Text.RegularExpressions;
using Canon.Eos.Framework.Internal.SDK;

namespace Canon.Eos.Framework.Helper
{
    internal static class Util
    {
        public static string ConvertCamelCasedStringToFriendlyString(string camelCase)
        {
            return Regex.Replace(camelCase, @"(?<a>(?<!^)((?:[A-Z][a-z])|(?:(?<!^[A-Z]+)[A-Z0-9]+(?:(?=[A-Z][a-z])|$))|(?:[0-9]+)))", @" ${a}");
        }

        public static byte[] ConvertStringToBytesWithNullByteAtEnd(string data)
        {
            return Encoding.ASCII.GetBytes(data + "\0");
        }

        public static bool HasFailed(uint result)
        {
            return result != Edsdk.EDS_ERR_OK;
        }

        public static void Assert(uint result, string message)
        {
            if (Util.HasFailed(result))
                throw new EosException(result, message);
        }

        public static void Assert(uint result, string message, Exception exception)
        {
            if (Util.HasFailed(result))
                throw new EosException(result, message, exception);
        }

        public static void Assert(uint result, string message, uint propertyId, object propertyValue = null)
        {
            if (Util.HasFailed(result))
                throw new EosPropertyException(result, message) { PropertyId = propertyId, PropertyValue = propertyValue };
        }

        public static void AssertIf(bool condition, string message, params object[] args)
        {
            if (condition)
                throw new EosException(-1, string.Format(message, args));
        }
    }
}
