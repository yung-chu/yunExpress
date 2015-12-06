var cookieUtil = {
		get : function(name){
			var cookieName = encodeURIComponent(name)+"=",
			cookieStart = document.cookie.indexOf(cookieName),
			cookieValue = null;
			if(cookieStart>-1)
			{
				var cookieEnd = document.cookie.indexOf("; ",cookieStart);
				if(cookieEnd ==-1)
				{
					cookieEnd = document.cookie.length;
				}
				var cookieValue = decodeURIComponent(document.cookie.substring(cookieStart+cookieName.length,cookieEnd));
			}
			else{
				return;
			}
			return cookieValue;
		},
		set : function(name,value,expries,path,domain,sesure){
			var cookieText = encodeURIComponent(name)+"=" +encodeURIComponent(value);
			if(expries instanceof Date)
			{
				cookieText +="; expries=" + expries;
			}
			if(path)
			{
				cookieText += "; path=" + path;
			}
			if(domain)
			{
				cookieText += "; domain" + domain;
			}
			if(sesure)
			{
				cookieText += "; sesure";
			}
			document.cookie = cookieText;
		},
		unCookie: function(name,path,domain,sesure)
		{
			this.set(name,"", new Date(0),path,domain,secure);
		}
		
	};