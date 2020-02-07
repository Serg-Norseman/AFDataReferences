using System;
using System.Windows.Forms;
using BSLib;
using OSIsoft.AF.Asset;

namespace AFTransformerDR
{
    internal partial class TransformerDRConfig : Form
    {
        private TransformerDR fDataReference;

        private void ProcessAttributes(AFAttributes attributes, string prefix)
        {
            foreach (AFAttribute attr in attributes) {
                if (attr == fDataReference.Attribute) continue;

                string attrName = prefix + "|" + attr.Name;

                if (TransformerCore.IsNumericVal(attr.Type)) {
                    cmbSourceAttribute.Items.Add(attrName);
                }

                ProcessAttributes(attr.Attributes, attrName);
            }
        }

        public TransformerDRConfig(TransformerDR dataReference, bool bReadOnly)
        {
            InitializeComponent();

            fDataReference = dataReference;

            AFElement elem = dataReference.Attribute.Element as AFElement;
            if (elem != null) {
                ProcessAttributes(elem.Attributes, "");
            }

            if (bReadOnly) {
                cmbSourceAttribute.Enabled = false;
                rbMethod.Enabled = false;

                btnOK.Visible = false;
                btnOK.Enabled = false;
                btnCancel.Left = (btnOK.Left + btnCancel.Left) / 2;
                btnCancel.Text = "Close";

                base.AcceptButton = btnCancel;
            }

            if (!string.IsNullOrEmpty(dataReference.SourceAttributeName)) {
                cmbSourceAttribute.Text = dataReference.SourceAttributeName;
            }

            if (!string.IsNullOrEmpty(dataReference.ConvertMethod)) {
                rbMethod.Value = Enum.Parse(typeof(MethodEnum), dataReference.ConvertMethod);
            }
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
                fDataReference.SourceAttributeName = cmbSourceAttribute.Text;
                fDataReference.ConvertMethod = rbMethod.Value.GetDescription();
                result = true;
            } catch (Exception ex) {
                MessageBox.Show(string.Format(Resources.ERR_UnableToApplyChanges, ex.Message), Resources.TITLE_Error);
                result = false;
            }
            
            return result;
        }
    }
}
