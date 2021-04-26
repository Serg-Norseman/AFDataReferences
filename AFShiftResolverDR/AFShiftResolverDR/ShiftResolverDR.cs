using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using OSIsoft.AF.Asset;
using OSIsoft.AF.Data;
using OSIsoft.AF.EventFrame;
using OSIsoft.AF.Time;

namespace AFShiftResolverDR
{
    [Guid("7C002DD9-F871-4AF0-9F5B-5207081EF227"), Description("Shift Resolver")]
    public class ShiftResolverDR : AFDataReference
    {
        private string fShiftMode;
        private int fStartOffset;
        private bool fChecked;
        private DateTime fLastLoadAttribute = DateTime.UtcNow;
        private AFAttributeList fParamAttributes;


        public string ShiftMode
        {
            get {
                return fShiftMode;
            }
            set {
                if (fShiftMode != value) {
                    fShiftMode = value;
                    if (fShiftMode != null) {
                        fShiftMode = fShiftMode.Trim();
                    }

                    base.SaveConfigChanges();
                    UnloadParameters();
                }
            }
        }

        public int StartOffset
        {
            get {
                return fStartOffset;
            }
            set {
                if (fStartOffset != value) {
                    fStartOffset = value;

                    base.SaveConfigChanges();
                    UnloadParameters();
                }
            }
        }

        public static object CreateDataPipe()
        {
            EventSource pipe = new EventSource();
            return pipe;
        }

        public override AFDataReferenceContext SupportedContexts
        {
            get {
                return AFDataReferenceContext.All;
            }
        }

        public override AFDataReferenceMethod SupportedMethods
        {
            get {
                return AFDataReferenceMethod.GetValue | AFDataReferenceMethod.GetValues;
            }
        }

        public override AFDataMethods SupportedDataMethods
        {
            get {
                //return base.DefaultSupportedDataMethods;
                return AFDataMethods.DataPipe;
            }
        }

        public override bool Step
        {
            get {
                return true;
            }
        }

        public override string ConfigString
        {
            get {
                string result = "";
                if (!string.IsNullOrEmpty(ShiftMode) || StartOffset != 0) {
                    result = string.Format("ShiftMode={0};StartOffset={1}", ShiftMode, StartOffset);
                }
                return result;
            }
            set {
                if (ConfigString != value) {
                    ShiftMode = "";
                    StartOffset = 0;

                    if (value != null) {
                        string[] array = value.Split(new char[] { ';', '=' });
                        int i = 0;
                        while (i < array.Length) {
                            string paramKey = array[i];
                            string paramVal = (++i < array.Length) ? array[i] : string.Empty;

                            if (!string.IsNullOrEmpty(paramKey)) {
                                string a = paramKey.ToUpper();
                                switch (a) {
                                    case "SHIFTMODE":
                                        ShiftMode = paramVal;
                                        break;

                                    case "STARTOFFSET":
                                        StartOffset = Convert.ToInt32(paramVal);
                                        break;

                                    default:
                                        throw new ArgumentException(string.Format(Resources.ERR_UnrecognizedConfigurationSetting, paramKey, value));
                                }
                                i++;
                                continue;
                            }
                        }
                    }
                    base.SaveConfigChanges();
                    UnloadParameters();
                }
            }
        }

        public override Type EditorType
        {
            get { return typeof(ShiftResolverDRConfig); }
        }

        private void CheckConfig()
        {
            if (Attribute == null) {
                UnloadParameters();
                string message = string.Format(Resources.ERR_AttributeHasNotBeenSet, base.Name);
                throw new InvalidOperationException(message);
            }

            if (string.IsNullOrEmpty(ConfigString)) {
                UnloadParameters();
                string message2 = string.Format(Resources.ERR_DataReferenceNotConfigured, base.Path);
                throw new ApplicationException(message2);
            }

            fChecked = true;
        }

        private void UnloadParameters()
        {
            fParamAttributes = null;
            fChecked = false;
        }

        private void LoadParameters()
        {
            if (Attribute == null || Attribute.Element == null) {
                return;
            }

            if (!string.IsNullOrEmpty(ShiftMode)) {
                fParamAttributes = new AFAttributeList();

                fLastLoadAttribute = DateTime.UtcNow;
            }
        }

        public override AFAttributeList GetInputs(object context)
        {
            if (DateTime.UtcNow.Subtract(fLastLoadAttribute).Seconds > 10 || fParamAttributes == null) {
                LoadParameters();
            }
            return fParamAttributes;
        }

        private AFTime ToAFTime(object timeContext)
        {
            if (timeContext is AFTime) {
                return (AFTime)timeContext;
            } else if (timeContext is AFTimeRange) {
                var timeRange = (AFTimeRange)timeContext;
                return (Attribute.Element is AFEventFrame) ? timeRange.StartTime : timeRange.EndTime;
            }

            return AFTime.NowInWholeSeconds;
        }

        public override AFValue GetValue(object context, object timeContext, AFAttributeList inputAttributes, AFValues inputValues)
        {
            if (!fChecked) {
                CheckConfig();
            }

            try {
                var time = ToAFTime(timeContext);

                AFValue result;
                result = Calculate(time, inputAttributes, inputValues);
                return result;
            } catch (Exception) {
                UnloadParameters();
                fChecked = false;
                throw;
            }
        }

        private AFValue Calculate(AFTime time, AFAttributeList inputAttributes, AFValues inputValues)
        {
            int shiftMode = SRHelper.TryGetShiftMode(ShiftMode);
            if (shiftMode == -1) {
                try {
                    var attr = AFAttribute.FindAttribute(ShiftMode, Attribute);
                    if (attr == null) {
                        throw new ArgumentException(string.Format(Resources.ERR_AttributeHasNotBeenFound, ShiftMode));
                    } else {
                        object val = attr.GetValue().Value;
                        if (SRHelper.IsIntVal(val)) {
                            shiftMode = (int)SRHelper.ConvertToType(val, TypeCode.Int32);
                        } else {
                            shiftMode = -1;
                        }
                    }
                } catch {
                    shiftMode = -1;
                }
            }

            if (shiftMode == -1) {
                throw new ArgumentException(Resources.ERR_SourceAttributeMustBeAnIntegerType);
            }

            double[] wsParams = SRHelper.GetWorkShiftParams(time.LocalTime, shiftMode, StartOffset);
            return new AFValue(base.Attribute, wsParams, time, Attribute.DefaultUOM);
        }
    }
}
