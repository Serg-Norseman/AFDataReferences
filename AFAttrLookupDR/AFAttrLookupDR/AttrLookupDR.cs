using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using OSIsoft.AF;
using OSIsoft.AF.Asset;
using OSIsoft.AF.Data;
using OSIsoft.AF.EventFrame;
using OSIsoft.AF.Time;

namespace AFAttrLookupDR
{
    internal enum MethodType
    {
        DynamicLink,
        DirectLinkAndTS
    }

    [Guid("53B692D6-39B2-4823-971A-68BEA625B647"), Description("Attribute Lookup;Attribute Lookup reference")]
    public class AttrLookupDR : AFDataReference
    {
        private string fAttributeName;
        private string fMethod;
        private bool fChecked;
        private DateTime fLastLoadAttribute = DateTime.UtcNow;
        private AFAttributeList fParamAttributes;
        private string fTimestampSource;
        private string fTimestampType;


        [Category("Configuration"), DefaultValue(""), Description("The source attribute for get values")]
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

        [Category("Configuration"), DefaultValue(""), Description("Method")]
        public string Method
        {
            get {
                return fMethod;
            }
            set {
                if (fMethod != value) {
                    fMethod = value;
                    if (fMethod != null) {
                        fMethod = fMethod.Trim();
                    }
                    base.SaveConfigChanges();
                }
            }
        }

        internal MethodType MethodType
        {
            get {
                if (fMethod == "DynamicLink") {
                    return MethodType.DynamicLink;
                } else if (fMethod == "DirectLinkAndTS") {
                    return MethodType.DirectLinkAndTS;
                }
                return MethodType.DynamicLink;
            }
            set {
                switch (value) {
                    case MethodType.DynamicLink:
                        Method = "DynamicLink";
                        break;
                    case MethodType.DirectLinkAndTS:
                        Method = "DirectLinkAndTS";
                        break;
                    default:
                        Method = "";
                        break;
                }
            }
        }


        [Category("Configuration"), DefaultValue(""), Description("Source of timestamp of value")]
        public string TimestampSource
        {
            get {
                return fTimestampSource;
            }
            set {
                if (fTimestampSource != value) {
                    fTimestampSource = value;
                    if (fTimestampSource != null) {
                        fTimestampSource = fTimestampSource.Trim();
                    }
                    base.SaveConfigChanges();
                }
            }
        }

        [Category("Configuration"), DefaultValue(""), Description("Type of timestamp of value")]
        public string TimestampType
        {
            get {
                return fTimestampType;
            }
            set {
                if (fTimestampType != value) {
                    fTimestampType = value;
                    if (fTimestampType != null) {
                        fTimestampType = fTimestampType.Trim();
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

        public override string ConfigString
        {
            get {
                string result = "";
                if (!string.IsNullOrEmpty(AttributeName)) {
                    //result = string.Format("Attribute={0};Method={1}", this.AttributeName, this.Method);
                    result = string.Format("Attribute={0}", AttributeName);
                }
                if (!string.IsNullOrEmpty(Method)) {
                    if (!string.IsNullOrEmpty(result)) result += ";";
                    result += string.Format("Method={0}", Method);
                }
                if (!string.IsNullOrEmpty(TimestampSource)) {
                    if (!string.IsNullOrEmpty(result)) result += ";";
                    result += string.Format("TSSource={0}", TimestampSource);
                }
                if (!string.IsNullOrEmpty(TimestampType)) {
                    if (!string.IsNullOrEmpty(result)) result += ";";
                    result += string.Format("TSType={0}", TimestampType);
                }
                return result;
            }
            set {
                if (ConfigString != value) {
                    AttributeName = "";
                    Method = "";

                    if (value != null) {
                        string[] array = value.Split(new char[] { ';', '=' });
                        int i = 0;
                        while (i < array.Length) {
                            string text = array[i];
                            string text2 = "";
                            if (++i < array.Length) {
                                text2 = array[i];
                            }
                            string a = text.ToUpper();
                            if (!string.IsNullOrEmpty(a)) {
                                switch (a) {
                                    case "ATTRIBUTE":
                                        AttributeName = text2;
                                        break;

                                    case "METHOD":
                                        Method = text2;
                                        break;

                                    case "TSSOURCE":
                                        TimestampSource = text2;
                                        break;

                                    case "TSTYPE":
                                        TimestampType = text2;
                                        break;

                                    default:
                                        throw new ArgumentException(string.Format(Resources.ERR_UnrecognizedConfigurationSetting, text, value));
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
            get { return typeof(AttrLookupDRConfig); }
        }

        private void CheckConfig()
        {
            if (Attribute == null) {
                UnloadParameters();
                string message = string.Format(Resources.ERR_AttributeHasNotBeenSet, base.Name);
                throw new InvalidOperationException(message);
            }

            if (ConfigString == null || ConfigString.Length <= 0) {
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

            if (!string.IsNullOrEmpty(AttributeName) && fParamAttributes == null) {
                fParamAttributes = new AFAttributeList();

                AFDatabase db = Attribute.Database;
                if (db == null) {
                    throw new ApplicationException("No database found");
                }

                // find Attribute's object by it name from parameters (this attribute contains link to other attribute with real values)
                var refAttr = AFAttribute.FindAttribute(AttributeName, Attribute);
                if (refAttr == null) {
                    throw new ApplicationException(string.Format(Resources.ERR_AttributeHasNotBeenFound, AttributeName));
                }

                if (MethodType == MethodType.DynamicLink) {
                    AFValue inVal = refAttr.GetValue();
                    object objVal = inVal.Value;
                    if (inVal.IsGood && objVal != null) {
                        if (Extensions.IsStringVal(objVal)) {
                            string attrName;
                            try {
                                attrName = objVal.ToString();
                            } catch (Exception ex) {
                                throw new ArgumentException(string.Format(Resources.ERR_UnrecognizedRefValue, inVal.ToString(), AttributeName));
                            }

                            var lookupAttr = AFAttribute.FindAttribute(attrName, db);
                            if (lookupAttr == null) {
                                throw new ArgumentException(string.Format(Resources.ERR_AttributeLookupHasNotBeenFound, attrName));
                            } else {
                                fParamAttributes.Add(lookupAttr);
                            }
                        } else {
                            throw new ApplicationException(Resources.ERR_SourceAttributeMustBeStringType);
                        }
                    } else {
                        throw new ApplicationException(string.Format(Resources.ERR_UnrecognizedRefValue, "null or bad", AttributeName));
                    }
                } else {
                    fParamAttributes.Add(refAttr);
                }

                if (TimestampType == "Attribute") {
                    var tsAttr = AFAttribute.FindAttribute(TimestampSource, Attribute);
                    if (tsAttr == null) {
                        throw new ApplicationException(string.Format(Resources.ERR_AttributeHasNotBeenFound, TimestampSource));
                    }
                    fParamAttributes.Add(tsAttr);
                }

                fLastLoadAttribute = DateTime.UtcNow;
            }
        }

        public override AFAttributeList GetInputs(object context)
        {
            if (DateTime.UtcNow.Subtract(fLastLoadAttribute).Seconds > 10 || fParamAttributes == null) {
                fParamAttributes = null;
                LoadParameters();
            }
            return fParamAttributes;
        }

        private AFTime ToAFTime(object timeContext)
        {
            if (timeContext is AFTime) {
                return (AFTime)timeContext;
            } else if (timeContext is AFTimeRange) {
                var baseElement = Attribute.Element;
                var useStartTime = (baseElement is AFEventFrame);
                var timeRange = (AFTimeRange)timeContext;
                return useStartTime ? timeRange.StartTime : timeRange.EndTime;
            }
            return AFTime.NowInWholeSeconds;
        }

        public override AFValue GetValue(object context, object timeContext, AFAttributeList inputAttributes, AFValues inputValues)
        {
            // Important to note that the order of inputValues matches the order of inputAttributes.

            // Note that timeContext is an object. 
            // We need to examine it further in order to resolve it to an AFTime.
            var time = ToAFTime(timeContext);

            if (!fChecked) {
                CheckConfig();
            }

            AFValue result;
            try {
                result = Calculate(time, inputAttributes, inputValues);
            } catch {
                UnloadParameters();
                fChecked = false;
                throw;
            }
            return result;
        }

        public override AFValues GetValues(object context, AFTimeRange timeContext, int numberOfValues, AFAttributeList inputAttributes, AFValues[] inputValues)
        {
            if (!fChecked) {
                CheckConfig();
            }

            AFValues values;
            try {
                values = base.GetValues(context, timeContext, numberOfValues, inputAttributes, inputValues);
            } catch {
                UnloadParameters();
                throw;
            }
            return values;
        }

        private AFValue Calculate(AFTime time, AFAttributeList inputAttributes, AFValues inputValues)
        {
            if (inputValues.Count == 0) {
                throw new ArgumentException(Resources.ERR_NoInputValues);
            }

            AFValue inVal;

            if (Method == "DirectLinkAndTS") {
                AFTime tsTime;

                if (TimestampType == "Attribute" && inputValues.Count > 1) {
                    AFValue tsVal = inputValues[1];
                    object objVal = tsVal.Value;
                    if (objVal != null) {
                        if (Extensions.IsDateTimeVal(objVal)) {
                            tsTime = new AFTime(objVal);
                        } else {
                            throw new ApplicationException(string.Format("Timestamp attribute must be datetime type {0}", objVal.ToString()));
                        }
                    } else {
                        throw new ApplicationException("Timestamp value is null");
                    }
                } else {
                    tsTime = time;
                }

                inVal = inputAttributes[0].GetValue(tsTime);
            } else {
                inVal = inputAttributes[0].GetValue(time);
                //inVal = inputValues[0];
            }

            return new AFValue(inVal.Value, inVal.Timestamp, this.Attribute.DefaultUOM);
            //return inVal;

            /*if (inVal.IsGood) {
                return inVal;
            } else {
                return AFValue.CreateSystemStateValue(Attribute, AFSystemStateCode.BadInput, inVal.Timestamp);
            }*/
        }
    }
}
