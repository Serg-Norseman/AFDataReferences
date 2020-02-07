using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;
using OSIsoft.AF.Asset;
using OSIsoft.AF.Time;

namespace AFRollupDR
{
    [Guid("9BAF575E-F679-2100-8C94-486571D79A59"), Description("Rollup;Rollup calculation")]
    public class RollupDR : AFDataReference
    {
        #region Private fields

        private string sCategoryName;
        private string sCalculation;
        private bool bChecked;
        private AFAttributeList paramAttributes;
        private DateTime dtLastLoadAttribute = DateTime.UtcNow;

        #endregion

        #region Public properties
		
        [Category("Configuration"), DefaultValue(""), Description("All attributes with the specified category name are operated on by the rollup.")]
        public string CategoryName
        {
            get {
                return this.sCategoryName;
            }
            set {
                if (this.sCategoryName != value) {
                    this.sCategoryName = value;
                    if (this.sCategoryName != null) {
                        this.sCategoryName = this.sCategoryName.Trim();
                    }

                    base.SaveConfigChanges();
                    this.UnloadParameters();
                }
            }
        }


        [Category("Configuration"), DefaultValue(""), Description("The type of the calculation to perform on the child attributes.")]
        public string Calculation
        {
            get {
                return this.sCalculation;
            }
            set {
                if (this.sCalculation != value) {
                    this.sCalculation = value;
                    if (this.sCalculation != null) {
                        this.sCalculation = this.sCalculation.Trim();
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


        public override string ConfigString
        {
            get {
                StringBuilder stringBuilder = new StringBuilder();
                if ((this.CategoryName != null && this.CategoryName.Length > 0) || (this.Calculation != null && this.Calculation.Length > 0)) {
                    stringBuilder.AppendFormat("CategoryName={0}", this.CategoryName);
                    stringBuilder.AppendFormat(";Calculation={0}", this.Calculation);
                }
                return stringBuilder.ToString();
            }
            set {
                if (this.ConfigString != value) {
                    this.CategoryName = "";
                    this.Calculation = "";
                    if (value != null) {
                        string[] array = value.Split(new char[] {
                            ';', 
                            '='
                        });
                        int i = 0;
                        while (i < array.Length) {
                            string text = array[i];
                            string text2 = "";
                            if (++i < array.Length) {
                                text2 = array[i];
                            }
                            string a;
                            if ((a = text.ToUpper()) != null) {
                                if (!(a == "CATEGORYNAME")) {
                                    if (!(a == "CALCULATION")) {
                                        goto IL_A1;
                                    }
                                    this.Calculation = text2;
                                } else {
                                    this.CategoryName = text2;
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
            get {
                return typeof(RollupDRConfig);
            }
        }

        #endregion
		
        #region Overriden functions

        public override AFAttributeList GetInputs(object context)
        {
            if (DateTime.UtcNow.Subtract(this.dtLastLoadAttribute).Seconds > 10 || this.paramAttributes == null) {
                this.paramAttributes = null;
                this.LoadParameters();
            }
            return this.paramAttributes;
        }


        public override AFValue GetValue(object context, object timeContext, AFAttributeList inputAttributes, AFValues inputValues)
        {
            if (!this.bChecked) {
                this.CheckConfig();
            }

            AFValue result;
            try {
                result = this.Calculate(inputValues);
            } catch {
                this.UnloadParameters();
                this.bChecked = false;
                throw;
            }
            return result;
        }


        public override AFValues GetValues(object context, AFTimeRange timeContext, int numberOfValues, AFAttributeList inputAttributes, AFValues[] inputValues)
        {
            if (!this.bChecked) {
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

        #endregion

        #region Private methods

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
            this.bChecked = true;
        }


        private void UnloadParameters()
        {
            this.paramAttributes = null;
            this.bChecked = false;
        }


        private void LoadParameters()
        {
            if (this.Attribute == null) {
                return;
            }

            if (this.Attribute.Element == null) {
                return;
            }

            if (string.IsNullOrEmpty(this.CategoryName)) {
                return;
            }

            if (this.paramAttributes == null) {
                this.paramAttributes = new AFAttributeList();
                AFElement aFElement = this.Attribute.Element as AFElement;

                if (aFElement == null) {
                    return;
                }

                foreach (AFElement current in aFElement.Elements) {
                    foreach (AFAttribute current2 in current.Attributes) {
                        if (current2.Categories.Contains(this.CategoryName)) {
                            this.paramAttributes.Add(current2);
                        }
                    }
                }
                this.dtLastLoadAttribute = DateTime.UtcNow;
            }
        }


        private AFValue Calculate(AFValues inputValues)
        {
            double tempVal = 0.0;

            string calc = this.Calculation.ToUpperInvariant();
            if (calc == null) {
                return new AFValue(tempVal, this.Attribute.DefaultUOM);
            }

            int count = 0;
            bool flag = false;

            foreach (AFValue current in inputValues) {
                if (current.IsGood && current.Value != null) {
                    double curVal;

                    if (current.UOM != null && this.Attribute.DefaultUOM != null && current.UOM != this.Attribute.DefaultUOM) {
                        curVal = this.Attribute.DefaultUOM.Convert(current.Value, current.UOM);
                    } else {
                        curVal = Convert.ToDouble(current.Value);
                    }

                    if (calc == "AVG") {
                        tempVal += curVal;
                        count++;
                    } else if (calc == "SUM") {
                        tempVal += curVal;
                    } else if (calc == "MIN") {
                        if (!flag) {
                            tempVal = curVal;
                            flag = true;
                        } else {
                            tempVal = Math.Min(tempVal, curVal);
                        }
                    } else if (calc == "MAX") {
                        if (!flag) {
                            tempVal = curVal;
                            flag = true;
                        } else {
                            tempVal = Math.Max(tempVal, curVal);
                        }
                    }
                }
            }

            if (calc == "AVG") {
                tempVal /= (double)count;
            }

            return new AFValue(tempVal, this.Attribute.DefaultUOM);
        }

        #endregion
    }
}
