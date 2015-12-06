/*
此插件基于Jquery
插件名：jquery.Sonline(在线客服插件)
版本 1.0
Down:www.datouwang.com
*/
(function($){
	$.fn.Sonline = function(options){
        var opts = $.extend({}, $.fn.Sonline.defualts, options); 
		$.fn.setList(opts); //调用列表设置
		if(opts.DefaultsOpen == false){
			$.fn.Sonline.close(opts.Position,0);
		}
		//展开
		$("#SonlineBox3 > .openTrigger").live("click",function(){$.fn.Sonline.open(opts.Position);});
		//关闭
		$("#SonlineBox3 > .contentBox > .closeTrigger").live("click",function(){$.fn.Sonline.close(opts.Position,"fast");});
		
		//Ie6兼容或滚动方式显示
		if ($.browser.msie && ($.browser.version == "6.0") && !$.support.style||opts.Effect==true) {$.fn.Sonline.scrollType();}
		else if(opts.Effect==false){$("#SonlineBox3").css({position:"fixed"});}
	}
	//plugin defaults
	$.fn.Sonline.defualts ={
		style:3,
		Position:"left",//left或right
		Top:200,//顶部距离，默认200px
		Effect:true, //滚动或者固定两种方式，布尔值：true或
		DefaultsOpen:true, //默认展开：true,默认收缩：false
		Qqlist:"" //多个QQ用','隔开，QQ和客服名用'|'隔开
	}
	
	//展开
	$.fn.Sonline.open = function(positionType){
		var widthValue = $("#SonlineBox3 > .contentBox").width();
		if(positionType=="left"){$("#SonlineBox3 > .contentBox").animate({left: 0},"fast");}
		else if(positionType=="right"){$("#SonlineBox3 > .contentBox").animate({right: 0},"fast");}
		$("#SonlineBox3").css({width:widthValue+4});
		$("#SonlineBox3 > .openTrigger").hide();
	}

	//关闭
	$.fn.Sonline.close = function(positionType,speed){
		$("#SonlineBox3 > .openTrigger").show();
		var widthValue =$("#SonlineBox3 > .openTrigger").width();
		var allWidth =(-($("#SonlineBox3 > .contentBox").width())-6);
		if(positionType=="left"){$("#SonlineBox3 > .contentBox").animate({left: allWidth},speed);}
		else if(positionType=="right"){$("#SonlineBox3 > .contentBox").animate({right: allWidth},speed);}
		$("#SonlineBox3").animate({width:widthValue},speed);
		
	}

	//子插件：设置列表参数
	$.fn.setList = function(opts){
		$("body").append("<div class='SonlineBox3' id='SonlineBox3' style='top:-600px;'><div class='openTrigger' style='display:none' title='展开'></div><div class='contentBox'><div class='closeTrigger'><img src='../images/closeBtnImg.gif' title='关闭' /></div><div class='titleBox'><span>客服中心</span></div><div class='listBox'></div></div></div>");
		if(opts.Qqlist==""){$("#SonlineBox3 > .contentBox > .listBox").append("<p style='padding:15px'>暂无在线客服。</p>")}
		else{var qqListHtml = $.fn.Sonline.splitStr(opts);$("#SonlineBox3 > .contentBox > .listBox").append(qqListHtml);	}
		if(opts.Position=="left"){$("#SonlineBox3").css({left:0});}
		else if(opts.Position=="right"){$("#SonlineBox3").css({right:0})}
		$("#SonlineBox3").css({top:opts.Top});
		var allHeights=0;
		if($("#SonlineBox3 > .contentBox").height() < $("#SonlineBox3 > .openTrigger").height()){
			allHeights = $("#SonlineBox3 > .openTrigger").height()+4;
		} else{allHeights = $("#SonlineBox3 > .contentBox").height()+4;}
		$("#SonlineBox3").height(allHeights);
		if(opts.Position=="left"){$("#SonlineBox3 > .openTrigger").css({left:0});}
		else if(opts.Position=="right"){$("#SonlineBox3 > .openTrigger").css({right:0});}
	}
	
	//滑动式效果
	$.fn.Sonline.scrollType = function(){
		$("#SonlineBox3").css({position:"absolute"});
		var topNum = parseInt($("#SonlineBox3").css("top")+"");
		$(window).scroll(function(){
			var scrollTopNum = $(window).scrollTop();//获取网页被卷去的高
			$("#SonlineBox3").stop(true,true).delay(0).animate({top:scrollTopNum+topNum},"slow");
		});
	}
	
	//分割QQ
	$.fn.Sonline.splitStr = function(opts){
		var strs= new Array(); //定义一数组
		var QqlistText = opts.Qqlist;
		strs=QqlistText.split(","); //字符分割
		var QqHtml=""
		for (var i=0;i<strs.length;i++){	
			var subStrs= new Array(); //定义一数组
			var subQqlist = strs[i];
			subStrs = subQqlist.split("|"); //字符分割
			QqHtml = QqHtml+"<div class='QQList'><span>"+subStrs[1]+"：</span><a target='_blank' href='http://wpa.qq.com/msgrd?v=3&uin="+subStrs[0]+"&site=qq&menu=yes'><img border='0' src='http://wpa.qq.com/pa?p=2:"+subStrs[0]+":41 &amp;r=0.22914223582483828' alt='点击这里'></a></div>"
		}
		return QqHtml;
	}
})(jQuery);    


 