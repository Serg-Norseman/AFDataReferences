using System;
using OSIsoft.AF.Asset;
using OSIsoft.AF.Time;

namespace AFShiftResolverDR
{
    public static class SRHelper
    {
        public static bool IsIntVal(object objVal)
        {
            return (objVal is Int64 || objVal is Int32 || objVal is Int16 || objVal is SByte || objVal is Byte);
        }

        public static bool IsIntVal(Type valType)
        {
            return (valType == typeof(Int64) || valType == typeof(Int32) || valType == typeof(Int16) || valType == typeof(SByte) || valType == typeof(Byte));
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

        public static AFValue CreateBadValue(AFAttribute attribute, AFTime time)
        {
            return AFValue.CreateSystemStateValue(attribute, AFSystemStateCode.NoResult, time);
        }

        public static int TryGetShiftMode(string value)
        {
            try {
                return Convert.ToInt32(value);
            } catch {
                return -1;
            }
        }

        private static readonly DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static double GetPITimeStamp(DateTime time)
        {
            return (time.ToUniversalTime() - epoch).TotalSeconds;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="time">time stamp in shift</param>
        /// <param name="shiftMode">shift mode, number of shifts per day (2 or 3) </param>
        /// <param name="startOffset">offset of the first shift relative to the beginning of the day (minutes)</param>
        /// <returns></returns>
        public static double[] GetWorkShiftParams(DateTime time, int shiftMode, int startOffset = 0)
        {
            var wsParams = new double[3];

            int shiftNum = GetWorkShift(time, shiftMode, startOffset);
            wsParams[0] = shiftNum;

            if (shiftNum == -1) {
                // invalid shift`s mode
                return wsParams;
            }

            int begShift = 0, endShift = 0;

            switch (shiftMode) {
                case 2:
                    // 1 shift: 00:00 - 12:00 -> 0 - 43200
                    // 2 shift: 12:00 - 24:00 -> 43200 - 86400
                    switch (shiftNum) {
                        case 1:
                            begShift = 0;
                            endShift = 43200;
                            break;
                        case 2:
                            begShift = 43200;
                            endShift = 86400;
                            break;
                    }
                    break;

                case 3:
                    // 1 shift: 00:00 - 08:00 -> 0 - 28800
                    // 2 shift: 08:00 - 16:00 -> 28800 - 57600
                    // 3 shift: 16:00 - 24:00 -> 57600 - 86400
                    switch (shiftNum) {
                        case 1:
                            begShift = 0;
                            endShift = 28800;
                            break;
                        case 2:
                            begShift = 28800;
                            endShift = 57600;
                            break;
                        case 3:
                            begShift = 57600;
                            endShift = 86400;
                            break;
                    }
                    break;
            }

            begShift += (startOffset * 60);
            endShift += (startOffset * 60);

            // if the beginning of the first shift is 23 o'clock, then we shift the time by 00 hours (forward by 1 hour, add(-(-1h) -> add(+1h)),
            // if the beginning is at 8 o'clock in the morning, then we shift the time by 00 hours (back by 8 hours, add(-(+8h) -> add(-8h))
            time = time.AddMinutes(-startOffset).Date;

            DateTime begTime, endTime;
            begTime = time.AddSeconds(begShift);
            endTime = time.AddSeconds(endShift);
            wsParams[1] = GetPITimeStamp(begTime);
            wsParams[2] = GetPITimeStamp(endTime);
            return wsParams;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="time">time stamp in shift</param>
        /// <param name="shiftMode">shift mode, number of shifts per day (2 or 3) </param>
        /// <param name="startOffset">offset of the first shift relative to the beginning of the day (minutes)</param>
        /// <returns></returns>
        public static int GetWorkShift(DateTime time, int shiftMode, int startOffset = 0)
        {
            int shiftNum;

            // if the beginning of the first shift is 23 o'clock, then we shift the time by 00 hours (forward by 1 hour, add(-(-1h) -> add(+1h)), 
            // if the beginning is at 8 o'clock in the morning, then we shift the time by 00 hours (back by 8 hours, add(-(+8h) -> add(-8h))
            time = time.AddMinutes(-startOffset);

            int daysecs = (int)Math.Round((time - time.Date).TotalSeconds);

            // 3600 seconds per hour

            if (shiftMode == 2) {
                // 1 shift: 00:00 - 12:00 -> 0 - 43200
                // 2 shift: 12:00 - 24:00 -> 43200 - 86400
                if (daysecs > 43200 & daysecs <= 86400) {
                    shiftNum = 2;
                } else {
                    shiftNum = 1;
                }
            } else if (shiftMode == 3) {
                // 1 shift: 00:00 - 08:00 -> 0 - 28800
                // 2 shift: 08:00 - 16:00 -> 28800 - 57600
                // 3 shift: 16:00 - 24:00 -> 57600 - 86400
                if (daysecs > 28800 & daysecs <= 57600) {
                    shiftNum = 2;
                } else if (daysecs > 57600 & daysecs <= 86400) {
                    shiftNum = 3;
                } else {
                    shiftNum = 1;
                }
            } else {
                // invalid shift`s mode
                shiftNum = -1;
            }

            return shiftNum;
        }
    }
}
