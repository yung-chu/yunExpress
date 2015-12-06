	function iFrameHeight(id,name) {
		var ifm= document.getElementById(id); 
		var subWeb = !document.frames ?  ifm.contentDocument : document.frames[name].document;    //后面那句兼容IE6，7， 前面那句兼容现代浏览器
		if(ifm != null && subWeb != null) { 
		ifm.height = subWeb.body.scrollHeight; 
		} 
	} 