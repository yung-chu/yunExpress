
//var quickPrintOnline = false;

//checkOnlineInterval();//检测

//setInterval("checkOnlineInterval()", 60000);//60秒检测一次

////检测快速打印是否开启
//function checkOnlineInterval() {
//    $.getJSON("http://127.0.0.1:49321/Hello.self?callback=?",
//        function (json) {
//            if (json == "OK") {
//                quickPrintOnline = true;
//            }
//        });
//}

//快速打印Url

function quickPrint(url, funSuccess, funFail) {

    if (url.substr(0, 4).toLowerCase() != "http") {
        url = window.location.protocol + "//" + window.location.host + url;
    }

    //if (quickPrintOnline) {

        //var t = setTimeout(function() {
        //    quickPrintOnline = false;
        //    window.open(url, "_blank");
        //}, 1000);

        $.getJSON("http://127.0.0.1:49321/Hello.self?callback=?",
            function(json) {
                if (json == "OK") {
                    //quickPrintOnline = true;
                    //clearTimeout(t);
                    $.getJSON("http://127.0.0.1:49321/Print.self?url=" + encodeURIComponent(url) + "&callback=?",
                        function(data) {
                            if (data == "PrintOK") {
                                if (funSuccess != undefined)
                                    funSuccess();
                            } else {
                                if (funFail != undefined)
                                    funFail(json);
                            }
                        });
                }
            })
            .fail(function () {
                //quickPrintOnline = false;
                window.open(url, "_blank");
            });

    //} else {
    //    window.open(url, "_blank");
    //}
}