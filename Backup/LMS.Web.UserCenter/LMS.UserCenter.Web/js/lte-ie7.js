/* Load this script using conditional IE comments if you need to support IE 7 and IE 6. */

window.onload = function() {
	function addIcon(el, entity) {
		var html = el.innerHTML;
		el.innerHTML = '<span style="font-family: \'icomoon\'">' + entity + '</span>' + html;
	}
	var icons = {
			'icon-shield' : '&#xe000;',
			'icon-cancel' : '&#xe03d;',
			'icon-print' : '&#xe001;',
			'icon-locked' : '&#xe039;',
			'icon-close' : '&#xe004;',
			'icon-open' : '&#xe005;',
			'icon-download' : '&#xe006;',
			'icon-users' : '&#xe007;',
			'icon-upload' : '&#xe008;',
			'icon-uniF47D' : '&#xe009;',
			'icon-twitter' : '&#xe00a;',
			'icon-truck' : '&#xe00b;',
			'icon-trashcan' : '&#xe00c;',
			'icon-storage' : '&#xe00d;',
			'icon-search' : '&#xe00e;',
			'icon-scissors' : '&#xe00f;',
			'icon-save' : '&#xe010;',
			'icon-refresh' : '&#xe011;',
			'icon-question' : '&#xe012;',
			'icon-printer' : '&#xe013;',
			'icon-password' : '&#xe014;',
			'icon-nav_truck_service' : '&#xe015;',
			'icon-nav_truck_ques' : '&#xe016;',
			'icon-nav_storage_service' : '&#xe017;',
			'icon-money' : '&#xe018;',
			'icon-message' : '&#xe019;',
			'icon-image' : '&#xe01a;',
			'icon-home' : '&#xe01b;',
			'icon-folder-add' : '&#xe01c;',
			'icon-feed' : '&#xe01d;',
			'icon-facebook' : '&#xe01e;',
			'icon-edit' : '&#xe01f;',
			'icon-download-2' : '&#xe020;',
			'icon-delete' : '&#xe021;',
			'icon-cogs' : '&#xe022;',
			'icon-add' : '&#xe023;',
			'icon-succeed' : '&#xe024;',
			'icon-warning' : '&#xe025;'
		},
		els = document.getElementsByTagName('*'),
		i, attr, html, c, el;
	for (i = 0; ; i += 1) {
		el = els[i];
		if(!el) {
			break;
		}
		attr = el.getAttribute('data-icon');
		if (attr) {
			addIcon(el, attr);
		}
		c = el.className;
		c = c.match(/icon-[^\s'"]+/);
		if (c && icons[c[0]]) {
			addIcon(el, icons[c[0]]);
		}
	}
};