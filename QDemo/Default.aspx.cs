using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Known.QDemo
{
    public partial class _Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            QueryView1.AddFilter("CREATED_BY", "test");
        }

        protected void ButtonExportCurrent_Click(object sender, EventArgs e)
        {
            string fileName = string.Format("POST_{0}.xls", DateTime.Now.ToString("yyyyMMddHHmmssfff"));
            QueryView1.ExportToExcel(fileName);
        }

        protected void ButtonExportAll_Click(object sender, EventArgs e)
        {
            string fileName = string.Format("POST_{0}.xls", DateTime.Now.ToString("yyyyMMddHHmmssfff"));
            QueryView1.AllowPaging = false;
            DataBind();
            QueryView1.ExportToExcel(fileName);
        }
    }
}
