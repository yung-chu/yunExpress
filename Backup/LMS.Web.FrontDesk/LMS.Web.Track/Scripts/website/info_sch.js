var az = document.getElementById("country_az");   
	var azBox = document.getElementById("country_az_box");
	var con_box = document.getElementById("country_box");
	var home_selWarp = document.getElementById("home_selWarp");
	var txt_country = document.getElementById("txt_country");
	var countryCode = document.getElementById("FreightTrialFilter_CountryCode");
	var packageType = document.getElementById("FreightTrialFilter_PackageType");
	var home_math_box = document.getElementById("home_math_box");
	var sch_tab = document.getElementById("sch_tab");
	var azArray = new Array();	
	var azBoxArray = new Array();
	var za_azBox = new Array();
	for(i=0; len=az.children.length,i<len; i++)
	{
		az.children[i].setAttribute("date_nb",i);
		azArray.push(az.children[i]);
	}
	for(i=0; len = azBox.children.length,i<len; i++)
	{
		azBox.children[i].setAttribute("date_nb",i);
		azBoxArray.push(azBox.children[i]);
	}
	/**这里是点击a-z对应的容器显隐**/
	EventUtil.addHandler(az, "click", counrty_dis);
	function counrty_dis(event)
	{
		var e = EventUtil.getEvent(event);
		var target = e.target;
		for(i=0;i<azBoxArray.length; i++)
		{
			azBoxArray[i].style.display = "none";
		}
		var bn = target.parentNode.getAttribute("date_nb");
		for( i=0 ; i<az.children.length; i++)
		{
			if(azBoxArray[i].getAttribute("date_nb") ==bn)
			{
				azBoxArray[i].style.display = "block";
				return ;
			}
			
		}
	}
	/**这里是除了点击电脑屏里边的元素，点击浏览器其他位置就会使得国家选择层消失**/
	EventUtil.addHandler(dis_one, "click", function(event) {
	    if (con_box.style.display == "none") {
	        con_box.style.display = "block";
	    } else if (con_box.style.display == "block") {
	        con_box.style.display = "none";
	    }
	});
	EventUtil.addHandler(home_math_box, "click", function(event) { //点击屏幕里面的容器，不会消失
	    var e = EventUtil.getEvent(event);
	    e.stopPropagation;
	});
	EventUtil.addHandler(con_box, "click", function(event) { //点击弹出层里面的容器，也不会消失
	    var e = EventUtil.getEvent(event);
	    e.stopPropagation;
	});
	EventUtil.addHandler(document.body,"click",function(){  //点击其他地方，就让弹出的国家层，消失
	    con_box.style.display = "none";
	});
	EventUtil.addHandler(document.getElementById("country_close"),"click",function(){  //这里有个例外，虽然在弹出层里面，但是还是希望点击XX的时候，关闭弹出层

	    con_box.style.display = "none";

	});
   /**这里实现，点击弹出层里面的具体国家名称，就让弹出层消失，然后把点击到的具体对象的文本赋给添加国家的input**/
	EventUtil.addHandler(azBox, "click", function (event) {
	    alert("a");
	    var e = EventUtil.getEvent(event);
	    var target = e.target;
	    if (target.nodeName == "SPAN") {
	        txt_country.value = target.lastChild.nodeValue;
	        alert(target.nextSibling.lastChild.nodeValue+"1");
	        alert(target.nextElementSibling.lastChild.nodeValue+"2");
	        countryCode.value = target.nextSibling.lastChild.nodeValue || target.nextElementSibling.lastChild.nodeValue;
	        cookieUtil.set("inputText2", target.lastChild.nodeValue, new Date("January 1,2020"));
	        cookieUtil.set("countryCode", target.nextSibling.lastChild.nodeValue || target.nextElementSibling.lastChild.nodeValue, new Date("January 1,2020"));
	        con_box.style.display = "none";
	    }
	    return;

	});
	EventUtil.addHandler(azBox.parentNode.children[1], "click", function (event) {
	    alert("b");
       var e = EventUtil.getEvent(event);
       var target = e.target;
       if (target.nodeName == "SPAN") {
           txt_country.value = target.lastChild.nodeValue;
           countryCode.value = target.nextSibling.lastChild.nodeValue || target.nextElementSibling.lastChild.nodeValue;
           cookieUtil.set("inputText2", target.lastChild.nodeValue, 0, "/");
           cookieUtil.set("countryCode", target.nextSibling.lastChild.nodeValue || target.nextElementSibling.lastChild.nodeValue, 0, "/");
           con_box.style.display = "none";
       }
       return;

   });
	   
	/**点击选择送货地点的按钮**/
   EventUtil.addHandler(home_selWarp, "mouseover", mouseIn);
   EventUtil.addHandler(home_selWarp, "mouseout", mouseOut);
	/*EventUtil.addHandler(sch_tab, "click", function(event) {
	    var e = EventUtil.getEvent(event);
	    var target = e.target;
	    target.className = "home_math_navOn tdn";
	    if (target.getAttribute("date_nb") == "1") {
	        sch_tab.children[1].children[0].className = "home_math_navOff tdn";
	        document.getElementById("home_price_sch").className = "db";
	        document.getElementById("home_order_sch").className = "dn";
	    } else if (target.getAttribute("date_nb") == "2") {
	        sch_tab.children[0].children[0].className = "home_math_navOff tdn";
	        document.getElementById("home_order_sch").className = "db";
	        document.getElementById("home_price_sch").className = "dn";
	    }

	});*/
   EventUtil.addHandler(home_selWarp.children[2], "click", function (event) {
       alert("c");
	    var e = EventUtil.getEvent(event);
	    var target = e.target;
	    home_selWarp.children[0].value = target.lastChild.nodeValue;
	    packageType.value = target.nextSibling.lastChild.nodeValue || target.nextElementSibling.lastChild.nodeValue;
	    cookieUtil.set("inputText", target.lastChild.nodeValue, 0, "/");
	    cookieUtil.set("packageType",target.nextSibling.lastChild.nodeValue || target.nextElementSibling.lastChild.nodeValue, 0, "/");
	    //home_selWarp.children[2].style.display = "none";
	    
	    if (hasClass(home_selWarp, "hover")) {
	        home_selWarp.className = home_selWarp.className.replace(/\s+hover/, "");
	    }
	    else {
	        return;
	    }

	    
	});
		/**cookie**/
	EventUtil.addHandler(window, "load", function() {
	    if (cookieUtil.get("inputText")) {
	        var t = cookieUtil.get("inputText");
	        home_selWarp.children[0].value = t;
	        packageType.value = cookieUtil.get("packageType");
	    } else return;

	    if (cookieUtil.get("inputText2")) {
	        var t2 = cookieUtil.get("inputText2");
	        txt_country.value = t2;
	        countryCode.value = cookieUtil.get("countryCode");
	    } else return;
	});

	function get_previoussibling(n) {
	    var x = n.previousSibling;
	    while (x.nodeType != 1) {
	        x = x.previousSibling;
	    }
	    return x;
	}