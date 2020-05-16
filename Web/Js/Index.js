/*首页banner图js开始*/
function WindFade(obj) {
    //声明变量
    var ol = $('<ol></ol>');
    var ul = obj.find('ul');
    var count = obj.find('li').length;  //图片数量
    var index = 0; //当前操作的索引
    var timer = null;
    //设置第一个显示
    obj.find('li').eq(0).css('opacity', '1');
    obj.find('li').eq(0).css('z-index', '1');
    var rep = function () {
        ul.children().stop();   //一定要先停止
        index = index == count ? 0 : index;
        var exeObj = null;
        ul.children().each(function () { if ($(this).css('z-index') == '1') { exeObj = $(this); return false; } });
        exeObj.animate({ opacity: 0 }, 300, function () {
            $(this).css('z-index', '-1');
            var x_obj = ul.children('li').eq(index);
            x_obj.css('z-index', '1');
            x_obj.animate({ opacity: 1 }, 300);
        });
        //按钮切换
        ol.children().removeClass('cheBtn');
        ol.children().eq(index).addClass('cheBtn');
    }
    //计时器
    var exeTime = function () { timer = setInterval(function () { index++; rep(); }, 5000); }
    exeTime();
    //生成按钮
    obj.append(ol);
    for (var i = 0; i < count; i++) { ol.append('<li></li>'); }
    ol.children().eq(0).addClass('cheBtn');
    ol.children().hover(function () {
        var ii = ol.children().index(this);
        if (ii != index) { index = ii; rep(); }
    });
    obj.hover(function () { clearInterval(timer); }, function () { exeTime(); });   //悬浮其上停止计时器
}
/*首页banner图js结束*/

$(function () {
    WindFade($('#WindFade'));
})