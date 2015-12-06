
	function dis_scecondmenu(id) {
	     
		if(document.getElementById(id).className=="selected")
		{
			document.getElementById(id).className = "un_selected";
			document.getElementById(id+"_inbox").style.display = "none";
		}
		else{
			document.getElementById(id).className = "selected";
			document.getElementById(id+"_inbox").style.display = "block";
		}
		
		
	}
	function dis_thirdmenu(id)
	{
		if(document.getElementById(id+"_thirdmenu").style.display =="block")
		{
			document.getElementById(id+"_thirdmenu").style.display = "none";

		}
		else{
			document.getElementById(id+"_thirdmenu").style.display = "block";
		}
	}
	function dis_pm_table(id)
	{
		document.getElementById(id+"_div").style.display = "block";

}

(function ($) {
    $.fn.extend({
        "SetTableBgColor": function (options) {
            option = $.extend({
                Odd: "data_row_shuang", //奇数
                Even: "data_row_dan", //偶数
                Selected: "tb_Selected", //选中
                Over: "tb_Over" //鼠标移上去
            }, options);  //此处options与function里的参数为同一个对象
            //隔行换色
            $("tbody > tr:even", this).addClass(option.Even);
            $("tbody > tr:odd", this).addClass(option.Odd);
            //鼠标移上去
            $("tbody > tr", this).mouseover(function () {
               
                if ($(this).hasClass(option.Selected) == false) {
                    $(this).addClass(option.Over);
                }
            });
            //鼠标移出
            $("tbody > tr", this).mouseout(function () {
                $(this).removeClass(option.Over);
            });
            //单击变色
            $("tbody > tr", this).click(function () {
                $("tbody > tr").removeClass(option.Over);
                if ($(this).hasClass(option.Selected) == false) {
                    $(this).addClass(option.Selected);
                } else {
                    $(this).removeClass(option.Selected);
                }
            });

            return this;
        }
    });
})(jQuery);

    $(function () {
        $("table").SetTableBgColor();

    
    });



