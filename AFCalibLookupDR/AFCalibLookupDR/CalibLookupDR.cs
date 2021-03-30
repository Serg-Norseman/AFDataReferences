using System;
using System.ComponentModel;
using System.Data;
using System.Runtime.InteropServices;
using OSIsoft.AF;
using OSIsoft.AF.Asset;
using OSIsoft.AF.Data;
using OSIsoft.AF.Time;

namespace AFCalibLookupDR
{
    [Guid("698C6198-C019-41B7-9590-BEC5E946C7FD"), Description("Calibration Lookup;Calibration Lookup reference")]
    public class CalibLookupDR : AFDataReference
    {
        private string fAttributeName;
        private bool fChecked;
        private string fTableName;
        private string fKeyField;
        private string fValueField;


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

        [Category("Configuration"), DefaultValue(""), Description("The calibration table name")]
        public string TableName
        {
            get {
                return fTableName;
            }
            set {
                if (fTableName != value) {
                    fTableName = value;
                    if (fTableName != null) {
                        fTableName = fTableName.Trim();
                    }

                    base.SaveConfigChanges();
                    UnloadParameters();
                }
            }
        }

        [Category("Configuration"), DefaultValue(""), Description("The key field name")]
        public string KeyField
        {
            get {
                return fKeyField;
            }
            set {
                if (fKeyField != value) {
                    fKeyField = value;
                    if (fKeyField != null) {
                        fKeyField = fKeyField.Trim();
                    }

                    base.SaveConfigChanges();
                    UnloadParameters();
                }
            }
        }

        [Category("Configuration"), DefaultValue(""), Description("The calibration table name")]
        public string ValueField
        {
            get {
                return fValueField;
            }
            set {
                if (fValueField != value) {
                    fValueField = value;
                    if (fValueField != null) {
                        fValueField = fValueField.Trim();
                    }

                    base.SaveConfigChanges();
                    UnloadParameters();
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
                    result = string.Format("Attribute={0}", AttributeName);
                }
                if (!string.IsNullOrEmpty(TableName)) {
                    if (!string.IsNullOrEmpty(result))
                        result += ";";
                    result += string.Format("Table={0}", TableName);
                }
                if (!string.IsNullOrEmpty(KeyField)) {
                    if (!string.IsNullOrEmpty(result))
                        result += ";";
                    result += string.Format("KeyField={0}", KeyField);
                }
                if (!string.IsNullOrEmpty(ValueField)) {
                    if (!string.IsNullOrEmpty(result))
                        result += ";";
                    result += string.Format("ValueField={0}", ValueField);
                }
                return result;
            }
            set {
                if (ConfigString != value) {
                    AttributeName = "";
                    TableName = "";
                    KeyField = "";
                    ValueField = "";

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

                                    case "TABLE":
                                        TableName = paramVal;
                                        break;

                                    case "KEYFIELD":
                                        KeyField = paramVal;
                                        break;

                                    case "VALUEFIELD":
                                        ValueField = paramVal;
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
            get { return typeof(CalibLookupDRConfig); }
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
            //fParamAttributes = null;
            fChecked = false;
        }

        private AFAttributeList LoadParameters()
        {
            if (Attribute == null || Attribute.Element == null) {
                throw new ApplicationException("Attribute and/or element is null");
            }

            var paramAttributes = new AFAttributeList();

            if (!string.IsNullOrEmpty(AttributeName)) {
                // find Attribute's object by it name from parameters (this attribute contains key values)
                var refAttr = AFAttribute.FindAttribute(AttributeName, Attribute);
                if (refAttr == null) {
                    throw new ApplicationException(string.Format(Resources.ERR_AttributeHasNotBeenFound, AttributeName));
                }

                if (!Extensions.IsNumericType(refAttr.Type)) {
                    throw new ApplicationException(string.Format("The attribute `{0}` has no numeric type ", AttributeName));
                }

                paramAttributes.Add(refAttr);

                //fLastLoadAttribute = DateTime.UtcNow;
            } else {
                throw new ApplicationException("Name of lookup attribute is null or empty");
            }

            return paramAttributes;
        }

        public override AFAttributeList GetInputs(object context)
        {
            /*if (DateTime.UtcNow.Subtract(fLastLoadAttribute).Seconds > 10 || fParamAttributes == null) {
                LoadParameters();
            }*/
            var paramAttributes = LoadParameters();
            return paramAttributes;
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
                var time = Extensions.ToAFTime(Attribute.Element, timeContext);

                AFValue result = Calculate(time, inputAttributes, inputValues);
                return result;
            } catch (Exception) {
                UnloadParameters();
                throw;
            }
        }

        private AFValue Calculate(AFTime time, AFAttributeList inputAttributes, AFValues inputValues)
        {
            if (time == null) {
                throw new ArgumentException("No timestamp");
            }

            if (inputAttributes == null || inputAttributes.Count == 0) {
                throw new ArgumentException("No input attributes");
            }

            if (inputValues == null || inputValues.Count == 0) {
                throw new ArgumentException(Resources.ERR_NoInputValues);
            }


            AFValue inVal;
            if (inputAttributes.Count > 0) {
                inVal = inputAttributes[0].GetValue(time);

                if (inVal == null) {
                    throw new ApplicationException("Input value is null");
                } else {
                    AFEnumerationValue enumValue = inVal.Value as AFEnumerationValue;
                    if (enumValue != null && enumValue.Value == 248) {
                        // Attempting to handle an error when the output value is 248 instead of the NoData state
                        return AFValue.CreateSystemStateValue(this.Attribute, AFSystemStateCode.NoData, time);
                    } else {
                        double keyValue = (double)inVal.Value;
                        double resultValue = GetCalibratedValue(keyValue);

                        if (!double.IsNaN(resultValue)) {
                            return new AFValue(this.Attribute, resultValue, inVal.Timestamp, this.Attribute.DefaultUOM);
                        } else {
                            return AFValue.CreateSystemStateValue(this.Attribute, AFSystemStateCode.NoData, time);
                        }
                    }
                }
            } else {
                throw new ApplicationException(string.Format("Input attribute not found (#{0}", 1));
            }
        }

        private double GetCalibratedValue(double keyValue, double epsilon = Extensions.EPSILON)
        {
            AFDatabase afDB = base.Database;
            AFTable afTable = afDB.Tables[fTableName];
            if (afTable == null) {
                throw new ArgumentException("Table not found");
            }

            DataRow[] retrievedRows = afTable.Table.Select();
            if (retrievedRows.Length > 0) {
                double prevKey = double.NaN;
                double prevValue = double.NaN;

                for (int i = 0; i < retrievedRows.Length; i++) {
                    var row = retrievedRows[i];

                    double currKey = Convert.ToDouble(row[fKeyField]);
                    double currValue = Convert.ToDouble(row[fValueField]);

                    if (Extensions.Equals(currKey, keyValue, epsilon)) {
                        return currValue;
                    } else {
                        if (i > 0) {
                            if (Extensions.GreaterThan(keyValue, prevKey, epsilon) && Extensions.LessThan(keyValue, currKey, epsilon)) {
                                double hk = (keyValue - prevKey) / (currKey - prevKey);
                                double k = prevValue + (currValue - prevValue) * hk;
                                return k;
                            }
                        }
                    }

                    prevKey = currKey;
                    prevValue = currValue;
                }

                return double.NaN;
            } else {
                throw new ArgumentException("Table is empty");
            }
        }
    }
}
