/// <reference path="jquery-1.8.0.min.js" />
/// <reference path="artdialog/jquery.artDialog.js" />
(function ($) {

    var us = function () {
        return new us.fn.init();
    };

    us.fn = us.prototype = {
        constructor: us,
        init: function () {
            return this;
        }
    };

    $.extend(us, {
        toggleCheckBoxStatus: function (className, isChecked, containerId) {
            if (!isChecked) {
                isChecked = false;
            }
            if (containerId) {
                $("#" + containerId).find("." + className).attr("checked", isChecked);
            }
            else {
                $("." + className).attr("checked", isChecked);
            }
        },
        mask: {
            load: function () {
                if (art) {
                    art.dialog({
                        width: 220,
                        height: 60,
                        esc: false,
                        dbClickHide: false,
                        opacity: 0.5,
                        padding: 0,
                        title: false,
                        content: "处理中......",
                        lock: true,
                        cancel: false
                    });
                }
            },
            close: function () {
                art.dialog.close();
            }
        }
    });

    $.extend({ us: us });
    $.us.fn.init.prototype = us.fn;



} (jQuery));

function getUrlPara(paraName) {
    var sUrl = location.href;
    var sReg = "(?:\\?|&){1}" + paraName + "=([^&]*)";
    var re = new RegExp(sReg, "gi");
    re.exec(sUrl);
    return RegExp.$1;
}


function showNavigate() {

    var nid = getUrlPara("nid");

    // $($("#navigate").find("div a:eq(" + (0) + ")"));
    var obj = $($("#navigate").find("div a:eq(" + (nid-1) + ")"));

    $(obj).addClass("tab_select");
}


function showMessage(type, spanText) {
    if (type == "e") {
        $("#message").html("<div class=\"message message_error mb10 mt10\"><div class=\"message_box\"><h6>error message</h6><span>" + spanText + "</span></div></div>");
    } else {
        $("#message").html("<div class=\"message message_success mb10 mt10\"><div class=\"message_box\"><h6>success message</h6><span>" + spanText + "</span></div></div>");
    }
}

var settingFormReadonly = function () {

    $("form").find("input,select,textarea").attr("disabled", "disabled");
}
$(function () {
    $(".easyui-datetimebox").datetimebox({
        width: 130,
        required: true,
        showSeconds: false
    });


})

var validBase = function (obj, reg) {
    var record = {
        num: ""
    };
    var decimalReg = reg; //var decimalReg=/^[-\+]?\d{0,8}\.{0,1}(\d{1,2})?$/; 
    if ($(obj).val() != "" && decimalReg.test($(obj).val())) {
        record.num = $(obj).val();
        $(obj).css("border", "1px solid #CCCCCC");
    } else {
        if ($(obj).val() != "") {
            $(obj).css("border", "1px solid red");
            $(obj).val(record.num);
        }
    }
};

var validDecimal = function (obj) {

    validBase(obj, /^\d{0,8}\.{0,1}(\d{1,2})?$/);
};

var validRate = function (obj) {

    validBase(obj,  /^0\.[1-9]{1,2}$/);
};

var validInt = function (obj) {
    validBase(obj, /^\d+$/);
};

function openIFrame(title, url, param) {

    var mainDoMain = $("#mainDoMain").val();
    var webDoMain = $("#adminDoMain").val();

    if (mainDoMain.indexOf("http://localhost") == 0 && url!="") {
        window.open("/"+url);
        return;
    }
    var subMain = mainDoMain.substring(7, mainDoMain.length);
    var urlParam = encodeURI(webDoMain + "Home/Between?tle=" + title + "&url=" + subMain + url + "&param=" + param);
    $("body").append("<iframe   src='" + urlParam + "' id=\"between\" style=\"display:none;\"></iframe>");

}

window.onload = function () {
  
    openIFrame("", "", "{flag:load}");
};

(function ($) {
    $.getUrlParam = function (name, urlParam) {
        var url;
        if (urlParam) {
            url = urlParam;
        } else {
            url = window.location.search;
        }
        var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)");
        var r = url.substr(1).match(reg);
        if (r != null) return unescape(r[2]);
        return null;
    };
})(jQuery);
