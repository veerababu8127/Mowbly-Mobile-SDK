(function(window){
	var mowbly = window.__mowbly__;
	var Bridge = mowbly.Bridge;
	
	/* Preferences Feature */
	var PreferencesFeatureName = "preferences";
	function Preferences(preferences) {
		if(!preferences) {
			preferences = {};
		}
		this.preferences = preferences;
	}
	Preferences.prototype.put = function(name, value) {
		if(typeof name === "undefined"){
			throw new TypeError("Preference key is required");
		} if(typeof value === "undefined") {
			throw new TypeError("Preference value is required");
		}
		this.preferences[name] = value;
	}
	Preferences.prototype.get = function(name) {
		return this.preferences[name];
	}
	Preferences.prototype.remove = function(name) {
		this.preferences[name] = null;
		delete this.preferences[name];
	}
	Preferences.prototype.loadFromContext = function(context){
		this.load(context.preferences);
	}
	Preferences.prototype.load = function(preferences) {
		this.preferences = preferences;
	}
	Preferences.prototype.toJSON = function() {
		return this.preferences;
	}
	Preferences.prototype.commit = function() {
		var preferences = JSON.stringify(this.preferences);
		Bridge.invoke(PreferencesFeatureName, "commit", [preferences]);
	}
	Preferences.prototype.removeAll = function() {
		this.preferences = {};
	}
	
	mowbly.addFeature(PreferencesFeatureName, "Preferences", Preferences);
	mowbly.preferences = new Preferences();
	mowbly.exportHelperMethod("putPref", "put", "preferences");
	mowbly.exportHelperMethod("getPref", "get", "preferences");
	mowbly.exportHelperMethod("savePref", "commit", "preferences");
	mowbly.exportHelperMethod("removePref", "remove", "preferences");
	mowbly.exportHelperMethod("removeAllPref", "removeAll", "preferences");
	
})(window);