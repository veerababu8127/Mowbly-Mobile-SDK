(function(window){
	var mowbly = window.__mowbly__;
	var Bridge = mowbly.Bridge;
	
	/* Device Feature */
	var DeviceFeatureName = "device";
	var Device = {
		APPLICATION_MEMORY: "applicationMemory",
		AVAILABLE_EXTERNAL_MEMORY: "availableExternalMemory",
		AVAILABLE_INTERNAL_MEMORY: "availableInternalMemory",
		TOTAL_EXTERNAL_MEMORY: "totalExternalMemory",
		TOTAL_INTERNAL_MEMORY: "totalInternalMemory",

		getDeviceId: function(fp_callback) {
			Bridge.invoke(DeviceFeatureName, "getDeviceId", [], fp_callback);
		},

		getMemoryStatus: function(fp_callback) {
			Bridge.invoke(DeviceFeatureName, "getMemoryStatus", [], fp_callback);
		},

		isAndroid: function(){
			return Bridge.getType() === Bridge.ANDROID;
		},

		isBlackBerry: function(){
			return Bridge.getType() === Bridge.BLACKBERRY;
		},

		isIPhone: function(){
			return Bridge.getType() === Bridge.IPHONE;
		},

		isIPad: function(){
			return Bridge.getType() === Bridge.IPAD;
		},

		isIOS: function(){
			return this.isIPad() || this.isIPhone();
		},

		isWindowsPhone: function(){
			return Bridge.getType() === Bridge.WINDOWSPHONE;
		},

		isWeb: function(){
			return Bridge.getType() === Bridge.WEB;
		}
	};
	
	// Add event library to features.
	__m_Utils.inherit(Device, Observable);
	
	mowbly.addFeature(DeviceFeatureName, "Device", Device);
	
	// Device
	mowbly.exportHelperProperty("MEM_APP", "APPLICATION_MEMORY", "Device");
	mowbly.exportHelperProperty("MEM_EXT_AVL", "AVAILABLE_EXTERNAL_MEMORY", "Device");
	mowbly.exportHelperProperty("MEM_INT_AVL", "AVAILABLE_INTERNAL_MEMORY", "Device");
	mowbly.exportHelperProperty("MEM_EXT_TOT", "TOTAL_EXTERNAL_MEMORY", "Device");
	mowbly.exportHelperProperty("MEM_INT_TOT", "TOTAL_INTERNAL_MEMORY", "Device");
	mowbly.exportHelperMethod("getDeviceId", "getDeviceId", "Device");
	mowbly.exportHelperMethod("getMemStat", "getMemoryStatus", "Device");
	mowbly.exportHelperMethod("isAndroid", "isAndroid", "Device");
	mowbly.exportHelperMethod("isBlackBerry", "isBlackBerry", "Device");
	mowbly.exportHelperMethod("isIPhone", "isIPhone", "Device");
	mowbly.exportHelperMethod("isIPad", "isIPad", "Device");
	mowbly.exportHelperMethod("isIOS", "isIOS", "Device");
	mowbly.exportHelperMethod("isWindowsPhone", "isWindowsPhone", "Device");
	mowbly.exportHelperMethod("isWeb", "isWeb", "Device");

})(window);