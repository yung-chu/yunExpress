var EventUtil = {
		addHandler:function(element,type,hander)
		{
			if(element.addEventLintener)
			{
				element.addEventLintener(type,hander,false);
			}
			else if(element.attachEvnet)
			{
				element.attachEvent("on"+type,function(){
						hander.apply(element,argument);
					})
			}
			else{
				element["on"+type] = hander;
			}
		},
		removeHander:function(element,type,hander)
		{
			if(element.removeEventListener)
			{
				element.removeEventListener(type,hander,false)
			}
			else if(element.detachEvent)
			{
				element.detachEvent("on"+type,hander);
			}
			else{
				element["on"+type] = null;
			}
			
		}
		,
		getEvent:function(event)
		{
			var tmpEvent =  event? event:window.event;
			var result ={
				GlobalEvent:tmpEvent,
				target:(function(){
						return  this.target || this.srcElement;
					}).apply(tmpEvent),
				relatedTarget :(function()
				{
					if(this.relatedTarget)
					{
						return this.relatedTarget;
					}
					else if (this.toElement)
					{
						return this.toElement;
					}
					else if(this.fromElement)
					{
						return this.fromElement;
					}
					else return null
				}).call(tmpEvent)
				,
				stopPropagation: (function()
				{
					if(this.stopPropagation)
					{
						this.stopPropagation();
					}
					else{
						this.cancelBubble = true;
					}
				}).apply(tmpEvent)
			}
			return result;
		}
};
function hasClass(obj, className) {
    var arrClassName = obj.className.split(/\s+/i);

    for (var i = 0; i < arrClassName.length; i++) {
        if (arrClassName[i].toLowerCase() == className.toLowerCase()) {
            return true;
        }
    }
    return false;
};
function mouseIn(event) {
    var e = EventUtil.getEvent(event);
    var target = e.target;
    if (hasClass(this, "hover")) {
        return;
    }
    this.className += " hover";
}
function mouseOut(event) {
    if (hasClass(this, "hover")) {
        this.className = this.className.replace(/\s+hover/, "");
    }
    else {
        return;
    }
};