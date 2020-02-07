using System;
using System.Windows.Forms;
using OSIsoft.AF.Asset;

namespace AFAttrLookupDR
{
    internal delegate bool TypeFilterHandler(Type valType);

    internal partial class AttrLookupDRConfig : Form
    {
        private AttrLookupDR fDataReference;

        private void ProcessAttributes(AFAttributes attributes, string prefix, TypeFilterHandler filterHandler, ComboBox comboBox)
        {
            foreach (AFAttribute attr in attributes) {
                string attrName = prefix + "|" + attr.Name;

                if (attr != fDataReference.Attribute) {
                    if (filterHandler(attr.Type)) {
                        comboBox.Items.Add(attrName);
                    }
                }

                ProcessAttributes(attr.Attributes, attrName, filterHandler, comboBox);
            }
        }

        private void ProcessAttributes(AFAttributeTemplates attributes, string prefix, TypeFilterHandler filterHandler, ComboBox comboBox)
        {
            foreach (AFAttributeTemplate attr in attributes) {
                string attrName = prefix + "|" + attr.Name;

                //if (attr != dataReference.Attribute.Template) {
                    if (filterHandler(attr.Type)) {
                        comboBox.Items.Add(attrName);
                    }
                //}

                ProcessAttributes(attr.AttributeTemplates, attrName, filterHandler, comboBox);
            }
        }

        public AttrLookupDRConfig(AttrLookupDR dataReference, bool bReadOnly)
        {
            InitializeComponent();

            fDataReference = dataReference;

            if (dataReference.Attribute != null) {
                AFElement elem = dataReference.Attribute.Element as AFElement;
                if (elem != null) {
                    ProcessAttributes(elem.Attributes, "", Extensions.IsStringVal, txtAttribute);
                }
            } else if (dataReference.Template != null) {
                AFElementTemplate templ = dataReference.Template.ElementTemplate as AFElementTemplate;
                if (templ != null) {
                    ProcessAttributes(templ.AttributeTemplates, "", Extensions.IsStringVal, txtAttribute);
                }
            }

            if (dataReference.Attribute != null) {
                AFElement elem = dataReference.Attribute.Element as AFElement;
                if (elem != null) {
                    ProcessAttributes(elem.Attributes, "", Extensions.IsDateTimeVal, cmbTSAttr);
                }
            } else if (dataReference.Template != null) {
                AFElementTemplate templ = dataReference.Template.ElementTemplate as AFElementTemplate;
                if (templ != null) {
                    ProcessAttributes(templ.AttributeTemplates, "", Extensions.IsDateTimeVal, cmbTSAttr);
                }
            }

            if (bReadOnly) {
                txtAttribute.Enabled = false;

                btnOK.Visible = false;
                btnOK.Enabled = false;
                btnCancel.Left = (btnOK.Left + btnCancel.Left) / 2;
                btnCancel.Text = "Close";

                base.AcceptButton = btnCancel;
            }

            if (!string.IsNullOrEmpty(dataReference.AttributeName)) {
                txtAttribute.Text = dataReference.AttributeName;
            }

            if (dataReference.TimestampType == "Attribute") {
                cmbTSAttr.Text = dataReference.TimestampSource;
                radAttribute.Checked = true;
            } else {
                radNone.Checked = true;
            }

            cmbMethod.SelectedIndex = (int)dataReference.MethodType;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (!GetValuesFromForm()) {
                base.DialogResult = DialogResult.None;
            }
        }

        private bool GetValuesFromForm()
        {
            bool result;
            
            try {
                fDataReference.MethodType = (MethodType)cmbMethod.SelectedIndex;

                fDataReference.AttributeName = txtAttribute.Text;

                bool tsAttr = (radAttribute.Checked);
                string tsType = (tsAttr) ? "Attribute" : "None";
                fDataReference.TimestampType = tsType;
                if (tsAttr) {
                    fDataReference.TimestampSource = cmbTSAttr.Text;
                } else {
                    fDataReference.TimestampSource = "";
                }

                result = true;
            } catch (Exception ex) {
                MessageBox.Show(string.Format(Resources.ERR_UnableToApplyChanges, ex.Message), Resources.TITLE_Error);
                result = false;
            }
            
            return result;
        }

        private void radNone_CheckedChanged(object sender, EventArgs e)
        {
            bool tsAttr = (radAttribute.Checked);
            cmbTSAttr.Enabled = tsAttr;
        }
    }
}
