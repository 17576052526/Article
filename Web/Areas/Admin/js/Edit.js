/********************* 文件图片上传代码开始 ******************/
/*
num=1：只上传一张图片，其他值上传多张图片，如果是非图片上传就不需要管这个属性
上传文件或图片之后如果num=1删除之前上传的文件或图片
结构必须为 .img或.file的类样式为容器，容器里面为按钮和隐藏域
此插件只能在后台使用，因为服务器没有验证上传文件的后缀名，而且还提供了删除功能
    移植说明：
        1.修改form提交的路径
        2.修改删除图片按钮的图标（现在是用BootStrap的图标类样式）
*/
if (!jQuery) { alert('请引入Jquery'); }
$(function () {
    if ($('.img').length == 0 && $('.file').length == 0) { return; }
    var url = '/Admin/Shared/UpFile';  //提交地址
    var btn = null;
    //根据图片重构隐藏域的值
    function ImgHidVal(contact) {
        var str = '';
        contact.find('.imgCon a').each(function () { str += ',' + $(this).attr('href'); });
        str = str.substr(1);
        contact.children('input[type=hidden]').val(str);
    }
    //根据隐藏域的值重构图片
    function addImg(contact) {
        var value = contact.children('input[type=hidden]').val();  //获取隐藏域的值
        if (value.length > 0) {
            var srcArray = value.split(',');
            var imgCon = contact.children('.imgCon');
            imgCon.html('');
            for (var i = 0; i < srcArray.length; i++) {
                imgCon.append('<a href="' + srcArray[i] + '" target="_blank" title="查看原图"><img src="' + srcArray[i] + '" /><i title="删除图片" class="glyphicon glyphicon-remove"></i></a>');
            }
        }
    }
    //根据隐藏域的值重构文件
    function addFile(contact) {
        var val = contact.children('input[type=hidden]').val();
        if (val.length > 0) {
            contact.children('a').remove();
            contact.append('<a href="' + val + '" target="_blank">查看文件<i title="删除文件" class="glyphicon glyphicon-remove"></i></a>');
        }
    }
    //加载显示图片
    $('.img').each(function () {
        var contact = $(this);  //整体的容器
        contact.prepend('<div class="imgCon"></div>');//图片容器
        addImg(contact);
        //单击删除图片
        contact.find('a i').live('click', function (event) {
            event.preventDefault();
            if (!confirm("确定删除")) { return; }
            var a = $(this).parent();
            var delUrl = a.attr('href');
            $.ajax({
                type: "post", url: url + "?HandID=DelFile", data: "path=" + delUrl, dataType: "text", success: function (msg) {
                    //-1失败 1成功 2文件不存在
                    if (msg == "-1") { alert('删除失败'); }
                    else if (msg == "2") { alert('文件不存在'); }
                    if (msg == "1" || msg == "2") {
                        a.remove();
                        ImgHidVal(contact);
                    }
                }
            });
        });
    });
    //加载显示文件路径
    $('.file').each(function () {
        var contact = $(this);  //整体的容器
        addFile(contact);
        //单击删除文件
        contact.find('a i').live('click', function (event) {
            event.preventDefault();
            if (!confirm("确定删除")) { return; }
            var a = $(this).parent();
            var delUrl = a.attr('href');
            $.ajax({
                type: "post", url: url + "?HandID=DelFile", data: "path=" + delUrl, dataType: "text", success: function (msg) {
                    //-1失败 1成功 2文件不存在
                    if (msg == "-1") { alert('删除失败'); }
                    else if (msg == "2") { alert('文件不存在'); }
                    if (msg == "1" || msg == "2") {
                        a.remove();
                        contact.children('input[type=hidden]').val('');
                    }
                }
            });
        });
    });
    //添加节点
    var file = $('<input type="file" name="fileUpFile" style="position:absolute;filter:alpha(opacity=0);opacity:0.0;display:none;" />');
    var form = $('<form method="post" action="' + url + '?HandID=Upload" target="iframeUpFile" enctype="multipart/form-data"></form>');
    var iframe = $('<iframe name="iframeUpFile" style="display:none;"></iframe>');
    form.append(file);
    $('body').append(form);
    $('body').append(iframe);
    //iframe 框架的 onload 事件，相当于文件上传完之后的回调函数
    iframe.on('load', function () {
        var str = $(window.frames["iframeUpFile"].document).find('body').html();  //获取 ashx返回的字符串
        if (str == null) { return; }
        var contact = btn.parent();
        var hid = contact.children('input[type=hidden]');
        if (contact.hasClass('img')) {
            if (hid.val().length == 0) {
                hid.val(str);
            } else {  //在此之前已经上传了一张或多张图片
                if (contact.attr('num') == 1) {
                    $.ajax({ type: "post", url: url + "?HandID=DelFile", data: "path=" + contact.find('a').attr('href'), dataType: "text", success: function () { } });  //删除之前上传的图片
                    hid.val(str);
                } else {  //允许多张图片上传
                    hid.val(hid.val() + ',' + str);
                }
            }
            addImg(contact);
        } else if (contact.hasClass('file')) {
            var a = contact.children('a');
            if (a.length > 0) {
                $.ajax({ type: "post", url: url + "?HandID=DelFile", data: "path=" + a.attr('href'), dataType: "text", success: function () { } });  //删除文件
            }
            hid.val(str);
            addFile(contact);
        }
    });
    //选择文件后，验证后缀
    file.on('change', function () {
        if (file.val().length == 0) { return; }
        if (btn.parent().hasClass('img')) {
            var gs = 'jpg,gif,png,bmp';
            var hz = file.val().substr(file.val().lastIndexOf('.') + 1).toLowerCase();
            if (gs.indexOf(hz) != -1) {
                form.submit();  //提交
            } else {
                alert('上传失败，只能上传后缀名为 ' + gs + ' 的图片');
            }
        } else if (btn.parent().hasClass('file')) {
            form.submit();  //提交
        }
    });
    //绑定鼠标悬浮其上事件
    $('.img input[type=button],.file input[type=button]').bind('mouseover', function () {
        btn = $(this);
        file.css('display', 'block');
        file.width(btn.outerWidth());
        file.height(btn.outerHeight());
        file.css('top', btn.offset().top);
        file.css('left', btn.offset().left);
    });
});
/********************* 文件图片上传代码结束 ******************/
