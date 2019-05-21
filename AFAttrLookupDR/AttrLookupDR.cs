using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using OSIsoft.AF;
using OSIsoft.AF.Asset;
using OSIsoft.AF.Data;
using OSIsoft.AF.Time;

namespace AFAttrLookupDR
{
    [Guid("53B692D6-39B2-4823-971A-68BEA625B647"), Description("Attribute Lookup;Attribute Lookup reference")]
    public class AttrLookupDR : AFDataReference
    {
        private string fAttributeName;
        private string fMethod;
        private bool fChecked;
        private DateTime fLastLoadAttribute = DateTime.UtcNow;
        private AFAttributeList fParamAttributes;


        [Category("Configuration"), DefaultValue(""), Description("The source attribute for extracting bits")]
        public string AttributeName
        {
            get {
                return this.fAttributeName;
            }
            set {
                if (this.fAttributeName != value) {
                    this.fAttributeName = value;
                    if (this.fAttributeName != null) {
                        this.fAttributeName = this.fAttributeName.Trim();
                    }

                    base.SaveConfigChanges();
                    this.UnloadParameters();
                }
            }
        }

        [Category("Configuration"), DefaultValue(""), Description("Extracted bit number.")]
        public string Method
        {
            get {
                return this.fMethod;
            }
            set {
                if (this.fMethod != value) {
                    this.fMethod = value;
                    if (this.fMethod != null) {
                        this.fMethod = this.fMethod.Trim();
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
                if (!string.IsNullOrEmpty(this.AttributeName) || !string.IsNullOrEmpty(this.Method)) {
                    //result = string.Format("Attribute={0};Method={1}", this.AttributeName, this.Method);
                    result = string.Format("Attribute={0}", this.AttributeName);
                }
                return result;
            }
            set {
                if (this.ConfigString != value) {
                    this.AttributeName = "";
                    this.Method = "";

                    if (value != null) {
                        string[] array = value.Split(new char[] { ';', '=' });
                        int i = 0;
                        while (i < array.Length) {
                            string text = array[i];
                            string text2 = "";
                            if (++i < array.Length) {
                                text2 = array[i];
                            }
                            string a;
                            if ((a = text.ToUpper()) != null) {
                                if (!(a == "ATTRIBUTE")) {
                                    if (!(a == "METHOD")) {
                                        goto IL_A1;
                                    }
                                    this.Method = text2;
                                } else {
                                    this.AttributeName = text2;
                                }
                                i++;
                                continue;
                            }
                            IL_A1:
                            throw new ArgumentException(string.Format(Resources.ERR_UnrecognizedConfigurationSetting, text, value));
                        }
                    }
                    base.SaveConfigChanges();
                    this.UnloadParameters();
                }
            }
        }

        public override Type EditorType
        {
            get { return typeof(AttrLookupDRConfig); }
        }

        private void CheckConfig()
        {
            if (this.Attribute == null) {
                this.UnloadParameters();
                string message = string.Format(Resources.ERR_AttributeHasNotBeenSet, base.Name);
                throw new InvalidOperationException(message);
            }

            if (this.ConfigString == null || this.ConfigString.Length <= 0) {
                this.UnloadParameters();
                string message2 = string.Format(Resources.ERR_DataReferenceNotConfigured, base.Path);
                throw new ApplicationException(message2);
            }
            this.fChecked = true;
        }

        private void UnloadParameters()
        {
            this.fParamAttributes = null;
            this.fChecked = false;
        }

        private void LoadParameters()
        {
            if (this.Attribute == null || this.Attribute.Element == null) {
                return;
            }

            if (!string.IsNullOrEmpty(this.AttributeName) && this.fParamAttributes == null) {
                this.fParamAttributes = new AFAttributeList();

                var refAttr = AFAttribute.FindAttribute(this.AttributeName, this.Attribute);
                if (refAttr == null) {
                    throw new ApplicationException(string.Format(Resources.ERR_AttributeHasNotBeenFound, this.AttributeName));
                }

                AFDatabase db = Attribute.Database;
                if (db == null) {
                    throw new ApplicationException("No database found");
                }

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
                            this.fParamAttributes.Add(lookupAttr);
                        }
                    } else {
                        throw new ApplicationException(Resources.ERR_SourceAttributeMustBeStringType);
                    }
                } else {
                    throw new ApplicationException(string.Format(Resources.ERR_UnrecognizedRefValue, "null or bad", AttributeName));
                }

                this.fLastLoadAttribute = DateTime.UtcNow;
            }
        }

        public override AFAttributeList GetInputs(object context)
        {
            if (DateTime.UtcNow.Subtract(this.fLastLoadAttribute).Seconds > 10 || this.fParamAttributes == null) {
                this.fParamAttributes = null;
                this.LoadParameters();
            }
            return this.fParamAttributes;
        }

        public override AFValue GetValue(object context, object timeContext, AFAttributeList inputAttributes, AFValues inputValues)
        {
            if (!this.fChecked) {
                this.CheckConfig();
            }

            AFValue result;
            try {
                result = this.Calculate(inputValues);
            } catch {
                this.UnloadParameters();
                this.fChecked = false;
                throw;
            }
            return result;
        }

        public override AFValues GetValues(object context, AFTimeRange timeContext, int numberOfValues, AFAttributeList inputAttributes, AFValues[] inputValues)
        {
            if (!this.fChecked) {
                this.CheckConfig();
            }

            AFValues values;
            try {
                values = base.GetValues(context, timeContext, numberOfValues, inputAttributes, inputValues);
            } catch {
                this.UnloadParameters();
                throw;
            }
            return values;
        }

        private AFValue Calculate(AFValues inputValues)
        {
            if (inputValues.Count == 0) {
                throw new ArgumentException(Resources.ERR_NoInputValues);
            }

            AFValue inVal = inputValues[0];
            if (inVal.IsGood) {
                return inVal;
            } else {
                return AFValue.CreateSystemStateValue(Attribute, AFSystemStateCode.BadInput, inVal.Timestamp);
            }
        }
    }
}
