//修改列表页高度
$(function () {
    function ListHeight() {
        if ($('.list').length > 0 && $('body').width() > 992) {
            var height = $('body').height();
            $('.list').height(height - 330);
        }
    }
    ListHeight();
    $(window).on('resize', ListHeight);
    //全选
    $('th.check input').click(function () {
        if ($(this).attr('checked')) {
            $('td.check input').attr('checked', true);
        } else {
            $('td.check input').attr('checked', false);
        }
    });

});

//多选删除
function CheDel(obj) {
    var str = '';
    $('td.check input').each(function () { if ($(this).attr('checked')) { str += $(this).attr('tabID') + ','; } });
    str = str.substr(0, str.length - 1);
    if (str.length > 0) {
        var href = $(obj).attr('url');
        var index = href.indexOf('?');
        if (index != -1) {
            $(obj).attr('href', href.substr(0, index) + '/' + str + href.substr(index));
        } else {
            $(obj).attr('href', href + '/' + str);
        }
        return confirm('确定删除所选记录');
    } else {
        alert('请选择一行或多行');
        return false;
    }
}