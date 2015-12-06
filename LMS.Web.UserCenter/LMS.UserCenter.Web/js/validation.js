function IsCheck(obj, reg) {
    var str = obj.val();
    str = Trim(str);
    if (str == "") {
        return true;
    }
    if (!reg.test(str)) {

        obj.focus();
        return false;
    }
    return true;
}

//功能说明：消除字串前后的空格
//参数说明：字符串
//返 回 值：无返回值
function Trim(str) {
    var str0 = "";
    str0 = String(str);
    var reg = /^\s*/; 	//消除前面的空格
    var str1 = "";
    str1 = str0.replace(reg, "");
    reg = /\s*$/; 		//消除后面的空格
    var str2 = "";
    str2 = str1.replace(reg, "");
    return str2;
}

//检测是否为值空 
//适用於:textBox,DropDownList ,ListBox 
function IsEmpty(obj) {
    var str = obj.val();

    str = Trim(str);
    
    if (str == "") {

        obj.focus();
        return false;
    }
    return true;

}

//功能说明：检证是否是整数
//参数说明：字符串
//返 回 值：bool,是整数返回true,否则返回：false
function isInt(obj) {
    var reg = /^(-|\+)?\d+$/;
    return IsCheck(obj, reg);
    ;
}

//功能说明：检证是否是大於等於0的整数
//参数说明：字符串
//返 回 值：bool,是大於0的整数返回true,否则返回：false
function isUInt(obj) {
    var reg = /^\d+$/;
    return IsCheck(obj, reg);

}

//功能说明：检证是否是大於0的整数
//参数说明：字符串
//返 回 值：bool,是大於0的整数返回true,否则返回：false
function isUIntM(obj) {
    var reg = /^[1-9](\d+)?$/;
    return IsCheck(obj, reg);

}

//功能说明：检证是否是数值float类型的数
//参数说明：字符串
//返 回 值：bool,返回：是数值(即float类型的变量)true,否则返回：false
function IsDecimal(obj) {
    var reg = /^(-|\+)?\d+(\.\d+)?$/;
    return IsCheck(obj, reg);

}
//功能说明：检证是否是大於等於0的float类型的数
//参数说明：字符串
//返 回 值：bool,返回：是数值(即是大於0的float类型的数)true,否则返回：false		
function IsUDecimal(obj) {
    var reg = /^\d+(\.\d+)?$/;
    return IsCheck(obj, reg);

}
//功能说明：检证是否是大於0的float类型的数
//参数说明：字符串
//返 回 值：bool,返回：是数值(即是大於0的float类型的数)true,否则返回：false		
function IsUDecimalM(obj) {
    var reg = /^[1-9]+|(\d+(\.[0-9]*[1-9]+)+)$/;
    return IsCheck(obj, reg);

}
//功能说明：检证是否符合EMail格式
//参数说明：字符串
//返 回 值：bool,返回：符合EMail格式true,否则返回：false
function IsEMail(obj) {
    var reg = /^[a-zA-Z0-9_.-]+@[a-zA-Z0-9_-]+(\.[a-zA-Z0-9_-]+)+$/;
    //var reg = /^([a-zA-Z0-9_.-])+@(([a-zA-Z0-9-])+.)+([a-zA-Z0-9]{2,4})+$/;
    return IsCheck(obj, reg);
}