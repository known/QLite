<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Known.QDemo._Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Default</title>
    <link href="Themes/Base/style.css" rel="stylesheet" type="text/css" />
    <link href="Themes/Base/jquery-ui.css" rel="stylesheet" type="text/css" />
    <script src="Scripts/jquery.min.js" type="text/javascript"></script>
    <script src="Scripts/jquery-ui.min.js" type="text/javascript"></script>
    <script src="Scripts/site.js" type="text/javascript"></script>
</head>
<body>
    <form id="form1" runat="server">
    <div id="loading" style="display:none"><span class="loading">数据处理中......</span></div>
    <div id="wrapper">
        <div class="buttonArea">
            <asp:Button ID="ButtonExportCurrent" runat="server" Text="导出当前页" OnClick="ButtonExportCurrent_Click" /><asp:Button ID="ButtonExportAll" runat="server" Text="导出所有页" OnClick="ButtonExportAll_Click" /><input type="button" id="toggle" onclick="javascript:toggleQuery(this);" value="显示查询" />
        </div>
<asp:QueryView ID="QueryView1" runat="server" CssClass="grid" QueryCssClass="form query" 
               ConnectionName="Default" EntityName="T_POST" AllowPaging="true" PagerSettings-Position="TopAndBottom">
    <AlternatingRowStyle CssClass="even" />
    <Columns>
        <asp:BoundDataField DataField="TYPE" HeaderText="类别" ItemStyle-Width="15%" CodeCategory="PostType"
                            Operator="Equal" ControlType="DropDownList" EmptyText="所有类别" />
        <asp:BoundDataField DataField="TITLE" HeaderText="标题" ItemStyle-Width="50%" Operator="Contain" />
        <asp:BoundDataField DataField="CREATED_BY" HeaderText="作者" ItemStyle-Width="15%" />
        <asp:BoundDataField DataField="CREATED_DATE" HeaderText="创建时间" ItemStyle-Width="20%"
                            DataFormatString="{0:yyyy-MM-dd HH:mm:ss}" ItemStyle-HorizontalAlign="Center" />
    </Columns>
</asp:QueryView>
    </div>
    </form>
</body>
</html>
