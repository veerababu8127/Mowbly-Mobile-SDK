(function(window){
	var mowbly = window.__mowbly__;
	var Bridge = mowbly.Bridge;
	
	/* Log Feature */
	var LogFeatureName = "logger";
	function _log(message, tag, severity) {
		if(typeof tag === "undefined") {
			tag = mowbly.Page.getName();
		}
		Bridge.invoke(LogFeatureName, "log", [message, tag, severity]);
	}
	var Logger = {
		LOG_DEBUG: 10000,
		LOG_INFO: 20000,
		LOG_WARN: 30000,
		LOG_ERROR: 40000,
		LOG_FATAL: 50000,

		debug: function(message, tag) {
			_log(message, tag, this.LOG_DEBUG);
		},
		
		info: function(message, tag) {
			_log(message, tag, this.LOG_INFO);
		},
		
		warn: function(message, tag) {
			_log(message, tag, this.LOG_WARN);
		},
		
		error: function(message, tag) {
			_log(message, tag, this.LOG_ERROR);
		},
		
		fatal: function(message, tag) {
			_log(message, tag, this.LOG_FATAL);
		}
	};	
	
	mowbly.addFeature(LogFeatureName, "Logger", Logger);
	
	// Log
	mowbly.exportHelperMethod("logDebug", "debug", "Logger");
	mowbly.exportHelperMethod("logInfo", "info", "Logger");
	mowbly.exportHelperMethod("logError", "error", "Logger");
	mowbly.exportHelperMethod("logFatal", "fatal", "Logger");
	mowbly.exportHelperMethod("logWarn", "warn", "Logger");	
})(window);