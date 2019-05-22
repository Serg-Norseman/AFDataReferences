using System;
using System.Windows.Forms;
using OSIsoft.AF.Asset;

namespace AFAttrLookupDR
{
    internal partial class AttrLookupDRConfig : Form
    {
        private AttrLookupDR dataReference;

        private void ProcessAttributes(AFAttributes attributes, string prefix)
        {
            foreach (AFAttribute attr in attributes) {
                string attrName = prefix + "|" + attr.Name;

                if (attr != dataReference.Attribute) {
                    if (Extensions.IsStringVal(attr.Type)) {
                        txtAttribute.Items.Add(attrName);
                    }
                }

                ProcessAttributes(attr.Attributes, attrName);
            }
        }

        private void ProcessAttributes(AFAttributeTemplates attributes, string prefix)
        {
            foreach (AFAttributeTemplate attr in attributes) {
                string attrName = prefix + "|" + attr.Name;

                //if (attr != dataReference.Attribute.Template) {
                    if (Extensions.IsStringVal(attr.Type)) {
                        txtAttribute.Items.Add(attrName);
                    }
                //}

                ProcessAttributes(attr.AttributeTemplates, attrName);
            }
        }

        public AttrLookupDRConfig(AttrLookupDR dataReference, bool bReadOnly)
        {
            this.InitializeComponent();

            this.dataReference = dataReference;

            if (dataReference.Attribute != null) {
                AFElement elem = dataReference.Attribute.Element as AFElement;
                if (elem != null) {
                    ProcessAttributes(elem.Attributes, "");
                }
            } else if (dataReference.Template != null) {
                AFElementTemplate templ = dataReference.Template.ElementTemplate as AFElementTemplate;
                if (templ != null) {
                    ProcessAttributes(templ.AttributeTemplates, "");
                }
            }

            if (bReadOnly) {
                this.txtAttribute.Enabled = false;

                this.btnOK.Visible = false;
                this.btnOK.Enabled = false;
                this.btnCancel.Left = (this.btnOK.Left + this.btnCancel.Left) / 2;
                this.btnCancel.Text = "Close";

                base.AcceptButton = this.btnCancel;
            }

            if (!string.IsNullOrEmpty(dataReference.AttributeName)) {
                this.txtAttribute.Text = dataReference.AttributeName;
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (!this.GetValuesFromForm()) {
                base.DialogResult = DialogResult.None;
            }
        }

        private bool GetValuesFromForm()
        {
            bool result;
            
            try {
                this.dataReference.AttributeName = this.txtAttribute.Text;
                result = true;
            } catch (Exception ex) {
                MessageBox.Show(string.Format(Resources.ERR_UnableToApplyChanges, ex.Message), Resources.TITLE_Error);
                result = false;
            }
            
            return result;
        }
    }
}
