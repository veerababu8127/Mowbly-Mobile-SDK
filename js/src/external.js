// External Applications api
$m.openGoogleMaps = function(fp_callback) {
	var scheme;
	if($m.isAndroid()) {
		scheme = "http://maps.google.com/maps";
	} else if($m.isIOS()) {
		scheme = "comgooglemaps://";
	} else if($m.isWindowsPhone()) {
	} else if($m.isBlackberry()) {
	}

	var options = { scheme: scheme };
	Framework.openExternal(options, fp_callback);
};