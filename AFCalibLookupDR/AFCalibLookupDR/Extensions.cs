using System;
using OSIsoft.AF.Asset;
using OSIsoft.AF.EventFrame;
using OSIsoft.AF.Time;

namespace AFCalibLookupDR
{
    public static class Extensions
    {
        public const double EPSILON = 0.00001;

        public static bool Equals(double a, double b, double epsilon)
        {
            return a == b ? true : Math.Abs(a - b) < epsilon;
        }

        public static bool GreaterThan(double a, double b, double epsilon)
        {
            return a - b > epsilon;
        }

        public static bool LessThan(double a, double b, double epsilon)
        {
            return b - a > epsilon;
        }

        public static bool IsNumericType(this Type type)
        {
            return IsNumericType(Type.GetTypeCode(type));
        }


        public static bool IsNumericType(this TypeCode typeCode)
        {
            switch (typeCode) {
                case TypeCode.Byte:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.SByte:
                case TypeCode.Single:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    return true;
                default:
                    return false;
            }
        }

        public static AFTime ToAFTime(AFBaseElement element, object timeContext)
        {
            if (timeContext is AFTime) {
                return (AFTime)timeContext;
            } else if (timeContext is AFTimeRange) {
                var timeRange = (AFTimeRange)timeContext;
                return (element is AFEventFrame) ? timeRange.StartTime : timeRange.EndTime;
            }

            return AFTime.NowInWholeSeconds;
        }
    }
}
