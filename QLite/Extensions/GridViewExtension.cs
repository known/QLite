using System.IO;

namespace System.Web.UI.WebControls
{
    public static class GridViewExtension
    {
        public static void MergeColumns(this GridView gridView, int[] columns)
        {
            if (gridView.Rows.Count < 1) return;

            Array.ForEach<int>(columns, ci =>
            {
                var cell = gridView.Rows[0].Cells[ci];

                for (int i = 1; i < gridView.Rows.Count; i++)
                {
                    var tempCell = gridView.Rows[i].Cells[ci];

                    if (cell.Text == tempCell.Text)
                    {
                        if (cell.RowSpan == 0)
                        {
                            cell.RowSpan = 1;
                        }

                        cell.RowSpan++;
                        cell.VerticalAlign = VerticalAlign.Middle;
                        tempCell.Visible = false;
                    }
                    else
                    {
                        cell = tempCell;
                    }
                }
            });
        }

        public static void MergeRows(this GridView gridView, int[] rows)
        {
            if (gridView.Rows.Count < 1) return;

            Array.ForEach<int>(rows, ri =>
            {
                var cell = gridView.Rows[ri].Cells[0];

                for (int i = 1; i < gridView.Rows[ri].Cells.Count; i++)
                {
                    var tempCell = gridView.Rows[ri].Cells[i];

                    if (cell.Text == tempCell.Text)
                    {
                        if (cell.ColumnSpan == 0)
                        {
                            cell.ColumnSpan = 1;
                        }

                        cell.ColumnSpan++;
                        cell.VerticalAlign = VerticalAlign.Middle;
                        tempCell.Visible = false;
                    }
                    else
                    {
                        cell = tempCell;
                    }
                }
            });
        }

        public static void ExportToExcel(this GridView gridView, string fileName)
        {
            var response = HttpContext.Current.Response;
            response.Clear();
            response.Charset = "GB2312";
            response.AddHeader("content-disposition", string.Format("attachment; filename={0}", fileName));
            response.ContentType = "application/ms-excel";

            using (var tw = new StringWriter())
            {
                using (var htw = new HtmlTextWriter(tw))
                {
                    var table = new Table();
                    table.GridLines = gridView.GridLines;

                    if (gridView.HeaderRow != null)
                    {
                        PrepareControlForExport(gridView.HeaderRow);
                        table.Rows.Add(gridView.HeaderRow);
                    }

                    foreach (GridViewRow row in gridView.Rows)
                    {
                        PrepareControlForExport(row);
                        table.Rows.Add(row);
                    }

                    if (gridView.FooterRow != null)
                    {
                        PrepareControlForExport(gridView.FooterRow);
                        table.Rows.Add(gridView.FooterRow);
                    }

                    table.RenderControl(htw);

                    response.Write(tw.ToString());
                    response.End();
                }
            }
        }

        private static void PrepareControlForExport(Control control)
        {
            for (int i = 0; i < control.Controls.Count; i++)
            {
                var current = control.Controls[i];

                if (current is LinkButton)
                {
                    control.Controls.Remove(current);
                    control.Controls.AddAt(i, new LiteralControl((current as LinkButton).Text));
                }
                else if (current is ImageButton)
                {
                    control.Controls.Remove(current);
                    control.Controls.AddAt(i, new LiteralControl((current as ImageButton).AlternateText));
                }
                else if (current is HyperLink)
                {
                    control.Controls.Remove(current);
                    control.Controls.AddAt(i, new LiteralControl((current as HyperLink).Text));
                }
                else if (current is DropDownList)
                {
                    control.Controls.Remove(current);
                    control.Controls.AddAt(i, new LiteralControl((current as DropDownList).SelectedItem.Text));
                }
                else if (current is CheckBox)
                {
                    control.Controls.Remove(current);
                    control.Controls.AddAt(i, new LiteralControl((current as CheckBox).Checked ? "True" : "False"));
                }

                if (current.HasControls())
                {
                    PrepareControlForExport(current);
                }
            }
        }
    }
}
