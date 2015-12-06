	$('.mwd_area_control').click(
	function() {
		if ($('.mwd_area').css('left') == '-234px') {
			$('.mwd_area').animate({
				left: '0'
			}, 200)
		} else {
			$('.mwd_area').animate({
				left: '-234px'
			}, 200)
		}
	});
	var closeTanchu = function() {
			$('.mwd_area').animate({
				left: '-234px'
			}, 200)
		};
	var copy = function (str) {
			setTimeout(function() {
			$('.Popup_area').fadeIn(100)
			}, 200)
			setTimeout(function() {
				$('.Popup_area').fadeOut(500)
			}, 1000)
		}
	$('.mwd_detele').click(function() {
		$('.mwd_Testarea').val('');
		$('.mwd_Testarea').css('height', 200);
		$('.mwd_L').css('left', 205).find('li').remove();
	});
	$('.mwd_close').click(closeTanchu);
	
   /*¸´ÖÆÁ´½Ó*/
  // $('.copy_wd_area').click(copy);
	//$("span[name='copyLinkHref']").click(copy);
	




	//$('.T').click(function() {
	//	var index = $(this).index();
	//	var date = $(this).parent().attr('date');
	//	var trueStr = "T_content_" + date.match(/[^_]+$/g);
	//	$(this).addClass('T_on def').removeClass('T_off');
	//	$(this).siblings().removeClass('T_on def').addClass('T_off');
	//	$('.T_content').attr('date', trueStr).children().eq(index).show().siblings().hide();
	//});
$('.T').click(function() {
		var thisdate = $(this).attr('date');
		var pdate = $(this).parent().attr('date');
		var trueStr = "T_content_" + pdate.match(/[^_]+$/g);
		$(this).addClass('T_on def').removeClass('T_off');
		$(this).siblings().removeClass('T_on def').addClass('T_off');
		$('.T_content').filter('[date='+trueStr+']').children().filter('[date='+thisdate+']').show().siblings().hide();
		if($(this).index()==0)
		{
			$('.T_content').attr('date', trueStr).children().show();
		}
	});
	$('.wd_Tb').toggle(

	function() {
		var last = $(this).next().find('.Step_r');
		var first = $(this).next().find('.Step_l');
		$(this).next().slideDown(200);
		$(this).find('tr').addClass('wd_Tb_on');
		first.css('margin-left', -(first.width() / 2))
		last.css('margin-right', -(last.width() / 2))
	}, function() {
		$(this).next().slideUp(200, function() {
			$(this).prev().find('tr').removeClass('wd_Tb_on')
		});
	}).hover(

	function() {
		$(this).addClass("bgf8")
	}, function() {
		$(this).removeClass("bgf8")
	});
	$(".active").click(function(){
			$(this).stop();
		 	$(this).children().last().show();
		 }).hover(function() {
		    $(this).children().last().hide()
		 });
	$(".warp .db_Link").live("click", function (event) {
			$(this).parents('.warp').hide();
			$(".warp").hide();
			copy();
		});