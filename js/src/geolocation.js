(function(window){
	var mowbly = window.__mowbly__;
	var Bridge = mowbly.Bridge;

	/* GeoLocation Feature */
	var GeoLocationFeatureName = "geolocation";
	var GeoLocation = {};
	GeoLocation.getCurrentPosition = function() {
		var defaultEnableHighAccuracy = false, defaultTimeout = 10000, defaultMaximumAge = 60000;

		var options, fp_callback, noOfArgs = arguments.length;
		if(noOfArgs == 1) {
			fp_callback = arguments[0];
		} else {
			options = arguments[0];
			fp_callback = arguments[1];
		}
		options = options || {};

		__m_Utils.checkRequired("callback", fp_callback, ["object", "function"]);

		function normalizeOptions(options) {
			if(!__m_Utils.isOfType(options.enableHighAccuracy, "boolean")) {
				options.enableHighAccuracy = defaultEnableHighAccuracy;
			}
			if(!__m_Utils.isOfType(options.timeout, "number")) {
				options.timeout = defaultTimeout;
			}
		}
		normalizeOptions(options);

		Bridge.invoke(GeoLocationFeatureName, "getCurrentPosition", [options], fp_callback);
	},
	GeoLocation.getLocationForCoordinates = function() {
		var latitude, longitude, maxResults = 1, fp_callback, noOfArgs = arguments.length;
		if(noOfArgs < 3) {
			throw new TypeError("The arguments latitude, longitude and fp_callback are required");
		} else if(noOfArgs == 3) {
			latitude = arguments[0];
			longitude = arguments[1];
			fp_callback = arguments[2];
		} else {
			latitude = arguments[0];
			longitude = arguments[1];
			maxResults = arguments[2];
			fp_callback = arguments[3];
		}

		__m_Utils.checkRequired("latitude", latitude, "number");
		__m_Utils.checkRequired("longitude", longitude, "number");
		__m_Utils.checkRequired("callback", fp_callback, ["object", "function"]);
		__m_Utils.checkType("maxResults", maxResults, "number");
		if(!__m_Utils.isInRange(latitude, -90, 90)) {
			throw new TypeError("The argument latitude should be between -90 & 90");
		}
		if(!__m_Utils.isInRange(longitude, -180, 180)) {
			throw new TypeError("The argument latitude should be between -180 & 180");
		}

		Bridge.invoke(GeoLocationFeatureName, "getLocationForCoordinates", [latitude, longitude, maxResults], fp_callback);
	};

	// Add event library to features.
	__m_Utils.inherit(GeoLocation, Observable);
	mowbly.addFeature(GeoLocationFeatureName, "GeoLocation", GeoLocation);
	
	// GeoLocation
	mowbly.exportHelperMethod("getLocation", "getCurrentPosition", "GeoLocation");

})(window);