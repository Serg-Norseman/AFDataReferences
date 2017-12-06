using System;
using System.Windows.Forms;
using BSLib;

namespace AFBitmaskDR
{
    internal partial class BitmaskDRConfig : Form
    {
        private BitmaskDR dataReference;

        public BitmaskDRConfig(BitmaskDR dataReference, bool bReadOnly)
        {
            this.InitializeComponent();

            this.dataReference = dataReference;
            if (bReadOnly) {
                this.txtAttribute.Enabled = false;
                this.rgbBits.Enabled = false;

                this.btnOK.Visible = false;
                this.btnOK.Enabled = false;
                this.btnCancel.Left = (this.btnOK.Left + this.btnCancel.Left) / 2;
                this.btnCancel.Text = "Close";

                base.AcceptButton = this.btnCancel;
            }

            if (!string.IsNullOrEmpty(dataReference.AttributeName)) {
                this.txtAttribute.Text = dataReference.AttributeName;
            }

            if (!string.IsNullOrEmpty(dataReference.Bit)) {
                int iBit;
                if (int.TryParse(dataReference.Bit, out iBit)) {
                    BitEnum eBit = (BitEnum)iBit;
                    this.rgbBits.Value = eBit;
                }
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
                BitEnum bit = (BitEnum)this.rgbBits.Value;

                this.dataReference.AttributeName = this.txtAttribute.Text;
                this.dataReference.Bit = bit.GetDescription();
                result = true;
            } catch (Exception ex) {
                MessageBox.Show(string.Format(Resources.ERR_UnableToApplyChanges, ex.Message), Resources.TITLE_Error);
                result = false;
            }
            
            return result;
        }
    }
}
