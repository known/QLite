using System.Web.UI.WebControls;

namespace Known.QLite.WebControls
{
    public class TemplateDataField : TemplateField, IDataField
    {
        public string DataField { get; set; }
        public string CodeCategory { get; set; }
        public string CodeValues { get; set; }
        public string EmptyText { get; set; }
        public string TypeName { get; set; }
        public string Value { get; set; }
        public QueryOperator Operator { get; set; }
        public ControlType ControlType { get; set; }
    }
}
