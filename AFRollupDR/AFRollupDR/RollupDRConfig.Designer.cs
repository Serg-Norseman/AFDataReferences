using System;

namespace AFRollupDR
{
	internal partial class RollupDRConfig
	{
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.ComboBox cboCalculations;
		private System.Windows.Forms.Label lblCalculation;
		private System.Windows.Forms.Label lblCategory;
		private System.Windows.Forms.ComboBox cboCategories;
		private System.ComponentModel.Container components;

		private void InitializeComponent()
		{
		    this.btnOK = new System.Windows.Forms.Button();
		    this.btnCancel = new System.Windows.Forms.Button();
		    this.lblCategory = new System.Windows.Forms.Label();
		    this.cboCategories = new System.Windows.Forms.ComboBox();
		    this.cboCalculations = new System.Windows.Forms.ComboBox();
		    this.lblCalculation = new System.Windows.Forms.Label();
		    this.SuspendLayout();
		    // 
		    // btnOK
		    // 
		    this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
		    this.btnOK.Location = new System.Drawing.Point(80, 120);
		    this.btnOK.Name = "btnOK";
		    this.btnOK.Size = new System.Drawing.Size(90, 27);
		    this.btnOK.TabIndex = 5;
		    this.btnOK.Text = "OK";
		    this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
		    // 
		    // btnCancel
		    // 
		    this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		    this.btnCancel.Location = new System.Drawing.Point(176, 120);
		    this.btnCancel.Name = "btnCancel";
		    this.btnCancel.Size = new System.Drawing.Size(90, 27);
		    this.btnCancel.TabIndex = 6;
		    this.btnCancel.Text = "Cancel";
		    // 
		    // lblCategory
		    // 
		    this.lblCategory.Location = new System.Drawing.Point(0, 9);
		    this.lblCategory.Name = "lblCategory";
		    this.lblCategory.Size = new System.Drawing.Size(77, 19);
		    this.lblCategory.TabIndex = 0;
		    this.lblCategory.Text = "Category:";
		    this.lblCategory.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		    // 
		    // cboCategories
		    // 
		    this.cboCategories.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		    this.cboCategories.Location = new System.Drawing.Point(77, 9);
		    this.cboCategories.Name = "cboCategories";
		    this.cboCategories.Size = new System.Drawing.Size(259, 24);
		    this.cboCategories.Sorted = true;
		    this.cboCategories.TabIndex = 1;
		    // 
		    // cboCalculations
		    // 
		    this.cboCalculations.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		    this.cboCalculations.Location = new System.Drawing.Point(77, 46);
		    this.cboCalculations.Name = "cboCalculations";
		    this.cboCalculations.Size = new System.Drawing.Size(259, 24);
		    this.cboCalculations.Sorted = true;
		    this.cboCalculations.TabIndex = 3;
		    // 
		    // lblCalculation
		    // 
		    this.lblCalculation.Location = new System.Drawing.Point(0, 46);
		    this.lblCalculation.Name = "lblCalculation";
		    this.lblCalculation.Size = new System.Drawing.Size(77, 19);
		    this.lblCalculation.TabIndex = 2;
		    this.lblCalculation.Text = "Calculation:";
		    this.lblCalculation.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		    // 
		    // RollupConfig
		    // 
		    this.AcceptButton = this.btnOK;
		    this.AutoScaleBaseSize = new System.Drawing.Size(6, 15);
		    this.CancelButton = this.btnCancel;
		    this.ClientSize = new System.Drawing.Size(351, 160);
		    this.Controls.Add(this.cboCalculations);
		    this.Controls.Add(this.lblCalculation);
		    this.Controls.Add(this.cboCategories);
		    this.Controls.Add(this.lblCategory);
		    this.Controls.Add(this.btnCancel);
		    this.Controls.Add(this.btnOK);
		    this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
		    this.MaximizeBox = false;
		    this.MinimizeBox = false;
		    this.Name = "RollupConfig";
		    this.ShowIcon = false;
		    this.ShowInTaskbar = false;
		    this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
		    this.Text = "Rollup Data Reference";
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
	}
}
