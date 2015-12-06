

//设置复选框的选择状态
function selpeihuo(obj1, obj2) {
    if (obj1 == 'gift') {
        $("#gift" + obj2).attr("checked", "checked");
        $("#commercial" + obj2).removeAttr("checked");
        $("#documents" + obj2).removeAttr("checked");
        $("#other" + obj2).removeAttr("checked");
    }
    if (obj1 == 'commercial') {
        if ($("#commercial" + obj2).attr("checked") != "checked") {
            $("#gift" + obj2).attr("checked", "checked");
            $("#commercial" + obj2).removeAttr("checked");
        } else {
            $("#commercial" + obj2).attr("checked", "checked");
            $("#gift" + obj2).removeAttr("checked");
        }
        $("#documents" + obj2).removeAttr("checked");
        $("#other" + obj2).removeAttr("checked");
    }
    if (obj1 == 'documents') {
        if ($("#documents" + obj2).attr("checked") != "checked") {
            $("#gift" + obj2).attr("checked", "checked");
            $("#documents" + obj2).removeAttr("checked");
        } else {
            $("#documents" + obj2).attr("checked", "checked");
            $("#gift" + obj2).removeAttr("checked");
        }
        $("#commercial" + obj2).removeAttr("checked");
        $("#other" + obj2).removeAttr("checked");
    }
    if (obj1 == 'other') {
        if ($("#other" + obj2).attr("checked") != "checked") {
            $("#gift" + obj2).attr("checked", "checked");
            $("#other" + obj2).removeAttr("checked");
        } else {
            $("#other" + obj2).attr("checked", "checked");
            $("#gift" + obj2).removeAttr("checked");
        }
        $("#commercial" + obj2).removeAttr("checked");
        $("#documents" + obj2).removeAttr("checked");
    }
}