//Layout.js只放共用的js 插件

/*图片自适应js开始*/
$(window).on('load', function () {
    //显示缩略图
    $('.imgAuto img').each(function () {
        var parent = $(this).parent();
        var bi = $(this).width() / $(this).height();
        var fBi = parent.width() / parent.height();
        if (bi < fBi) {
            $(this).css('max-width', parent.width());
            $(this).css('top', -($(this).height() - parent.height()) / 2);
        } else {
            $(this).css('max-height', parent.height());
            $(this).css('left', -($(this).width() - parent.width()) / 2);
        }
    });
});
/*图片自适应js结束*/

/*滚动滚动条设置 相对浏览器定位 js开始*/
function WindScroll(obj) {
    var top = obj.offset().top;
    var div = $('<div style="width:' + obj.outerWidth(true) + 'px;float:' + obj.css('float') + ';height:1px;display:none;"></div>');
    obj.after(div);
    $(window).scroll(function () {
        var domeTop = $(window).scrollTop();
        if (domeTop > top) {
            obj.css('position', 'fixed');
            div.css('display', 'block');
        } else {
            obj.css('position', 'static');
            div.css('display', 'none');
        }
    });
}
/*滚动滚动条设置 相对浏览器定位 js结束*/