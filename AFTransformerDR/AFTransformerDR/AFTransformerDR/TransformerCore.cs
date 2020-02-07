using System;
using System.ComponentModel;
using OSIsoft.AF.Asset;

namespace AFTransformerDR
{
    public enum MethodEnum
    {
        [Description("None")]
        None,
        [Description("Simple")]
        Simple,
        [Description("Full")]
        Full,
    }


    public static class TransformerCore
    {
        public static bool IsNumericVal(object objVal)
        {
            return (objVal is Int64 || objVal is Int32 || objVal is Int16 ||
                objVal is SByte || objVal is Byte ||
                objVal is Single || objVal is Double);
        }


        public static bool IsNumericVal(Type valType)
        {
            return (valType == typeof(Int64) || valType == typeof(Int32) || valType == typeof(Int16)||
                valType == typeof(SByte) || valType == typeof(Byte) ||
                valType == typeof(Single) || valType == typeof(Double));
        }


        public static object ConvertToType(object value, TypeCode typeCode)
        {
            switch (typeCode) {
                case TypeCode.Byte:
                    return Convert.ToByte(value);
                case TypeCode.Decimal:
                    return Convert.ToDecimal(value);
                case TypeCode.Double:
                    return value;
                case TypeCode.Int16:
                    return Convert.ToInt16(value);
                case TypeCode.Int32:
                    return Convert.ToInt32(value);
                case TypeCode.Int64:
                    return Convert.ToInt64(value);
                case TypeCode.SByte:
                    return Convert.ToSByte(value);
                case TypeCode.Single:
                    return Convert.ToSingle(value);
                case TypeCode.UInt16:
                    return Convert.ToUInt16(value);
                case TypeCode.UInt32:
                    return Convert.ToInt32(value);
                case TypeCode.UInt64:
                    return Convert.ToUInt64(value);
                default:
                    return AFSystemStateCode.WrongType;
            }
        }
    }
}
