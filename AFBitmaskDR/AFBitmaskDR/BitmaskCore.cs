using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using OSIsoft.AF.Asset;

namespace AFBitmaskDR
{
    public enum BitEnum
    {
        [Description("00")]
        bit00,
        [Description("01")]
        bit01,
        [Description("02")]
        bit02,
        [Description("03")]
        bit03,
        [Description("04")]
        bit04,
        [Description("05")]
        bit05,
        [Description("06")]
        bit06,
        [Description("07")]
        bit07,
        [Description("08")]
        bit08,
        [Description("09")]
        bit09,
        [Description("10")]
        bit10,
        [Description("11")]
        bit11,
        [Description("12")]
        bit12,
        [Description("13")]
        bit13,
        [Description("14")]
        bit14,
        [Description("15")]
        bit15,
        [Description("16")]
        bit16,
        [Description("17")]
        bit17,
        [Description("18")]
        bit18,
        [Description("19")]
        bit19,
        [Description("20")]
        bit20,
        [Description("21")]
        bit21,
        [Description("22")]
        bit22,
        [Description("23")]
        bit23,
        [Description("24")]
        bit24,
        [Description("25")]
        bit25,
        [Description("26")]
        bit26,
        [Description("27")]
        bit27,
        [Description("28")]
        bit28,
        [Description("29")]
        bit29,
        [Description("30")]
        bit30,
        [Description("31")]
        bit31
    }

    public static class BitmaskCore
    {
        public static bool IsIntVal(object objVal)
        {
            return (objVal is Int64 || objVal is Int32 || objVal is Int16 || objVal is SByte || objVal is Byte);
        }


        public static bool IsIntVal(Type valType)
        {
            return (valType == typeof(Int64) || valType == typeof(Int32) || valType == typeof(Int16) || valType == typeof(SByte) || valType == typeof(Byte));
        }


        public static int GetBit(ulong val, byte pos)
        {
            return ((val & (1u << pos)) != 0) ? 1 : 0;
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
