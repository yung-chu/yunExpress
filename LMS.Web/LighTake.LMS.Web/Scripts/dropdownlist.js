/*!
 * JQuery dropdownlist
 * http://www.pangyuyu.com/
 * pangyuyu.dada@gmail.com
 *
 * Date: 2010-12-09
 */
(function($){
	$.fn.dropdownlist=function(opts){
		var settings = {
						id:'',
						width:100,
						listboxwidth:0,
						listboxmaxheight:200,
						listboxalign:'auto',
						columns:1, 
						checkbox:false,
						maxchecked:3,
						selectedtext:'',
						requiredvalue:[],
						disabled:false,
						selectclass:'ddl-select',
						listboxclass:'ddl-listbox',
						selected:[],
						data:{},
						onchange:null
						};
		
		var stopBubble=function (a) {
		    var a = a || window.event;
		    if (a.stopPropagation) {
		        a.stopPropagation()
		    } else {
		        window.event.cancelBubble = true
		    }
		};
		return this.each(function(){
			$.extend(settings , opts || {});			
			if(settings.id=='') throw new Error('dropdownlist 的id不能为空。');
			if(settings.checkbox){
				for(var _i=0;_i<settings.requiredvalue.length;_i++){
					if($.inArray(settings.requiredvalue[_i],settings.selected)==-1) {
						throw new Error(settings.id+'错误：必须选择的值（requiredvalue）没有设置为选中（selected）');
					}
				}
				if(settings.selected.length>settings.maxchecked)throw new Error(settings.id+'错误：默认选中的值（selected）多于设置的最多同时选择的值（maxchecked）');
			}
			
			var $this = $(this);
			$this.empty();
			window[settings.id]={};
			var objid = eval(settings.id);
			objid.onchange=settings.onchange;
			var select = $('<div>',
							{'class':(settings.disabled?settings.disabledclass:settings.selectclass),
							 'width':settings.width,
							 'click':function(evt){
							 	$('.ddlclass').hide();
							 	if(settings.disabled)return;
							 	var $thispos = $(this).offset();
							 	var $thisheight = $(this).outerHeight();
							 	var $lb = $('div.'+settings.listboxclass,$this);
							 	var $lbheight = $lb.outerHeight();
							 	var $lbwidth =  $lb.outerWidth();
							 	
							 	var lbtop = (($thispos.top + $thisheight + $lbheight)>$(document).height() && $thispos.top>$lbheight)?
							 			$thispos.top-$lbheight:$thispos.top + $thisheight;
							 	
							 	var posright = $thispos.left+$(this).outerWidth()-$lbwidth;
							 	var posleft = $thispos.left;
							 	var lbleft = $lbwidth+$thispos.left>$(document).width()?posright:posleft;
							 	switch(settings.listboxalign){
							 		case 'left':
							 			lbleft = posleft;
							 			break;
							 		case 'right':
							 			lbleft = posright;
							 			break
							 		default:
							 			break;
							 	}
							 	
							 	$('div.'+settings.listboxclass,$this).css({'top':lbtop,'left':lbleft}).show();
							 	stopBubble(evt);
							 }
							}).appendTo($this);
			
            if(navigator.userAgent.toLowerCase().indexOf('msie 6.0')!=-1)
            {
            	select.mouseover(function(){
            		if(!select.hasClass('hover'))select.addClass('hover');
            	}).mouseout(function(){
            		if(select.hasClass('hover'))select.removeClass('hover');
            	});
            }
			settings.listboxwidth = settings.listboxwidth<=0?select.outerWidth():settings.listboxwidth;
			var listbox = $('<div>',{
							'class':settings.listboxclass+' ddlclass',
							'html':'<table width="100%" cellpadding="0" cellspacing="0" border="0" style="line-height:15px;"><tbody></tbody></table>',
							'css': {'position':'absolute','width':settings.listboxwidth,'overflow':'hidden','overflow-y':'auto','display':'none','z-index':1000}
						}).appendTo($this);
			if(settings.checkbox)
			{
			    //update by yungchu
			    var getNumber = settings.maxchecked - 2;

			    var tfoot = $('<tfoot><td colspan="' + settings.columns + '"><span class="info">最多可选' + getNumber + '项</span><span class="btn-area"><input type="button" value="确定" class="btn-ok"><input type="button" value="取消" class="btn-cancel"></span></td></tfoot>');
				$('table',listbox).append(tfoot);
				var _btn = $('input',tfoot);
				
				$(_btn[0]).click(function () {
				    //debugger;
					if(typeof(select.attr('selectedTmpValue'))!='undefined'){
						select.attr({'selectedText':select.attr('selectedTmpText'),'selectedValue':select.attr('selectedTmpValue')});
						select.attr({'selectedTmpText':'','selectedTmpValue':''});
					}
					objid.text = select.attr('selectedText').replace(/^;*/, "");
					objid.value = select.attr('selectedValue').replace(/^;*/, "");
					if(settings.selectedtext=='')select.html(objid.text);
					changeval();
					objid.hide(true);
				});
				$(_btn[1]).click(function () {

					objid.hide();//隐藏弹框
				});
			};
			var table = $('table>tbody',listbox).get(0);	
			objid.disable=function(){
				settings.disabled=true;
				if(!select.hasClass('ddl-disabled'))select.addClass('ddl-disabled');
			};
			objid.enable=function(){
				select.removeClass('ddl-disabled');
				settings.disabled=false;
			};
			objid.init = function(mdata,selectedValue){
				var row,cell,chk,lbl;
				var index = 0;		
				var selecttextarr = [];
				if(selectedValue){
					$.each(selectedValue,function(i,n){
						var tmptxt = mdata[n]?mdata[n]:'';
						tmptxt = tmptxt.replace(/;/g,';');
						selecttextarr[selecttextarr.length]=tmptxt;
					});				
				}
				var selecttext = selecttextarr.join(';');
				selecttext = selecttext==''?'':selecttext;
				selecthtml = settings.checkbox&&settings.selectedtext!=''?settings.selectedtext:selecttext;				
				select.attr({'selectedText':selecttext,'selectedValue':settings.selected.join(';')}).html(selecthtml);
					
				$(table).html('');
				$.each(mdata,function(k,v){					
		            if (index % settings.columns == 0) { row = table.insertRow(-1); }
		            cell = row.insertCell(index%settings.columns);
		            var $cell = $(cell);
		            if(navigator.userAgent.toLowerCase().indexOf('msie 6.0')!=-1)
		            {
		            	$cell.mouseover(function(){
		            		if(!$cell.hasClass('hover'))$cell.addClass('hover');
		            	}).mouseout(function(){
		            		if($cell.hasClass('hover'))$cell.removeClass('hover');
		            	});
		            }
		            lbl = v.replace(/;/g,'；');
		            if(settings.checkbox)
		            {
		            	chk=$('<input>',{type:'checkbox',value:k,'txt':lbl});
		            	if($.inArray(k,settings.selected)!=-1)chk.attr('checked','checked');
		            	if($.inArray(k,settings.requiredvalue)!=-1){chk.attr('disabled','disabled');}
		            	$cell.append(chk);
		            }
		            
		            $cell.append(lbl);
		            $cell.attr('value',k).css({'cursor':'pointer'}).click(function(evt){
						var tmp = $('input',$(this));
						if(tmp.attr('disabled')){stopBubble(evt);return;}
						if(settings.checkbox){
							if(tmp.attr('disabled')=='disabled')return;
							var obj = evt.srcElement || evt.target;
							
							var tmptext = select.attr('selectedTmpText');
							var tmpvalue = select.attr('selectedTmpValue');
							tmptext = typeof(tmptext)!='undefined'?(tmptext!=''?tmptext.split(';'):[]):objid.text.split(';');
							tmpvalue = typeof(tmpvalue)!='undefined'?(tmpvalue!=''?tmpvalue.split(';'):[]):objid.value.split(';');
							
							if(obj.tagName && obj.tagName.toLowerCase()!='input'){
								if(tmp.attr('checked')) tmp.removeAttr('checked');
								else tmp.attr('checked','checked');
							}
							if(!tmp.attr('checked'))
							{
								var _i = $.inArray(tmp.val(),tmpvalue);
								tmpvalue.splice(_i,1);
								tmptext.splice(_i,1);
							}
							else{
								tmpvalue[tmpvalue.length]=tmp.val();
								tmptext[tmptext.length]=tmp.attr('txt');
							}
							
							if(tmpvalue.length>settings.maxchecked){
								var _i=0
								for(;_i<tmpvalue.length;_i++)
								{
									if($.inArray(tmpvalue[_i],settings.requiredvalue)==-1)break;
								}
								$('input[value="'+tmpvalue[_i]+'"]',listbox).removeAttr('checked');
								tmpvalue.splice(_i,1);
								tmptext.splice(_i,1);
							}
							select.attr({'selectedTmpText':tmptext.join(';'),'selectedTmpValue':tmpvalue.join(';')});
							stopBubble(evt);
						}else
						{
							select.attr({'selectedText':$(this).text(),'selectedValue':$(this).attr('value')}).html($(this).text());
							objid.value = select.attr('selectedValue');
							objid.text = select.attr('selectedText');
						}
						if(!settings.checkbox)changeval();
		            });		            
					index++;
				});
				if(cell && index%settings.columns!=0)$(cell).attr('colspan',settings.columns+1-index%settings.columns);

				if(listbox.height()>settings.listboxmaxheight)listbox.height(settings.listboxmaxheight).css({'overflow':'hidden','overflow-y':'auto'});
			};
			objid.hide = function(clear){
				if(settings.checkbox && !clear){
					select.removeAttr('selectedTmpText');
					select.removeAttr('selectedTmpValue');
					var tmpsv = select.attr('selectedValue').split(';');
					$('input',listbox).each(function(){
						$(this).removeAttr('checked');
					});
					$.each(tmpsv,function(i,n){
						$('input[value="'+n+'"]',listbox).attr('checked','checked');
					});
				}
				$('div.'+settings.listboxclass,$this).hide();
			};
			var changeval=function(){
				$('#'+settings.id).val(objid.value);
				if(objid.onchange)objid.onchange(objid.text,objid.value);
			}
			objid.init(settings.data,settings.selected);
			objid.text = select.attr('selectedText').replace(/^;*/,"");
			objid.value = select.attr('selectedValue').replace(/^;*/, "");
			
			$('<input>',{type:'hidden',value:objid.value,id:settings.id,name:settings.id}).appendTo($this);
			$(document).click(function(){objid.hide();});
			$(window).resize(function(){objid.hide();});
		});
	};
	
})(jQuery);
