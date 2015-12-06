(function ($) {
    $.extend({
        Request: function (m) {
            var sValue = location.search.match(new RegExp("[\?\&]" + m + "=([^\&]*)(\&?)", "i"));
            return sValue ? sValue[1] : sValue;
        },
        UrlUpdateParams: function (url, name, value) {
            var r = url;
            if (r != null && r != 'undefined' && r != "") {
                value = encodeURIComponent(value);
                var reg = new RegExp("(^|)" + name + "=([^&]*)(|$)");
                var tmp = name + "=" + value;
                if (url.match(reg) != null) {
                    r = url.replace(eval(reg), tmp);
                }
                else {
                    if (url.match("[\?]")) {
                        r = url + "&" + tmp;
                    } else {
                        r = url + "?" + tmp;
                    }
                }
            }
            return r;
        }

    });
})(jQuery);

(function($) {
    $.extend({
        RegExpHelper: {
            
            /* 
            用途：检查输入字符串是否为空或者全部都是空格 
            输入：str 
            返回：如果全是空返回true,否则返回false 
            */
            IsNull: function(str) {
                if (str == "") {
                    return true;
                }
                var regu = "^[ ]+$";
                var re = new RegExp(regu);
                return re.test(str);
            },

            /* 
            用途：检查输入对象的值是否符合整数格式 
            输入：str 输入的字符串 
            返回：如果通过验证返回true,否则返回false 
            */
            IsInteger: function(str) {
                var regu = /^[-]{0,1}[0-9]{1,}$/;
                return regu.test(str);
            },

            /* 
            用途：检查输入字符串是否符合正整数格式 
            输入：s：字符串 
            返回：如果通过验证返回true,否则返回false 
            */
            IsNumber: function(s) {
                var regu = "^[0-9]+$";
                var re = new RegExp(regu);
                if (s.search(re) != -1) {
                    return parseInt(s) > 0;
                } else {
                    return false;
                }
            },

            /* 
            用途：检查输入字符串是否符合金额格式,格式定义为带小数的正数，小数点后最多三位 
            输入：s：字符串 
            返回：如果通过验证返回true,否则返回false 
            */
            IsMoney: function(s) {
                var regu = "^[0-9]+[\.][0-9]{0,3}$";
                var re = new RegExp(regu);
                if (re.test(s)) {
                    return true;
                } else {
                    return false;
                }
            },

            /* 
            用途：检查输入的Email信箱格式是否正确 
            输入：strEmail：字符串 
            返回：如果通过验证返回true,否则返回false 
            */
            CheckEmail: function(strEmail) {
                //var emailReg = /^[_a-z0-9]+@([_a-z0-9]+\.)+[a-z0-9]{2,3}$/; 
                var emailReg = /^[\w-]+(\.[\w-]+)*@[\w-]+(\.[\w-]+)+$/;
                if (emailReg.test(strEmail)) {
                    return true;
                } else {
                    alert("您输入的Email地址格式不正确！");
                    return false;
                }
            },

            /* 
            用途：检查输入字符串是否是带小数的数字格式,可以是负数 
            输入：str：字符串 
            返回：如果通过验证返回true,否则返回false 
            */
            IsDecimal: function(str) {
                if (IsInteger(str)) {
                    return true;
                }
                var re = /^[-]{0,1}(\d+)[\.]+(\d+)$/;
                if (re.test(str)) {
                    if (RegExp.$1 == 0 && RegExp.$2 == 0) {
                        return false;
                    }
                    return true;
                } else {
                    return false;
                }
            },
            
            /* 
            用途：检查输入字符串是否是带小数的数字格式,必须是正数 
            输入：str：字符串 
            返回：如果通过验证返回true,否则返回false 
            */
            IsDecimalP: function (str) {
                if (this.IsNumber(str)) {
                    return true;
                }
                var re = /^(\d+)[\.]+(\d+)$/;
                if (re.test(str)) {
                    if (RegExp.$1 == 0 && RegExp.$2 == 0) {
                        return false;
                    }
                    return parseFloat(str) > 0;
                } else {
                    return false;
                }
            }
        }
    });
})(jQuery);