namespace System.Web.UI.WebControls
{
    public static class TableExtension
    {
        public static TableRow NewRow(this Table table)
        {
            var row = new TableRow();
            table.Rows.Add(row);
            return row;
        }

        public static TableRow NewRow<T>(this Table table, int cellCount) where T : TableCell
        {
            var row = table.NewRow();

            for (int i = 0; i < cellCount; i++)
            {
                row.AddCell<T>();
            }

            return row;
        }

        public static T AddCell<T>(this TableRow row) where T : TableCell
        {
            var cell = Activator.CreateInstance<T>();
            row.Cells.Add(cell);
            return cell;
        }

        public static T AddCell<T>(this TableRow row, int columnSpan) where T : TableCell
        {
            var cell = row.AddCell<T>();
            cell.ColumnSpan = columnSpan;
            return cell;
        }

        public static void AddLiteralControl(this TableCell cell, string text)
        {
            var control = cell.AddControl<LiteralControl>("");
            control.Text = text;
        }

        public static void AddLiteralControl(this TableCell cell, string format, params object[] args)
        {
            string text = string.Format(format, args);
            cell.AddLiteralControl(text);
        }

        public static T AddControl<T>(this TableCell cell, string id) where T : Control
        {
            var control = Activator.CreateInstance<T>();
            control.ID = id;
            cell.Controls.Add(control);
            return control;
        }
    }
}
