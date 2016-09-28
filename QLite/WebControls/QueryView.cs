using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Known.QLite.Helpers;

namespace Known.QLite.WebControls
{
    [ToolboxData("<{0}:QueryView runat=\"server\"></{0}:QueryView>")]
    public class QueryView : GridView
    {
        public QueryView()
        {
            BorderWidth = 0;
            AutoGenerateColumns = false;
            LineFields = 3;
            Filters = new List<IDataField>();
            PageSize = int.Parse(ConfigurationManager.AppSettings["PageSize"] ?? "10");
        }

        public string QueryCssClass { get; set; }
        public string ConnectionName { get; set; }
        public string EntityName { get; set; }
        public string MergeColumns { get; set; }
        public string MergeRows { get; set; }
        public int LineFields { get; set; }
        protected List<IDataField> Filters { get; private set; }

        public void AddFilter(string fieldName, object value)
        {
            AddFilter(fieldName, QueryOperator.Equal, value);
        }

        public void AddFilter(string fieldName, QueryOperator queryOperator, object value)
        {
            Filters.Add(new BoundDataField
            {
                DataField = fieldName,
                Operator = queryOperator,
                Value = Convert.ToString(value)
            });
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!string.IsNullOrEmpty(EntityName))
            {
                var fields = Columns.OfType<IDataField>();
                DataSource = GetQueryResult(fields, EntityName);
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            DataBind();

            if (!string.IsNullOrEmpty(MergeColumns))
            {
                var columns = Array.ConvertAll<string, int>(MergeColumns.Split(','), s => int.Parse(s));
                this.MergeColumns(columns);
            }

            if (!string.IsNullOrEmpty(MergeRows))
            {
                var rows = Array.ConvertAll<string, int>(MergeRows.Split(','), s => int.Parse(s));
                this.MergeRows(rows);
            }
        }

        protected override void Render(HtmlTextWriter writer)
        {
            var fields = Columns.OfType<IDataField>()
                                .Where(f => f.Operator != QueryOperator.None)
                                .ToList();

            if (fields.Count > 0)
            {
                var table = CreateQueryTable(fields);
                table.RenderControl(writer);
            }

            base.Render(writer);
        }

        protected override void OnPageIndexChanging(GridViewPageEventArgs e)
        {
            PageIndex = e.NewPageIndex;
        }

        protected override void InitializePager(GridViewRow row, int columnSpan, PagedDataSource pagedDataSource)
        {
            int recordCount = pagedDataSource.DataSourceCount;
            int pageIndex = pagedDataSource.CurrentPageIndex + 1;
            int pageCount = pagedDataSource.PageCount;
            row.Cells.Clear();
            var cell = row.AddCell<TableCell>(columnSpan);
            cell.Style["border"] = "none";
            cell.HorizontalAlign = HorizontalAlign.Right;
            cell.AddLiteralControl("共{0}条 第{1}/{2}页 每页{3}条", recordCount, pageIndex, pageCount, PageSize);
            CreatePagerLinkButton(cell, PagerButtonType.First, pageIndex, pageCount);
            CreatePagerLinkButton(cell, PagerButtonType.Previous, pageIndex, pageCount);
            CreatePagerLinkButton(cell, PagerButtonType.Next, pageIndex, pageCount);
            CreatePagerLinkButton(cell, PagerButtonType.Last, pageIndex, pageCount);
        }

        protected virtual object GetQueryResult(IEnumerable<IDataField> fields, string entityName)
        {
            var columns = fields.Where(f => f.Visible && !string.IsNullOrEmpty(f.DataField))
                                .Select(f => f.DataField)
                                .ToArray();
            Filters.AddRange(fields.Where(f => f.Operator != QueryOperator.None));
            var where = Filters.Select(f => new
                                {
                                    Expression = DataFieldHelper.GetExpression(f),
                                    Name = f.DataField,
                                    Value = GetDataFieldValue(f)
                                })
                               .Where(f => f.Value != null && f.Value.Trim() != "")
                               .ToList();
            var sort = fields.Where(f => !string.IsNullOrEmpty(f.SortExpression))
                             .Select(f => f.SortExpression)
                             .ToArray();
            var helper = DbHelperFactory.Create(ConnectionName);
            var parameters = helper.CreateParameters();
            string columnString = columns.Length == 0 ? "*" : string.Join(",", columns);
            string commandText = string.Format("SELECT {0} FROM {1}", columnString, entityName);

            if (where.Count > 0)
            {
                var whereString = string.Join(" AND ", where.Select(w => w.Expression).ToArray());
                commandText += string.Format(" WHERE {0}", whereString);
                where.ForEach(w => parameters.Add(w.Name, w.Value));
            }

            if (sort.Length > 0)
            {
                commandText += string.Format(" ORDER BY {0}", string.Join(",", sort));
            }

            var dataSource = new DataTable();
            helper.Fill(dataSource, commandText, parameters);
            return dataSource;
        }

        protected virtual Table CreateQueryTable(List<IDataField> fields)
        {
            int rowCount = (int)Math.Ceiling(fields.Count / (double)LineFields);
            var table = new Table();
            table.ID = "QueryForm";
            table.Width = Unit.Percentage(100);
            table.CssClass = QueryCssClass;
            TableRow row = null;

            for (int i = 0; i < fields.Count; i++)
            {
                if (i % LineFields == 0)
                {
                    row = table.NewRow();
                }

                var field = fields[i];
                field.Value = GetDataFieldValue(field);
                var th = row.AddCell<TableHeaderCell>();
                th.Text = field.HeaderText + ":";
                var td = row.AddCell<TableCell>();

                if (i == fields.Count - 1)
                {
                    int columnSpan = 2 * (LineFields - (i % LineFields + 1)) + 1;

                    if (columnSpan > 1)
                    {
                        td.ColumnSpan = columnSpan;
                    }
                }

                var control = DataFieldHelper.CreateFieldControl(field);
                td.Controls.Add(control);

                if (field.ControlType == ControlType.Date)
                {
                    ((WebControl)control).CssClass = "date";
                }

                if (control is ListControl)
                {
                    if (!string.IsNullOrEmpty(field.CodeCategory))
                    {
                        var helper = new PageHelper();
                        helper.PopulateListControl((ListControl)control, field.CodeCategory, field.Value, field.EmptyText);
                    }
                    else if (!string.IsNullOrEmpty(field.CodeValues))
                    {
                        var helper = new PageHelper();
                        var codes = new List<object>();
                        var codeValues = field.CodeValues.Split(';');
                        Array.ForEach<string>(codeValues, c => codes.Add(new
                        {
                            Code = c.Split('|')[0],
                            Text = c.Split('|')[1]
                        }));
                        helper.PopulateListControl((ListControl)control, codes, "Code", "Text", field.Value, field.EmptyText);
                    }
                }
                else if (control is ITextControl && !string.IsNullOrEmpty(field.Value))
                {
                    ((ITextControl)control).Text = field.Value;
                }
                else if (control is ICheckBoxControl)
                {
                    ((ICheckBoxControl)control).Checked = field.Value.Equals("on", StringComparison.OrdinalIgnoreCase);
                }

                if (i == LineFields - 1 || (fields.Count <= LineFields && i == fields.Count - 1))
                {
                    var cell = row.AddCell<TableCell>();
                    cell.Style["text-align"] = "center";

                    if (rowCount > 1)
                    {
                        cell.RowSpan = rowCount;
                    }

                    var button = cell.AddControl<Button>("ButtonQuery");
                    button.OnClientClick = "javascript:showLoading();";
                    button.Text = "查询";
                    button.Click += delegate { DataBind(); };
                }
            }

            return table;
        }

        protected virtual string GetDataFieldValue(IDataField field)
        {
            return Page.Request.Form[field.DataField] ?? field.Value;
        }

        private void CreatePagerLinkButton(TableCell cell, PagerButtonType buttonType, int pageIndex, int pageCount)
        {
            var button = new LinkButton();
            button.CommandName = "Page";

            switch (buttonType)
            {
                case PagerButtonType.First:
                    button.ToolTip = button.Text = "首页";
                    button.CommandArgument = "First";
                    button.Enabled = (pageIndex > 1);
                    break;
                case PagerButtonType.Previous:
                    button.ToolTip = button.Text = "上一页";
                    button.CommandArgument = "Prev";
                    button.Enabled = (pageIndex > 1);
                    break;
                case PagerButtonType.Next:
                    button.ToolTip = button.Text = "下一页";
                    button.CommandArgument = "Next";
                    button.Enabled = (pageIndex + 1 <= pageCount);
                    break;
                case PagerButtonType.Last:
                    button.ToolTip = button.Text = "尾页";
                    button.CommandArgument = "Last";
                    button.Enabled = (pageIndex + 1 <= pageCount);
                    break;
            }

            cell.AddLiteralControl(" ");
            cell.Controls.Add(button);
        }

        private enum PagerButtonType
        {
            First,
            Previous,
            Next,
            Last
        }
    }
}
