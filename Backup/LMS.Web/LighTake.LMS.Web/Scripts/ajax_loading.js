
$(function() {
    $("body").prepend("<div id=\"loading\" class=\"loadingdiv\"><img src=\"/Images/loading.jpg\" alt=\"加载中···\" /></div>");
    //注册ajax加载事件
    $("#loading").ajaxStart(function() {
        //一个div，用来遮挡页面，请求过程中，不可操作页面
        var lockwin = $(this);
        //div占满整个页面
        lockwin.css("width", "100%");
        lockwin.css("display", "block");
        lockwin.css("height", $(window).height() + $(window).scrollTop());
        //设置图片居中
        $("#loading img").css("display", "block");
        $("#loading img").css("left", ($(window).width() - 88) / 2);
        $("#loading img").css("top", ($(window).height() + $(window).scrollTop() - 130) / 2);
    });

    $("#loading").ajaxStop(function() {
        //隐藏div
        var lockwin = $(this);
        lockwin.css("width", "0");
        lockwin.css("display", "none");
        lockwin.css("height", "0");
        //设置图片隐藏
        $("#loading img").css("display", "none");
    });
});
