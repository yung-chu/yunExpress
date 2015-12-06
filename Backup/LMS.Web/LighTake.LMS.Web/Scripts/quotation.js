


//删除当前行
var removeRow = function (obj) {
    $(obj).parents("tr").remove();
};

var createEditButton = function (text, type) {
    return " <a class=\"l-btn l-btn-plain\" href=\"javascript:void(0)\" onclick=modifyDetail(this,'" + type + "')  title=\"" + text + "\"><span"
        + " class=\"l-btn-left\"><span style=\"padding-left: 20px;\" class=\"l-btn-text icon-" + type + "\">" + text + "</span></span></a>";
};

var modifyDetail = function (obj, type) {


    //ajax 请求做编辑操作
    var currentRow = $(obj).parents("tr");
    var qid = $.trim($("#QuotationID").text());
    var code = currentRow.find("td:nth-child(1) input").val();
    var mid = currentRow.find("td:nth-child(2) input").val();
    var time = $.trim(currentRow.find("td:nth-child(3)").text());
    var fee = currentRow.find("td:nth-child(4) input").val();
    var cost = $.trim(currentRow.find("td:nth-child(5)").text());
    var rate = currentRow.find("td:nth-child(6) input").val();
    var sts = $(obj).attr("title") == "停用" ? 2 : 1;
    var dataParam = "";


    if (type == "edit") {
        if ($(obj).attr("title") != "停用" && $(obj).attr("title") != "恢复") {
            $.messager.alert("操作提示", "不允许修改title值");
            return;
        }
        dataParam = "{ \"QuationID\":\"" + qid + "\",\"ShippingCountry\":\"" + code + "\",\"ShippingMethod\":\"" + mid + "\"}";
    }
    else if (type == 'save') {
        if (fee == "" || fee == "0") {
            currentRow.find("td:nth-child(4) input").focus();
            return;
        }
        if (rate == "" || rate == "0") {
            currentRow.find("td:nth-child(6) input").focus();
            return;
        }
        dataParam = "{ \"QuationID\":\"" + qid + "\",\"ShippingCountry\":\"" + code + "\",\"ShippingMethod\":\"" + mid + "\",\"DeliveryTime\":\"" + time + "\",\"ProductCost\":\"" + cost + "\",\"ShippingFee\":\"" + fee + "\",\"ProfitRate\":\"" + rate + "\"}";
    }


    $.ajax({
        url: 'ModifyQuotationInfos',
        type: 'post',
        data: { data: dataParam, type: type, sts: sts },
        success: function (data) {

            var result = eval('(' + data + ')');

            if (result.status == "200") {

                if (type == "edit") {


                    if (sts == 1) {
                        currentRow.find("td:nth-child(8)").html(createEditButton("停用", "edit") + "&nbsp;&nbsp;&nbsp;&nbsp;" + createEditButton("保存", "save"));
                    }
                    else {
                        //修改按钮
                        currentRow.find("td:nth-child(8)").html(createEditButton("恢复", "edit"));
                    }
                    $.messager.alert("操作提示", "状态已修改成功", "success");
                }
                else if (type == "save") {
                    $.messager.alert("操作提示", "数据已保存成功", "success");
                }
            }

            else {
                $.messager.alert("操作提示", result.msg, "error");
            }
        }
    });
};

//验证添加的行数据是否有重复
var valid = function (methodID, countryCode, index) {
    var flag = true;
    $("#divShippingList > table > tbody >tr:gt(0)").each(function (v, i) {

        if (index != v + 1) {
            var code = $(this).children("td:nth-child(1)").find("input[type='hidden']").val();
            var mid = $(this).children("td:nth-child(2)").find("input[type='hidden']").val();

            if (methodID == mid && countryCode == code) {
                $.messager.alert('当前操作', '不能进行重复添加', 'error');
                flag = false;
                return false;
            }
        }
    });

    return flag;
};


//验证添加的数据格式是否正确
var validCurrentRowData = function () {
    var trFirst = $($("#divShippingList > table > tbody >tr:eq(0)"));

    var countryCode = trFirst.children("td:nth-child(1)").find("select").val();
    var mid = trFirst.children("td:nth-child(2)").find("select").val();

    var shippingFee = trFirst.children("td:nth-child(4)").find("input").val();
    var rate = trFirst.children("td:nth-child(6)").find("input").val();
    if (countryCode == "" | countryCode == null || countryCode == undefined) {
        $.messager.alert("操作提示", "请选择国家", "error");
        return false;
    }
    if (mid == "" || mid == "请选择" || mid == "0" || mid == null) {
        $.messager.alert("操作提示", "请选择运输方式", "error");
        return false;
    }
    if (shippingFee == "" || shippingFee == "0") {
        $.messager.alert("操作提示", "运费必须要大于0", "error");
        return false;
    }
    if (rate == "" || rate == "0") {
        $.messager.alert("操作提示", "利润率必须要大于0", "error");
        return false;
    }

    return valid(mid, countryCode, 0);

};


//添加一条数据
var addShippingRow = function (obj) {

    var costPrice = $("#costPrice").val();
    //判断当前添加行的数据是否有空值
    if (!validCurrentRowData()) return false;

    var trFirst = $($("#divShippingList > table > tbody >tr:eq(0)"));
    //    var countryCode = trFirst.children("td:nth-child(1)").find("select").combobox("getValue");
    //    var countryName = trFirst.children("td:nth-child(1)").find("select").combobox("getText");
    //    var methodID = trFirst.children("td:nth-child(2)").find("select").combobox("getValue");
    //    var methodName = trFirst.children("td:nth-child(2)").find("select").combobox("getText");
    var countryCode = trFirst.children("td:nth-child(1)").find("select").val();
    var countryName = trFirst.children("td:nth-child(1)").find("select").find("option:selected").text();

    var methodID = trFirst.children("td:nth-child(2)").find("select").val();
    var methodName = trFirst.children("td:nth-child(2)").find("select").find("option:selected").text();

    var cycle = $.trim(trFirst.children("td:nth-child(3)").text());
    var shippingFee = trFirst.children("td:nth-child(4)").find("input").val();
    var purPrice = trFirst.children("td:nth-child(5)").text(costPrice);
    var rate = trFirst.children("td:nth-child(6)").find("input").val();
    var totalPrice = trFirst.children("td:nth-child(7)").text();

    $("<tr class='data_row_dan'>" + trFirst.html() + "</tr>").appendTo("#divShippingList > table > tbody");
    var trLast = $($("#divShippingList > table > tbody >tr").last());
    trLast.children("td:nth-child(2)").html(methodName + "<input type='hidden' value=" + methodID + " />");
    trLast.children("td:nth-child(1)").html(countryName + "<input type='hidden' value=" + countryCode + " />");

    trLast.children("td:nth-child(3)").text(cycle);
    trLast.children("td:nth-child(4)").find("input").val(shippingFee);
    trLast.children("td:nth-child(5)").text(costPrice);  //暂时写死
    trLast.children("td:nth-child(6)").find("input").val(rate);
    trLast.children("td:nth-child(7)").text(totalPrice);

    if ($(obj).attr("data-option") == "true") {
        trLast.children("td:nth-child(8)").html("<a class=\"l-btn l-btn-plain\" href=\"javascript:void(0)\" ><span"
            + " class=\"l-btn-left\"><span style=\"padding-left: 20px;\" >&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</span></span></a>"
        + "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<a class=\"l-btn l-btn-plain\" href=\"javascript:void(0)\" onclick=modifyDetail(this,'save')  title=\"保存\"><span"
            + "class=\"l-btn-left\"><span style=\"padding-left: 20px;\" class=\"l-btn-text icon-save\">保存</span></span></a>");
    } else {
        trLast.children("td:nth-child(8)").html("<a class=\"l-btn l-btn-plain\" href=\"javascript:void(0)\" onclick=removeRow(this)  title=\"删除\"><span"
            + " class=\"l-btn-left\"><span style=\"padding-left: 20px;\" class=\"l-btn-text icon-remove\">删除</span></span></a>");
    }
};


$(function () {
    //计算价格变动
    $("input[name='profitRate_List'],input[name='shippingFee_List']").live("blur", function () {


        var currentRow = $(this).parents("tr");

        var fee = currentRow.children("td:nth-child(4)").find("input").val();
        var rate = currentRow.children("td:nth-child(6)").find("input").val();
        var costPrice = $.trim(currentRow.children("td:nth-child(5)").text());

        if (costPrice == "" || costPrice == null) {
            return;
        }

        if (fee == "") {
            currentRow.children("td:nth-child(4)").find("input").css("border", "1px solid red");
            currentRow.children("td:nth-child(4)").find("input").focus();
            //$.messager.alert("操作提示", "运费不能为空", "error");
            return;
        } else
            currentRow.children("td:nth-child(4)").find("input").css("border", "1px solid  #CCCCCC");
        if (rate == "") {
            // $.messager.alert("操作提示", "利润率不能为空","error");
            currentRow.children("td:nth-child(6)").find("input").css("border", "1px solid red");
            return;
        } else {
            currentRow.children("td:nth-child(6)").find("input").css("border", "1px solid  #CCCCCC");
        }

        // (运费 + 成本) * (1 + 利润率)

        // alert((parseFloat(fee) + parseFloat(1.3)));
        //        var costPrice = $("#costPrice").val();
        //        alert(costPrice);
        var total = (parseFloat(fee) + parseFloat(costPrice)) / (1 - parseFloat(rate));
        //alert(total);
        currentRow.children("td:nth-child(7)").text(total.toFixed(2));


    });


    //    $("#divShippingList > table > tbody >tr:eq(0)").children("td:nth-child(1)").find("select").live("change", function () {
    //        alert("");
    //    });

    //    $("#divShippingList > table > tbody >tr:eq(0)").children("td:nth-child(1)").find("input").last().live("blur", function () {

    //        alert("");
    //        if ($("#txtProductID").val() == "") {
    //            $.messager.alert("操作提示", "请先输入商品信息", "error");
    //            return;
    //        }

    //        $.ajax({
    //            url: 'GetShippingMethodByCountryCode?code=' + $(this).val(),
    //            type: 'get',
    //            success: function (data) {
    //                $("#ShippingMethod").find("options").remove();

    //                if (data.length > 0) {
    //                    var options = ""; // "<select class='chzn-select' style='width:220px;' id='ShippingMethod'></select>";
    //                    for (var item in data) {
    //                        if (data[item].Value != "" && data[item].Value != undefined)
    //                            options += "<option value=" + data[item].Value + ">" + data[item].Text + "</option>";
    //                    }

    //                    $("#divShippingList > table > tbody >tr:eq(0)").children("td:nth-child(2)").html("<select class='chzn-select' style='width:220px;' id='ShippingMethod'>" + options + "</select>");

    //                    $("#ShippingMethod").show();
    //                    if (!$("#ShippingMethod").hasClass("chzn-select")) {
    //                        $("#ShippingMethod").addClass("chzn-select");
    //                        $("#ShippingMethod").chosen();
    //                    } else {
    //                        $("#ShippingMethod").chosen();
    //                    }
    //                }
    //            }
    //        });
    //    });

    $("#ShippingMethod").live("change", function () {

        if ($(this).val() == "" || $(this).val() == undefined) return;

        var pid = $("#txtProductID").val();
        var code = $("#CountryCode").val();
        //绑定运输时间及运费
        $.ajax({
            url: 'GetShippingFee',
            data: { mid: $(this).val(), code: code, pid: pid },
            type: 'post',
            success: function (data) {

                var trFirst = $($("#divShippingList > table > tbody >tr:eq(0)"));
                if (data.Msg != null) {
                    $.messager.alert("操作提示", data.Msg, "error");
                    trFirst.children("td:nth-child(3)").text("");
                    trFirst.children("td:nth-child(4)").find("input").val("");
                    trFirst.children("td:nth-child(5)").text("");
                } else {
                    trFirst.children("td:nth-child(3)").text(data.DeliveryTime);
                    trFirst.children("td:nth-child(4)").find("input").val(data.ShippingFee);
                    trFirst.children("td:nth-child(5)").text($("#costPrice").val());
                    trFirst.children("td:nth-child(6)").find("input").focus();
                }
            }
        });
    });


    $("#addShipping").live("click", function () {
        if (document.getElementById("mainFlag")) {
            if ($("#mainFlag").val() == "2") {
                $.messager.alert("", "当前主表状态为禁止，不允许添加明细", "error");
                return;
            }
        }

        var trAll = $("#divShippingList > table > tbody >tr:gt(0)");
        var trFirst = $($("#divShippingList > table > tbody >tr:eq(0)"));
        if (trAll.length == 0) {

            addShippingRow(this);
        } else {
            var methodID = trFirst.children("td:nth-child(1)").find("select").val();
            var countryCode = trFirst.children("td:nth-child(2)").find("select").val();


            if (valid(methodID, countryCode, 0)) {
                addShippingRow(this);
            }
        }
    });


    $("input[name='profitRate_List']").live("keyup", function () {
        //判断当前价格格式是否正确
         
        if ($(this).val().length <= 2) {
            //  validInt(this);
            if ($(this).val().length == 1) {
                if ($(this).val() != "0") {
                    $(this).val("");
                    $(this).css("border", "1px solid red");
                }

            }
            else {
                if ($(this).val() != "0.") {
                    $(this).val("");
                    $(this).css("border", "1px solid red");
                }
            }
        }

        else {
            validRate(this);
        }
    });

    $("input[name='shippingFee_List']").live("keyup", function () {
        //判断当前汇率格式是否正确

        validDecimal(this);
    });
    $("select[name='CountryCode']").combobox({
        select: function () {
            getShippingMethodByCountry(this);
        }
    });


});

var getShippingMethodByCountry = function (obj) {

    if ($("#txtProductID").val() == "") {
        {
            $("#txtProductID").focus();
            $("select[name='CountryCode']").val("请选择");
            return;
        }
    }
    if (document.getElementById("requestStatus")) {
        var requestStatus = $("#requestStatus").val();
        if (requestStatus != "" && requestStatus != "200") return;
    }

    $.ajax({
        url: 'GetShippingMethodByCountryCode?code=' + $(obj).val(),
        type: 'get',
        success: function (data) {
            $("#ShippingMethod").find("options").remove();

            if (data.length > 0) {
                var options = "";
                for (var item in data) {
                    if (data[item].Value != "" && data[item].Value != undefined)
                        options += "<option value=" + data[item].Value + ">" + data[item].Text + "</option>";
                }
                $("#divShippingList > table > tbody >tr:eq(0)").children("td:nth-child(2)").html("<select class='' style='width:220px;' id='ShippingMethod'>" + options + "</select>");

                $("#ShippingMethod").show();
                $("#ShippingMethod").combobox({
                    select: function () {
                        var code = $('#CountryCode').val();
                        var mid = $(this).val();

                        var pid = $("#txtProductID").val();
                        var trFirst = $($("#divShippingList > table > tbody >tr:eq(0)"));
                        //绑定运输时间及运费
                        $.ajax({
                            url: 'GetShippingFee',
                            data: { mid: mid, code: code, pid: pid },
                            type: 'post',
                            success: function (data2) {
                                if (data2.Msg != null) {
                                    $.messager.alert("操作提示", data2.Msg, "error");
                                    trFirst.children("td:nth-child(3)").text("");
                                    trFirst.children("td:nth-child(4)").find("input").val("");
                                    trFirst.children("td:nth-child(5)").text("");
                                } else {
                                    if (data2.DeliveryTime != null && data2.DeliveryTime != undefined)
                                        trFirst.children("td:nth-child(3)").text(data2.DeliveryTime);
                                    trFirst.children("td:nth-child(4)").find("input").val(data2.ShippingFee);
                                    trFirst.children("td:nth-child(5)").text($("#costPrice").val());
                                    trFirst.children("td:nth-child(6)").find("input").focus();
                                }
                            }
                        });
                    }
                });
            }
        }
    });
}