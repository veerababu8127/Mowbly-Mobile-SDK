(function(window){
	var mowbly = window.__mowbly__;
	var Bridge = mowbly.Bridge;	

	/* Network Feature */
	var NetworkFeatureName = "network";
	var Network = {
		NONE: 0,
		WIFI: 1,
		CELLULAR: 2,
		OTHER: 3,

		connected: false,

		isConnected: function() {
			return this.connected;
		},

		isHostReachable: function(hostname, fp_callback, timeout) {
			__m_Utils.checkRequired("hostname", hostname, "string");
			timeout = timeout || 5000;
			Bridge.invoke(NetworkFeatureName, "isHostReachable", [hostname, timeout], fp_callback);
		},

		getActiveNetwork: function(fp_callback) {
			Bridge.invoke(NetworkFeatureName, "getActiveNetwork", [], fp_callback);
		},

		onConnect: function(r) {
			this.connected = true;
			this.fireEvent("connect", r);
		},

		onDisconnect: function() {
			this.connected = false;
			this.fireEvent("disconnect");
		}
	};
	
	// Add event library to features.
	__m_Utils.inherit(Network, Observable);
	
	mowbly.addFeature(NetworkFeatureName, "Network", Network);

	// Network
	mowbly.exportHelperProperty("NETWORK_NONE", "NONE", "Network");
	mowbly.exportHelperProperty("NETWORK_WIFI", "WIFI", "Network");
	mowbly.exportHelperProperty("NETWORK_CELL", "CELLULAR", "Network");
	mowbly.exportHelperProperty("NETWORK_OTHER", "OTHER", "Network");
	mowbly.exportHelperMethod("getNetwork", "getActiveNetwork", "Network");
	mowbly.exportHelperMethod("networkConnected", "isConnected", "Network");
	mowbly.exportHelperMethod("isHostReachable", "isHostReachable", "Network");
	mowbly.exportHelperEvent("onNetworkDisconnect", "disconnect", "Network");
	mowbly.exportHelperEvent("onNetworkConnect", "connect", "Network");

})(window);