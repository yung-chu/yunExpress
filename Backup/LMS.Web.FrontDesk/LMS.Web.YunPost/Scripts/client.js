var htmlAdBtn = '';
$("#solideAdSlide img").each(function(index, image) {
	var id = "adImage" + index;
	htmlAdBtn = htmlAdBtn + '<a href="javascript:" class="solide_ad_btn_a" data-rel="'+ id +'"></a>';
	image.id = id;
});
$("#solideAdBtn").html(htmlAdBtn).find("a").powerSwitch({
	eventType: "hover",
	classAdd: "active",
	animation: "slide",
	autoTime: 5000,
	onSwitch: function(image) {
		if (!image.attr("src")) {
			image.attr("src", image.attr("data-src"));	
		}
	}
}).eq(0).trigger("mouseover");

/***首页头部交互***/

var navAnim = (function(){
			var navAbs = $('.nav_position');
			var navActive = $('.nav_A');
			var el = $('.nav_A').filter('.nav_select'); //选中的el
			var idx = el.index()-1;
			var meuBox = $('.meu_area');
			var slide = $('#slide,.up_warp');
			function publicb(){
				window.timer = window.setTimeout(function() {
							navAbs.animate({'width':navActive.eq(idx).width()+40,'marginLeft':navActive.eq(idx).position().left-3},200);
						}, 200);
			};
			function publicc(idx){
				meuBox.children().eq(idx-1).show().siblings().hide();
			}
			function publicd(){
				clearTimeout(window.timer);
				navAbs.stop(true,true);	
				navAbs.animate({'width':$(this).width()+40,'marginLeft':$(this).position().left-3},200);
				  };
			slide.bind('mouseenter',function(){
				if(meuBox.is(':visible'))
				{
					window.setTimeout(function() {
						meuBox.slideUp(100);
						publicb();
						navActive.bind('mouseenter',publicd);
						navActive.bind('mouseleave',publicb);
					}, 200);
				}
			});
			return{
				publica:function(){
					//设置初始化的时候框框弹到指定的位置上；
					navAbs.css({width:el.width()+40,marginLeft:el.position().left-3});
					//为菜单添加鼠标经过、鼠标离开事件
					navActive.bind('mouseenter',publicd).bind('mouseleave',publicb);
					
					//添加点击事件
					navActive.click(function(){
						navActive.unbind("mouseenter",publicd);
						var index = $(this).index()-1;  //得到点击的元素index
						navAbs.animate({'width':$(this).width()+40,'marginLeft':$(this).position().left-3},200);
						navActive.unbind("mouseleave",publicb); //去掉鼠标移开事件
						if(meuBox.is(':visible'))
						{
							publicc(index);
						}
						else
						{
							publicc(index);
							meuBox.slideDown(100);
							
						}
					});
				}
			};
		})();
		navAnim.publica();
/****QQ*/
$("body").Sonline({
		Position:"right",//left或right
		Top:200,//顶部距离，默认200px
		Effect:true, //滚动或者固定两种方式，布尔值：true或false
		DefaultsOpen:false, //默认展开：true,默认收缩：false
		Qqlist:"2851260175|业务01,2851260176|业务02,2851260179|业务03,2851260155|业务04,2851260178|推广专员,2851260158|小包客服,2851260167|快递客服,2851260189|投诉建议" //多个QQ用','隔开，QQ和客服名用'|'隔开
	});
$('.head_Avtive').click(function(){
			$('.head_Avtive').css('border-color','#fff');
			$('.head_Warp').hide();
			var t = $(this);
			var warp = $('.'+$(this).attr('data'));
			warp.show().css({ left: $(this).offset().left-warp.width()/2});
			$(this).css('border-color','#fdb029');
			warp.bind('mouseleave', function(){
				window.tt = window.setTimeout(function() {
					warp.hide();
					t.css('border-color','#fff');
				}, 300).call($(this));
			});
		});
$(".Key").focus(function(){
	$(this).val('');	
});
$(".Key").blur(function(){
	if($(this).val().replace(/(^\s*)|(\s*$)/g, "")=="")
	{
		$(this).val($(this).attr('date-value'));
	}
});
$(window).scroll((function() {
	if($('.Fixed').length>0)
	{
		var t = $(".Fixed").offset().top;
		var w = $(".Fixed").width();
		var fixed = function() {
				var b = $(this).scrollTop();
				if (b >= t) {
					$(".Fixed").css("position", "fixed");
					$(".Fixed").css("top", "0");
					$(".Fixed").css("width", w);
					$(".Fixed").css("box-shadow", '0 0 5px #dedede');
				}
				if (b < t) {
					$(".Fixed").css("position", "static");
				}
			};
		return fixed;
	}
})());	
$('.pack_in table tr:even').addClass('double');
$('.pack_in table tr').not( $(".tb_head")).hover(function(){
	$(this).addClass('tb_select');	
},
function(){
	$(this).removeClass('tb_select');	
}
);

	
