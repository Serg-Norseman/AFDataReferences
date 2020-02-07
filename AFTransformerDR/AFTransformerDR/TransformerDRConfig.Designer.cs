using System;

namespace AFTransformerDR
{
	internal partial class TransformerDRConfig
	{
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Button btnCancel;
		private System.ComponentModel.Container components;
		private System.Windows.Forms.Label lblAttribute;
		private System.Windows.Forms.ComboBox cmbSourceAttribute;

		private void InitializeComponent()
		{
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.lblAttribute = new System.Windows.Forms.Label();
            this.cmbSourceAttribute = new System.Windows.Forms.ComboBox();
            this.rbMethod = new BSLib.WinForms.Controls.EnumRadioBox();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(366, 272);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(90, 27);
            this.btnOK.TabIndex = 5;
            this.btnOK.Text = "OK";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(462, 272);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(90, 27);
            this.btnCancel.TabIndex = 6;
            this.btnCancel.Text = "Cancel";
            // 
            // lblAttribute
            // 
            this.lblAttribute.AutoSize = true;
            this.lblAttribute.Location = new System.Drawing.Point(12, 9);
            this.lblAttribute.Name = "lblAttribute";
            this.lblAttribute.Size = new System.Drawing.Size(145, 17);
            this.lblAttribute.TabIndex = 2;
            this.lblAttribute.Text = "SourceValue attribute";
            // 
            // cmbSourceAttribute
            // 
            this.cmbSourceAttribute.FormattingEnabled = true;
            this.cmbSourceAttribute.Location = new System.Drawing.Point(174, 6);
            this.cmbSourceAttribute.Name = "cmbSourceAttribute";
            this.cmbSourceAttribute.Size = new System.Drawing.Size(378, 24);
            this.cmbSourceAttribute.TabIndex = 11;
            // 
            // rbMethod
            // 
            this.rbMethod.EnumType = typeof(AFTransformerDR.MethodEnum);
            this.rbMethod.Location = new System.Drawing.Point(15, 46);
            this.rbMethod.Name = "rbMethod";
            this.rbMethod.Size = new System.Drawing.Size(537, 93);
            this.rbMethod.TabIndex = 12;
            this.rbMethod.TabStop = false;
            this.rbMethod.Text = "Method";
            this.rbMethod.Value = null;
            // 
            // TransformerDRConfig
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleBaseSize = new System.Drawing.Size(6, 15);
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(564, 311);
            this.Controls.Add(this.rbMethod);
            this.Controls.Add(this.cmbSourceAttribute);
            this.Controls.Add(this.lblAttribute);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TransformerDRConfig";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Transformer Data Reference";
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && this.components != null)
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

        private BSLib.WinForms.Controls.EnumRadioBox rbMethod;
    }
}
