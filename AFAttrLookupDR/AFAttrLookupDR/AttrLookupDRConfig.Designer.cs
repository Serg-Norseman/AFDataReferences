using System;

namespace AFAttrLookupDR
{
	internal partial class AttrLookupDRConfig
	{
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Button btnCancel;
		private System.ComponentModel.Container components;
		private System.Windows.Forms.Label lblAttribute;
		private System.Windows.Forms.ComboBox txtAttribute;

		private void InitializeComponent()
		{
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.lblAttribute = new System.Windows.Forms.Label();
            this.txtAttribute = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cmbTSAttr = new System.Windows.Forms.ComboBox();
            this.grpTimestamp = new System.Windows.Forms.GroupBox();
            this.radAttribute = new System.Windows.Forms.RadioButton();
            this.radNone = new System.Windows.Forms.RadioButton();
            this.label2 = new System.Windows.Forms.Label();
            this.cmbMethod = new System.Windows.Forms.ComboBox();
            this.grpTimestamp.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(299, 186);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(90, 27);
            this.btnOK.TabIndex = 5;
            this.btnOK.Text = "OK";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(395, 186);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(90, 27);
            this.btnCancel.TabIndex = 6;
            this.btnCancel.Text = "Cancel";
            // 
            // lblAttribute
            // 
            this.lblAttribute.Location = new System.Drawing.Point(12, 54);
            this.lblAttribute.Name = "lblAttribute";
            this.lblAttribute.Size = new System.Drawing.Size(77, 19);
            this.lblAttribute.TabIndex = 2;
            this.lblAttribute.Text = "Attribute";
            // 
            // txtAttribute
            // 
            this.txtAttribute.FormattingEnabled = true;
            this.txtAttribute.Location = new System.Drawing.Point(95, 51);
            this.txtAttribute.Name = "txtAttribute";
            this.txtAttribute.Size = new System.Drawing.Size(390, 24);
            this.txtAttribute.TabIndex = 11;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(12, 159);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(77, 19);
            this.label1.TabIndex = 2;
            this.label1.Text = "TS Attr";
            // 
            // cmbTSAttr
            // 
            this.cmbTSAttr.Enabled = false;
            this.cmbTSAttr.FormattingEnabled = true;
            this.cmbTSAttr.Location = new System.Drawing.Point(95, 156);
            this.cmbTSAttr.Name = "cmbTSAttr";
            this.cmbTSAttr.Size = new System.Drawing.Size(390, 24);
            this.cmbTSAttr.TabIndex = 11;
            // 
            // grpTimestamp
            // 
            this.grpTimestamp.Controls.Add(this.radAttribute);
            this.grpTimestamp.Controls.Add(this.radNone);
            this.grpTimestamp.Location = new System.Drawing.Point(15, 90);
            this.grpTimestamp.Name = "grpTimestamp";
            this.grpTimestamp.Size = new System.Drawing.Size(470, 60);
            this.grpTimestamp.TabIndex = 12;
            this.grpTimestamp.TabStop = false;
            this.grpTimestamp.Text = "Timestamp";
            // 
            // radAttribute
            // 
            this.radAttribute.AutoSize = true;
            this.radAttribute.Location = new System.Drawing.Point(219, 21);
            this.radAttribute.Name = "radAttribute";
            this.radAttribute.Size = new System.Drawing.Size(82, 21);
            this.radAttribute.TabIndex = 1;
            this.radAttribute.TabStop = true;
            this.radAttribute.Text = "Attribute";
            this.radAttribute.UseVisualStyleBackColor = true;
            this.radAttribute.CheckedChanged += new System.EventHandler(this.radNone_CheckedChanged);
            // 
            // radNone
            // 
            this.radNone.AutoSize = true;
            this.radNone.Checked = true;
            this.radNone.Location = new System.Drawing.Point(6, 21);
            this.radNone.Name = "radNone";
            this.radNone.Size = new System.Drawing.Size(63, 21);
            this.radNone.TabIndex = 0;
            this.radNone.TabStop = true;
            this.radNone.Text = "None";
            this.radNone.UseVisualStyleBackColor = true;
            this.radNone.CheckedChanged += new System.EventHandler(this.radNone_CheckedChanged);
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(12, 15);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(77, 19);
            this.label2.TabIndex = 2;
            this.label2.Text = "Method";
            // 
            // cmbMethod
            // 
            this.cmbMethod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbMethod.FormattingEnabled = true;
            this.cmbMethod.Items.AddRange(new object[] {
            "Dynamic link",
            "Direct link to attribute, get at specific time"});
            this.cmbMethod.Location = new System.Drawing.Point(95, 12);
            this.cmbMethod.Name = "cmbMethod";
            this.cmbMethod.Size = new System.Drawing.Size(390, 24);
            this.cmbMethod.TabIndex = 11;
            // 
            // AttrLookupDRConfig
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleBaseSize = new System.Drawing.Size(6, 15);
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(497, 225);
            this.Controls.Add(this.grpTimestamp);
            this.Controls.Add(this.cmbTSAttr);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cmbMethod);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtAttribute);
            this.Controls.Add(this.lblAttribute);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AttrLookupDRConfig";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "AttrLookup Data Reference";
            this.grpTimestamp.ResumeLayout(false);
            this.grpTimestamp.PerformLayout();
            this.ResumeLayout(false);

		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && this.components != null)
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbTSAttr;
        private System.Windows.Forms.GroupBox grpTimestamp;
        private System.Windows.Forms.RadioButton radAttribute;
        private System.Windows.Forms.RadioButton radNone;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmbMethod;
    }
}
