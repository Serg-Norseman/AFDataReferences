using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using OSIsoft.AF.Asset;
using OSIsoft.AF.Data;
using OSIsoft.AF.EventFrame;
using OSIsoft.AF.Time;

namespace AFBitmaskDR
{
    [Guid("3AA544C3-FB5E-41C5-AC87-D3BC21696D99"), Description("Bitmask;Bitmask calculation")]
    public class BitmaskDR : AFDataReference
    {
        private string fAttributeName;
        private string fBit;
        private bool fChecked;
        private DateTime fLastLoadAttribute = DateTime.UtcNow;
        private AFAttributeList fParamAttributes;


        [Category("Configuration"), DefaultValue(""), Description("The source attribute for extracting bits")]
        public string AttributeName
        {
            get {
                return fAttributeName;
            }
            set {
                if (fAttributeName != value) {
                    fAttributeName = value;
                    if (fAttributeName != null) {
                        fAttributeName = fAttributeName.Trim();
                    }

                    base.SaveConfigChanges();
                    UnloadParameters();
                }
            }
        }

        [Category("Configuration"), DefaultValue(""), Description("Extracted bit number.")]
        public string Bit
        {
            get {
                return fBit;
            }
            set {
                if (fBit != value) {
                    fBit = value;
                    if (fBit != null) {
                        fBit = fBit.Trim();
                    }
                    base.SaveConfigChanges();
                }
            }
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
                return base.DefaultSupportedDataMethods;
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
                if (!string.IsNullOrEmpty(AttributeName) || !string.IsNullOrEmpty(Bit)) {
                    result = string.Format("Attribute={0};Bit={1}", AttributeName, Bit);
                }
                return result;
            }
            set {
                if (ConfigString != value) {
                    AttributeName = "";
                    Bit = "";

                    if (value != null) {
                        string[] array = value.Split(new char[] { ';', '=' });
                        int i = 0;
                        while (i < array.Length) {
                            string paramKey = array[i];
                            string paramVal = (++i < array.Length) ? array[i] : string.Empty;

                            if (!string.IsNullOrEmpty(paramKey)) {
                                string a = paramKey.ToUpper();
                                switch (a) {
                                    case "ATTRIBUTE":
                                        AttributeName = paramVal;
                                        break;

                                    case "BIT":
                                        Bit = paramVal;
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
            get { return typeof(BitmaskDRConfig); }
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

            if (!string.IsNullOrEmpty(AttributeName)) {
                fParamAttributes = new AFAttributeList();

                var attr = AFAttribute.FindAttribute(AttributeName, Attribute);
                if (attr == null) {
                    throw new ArgumentException(string.Format(Resources.ERR_AttributeHasNotBeenFound, AttributeName));
                } else {
                    fParamAttributes.Add(attr);
                }

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
            if (inputValues.Count == 0) {
                throw new ArgumentException(Resources.ERR_NoInputValues);
            }

            int bit;
            if (!int.TryParse(Bit, out bit)) {
                throw new ArgumentException(Resources.ERR_NoBitSpecified);
            }

            if (bit < 0 || bit > (int)BitEnum.bit31) {
                throw new ArgumentException(Resources.ERR_InvalidBit);
            }

            AFValue inVal = inputValues[0];
            object objVal = inVal.Value;
            if (inVal.IsGood && objVal != null) {
                if (BitmaskCore.IsIntVal(objVal)) {
                    ulong curVal = (ulong)BitmaskCore.ConvertToType(objVal, TypeCode.UInt64);
                    int tempVal = BitmaskCore.GetBit(curVal, (byte)bit);
                    return new AFValue(base.Attribute, tempVal, inVal.Timestamp, Attribute.DefaultUOM);
                } else {
                    throw new ArgumentException(Resources.ERR_SourceAttributeMustBeAnIntegerType);
                }
            } else {
                return AFValue.CreateSystemStateValue(Attribute, AFSystemStateCode.BadInput, inVal.Timestamp);
            }
        }
    }
}
