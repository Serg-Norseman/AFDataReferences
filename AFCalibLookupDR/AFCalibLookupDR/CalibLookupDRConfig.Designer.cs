namespace AFCalibLookupDR
{
    internal partial class CalibLookupDRConfig
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
            this.cmbTable = new System.Windows.Forms.ComboBox();
            this.lblTable = new System.Windows.Forms.Label();
            this.lblKeyField = new System.Windows.Forms.Label();
            this.cmbKeyField = new System.Windows.Forms.ComboBox();
            this.lblValueField = new System.Windows.Forms.Label();
            this.cmbValueField = new System.Windows.Forms.ComboBox();
            this.lblEpsilon = new System.Windows.Forms.Label();
            this.txtEpsilon = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(287, 162);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(90, 27);
            this.btnOK.TabIndex = 5;
            this.btnOK.Text = "OK";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(383, 162);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(90, 27);
            this.btnCancel.TabIndex = 6;
            this.btnCancel.Text = "Cancel";
            // 
            // lblAttribute
            // 
            this.lblAttribute.Location = new System.Drawing.Point(12, 9);
            this.lblAttribute.Name = "lblAttribute";
            this.lblAttribute.Size = new System.Drawing.Size(77, 19);
            this.lblAttribute.TabIndex = 2;
            this.lblAttribute.Text = "Attribute";
            // 
            // txtAttribute
            // 
            this.txtAttribute.FormattingEnabled = true;
            this.txtAttribute.Location = new System.Drawing.Point(95, 6);
            this.txtAttribute.Name = "txtAttribute";
            this.txtAttribute.Size = new System.Drawing.Size(378, 24);
            this.txtAttribute.TabIndex = 11;
            // 
            // cmbTable
            // 
            this.cmbTable.FormattingEnabled = true;
            this.cmbTable.Location = new System.Drawing.Point(95, 36);
            this.cmbTable.Name = "cmbTable";
            this.cmbTable.Size = new System.Drawing.Size(378, 24);
            this.cmbTable.TabIndex = 13;
            this.cmbTable.SelectedIndexChanged += new System.EventHandler(this.cmbTable_SelectedIndexChanged);
            // 
            // lblTable
            // 
            this.lblTable.Location = new System.Drawing.Point(12, 39);
            this.lblTable.Name = "lblTable";
            this.lblTable.Size = new System.Drawing.Size(77, 19);
            this.lblTable.TabIndex = 12;
            this.lblTable.Text = "Table";
            // 
            // lblKeyField
            // 
            this.lblKeyField.Location = new System.Drawing.Point(12, 69);
            this.lblKeyField.Name = "lblKeyField";
            this.lblKeyField.Size = new System.Drawing.Size(77, 19);
            this.lblKeyField.TabIndex = 12;
            this.lblKeyField.Text = "KeyField";
            // 
            // cmbKeyField
            // 
            this.cmbKeyField.FormattingEnabled = true;
            this.cmbKeyField.Location = new System.Drawing.Point(95, 66);
            this.cmbKeyField.Name = "cmbKeyField";
            this.cmbKeyField.Size = new System.Drawing.Size(378, 24);
            this.cmbKeyField.TabIndex = 13;
            // 
            // lblValueField
            // 
            this.lblValueField.Location = new System.Drawing.Point(12, 99);
            this.lblValueField.Name = "lblValueField";
            this.lblValueField.Size = new System.Drawing.Size(77, 19);
            this.lblValueField.TabIndex = 12;
            this.lblValueField.Text = "ValueField";
            // 
            // cmbValueField
            // 
            this.cmbValueField.FormattingEnabled = true;
            this.cmbValueField.Location = new System.Drawing.Point(95, 96);
            this.cmbValueField.Name = "cmbValueField";
            this.cmbValueField.Size = new System.Drawing.Size(378, 24);
            this.cmbValueField.TabIndex = 13;
            // 
            // lblEpsilon
            // 
            this.lblEpsilon.Location = new System.Drawing.Point(12, 131);
            this.lblEpsilon.Name = "lblEpsilon";
            this.lblEpsilon.Size = new System.Drawing.Size(77, 19);
            this.lblEpsilon.TabIndex = 12;
            this.lblEpsilon.Text = "Epsilon";
            // 
            // txtEpsilon
            // 
            this.txtEpsilon.Location = new System.Drawing.Point(95, 128);
            this.txtEpsilon.Name = "txtEpsilon";
            this.txtEpsilon.Size = new System.Drawing.Size(177, 22);
            this.txtEpsilon.TabIndex = 14;
            // 
            // CalibLookupDRConfig
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleBaseSize = new System.Drawing.Size(6, 15);
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(486, 203);
            this.Controls.Add(this.txtEpsilon);
            this.Controls.Add(this.cmbValueField);
            this.Controls.Add(this.lblEpsilon);
            this.Controls.Add(this.lblValueField);
            this.Controls.Add(this.cmbKeyField);
            this.Controls.Add(this.lblKeyField);
            this.Controls.Add(this.cmbTable);
            this.Controls.Add(this.lblTable);
            this.Controls.Add(this.txtAttribute);
            this.Controls.Add(this.lblAttribute);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CalibLookupDRConfig";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Calibration Lookup Reference";
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

        private System.Windows.Forms.ComboBox cmbTable;
        private System.Windows.Forms.Label lblTable;
        private System.Windows.Forms.Label lblKeyField;
        private System.Windows.Forms.ComboBox cmbKeyField;
        private System.Windows.Forms.Label lblValueField;
        private System.Windows.Forms.ComboBox cmbValueField;
        private System.Windows.Forms.Label lblEpsilon;
        private System.Windows.Forms.TextBox txtEpsilon;
    }
}
