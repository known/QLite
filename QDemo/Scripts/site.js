$(function() {
    $('.buttonArea').buttonset();
    $('input[id$="ButtonQuery"]').button();
    $('a[href^="javascript:__doPostBack"]').bind('click', showLoading);
});

function showLoading() {
    $('#loading').dialog('destroy');
    $('#loading').dialog({
        resizable: false,
        width: 200,
        height: 60,
        modal: true
    });
    $('.ui-dialog-titlebar').hide();
}

function toggleQuery(obj) {
    $('.query').toggle(); 
    $(obj).val($(obj).val() == '显示查询' ? '隐藏查询' : '显示查询');
}