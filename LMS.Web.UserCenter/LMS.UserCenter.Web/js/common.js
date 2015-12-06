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
                //debugger;
                if (art) {
                    window.com_dialog = art.dialog({
                        width: 220,
                        height: 60,
                        esc: false,
                        dbClickHide: false,
                        opacity: 0.5,
                        title: "处理中....",
                       // content: "<img src='../images/loader.gif' />",
                        lock: true,
                        cancel: false
                    });
                }
            },
            close: function () {
                window.com_dialog.close();
            }
        },
        setLocation: function (url) {
            window.location.href = url;
        }
    });

    $.extend({ us: us });
    $.us.fn.init.prototype = us.fn;
}(jQuery));