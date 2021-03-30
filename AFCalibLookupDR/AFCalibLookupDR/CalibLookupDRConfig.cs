using System;
using System.Data;
using System.Windows.Forms;
using OSIsoft.AF;
using OSIsoft.AF.Asset;

namespace AFCalibLookupDR
{
    internal partial class CalibLookupDRConfig : Form
    {
        private CalibLookupDR fDataReference;
        private AFTables fTables;

        private AFDatabase Database
        {
            get {
                if (fDataReference.Attribute != null) {
                    return fDataReference.Attribute.Database;
                }
                return fDataReference.Template.Database;
            }
        }

        private void ProcessAttributes(AFAttributes attributes, string prefix)
        {
            foreach (AFAttribute attr in attributes) {
                if (attr == fDataReference.Attribute)
                    continue;

                string attrName = prefix + "|" + attr.Name;

                if (Extensions.IsNumericType(attr.Type)) {
                    txtAttribute.Items.Add(attrName);
                }

                ProcessAttributes(attr.Attributes, attrName);
            }
        }

        private void ProcessAttributes(AFElementTemplate templ, AFAttributeTemplates attributes, string prefix)
        {
            foreach (AFAttributeTemplate attr in attributes) {
                if (attr == fDataReference.Attribute)
                    continue;

                string attrName = prefix + "|" + attr.Name;

                if (Extensions.IsNumericType(attr.Type)) {
                    txtAttribute.Items.Add(attrName);
                }

                ProcessAttributes(templ, attr.AttributeTemplates, attrName);
            }
        }

        private void ProcessTemplate(AFElementTemplate templ, string prefix)
        {
            ProcessAttributes(templ, templ.AttributeTemplates, prefix);

            var baseTemplate = templ.BaseTemplate;
            if (baseTemplate != null) {
                ProcessTemplate(baseTemplate, prefix);
            }
        }


        private void PopulateTableCombo(AFTables tablesToPopulate)
        {
            cmbTable.BeginUpdate();
            cmbTable.Items.Add(string.Empty);
            foreach (AFTable current in tablesToPopulate) {
                cmbTable.Items.Add(current.Name);
            }
            cmbTable.EndUpdate();
        }

        public CalibLookupDRConfig(CalibLookupDR dataReference, bool bReadOnly)
        {
            InitializeComponent();

            fDataReference = dataReference;

            if (dataReference.Attribute != null) {
                AFElement elem = dataReference.Attribute.Element as AFElement;
                if (elem != null) {
                    ProcessAttributes(elem.Attributes, "");
                }
            } else if (dataReference.Template != null) {
                AFElementTemplate templ = dataReference.Template.ElementTemplate as AFElementTemplate;
                if (templ != null) {
                    ProcessTemplate(templ, "");
                }
            }
            txtAttribute.Sorted = true;

            fTables = Database.Tables;
            PopulateTableCombo(fTables);

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

            if (!string.IsNullOrEmpty(dataReference.TableName)) {
                cmbTable.Text = dataReference.TableName;
            }

            if (!string.IsNullOrEmpty(dataReference.KeyField)) {
                cmbKeyField.Text = dataReference.KeyField;
            }

            if (!string.IsNullOrEmpty(dataReference.ValueField)) {
                cmbValueField.Text = dataReference.ValueField;
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
                fDataReference.AttributeName = txtAttribute.Text;
                fDataReference.TableName = cmbTable.Text;
                fDataReference.KeyField = cmbKeyField.Text;
                fDataReference.ValueField = cmbValueField.Text;
                result = true;
            } catch (Exception ex) {
                MessageBox.Show(string.Format(Resources.ERR_UnableToApplyChanges, ex.Message), Resources.TITLE_Error);
                result = false;
            }

            return result;
        }

        private void cmbTable_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedTableName = cmbTable.Text;

            AFTable selectedTable = fTables[selectedTableName];
            if (selectedTable != null && selectedTable.IsDeleted) {
                selectedTable = null;
            }

            if (selectedTable != null) {
                cmbKeyField.Items.Clear();
                cmbValueField.Items.Clear();

                DataTable dataTable;
                try {
                    dataTable = selectedTable.Table;
                } catch (Exception) {
                    dataTable = null;
                }

                if (dataTable != null) {
                    cmbKeyField.BeginUpdate();
                    cmbValueField.BeginUpdate();
                    foreach (DataColumn dataColumn in dataTable.Columns) {
                        cmbKeyField.Items.Add(dataColumn.ColumnName);
                        cmbValueField.Items.Add(dataColumn.ColumnName);
                    }
                    cmbKeyField.EndUpdate();
                    cmbValueField.EndUpdate();
                }
            }
        }
    }
}
