using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using OSIsoft.AF.Asset;
using OSIsoft.AF.Data;
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
        public string Bit
        {
            get {
                return this.fBit;
            }
            set {
                if (this.fBit != value) {
                    this.fBit = value;
                    if (this.fBit != null) {
                        this.fBit = this.fBit.Trim();
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
                if (!string.IsNullOrEmpty(this.AttributeName) || !string.IsNullOrEmpty(this.Bit)) {
                    result = string.Format("Attribute={0};Bit={1}", this.AttributeName, this.Bit);
                }
                return result;
            }
            set {
                if (this.ConfigString != value) {
                    this.AttributeName = "";
                    this.Bit = "";

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
                                    if (!(a == "BIT")) {
                                        goto IL_A1;
                                    }
                                    this.Bit = text2;
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
            get { return typeof(BitmaskDRConfig); }
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

                var attr = AFAttribute.FindAttribute(this.AttributeName, this.Attribute);
                if (attr == null) {
                    throw new ArgumentException(string.Format(Resources.ERR_AttributeHasNotBeenFound, this.AttributeName));
                } else {
                    this.fParamAttributes.Add(attr);
                }

                this.fLastLoadAttribute = DateTime.UtcNow;
            }
        }

        private AFValue Calculate(AFValues inputValues)
        {
            int bit;
            if (!int.TryParse(this.Bit, out bit)) {
                throw new ArgumentException(Resources.ERR_NoBitSpecified);
            }

            if (bit < 0 || bit > (int)BitEnum.bit31) {
                throw new ArgumentException(Resources.ERR_InvalidBit);
            }

            if (inputValues.Count == 0) {
                throw new ArgumentException(Resources.ERR_NoInputValues);
            }

            AFValue inVal = inputValues[0];
            object objVal = inVal.Value;
            if (inVal.IsGood && objVal != null) {
                if (BitmaskCore.IsIntVal(objVal)) {
                    ulong curVal = (ulong)BitmaskCore.ConvertToType(objVal, TypeCode.UInt64);
                    int tempVal = BitmaskCore.GetBit(curVal, (byte)bit);
                    return new AFValue(tempVal, inVal.Timestamp, this.Attribute.DefaultUOM);
                } else {
                    throw new ArgumentException(Resources.ERR_SourceAttributeMustBeAnIntegerType);
                }
            } else {
                return AFValue.CreateSystemStateValue(Attribute, AFSystemStateCode.BadInput, inVal.Timestamp);
            }
        }
    }
}
