using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Known.QLite.WebControls
{
    public class PageHelper
    {
        public void FormatCodeText(ITextControl textControl, string category, string code)
        {
            textControl.Text = CacheHelper.GetCodeText(category, code);
        }

        public void FormatDateTime(ITextControl textControl, string format)
        {
            string text = textControl.Text;

            if (!string.IsNullOrEmpty(text))
            {
                textControl.Text = DateTime.Parse(text).ToString(format);
            }
        }

        public void PopulateListControl(ListControl control, string category, string emptyText)
        {
            PopulateListControl(control, category, "", emptyText);
        }

        public void PopulateListControl(ListControl control, string category, string defaultValue, string emptyText)
        {
            var dataSource = CacheHelper.GetCodeTables(category);
            PopulateListControl(control, dataSource, "CODE", "TEXT", defaultValue, emptyText);
        }

        public void PopulateListControl(ListControl control, object dataSource, string valueField, string textField, string defaultValue, string emptyText)
        {
            control.DataSource = dataSource;
            control.DataValueField = valueField;
            control.DataTextField = textField;
            control.DataBind();

            if (!string.IsNullOrEmpty(defaultValue))
            {
                var item = control.Items.FindByValue(defaultValue);

                if (item != null)
                {
                    item.Selected = true;
                }
            }

            if (!string.IsNullOrEmpty(emptyText))
            {
                control.Items.Insert(0, new ListItem(emptyText, ""));
            }
        }

        public T FindControl<T>(Control contaner, string id) where T : Control
        {
            return contaner.FindControl(id) as T;
        }

        public void ClearControls(Control container)
        {
            foreach (var control in container.Controls)
            {
                if (control is ITextControl)
                {
                    ((ITextControl)control).Text = string.Empty;
                }
                else if (control is ListControl)
                {
                    ((ListControl)control).SelectedIndex = -1;
                }
                else if (control is CheckBox)
                {
                    ((CheckBox)control).Checked = false;
                }
                //else if (control is RadioButtonList)
                //{
                //    ((RadioButtonList)control).ClearSelection();
                //}
            }
        }

        public void EnableControls(Control container, bool enable)
        {
            foreach (var control in container.Controls)
            {
                if (control is WebControl)
                {
                    ((WebControl)control).Enabled = enable;
                }
            }
        }

        public int GetBoundFieldColumnIndex(string columnDataField, GridView gridView)
        {
            int columnIndex = -1;

            foreach (DataControlField field in gridView.Columns)
            {
                if (field is BoundField)
                {
                    if (((BoundField)field).DataField == columnDataField)
                    {
                        columnIndex = gridView.Columns.IndexOf(field);
                        break;
                    }
                }
            }

            return columnIndex;
        }

        public void FillRow(Control container, DataRow row)
        {
            foreach (DataColumn column in row.Table.Columns)
            {
                foreach (Control control in container.Controls)
                {
                    if (column.ColumnName.Replace("_", "").Equals(GetControlId(control.ID), StringComparison.OrdinalIgnoreCase))
                    {
                        SetControlValue(control, row[column.ColumnName]);
                    }
                }
            }
        }

        public void UpdateRow(Control container, DataRow row)
        {
            foreach (DataColumn column in row.Table.Columns)
            {
                foreach (Control control in container.Controls)
                {
                    if (column.ColumnName.Replace("_", "").Equals(GetControlId(control.ID), StringComparison.OrdinalIgnoreCase))
                    {
                        row[column.ColumnName] = GetControlValue(control);
                    }
                }
            }
        }

        private static string GetControlId(string controlId)
        {
            if (!string.IsNullOrEmpty(controlId))
            {
                return controlId.Replace("_", "").Replace("txt", "").Replace("ddl", "");
            }

            return string.Empty;
        }

        public string GetControlValue(Control control)
        {
            if (control is ITextControl)
            {
                return ((ITextControl)control).Text.Trim();
            }
            else if (control is ListControl)
            {
                return ((ListControl)control).SelectedValue;
            }
            else if (control is CheckBox)
            {
                return ((CheckBox)control).Checked ? "True" : "False";
            }
            else if (control is HiddenField)
            {
                return ((HiddenField)control).Value.Trim();
            }

            return string.Empty;
        }

        public void SetControlValue(Control control, object value)
        {
            string valueString = string.Empty;

            if (value != null)
            {
                valueString = value.ToString();
            }

            if (control is ITextControl)
            {
                ((ITextControl)control).Text = valueString;
            }
            else if (control is ListControl)
            {
                var item = ((ListControl)control).Items.FindByValue(valueString);

                if (item != null)
                {
                    item.Selected = true;
                }
            }
            else if (control is CheckBox)
            {
                ((CheckBox)control).Checked = bool.Parse(valueString);
            }
            else if (control is HiddenField)
            {
                ((HiddenField)control).Value = valueString;
            }
        }
    }
}
