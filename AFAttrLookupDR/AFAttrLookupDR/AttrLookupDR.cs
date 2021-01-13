using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;
using OSIsoft.AF;
using OSIsoft.AF.Asset;
using OSIsoft.AF.Data;
using OSIsoft.AF.EventFrame;
using OSIsoft.AF.Time;

namespace AFAttrLookupDR
{
    internal enum RefAttributeType
    {
        DynamicPath, // Attribute is a dynamic StringBuilder's path to another attribute with an actual value
        DirectValue  // Attribute is a direct pointing of an attribute with an actual value
    }

    [Guid("53B692D6-39B2-4823-971A-68BEA625B647"), Description("Attribute Lookup;Attribute Lookup reference")]
    public class AttrLookupDR : AFDataReference
    {
        private string fAttributeName;
        private bool fChecked;
        private DateTime fLastLoadAttribute = DateTime.UtcNow;
        private AFAttributeList fParamAttributes;
        private string fRefAttrType;
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
                return fRefAttrType;
            }
            set {
                if (fRefAttrType != value) {
                    fRefAttrType = value;
                    if (fRefAttrType != null) {
                        fRefAttrType = fRefAttrType.Trim();
                    }
                    base.SaveConfigChanges();
                }
            }
        }

        // Obsolete property name
        internal RefAttributeType MethodType
        {
            get {
                if (fRefAttrType == "DynamicLink") {
                    return RefAttributeType.DynamicPath;
                } else if (fRefAttrType == "DirectLinkAndTS") {
                    return RefAttributeType.DirectValue;
                }
                return RefAttributeType.DynamicPath;
            }
            set {
                switch (value) {
                    case RefAttributeType.DynamicPath:
                        Method = "DynamicLink";
                        break;
                    case RefAttributeType.DirectValue:
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

        public override bool Step
        {
            get {
                return false;
            }
        }

        public override string ConfigString
        {
            get {
                string result = "";
                if (!string.IsNullOrEmpty(AttributeName)) {
                    result = string.Format("Attribute={0}", AttributeName);
                }
                if (!string.IsNullOrEmpty(Method)) {
                    if (!string.IsNullOrEmpty(result))
                        result += ";";
                    result += string.Format("Method={0}", Method);
                }
                if (!string.IsNullOrEmpty(TimestampSource)) {
                    if (!string.IsNullOrEmpty(result))
                        result += ";";
                    result += string.Format("TSSource={0}", TimestampSource);
                }
                if (!string.IsNullOrEmpty(TimestampType)) {
                    if (!string.IsNullOrEmpty(result))
                        result += ";";
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
                            string paramKey = array[i];
                            string paramVal = (++i < array.Length) ? array[i] : string.Empty;

                            if (!string.IsNullOrEmpty(paramKey)) {
                                string a = paramKey.ToUpper();
                                switch (a) {
                                    case "ATTRIBUTE":
                                        AttributeName = paramVal;
                                        break;

                                    case "METHOD":
                                        Method = paramVal;
                                        break;

                                    case "TSSOURCE":
                                        TimestampSource = paramVal;
                                        break;

                                    case "TSTYPE":
                                        TimestampType = paramVal;
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
            get { return typeof(AttrLookupDRConfig); }
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

                AFDatabase db = Attribute.Database;
                if (db == null) {
                    throw new ApplicationException("No database found");
                }

                // find Attribute's object by it name from parameters (this attribute contains link to other attribute with real values)
                var refAttr = AFAttribute.FindAttribute(AttributeName, Attribute);
                if (refAttr == null) {
                    throw new ApplicationException(string.Format(Resources.ERR_AttributeHasNotBeenFound, AttributeName));
                }

                if (MethodType == RefAttributeType.DynamicPath) {
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
                // Important to note that the order of inputValues matches the order of inputAttributes.
                // Note that timeContext is an object. 
                // We need to examine it further in order to resolve it to an AFTime.
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

            if (TimestampType == "Attribute") {
                if (inputValues.Count > 1) {
                    AFValue tsVal = inputValues[1];
                    object objVal = tsVal.Value;
                    if (objVal != null) {
                        if (Extensions.IsDateTimeVal(objVal)) {
                            time = new AFTime(objVal);
                        } else {
                            throw new ApplicationException(string.Format("Timestamp attribute must be datetime type {0}", objVal.ToString()));
                        }
                    } else {
                        throw new ApplicationException("Timestamp value is null");
                    }
                } else {
                    throw new ApplicationException("Insufficient input values");
                }
            }

            AFValue inVal = inputAttributes[0].GetValue(time);

            return new AFValue(base.Attribute, inVal.Value, inVal.Timestamp, this.Attribute.DefaultUOM);
        }

        public override string GetToolTip()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("Attribute Lookup CDR tooltip:\n");
            if (fParamAttributes != null && fParamAttributes.Count > 0) {
                foreach (var param in fParamAttributes) {
                    stringBuilder.Append("  Parameter attribute: ");
                    stringBuilder.Append(param.GetPath());
                    stringBuilder.Append("\n");
                }
            }
            return stringBuilder.ToString();
        }
    }
}
