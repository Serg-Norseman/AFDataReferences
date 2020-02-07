using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using OSIsoft.AF;
using OSIsoft.AF.Asset;
using OSIsoft.AF.Data;
using OSIsoft.AF.Time;

namespace AFTransformerDR
{
    [Guid("35147F94-5878-43F0-99BD-2AD2633226E9"), Description("Transformer;Attribute values transformer")]
    public class TransformerDR : AFDataReference
    {
        private string fSourceAttributeName;
        private string fConvertMethod;
        private bool fChecked;
        private DateTime fLastLoadAttribute = DateTime.UtcNow;
        private AFAttributeList fParamAttributes;


        [Category("Configuration"), DefaultValue(""), Description("The source attribute for converting values")]
        public string SourceAttributeName
        {
            get {
                return fSourceAttributeName;
            }
            set {
                if (fSourceAttributeName != value) {
                    fSourceAttributeName = value;
                    if (fSourceAttributeName != null) {
                        fSourceAttributeName = fSourceAttributeName.Trim();
                    }

                    base.SaveConfigChanges();
                    UnloadParameters();
                }
            }
        }

        [Category("Configuration"), DefaultValue(""), Description("Convert method (none, simple, full).")]
        public string ConvertMethod
        {
            get {
                return fConvertMethod;
            }
            set {
                if (fConvertMethod != value) {
                    fConvertMethod = value;
                    if (fConvertMethod != null) {
                        fConvertMethod = fConvertMethod.Trim();
                    }

                    base.SaveConfigChanges();
                    UnloadParameters();
                }
            }
        }

        #region DR standard properties

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
                if (!string.IsNullOrEmpty(fSourceAttributeName) || !string.IsNullOrEmpty(fConvertMethod)) {
                    result = string.Format("SourceAttribute={0};ConvertMethod={1}", fSourceAttributeName, fConvertMethod);
                }
                return result;
            }
            set {
                if (ConfigString != value) {
                    fSourceAttributeName = "";
                    fConvertMethod = "";

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
                                if (a != "SOURCEATTRIBUTE") {
                                    if (a != "CONVERTMETHOD") {
                                        goto IL_A1;
                                    }
                                    fConvertMethod = text2.Trim();
                                } else {
                                    fSourceAttributeName = text2.Trim();
                                }
                                i++;
                                continue;
                            }
                            IL_A1:
                            throw new ArgumentException(string.Format(Resources.ERR_UnrecognizedConfigurationSetting, text, value));
                        }
                    }

                    base.SaveConfigChanges();
                    UnloadParameters();
                }
            }
        }

        public override Type EditorType
        {
            get { return typeof(TransformerDRConfig); }
        }

        #endregion


        public override AFAttributeList GetInputs(object context)
        {
            if (DateTime.UtcNow.Subtract(fLastLoadAttribute).Seconds > 10 || fParamAttributes == null) {
                fParamAttributes = null;
                LoadParameters();
            }
            return fParamAttributes;
        }

        public override AFValue GetValue(object context, object timeContext, AFAttributeList inputAttributes, AFValues inputValues)
        {
            if (!fChecked) {
                CheckConfig();
            }

            AFValue result;
            try {
                result = Calculate(inputValues);
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
                fChecked = false;
                throw;
            }
            return values;
        }


        private void CheckConfig()
        {
            if (Attribute == null) {
                UnloadParameters();
                throw new InvalidOperationException(string.Format(Resources.ERR_AttributeHasNotBeenSet, base.Name));
            }

            if (string.IsNullOrEmpty(ConfigString)) {
                UnloadParameters();
                throw new ApplicationException(string.Format(Resources.ERR_DataReferenceNotConfigured, base.Path));
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

            if (!string.IsNullOrEmpty(fSourceAttributeName) && fParamAttributes == null) {
                fParamAttributes = new AFAttributeList();

                var attr = AFAttribute.FindAttribute(fSourceAttributeName, Attribute);
                if (attr == null) {
                    throw new ArgumentException(string.Format(Resources.ERR_AttributeHasNotBeenFound, SourceAttributeName));
                } else {
                    fParamAttributes.Add(attr);
                }

                fLastLoadAttribute = DateTime.UtcNow;
            }
        }


        private AFValue Calculate(AFValues inputValues)
        {
            MethodEnum method;
            if (!Enum.TryParse(ConvertMethod, out method)) {
                throw new ArgumentException(Resources.ERR_NoBitSpecified);
            }

            if (inputValues == null || inputValues.Count == 0) {
                throw new ArgumentException(Resources.ERR_NoInputValues);
            }

            AFValue inVal = inputValues[0];
            AFDatabase db = inVal.Attribute.Database;

            if (db == null) {
                throw new ArgumentException("No db found");
            }

            object objVal = inVal.Value;
            if (inVal.IsGood && objVal != null) {
                if (TransformerCore.IsNumericVal(objVal)) {
                    AFValue newVal;

                    switch (method) {
                        default:
                        case MethodEnum.None:
                            newVal = new AFValue(objVal, inVal.Timestamp, inVal.UOM);
                            break;

                        case MethodEnum.Simple:
                            var targetUOMAttrS = AFAttribute.FindAttribute("UOM", Attribute);
                            newVal = Convert(inVal, db, targetUOMAttrS);
                            break;

                        case MethodEnum.Full:
                            var sourceUOMAttr = AFAttribute.FindAttribute("UOM", inVal.Attribute);
                            newVal = Convert(inVal, db, sourceUOMAttr);
                            var targetUOMAttrF = AFAttribute.FindAttribute("UOM", Attribute);
                            newVal = Convert(newVal, db, targetUOMAttrF);
                            break;
                    }

                    return newVal;
                } else {
                    throw new ArgumentException(Resources.ERR_SourceAttributeMustBeAnIntegerType);
                }
            } else {
                return AFValue.CreateSystemStateValue(Attribute, AFSystemStateCode.BadInput, inVal.Timestamp);
            }
        }


        private AFValue Convert(AFValue inVal, AFDatabase db, AFAttribute uomAttr)
        {
            AFValue result;

            if (uomAttr == null) {
                throw new ArgumentException("No UOM attribute " + Attribute.Name + "|UOM");
            } else {
                var targetUOMVal = uomAttr.GetValue().Value as string;
                if (string.IsNullOrEmpty(targetUOMVal)) {
                    throw new ArgumentException("UOM attribute " + Attribute.Name + "|UOM is empty");
                } else {
                    result = inVal.Convert(db.UOMs[targetUOMVal]);
                }
            }

            return result;
        }
    }
}
