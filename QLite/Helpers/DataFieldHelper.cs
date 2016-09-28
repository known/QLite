using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Known.QLite.Helpers
{
    internal static class DataFieldHelper
    {
        public static string GetExpression(IDataField field)
        {
            switch (field.Operator)
            {
                case QueryOperator.NotEqual:
                    return string.Format("{0}<>?{0}", field.DataField);
                case QueryOperator.Equal:
                    return string.Format("{0}=?{0}", field.DataField);
                case QueryOperator.LessThan:
                    return string.Format("{0}<?{0}", field.DataField);
                case QueryOperator.LessEqual:
                    return string.Format("{0}<=?{0}", field.DataField);
                case QueryOperator.GreatThan:
                    return string.Format("{0}>?{0}", field.DataField);
                case QueryOperator.GreatEqual:
                    return string.Format("{0}>=?{0}", field.DataField);
                case QueryOperator.Between:
                    return string.Format("?l_{0}<={0}<=?g_{0}", field.DataField);
                case QueryOperator.BetweenNotEqual:
                    return string.Format("?l_{0}<{0}<?g_{0}", field.DataField);
                case QueryOperator.BetweenLessEqual:
                    return string.Format("?l_{0}<{0}<=?g_{0}", field.DataField);
                case QueryOperator.BetweenGreatEqual:
                    return string.Format("?l_{0}<={0}<?g_{0}", field.DataField);
                case QueryOperator.Contain:
                    return string.Format("{0} LIKE '%'+?{0}+'%'", field.DataField);
                case QueryOperator.StartWith:
                    return string.Format("{0} LIKE ?{0}+'%'", field.DataField);
                case QueryOperator.EndWith:
                    return string.Format("{0} LIKE '%'+?{0}", field.DataField);
                default:
                    return string.Empty;
            }
        }

        public static Control CreateFieldControl(IDataField field)
        {
            Control control = null;

            switch (field.ControlType)
            {
                case ControlType.TextBox:
                case ControlType.Date:
                    control = new TextBox();
                    break;
                case ControlType.CheckBox:
                    control = new CheckBox();
                    break;
                case ControlType.CheckBoxList:
                    control = new CheckBoxList();
                    break;
                case ControlType.DropDownList:
                    control = new DropDownList();
                    break;
                case ControlType.RadioButtonList:
                    control = new RadioButtonList();
                    break;
                case ControlType.Defined:
                    var typeNames = field.TypeName.Split(',');
                    control = (Control)Activator.CreateInstance(typeNames[1], typeNames[0]).Unwrap();
                    break;
            }

            control.ID = field.DataField;
            return control;
        }
    }
}
