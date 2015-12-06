$(function () {

    $("#txtQuantity").blur(function () {
        validInt($("#txtQuantity"));
    });

    $("#txtSalePrice").blur(function () {
        validDecimal($("#txtSalePrice"));
    });


    $("select[name='CountryCode']").combobox({
        select: function () {
            $.ajax({
                url: '../Quotation/GetShippingMethodByCountryCode?code=' + $(this).val(),
                type: 'get',
                success: function (data) {
                    $("#ShippingMethod").find("option").remove();

                    if (typeof data == "object") {
                        for (var item in data) {
                            $("<option value=" + data[item].Value + ">" + data[item].Text + "</option>").appendTo("#ShippingMethod");

                        }

                    }
                }
            });
        }
    });

    $("#txtProductID").change(function () {

        if (!validAddress()) {
            $(this).val("");
            return;
        }

        if ($(this).val() == "") {
            $(this).css("border 1px solid");
            return;
        }

        $("#txtProductID").parent("td").next().html("");
        $("#txtProductID").parent("td").next().next().html("");
        $("#txtProductID").parent("td").next().next().next().html("");
        var flag = true;

        if (document.getElementById("IsPrice")) {
            //表示当前操作是重寄订单添加明细
            //需要根据相关的参数获取报价
            if ($("#IsPrice").attr("checked") == "checked") {
                //表示没有选中要收钱
                //获取国家，运输方式，客户，商品编号得到报价
                var json = "{\"ProductID\":\"" + $(this).val() + "\",\"ShippingCountry\":\"" + $("#CountryCode").val() + "\",\"ShippingMethod\":\"" + $("#ShippingMethod").val() + "\",\"CustomerID\":\"" + $("#customerID").val() + "\"}";
                //                $("#txtSalePrice").attr("readonly", "readonly");
                $.ajax({
                    url: 'GetQuotatonFinalPrice',
                    data: { paramJSON: json },
                    async: false,
                    success: function (data) {

                        var obj = eval('(' + data + ')');
                        if (obj.status == 200) {

                            $("#txtSalePrice").val(obj.msg);
                        } else {
                            $.messager.alert("操作提示", obj.msg, "error");
                            $("#txtSalePrice").val("");
                            flag = false;
                            $("#hdProductStatus").val("500");
                        }
                    }
                });
            }
            //            else
            //                $("#txtSalePrice").removeAttr("readonly");
        }

        if (!flag) return;

        $.ajax({
            url: 'GetSampleProductInfo',
            data: { pid: $(this).val() },
            async: false,
            success: function (data) {
                if (data.Msg != "" && data.Msg != null) {
                    $("#txtProductID").css("border", "1px solid red");
                    $.messager.alert("操作提示", data.Msg, "error");
                    $("#hdProductStatus").val("500");
                    return;
                }
                //清除之前记录


                var attrJson = eval('(' + data.AttributeInfo + ')');
                for (var item in attrJson) {

                    var select = document.createElement("select");
                    select.id = item;
                    for (var child in attrJson[item]) {
                        var option = document.createElement("option");
                        option.textContent = attrJson[item][child].value;
                        option.value = item;
                        select.appendChild(option);
                    }
                    $("<span>" + item + ":</span>").appendTo($("#txtProductID").parent("td").next());
                    $(select).appendTo($("#txtProductID").parent("td").next());
                }

                $("<img src=" + data.ProductImage + " />").appendTo($("#txtProductID").parent("td").next().next());
                $("<span>" + data.ProductName + "</span>").appendTo($("#txtProductID").parent("td").next().next().next());


                if (!document.getElementById("IsPrice")) {
                    $("#txtSalePrice").val(data.SalePrice);
                } else {
                    if ($("#IsPrice").attr("checked") != "checked")
                        $("#txtSalePrice").val("0");
                }


                $("#txtProductID").css("border", "1px solid #CCCCCC");

                $("#hdProductStatus").val("200");
            }
        });

    });


    $("#addDetail").bind("click", function () {

        if ($("#hdProductStatus").val() != "200") {

            $("#txtProductID").focus();
            return;
        }

        var productID = $("#txtProductID").val();


        if (productID == "") {
            $("#txtProductID").css("border 1px solid");
            $("#txtProductID").focus();
            return;
        }
        if ($("#txtQuantity").val() == "") {
            $("#txtQuantity").focus();
            $("#txtQuantity").css("border", "1px solid red");
            return;
        } else
            $("#txtQuantity").css("border", "1px solid #CCCCCC");

        if ($("#txtSalePrice").val() == "") {
            $("#txtSalePrice").focus();
            $("#txtSalePrice").css("border", "1px solid red");
            return;
        } else
            $("#txtSalePrice").css("border", "1px solid #CCCCCC");

        var tr = document.createElement("tr");
        $(tr).addClass("data_row_dan");

        var attr = "";
        $("#tb_List > tbody > tr:eq(0)").find("td:nth-child(2) select").each(function () {
            attr += $(this).val() + ":" + $(this).find("option:selected").text() + ",";
        });
        attr = attr.substring(0, attr.length - 1);
        //判断是否有重复添加
        var bl = false;
        $("#tb_List > tbody > tr:gt(0)").each(function () {
            var pid = $.trim($(this).find("td:nth-child(1)").text());
            var attr2 = $.trim($(this).find("td:nth-child(2)").text());

            if (pid == $.trim($("#txtProductID").val()) && attr == attr2) {
                $.messager.alert("操作提示", "已出现过相同的SKU不允许重复添加", "error");
                bl = tr;
                return;
            }
        });
        if (!bl) {
            var pname = $("#txtProductID").parent("td").next().next().next().html();

            var operation = "<td>  <a class=\"l-btn l-btn-plain\" onclick=\"removeRow(this)\" href=\"javascript:void(0)\" id=\"addDetail\" title=\"删除\" data-option=\"false\"><span class=\"l-btn-left\"><span style=\"padding-left: 20px;\" class=\"l-btn-text icon-remove\"> 删除</span></span></a>  </td>";

            $(tr).html("<td>" + $("#txtProductID").val() + "</td><td>" + attr + "</td><td>" + $("#txtProductID").parent("td").next().next().html() + "</td><td>" + pname + "</td><td>" + $("#txtQuantity").val() + "</td><td>" + $("#txtSalePrice").val() + "</td>" + operation);

            $(tr).appendTo($(".data_table > tbody "));
        }

    });


    $("#btnSave").click(function () {
        if (!validAddress()) return;

        //验证是否有添加的明细信息
        var trLength = $("#tb_List > tbody  >tr:gt(0)").length;
        if (trLength == 0) {
            $.messager.alert("", "至少要有一条订单明细", "error");
            return;
        }
        //获取表单内容 保存
        var firstName = $("#txtFirstName").val();
        var lastName = $("#txtLastName").val();
        var address1 = $("#txtAddress1").val();
        var address2 = $("#txtAddress2").val();
        var city = $("#txtCity").val();
        var province = $("#txtProvince").val();
        var zipCode = $("#txtZipCode").val();
        var phone = $("#txtPhone").val();
        var countryCode = $("#CountryCode").val();
        var shippingMethod = $("#ShippingMethod").val();
        var serviceRemark = $("#txtServiceRemark").val();
        var customerID = $("#customerID").val();
        var orderID = "";
        if (document.getElementById("OrderID"))
            orderID = $("#OrderID").val();

        var shipping = "{\"CustomerID\":\"" + customerID + "\",\"OrderID\":\"" + orderID + "\",\"FirstName\":\"" + firstName + "\",\"LastName\":\"" + lastName + "\",\"Address1\":\"" + address1 + "\",\"Address2\":\"" + address2 + "\",\"City\":\"" + city + "\",\"Province\":\"" + province + "\",\"ZipCode\":\"" + zipCode + "\",\"CountryCode\":\"" + countryCode + "\",\"ShippingMethod\":\"" + shippingMethod + "\",\"Phone\":\"" + phone + "\",\"Remark\":\"" + serviceRemark + "\"}";

        var details = "[";
        $("#tb_List > tbody  >tr:gt(0)").each(function () {
            var pid = $.trim($(this).find("td:nth-child(1)").text());
            var attr = $.trim($(this).find("td:nth-child(2)").text());
            var qty = $.trim($(this).find("td:nth-child(5)").text());
            var price = $.trim($(this).find("td:nth-child(6)").text());
            attr = attr.replace(":", "-").replace(",", "^");
            attr = attr.replace(":", "-").replace(",", "^");
            details += "{\"ProductID\":\"" + pid + "\",\"AttributeInfo\":\"" + attr + "\",\"Quantity\":\"" + qty + "\",\"SalePrice\":\"" + price + "\" },";
        });

        details = details.substring(0, details.length - 1) + "]";

        var dataFlag = $(this).attr("data-flag");
        var isPrice = "no";
        if (document.getElementById("IsPrice")) {
            if ($("#IsPrice").attr("checked"))
                isPrice = "yes";
        }

   
        $.ajax({
            url: 'SaveOrder',
            data: { shipping: shipping, details: details, type: dataFlag, isprice: isPrice },
            type: 'post',
            success: function (data) {
                var result = eval('(' + data + ')');
                if (result.status == "200") {
                    $.messager.alert("操作提示", "保存成功,新的订单为:" + result.msg, "info", function () {
                        if (dataFlag == "R")
                            window.location.href = "/Order/Rework?id=" + result.msg;
                        else
                            window.location.href = "/Order/Sample";
                    });
                } else {
                    $.messager.alert("", result.msg, "error");
                }
            }
        });
    });

});

var validAddress = function () {
    if (!$("form").form("validate")) return false;
    if ($(".custom-combobox-input").val() == "") {
        $.messager.alert("操作提示", "国家不允许为空", "error");
        return false;
    }
    if ($("#ShippingMethod").val() == "" || $("#ShippingMethod").val() == null || $("#ShippingMethod").val() == "请选择") {
        $.messager.alert("操作提示", "运输方式不允许为空", "error");
        return false;
    }

    return true;
    //验证是否有明细
};

function removeRow(obj) {
    var index = $(obj).parents("tr").index();
    var length = $("#tb_List > tbody > tr").length;
    if (length <= 2) {
        $.messager.alert("操作提示", "必须要有一条明细不能全部删除", "error");
        return;
    }
    $(".data_table tbody").find("tr:eq(" + index + ")").remove();
}