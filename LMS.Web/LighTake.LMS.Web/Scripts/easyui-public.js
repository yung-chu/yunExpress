
jQuery.fn.showWindow = function (url, title, width, height) {
  
    if (width == "" || width == undefined)
        width = 550;
    if (height == "" || height == undefined)
        height = 380;
    var id = this.attr("id");
    newWindow(url, width, height, id, title);
};

jQuery.extend({
    showWindow: function (url, title,id, width, height) {
        var windowId;
        if (id) {windowId = id;} else {windowId = "window_" + new Date().getTime();}
        $("<div id=\"" + windowId + "\" style=\"overflow: scroll; display: none; position: relative\"></div>").appendTo($("body"));
        if (width == "" || width == undefined)
            width = 550;
        if (height == "" || height == undefined)
            height = 380;
        var thisDiv = $("#" + windowId);
        thisDiv.window({
            width: width,
            top: 100,
            title: title,
            modal: true,
            shadow: false,
            closed: false,
            height: height,
            cache: false,
            collapsible: false,
            minimizable: false,
            maximizable: false,
            href: url,
            onClose: function() {
                $(this).window('destroy');
            }
        });
        var parentWindow = thisDiv.parent("div");
        $(parentWindow).css("top", "0");
        $(parentWindow).animate({
            top: 120
        }, {
            duration: 1000,
            step: function (now) {
                $(".window").css("top", now);
            }
        });
        thisDiv.show();
    }
});

var newWindow = function(url, width, height, controlID, title) {
    $("#" + controlID).window({
        width: width,
        top: 100,
        title: title,
        modal: true,
        shadow: false,
        closed: false,
        height: height,
        cache: false,
        collapsible: false,
        minimizable: false,
        maximizable: false,
        href: url
    }
    );
};

//构建添加工具栏
var add_toolbar = function ($dg) {
    return {
        text: '添加',
        'iconCls': 'icon-add',
        handler: function () { appentRowFor$dg($dg); }
    };
};

//构建添加工具栏
var edit_toolbar = function ($dg) {
    return {
        text: '编辑',
        'iconCls': 'icon-edit',
        handler: function () { startEdit($dg); }
    };
};


//构建添加撤销工具栏
var reject_toolbar = function ($dg) {
    return {
        text: '撤销',
        'iconCls': 'icon-undo',
        handler: function () {
            $dg.datagrid('rejectChanges');
        }
    };
};

//构建移除工具栏
var remove_toolbar = function ($dg) {
    return {
        text: '移除',
        'iconCls': 'icon-remove',
        handler: function () {
            removeSelectRow($dg);
        }
    };
};



//构建保存工具栏 及操作
var save_toolbar = function ($dg, saveObj, fun) {
    return {
        text: '保存',
        'iconCls': 'icon-save',
        handler: function () {
            saveBeforeEndEdit($dg);
            var effectRow = batchSaveEditData($dg, saveObj.operationType); //得到添加后的结果集
            if (validDatagridData($dg, saveObj.validAttrName)) {
                requestAjaxSave(saveObj.url, "post", effectRow, saveObj.msg, fun);
            }
            else {
                $.messager.alert('当前操作', '异常信息为：数据验证没有通过', 'error');
            }
        }
    };
};



//适用于表格批量操作的时候 保存时的参数对象
var saveParams = function (obj) {
    if (obj.url != "")
        this.url = obj.url;
    if (obj.operationType != "")
        this.operationType = obj.operationType;
    if (obj.validAttrName != "")
        this.validAttrName = obj.validAttrName;
    if (obj.msg != "")
        this.msg = obj.msg;
    if (obj.isCloseWindow == false)
        this.isCloseWindow = false;
    if (obj.windowName != "")
        this.windowName = obj.windowName;
    if (obj.params != null)
        this.params = obj.params;
};
saveParams.prototype = {
    url: '',
    operationType: "inserted",
    validAttrName: 'AttributeName',
    msg: "保存成功",
    params: null,
    isCloseWindow: true,
    windowName: "openWindow"
};

//适用于表格参数初始化时的参数对象
var dgParam = function (obj) {
    if (obj.url == "")
        return null;
    else this.url = obj.url;
    if (obj.pagination == false)
        this.pagination = false;
    if (obj.columns != null)
        this.columns = obj.columns;
    if (obj.toolbar != null)
        this.toolbar = obj.toolbar;
    if (obj.pageList == null)
        this.pageList = [20, 40, 60];
    else
        this.pageList = obj.pageList;
    if (obj.rownumbers == true)
        this.rownumbers = true;
    if (obj.sortName != null)
        this.sortName = obj.sortName;
    if (obj.params != null)
        this.params = obj.params;

    return this;
};

dgParam.prototype = {
    url: '',
    pageList: [20, 40, 60],
    params: null,
    sortName: '',
    columns: null,
    toolbar: null,
    pagination: true,
    rownumbers: true
};

//适合于简单的批量添加，编辑操作的绑定
var initSimpleDatagrid = function ($dg, toolbar, columns, width, height) {
    if (width == "")
        width = 500;
    if (height == "")
        height = 330;
    $dg.datagrid({
        width: width,
        height: height,
        columns: [columns],
        toolbar: toolbar
    });
};

//适合于列表绑定
var initComplexDatagrid = function ($dg, $dgParam) {
    if ($dgParam == null || $dgParam.url == "") {
        $.messager.alert('当前操作', '异常信息为：请求数据的URL没有获取到', 'error');
        return;
    }
    $dg.datagrid(
             {
                 iconCls: $dgParam.iconCls,
                 nowrap: false,
                 fitColumns: true,
                 striped: true,
                 pageList: $dgParam.pageList,
                 url: $dgParam.url,
                 loadMsg: '数据装载中......',
                 onBeforeLoad: function (param) {
                     if ($dgParam.params != null)
                         param.params = $dgParam.params;
                     else
                         param.params = getParams(param.page, param.rows);
                 },
                 sortName: $dgParam.sortName,
                 sortOrder: "desc",
                 remoteSort: false,
                 columns: [$dgParam.columns],
                 toolbar: $dgParam.toolbar,
                 pagination: $dgParam.pagination,
                 rownumbers: $dgParam.rownumbers,
                 pageSize: 20,
                 pageNumber: 1 //默认索引页
             }
    );
};



//获取表彰参数值进行序转换成json串
var getParams = function (pageNumber, pageSize) {
    var obj = $("form").serializeArray();

    if (obj == null) {
        return null;
    }
    var json = "{";
    if (pageNumber != "")
        json += "\"Page\":" + pageNumber + ",\"PageSize\":" + pageSize + "\,";
    for (var item in obj) {
        if (obj != undefined && obj[item].value != undefined && obj[item].value != "")
            json += "\"" + obj[item].name + "\":" + "\"" + obj[item].value + "\",";
    }
    return json.substring(0, json.length - 1) + "}";
};


//用来创建表格里面操作栏的按钮
var newOperationButton = function (value, text, style, funName) {
    return "<a href='#' class=\"l-btn l-btn-plain\" onclick=" + funName + "('" + value + "') ><span class=\"l-btn-left\"><span class=\"l-btn-text icon-" + style + "\" style=\"padding-left: 20px;\">" + text + "</span></span></a>";
};

//ajax请求操作保存方法
var requestAjaxSave = function (url, type, param, message, fun) {
    $.ajax({
        url: url,
        type: type,
        cache: false,
        async: false,
        data: param,
        success: function (data) {
            if (data == "True") {
                $.messager.alert('当前操作', message);
                if (fun != "" && fun != undefined)
                    ExcuteFunction(fun);
            } else {
                $.messager.alert('当前操作', '异常信息:' + data, 'error');
            }
        }
    });
};

//转换参数对象
function ExcuteFunction(data) {
    var b = new Function("return " + data);
    return b.apply(this);
}


// 批量保存表格 处理的数据 saveType(备注：值必须为 inserted,deleted,updated)
var batchSaveEditData = function ($dg, saveType) {
    if ($dg.datagrid('getChanges').length) {
        var operationResult = $dg.datagrid('getChanges', saveType);
        var effectRow = new Object();
        if (operationResult.length)
            effectRow[saveType] = JSON.stringify(operationResult);
        return effectRow;
    }
    return null;
};


//添加一条到表格上面
var appentRowFor$dg = function ($dg) {
    $dg.datagrid('appendRow', {});
    var rows = $dg.datagrid('getRows');
    $dg.datagrid('beginEdit', rows.length - 1);
};

//保存数据之前要先结束掉编辑状态
var saveBeforeEndEdit = function ($dg) {
    var rows = $dg.datagrid('getRows');
    for (var i = 0; i < rows.length; i++) {
        $dg.datagrid('endEdit', i);
    }
};

//移除选中的行 columnNamePK 表示是否需要返回当前删除行的主键列的字段值
var removeSelectRow = function ($mydg, columnNamePK) {
    var rows = $mydg.datagrid("getSelections"); // 获取所有选中的行
    if (rows.length == 0) {
        $.messager.alert('当前操作', "没有选择需要删除的数据", "info");
        return null;
    }
    var numberArray = [];
    var returnPKValues = [];
    if (columnNamePK != "")
        for (var i = 0; rows && i < rows.length; i++) {
            var row = rows[i];
            var index = $mydg.datagrid("getRowIndex", row); // 获取该行的索引
            numberArray.push(index);
            returnPKValues.push(row[columnNamePK]);
        }
    numberArray.sort(function (a, b) { return a < b ? 1 : -1; });
    for (var j = 0; j < numberArray.length; j++) {
        $mydg.datagrid('deleteRow', (numberArray[j]));
    }
    if (returnPKValues.length > 0)
        return returnPKValues.join(",");
    return null;
};

//针对于列表保存时错验证是否正确 attributeName 需要验证的主要属性字段 
var validDatagridData = function ($dg, attributeName) {
    var rows = $dg.datagrid("getChanges");    // 获取所有选中的行
    for (var i = 0; i < rows.length; i++) {
        var rowi = rows[i];
        if (!rowi.hasOwnProperty(attributeName))
            return false;
    }
    return true;
};

//结束datagrid编辑状态
var endEdit = function ($dg) {
    var rows = $dg.datagrid('getRows');
    for (var i = 0; i < rows.length; i++) {
        $dg.datagrid('endEdit', i);
    }
};

//启用datagrid 可编辑状态
var startEdit = function ($dg) {
    var rows = $dg.datagrid('getRows');    // 获取所有选中的行
    for (var i = 0; rows && i < rows.length; i++) {
        var row = rows[i];
        var index = $dg.datagrid("getRowIndex", row);    // 获取该行的索引
        $dg.datagrid('beginEdit', index);
    }
}

$(function () {
    $("#selectCustomer").bind("click", function () {
        if ($("#showCustomerList").attr("tabindex") == 200) {
            $("#showCustomerList").showWindow("/Customer/SelectList?IsAll=true&ShowPaymentType=true", "选择客户");
        } else if ($("#showCustomerList").attr("tabindex") == 500) {
            var startTime, endTime;
            if ($("#FilterModel_StartTime").length > 0) {
                startTime = $("#FilterModel_StartTime").val();
            }
            if ($("#FilterModel_EndTime").length > 0) {
                endTime = $("#FilterModel_EndTime").val();
            }
            $("#showCustomerList").showWindow("/Customer/SelectReceivingExpenseList?startTime=" + startTime + "&endTime="+endTime, "选择客户");
        }else {
            $("#showCustomerList").showWindow("/Customer/SelectList", "选择客户");
        }
        var parentWindow = $("#showCustomerList").parent("div");
        $(parentWindow).css("top", "0");
        $(parentWindow).animate({
            top: 120
        }, {
            duration: 1000,
            step: function (now, fx) {

                $(".window").css("top", now);

            }
        });
        $("#showCustomerList").show();

    });
})