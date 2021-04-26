namespace AFShiftResolverDR
{
    internal partial class ShiftResolverDRConfig
    {
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.ComponentModel.Container components;
        private System.Windows.Forms.Label lblShiftMode;
        private System.Windows.Forms.ComboBox txtShiftMode;
        private System.Windows.Forms.ComboBox txtStartOffset;
        private System.Windows.Forms.Label lblStartOffset;

        protected override void Dispose(bool disposing)
        {
            if (disposing && this.components != null) {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.lblShiftMode = new System.Windows.Forms.Label();
            this.txtShiftMode = new System.Windows.Forms.ComboBox();
            this.txtStartOffset = new System.Windows.Forms.ComboBox();
            this.lblStartOffset = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(211, 173);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(90, 27);
            this.btnOK.TabIndex = 5;
            this.btnOK.Text = "OK";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(307, 173);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(90, 27);
            this.btnCancel.TabIndex = 6;
            this.btnCancel.Text = "Cancel";
            // 
            // lblShiftMode
            // 
            this.lblShiftMode.Location = new System.Drawing.Point(12, 9);
            this.lblShiftMode.Name = "lblShiftMode";
            this.lblShiftMode.Size = new System.Drawing.Size(381, 19);
            this.lblShiftMode.TabIndex = 2;
            this.lblShiftMode.Text = "Shift Mode (number of attribute with number)";
            // 
            // txtShiftMode
            // 
            this.txtShiftMode.FormattingEnabled = true;
            this.txtShiftMode.Location = new System.Drawing.Point(15, 31);
            this.txtShiftMode.Name = "txtShiftMode";
            this.txtShiftMode.Size = new System.Drawing.Size(382, 24);
            this.txtShiftMode.TabIndex = 11;
            // 
            // txtStartOffset
            // 
            this.txtStartOffset.FormattingEnabled = true;
            this.txtStartOffset.Location = new System.Drawing.Point(15, 94);
            this.txtStartOffset.Name = "txtStartOffset";
            this.txtStartOffset.Size = new System.Drawing.Size(382, 24);
            this.txtStartOffset.TabIndex = 13;
            // 
            // lblStartOffset
            // 
            this.lblStartOffset.Location = new System.Drawing.Point(12, 72);
            this.lblStartOffset.Name = "lblStartOffset";
            this.lblStartOffset.Size = new System.Drawing.Size(381, 19);
            this.lblStartOffset.TabIndex = 12;
            this.lblStartOffset.Text = "StartOffset (minutes from day begin)";
            // 
            // ShiftResolverDRConfig
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleBaseSize = new System.Drawing.Size(6, 15);
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(409, 212);
            this.Controls.Add(this.txtStartOffset);
            this.Controls.Add(this.lblStartOffset);
            this.Controls.Add(this.txtShiftMode);
            this.Controls.Add(this.lblShiftMode);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ShiftResolverDRConfig";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "ShiftResolver Data Reference";
            this.ResumeLayout(false);
        }
    }
}
