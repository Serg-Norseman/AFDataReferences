using System;
using System.Windows.Forms;
using OSIsoft.AF;

namespace AFRollupDR
{
    internal partial class RollupDRConfig : Form
    {
        private RollupDR dataReference;

        public RollupDRConfig(RollupDR dataReference, bool bReadOnly)
        {
            this.InitializeComponent();
            this.dataReference = dataReference;
            if (bReadOnly) {
                this.cboCategories.Enabled = false;
                this.cboCalculations.Enabled = false;
                this.btnOK.Visible = false;
                this.btnOK.Enabled = false;
                this.btnCancel.Left = (this.btnOK.Left + this.btnCancel.Left) / 2;
                this.btnCancel.Text = "Close";
                base.AcceptButton = this.btnCancel;
            }
            AFCategories aFCategories = null;
            if (dataReference.Attribute != null) {
                aFCategories = dataReference.Attribute.Database.AttributeCategories;
            } else {
                if (dataReference.Template != null) {
                    aFCategories = dataReference.Template.Database.AttributeCategories;
                }
            }
            foreach (AFCategory current in aFCategories) {
                this.cboCategories.Items.Add(current.Name);
            }
            if (this.cboCategories.Items.Count > 0) {
                this.cboCategories.SelectedIndex = 0;
            }
            this.cboCalculations.Items.Add("Avg");
            this.cboCalculations.Items.Add("Max");
            this.cboCalculations.Items.Add("Min");
            this.cboCalculations.Items.Add("Sum");
            if (this.cboCalculations.Items.Count > 0) {
                this.cboCalculations.SelectedIndex = 0;
            }
            if (dataReference.CategoryName != null && this.cboCategories.Items.Contains(dataReference.CategoryName)) {
                this.cboCategories.Text = dataReference.CategoryName;
            }
            if (dataReference.Calculation != null && this.cboCalculations.Items.Contains(dataReference.Calculation)) {
                this.cboCalculations.Text = dataReference.Calculation;
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
                this.dataReference.CategoryName = this.cboCategories.Text;
                this.dataReference.Calculation = this.cboCalculations.Text;
                result = true;
            } catch (Exception ex) {
                MessageBox.Show(string.Format(Resources.ERR_UnableToApplyChanges, ex.Message), Resources.TITLE_Error);
                result = false;
            }
			
            return result;
        }
    }
}
