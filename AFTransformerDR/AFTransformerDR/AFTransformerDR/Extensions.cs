/***************************************************************************
   Copyright 2017 OSIsoft, LLC.
   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at
 
   http://www.apache.org/licenses/LICENSE-2.0
   
   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
 ***************************************************************************/

using System;
using System.Linq;
using OSIsoft.AF.Time;
using OSIsoft.AF.Asset;

namespace OSIsoft.AF.Utils
{
    public static class Extensions
    {
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


        // If value could be null, then you must invoke as regular method and not as an extension method.
        /*public static double ToDouble(this AFValue value)
        {
            if (value == null || !value.IsGood || !IsNumericType(value.ValueTypeCode)) {
                return double.NaN;
            }
            // https://techsupport.osisoft.com/Documentation/PI-AF-SDK/html/M_OSIsoft_AF_Asset_AFValue_ValueAsDouble.htm
            return value.ValueAsDouble();
        }*/


        public static object CoerceToType(this double value, Type type)
        {
            return CoerceToType(value, Type.GetTypeCode(type));
        }


        public static object CoerceToType(this double value, TypeCode typeCode)
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



        public static AFValue CreateBadValue(this AFAttribute attribute, AFTime time)
        {
            return AFValue.CreateSystemStateValue(attribute, AFSystemStateCode.NoResult, time);
        }


        public static void ConfirmValuesAtRangeEndpoints(this AFValues values, AFAttribute Attribute, AFTimeRange TimeRange)
        {
            AFTime StartTime = TimeRange.StartTime.ToPIPrecision();
            AFTime EndTime = TimeRange.EndTime.ToPIPrecision();

            // Don't merely add a StartTime and EndTime.  
            // First you must check to make sure they aren't already there.
            // Corner case: where EndTime == StartTime.
            if (values.Count == 0) {
                values.Add(CreateBadValue(Attribute, StartTime));
            } else if (values[0].Timestamp > StartTime) {
                // This will probably never happen but in extremely rare
                // case that it does, set a dummy value.
                values.Insert(0, CreateBadValue(Attribute, StartTime));
            }

            var last = values[values.Count - 1];

            if (last.Timestamp < EndTime) {
                // Carry the last value to the end of the range, including its status, etc.
                AFValue val = new AFValue(last);
                // Except we want to change its Timestamp
                val.Timestamp = EndTime;
                values.Add(val);
            }
        }
    }
}
