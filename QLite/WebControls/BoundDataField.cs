using System;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Known.QLite.WebControls
{
    public class BoundDataField : BoundField, IDataField
    {
        public string CodeCategory { get; set; }
        public string CodeValues { get; set; }
        public string EmptyText { get; set; }
        public string TypeName { get; set; }
        public string Value { get; set; }
        public QueryOperator Operator { get; set; }
        public ControlType ControlType { get; set; }

        protected override object GetValue(Control controlContainer)
        {
            string dataField = DataField;

            if (dataField.Contains(","))
            {
                string value = string.Empty;
                var component = DataBinder.GetDataItem(controlContainer);
                var descriptors = TypeDescriptor.GetProperties(component);

                Array.ForEach(dataField.Split(','), f =>
                {
                    var descriptor = descriptors.Find(f, true);

                    if (descriptor != null)
                    {
                        value += descriptor.GetValue(component);
                    }
                });

                return value;
            }

            return base.GetValue(controlContainer);
        }

        protected override string FormatDataValue(object dataValue, bool encode)
        {
            if (Visible && !string.IsNullOrEmpty(CodeCategory) && dataValue != null)
            {
                string text = CacheHelper.GetCodeText(CodeCategory, dataValue.ToString());

                if (!string.IsNullOrEmpty(DataFormatString))
                {
                    return string.Format(DataFormatString, dataValue, text);
                }

                return text;
            }

            return base.FormatDataValue(dataValue, encode);
        }
    }
}
