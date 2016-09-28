namespace Known.QLite
{
    public interface IDataField
    {
        string HeaderText { get; set; }
        string DataField { get; set; }
        string SortExpression { get; set; }
        string CodeCategory { get; set; }
        string CodeValues { get; set; }
        string EmptyText { get; set; }
        string Value { get; set; }
        string TypeName { get; set; }
        bool Visible { get; set; }
        QueryOperator Operator { get; set; }
        ControlType ControlType { get; set; }
    }
}
