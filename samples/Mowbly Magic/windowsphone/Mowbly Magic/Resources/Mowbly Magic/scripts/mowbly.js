/*	mowbly-4.0.0 09-04-2014	*/
/*	Mowbly - Enterprise Mobile Applications Framework.	*/
(function(){
/* Custom Event Library */
/**
	@namespace
	@name EventObject
	@description EventObject for custom events
	@property {EventObject|HTMLEvent} srcEvent Source event for the current event
	@property {String} type Type of the event
*/
function EventObject(e, src){
	if(!e){
		e = {};
	}
	for(var a in e){
		if(e.hasOwnProperty(a)){
			this[a] = e[a];
		}
	}
	this.srcEvent = src;
	this.returnValue = true;
	this.cancelDefaultAction = false;
}
/**
	@description Prevents further propagation of the current event.
	@see https://developer.mozilla.org/en-US/docs/DOM/event.stopPropagation
*/
EventObject.prototype.stopPropagation = function(){
}
/**
	@description Cancels the event if it is cancelable, without stopping further propagation of the event.
	@see https://developer.mozilla.org/en-US/docs/DOM/event.preventDefault
*/
EventObject.prototype.preventDefault = function(){
	this.returnValue = false;
	this.cancelDefaultAction = true;
}

EventObject.prototype.isCancelled = function(){
	return !!this.cancelDefaultAction;
}
/**
	@namespace
	@name Observable
	@description Observable for custom event library
*/
function Observable() {
	this.listeners = {};
}
/**
	@description Registers a one time handler to be called when <i>eventName</i> fires. It is automatically removed after the first time the event fires.
	@param {String} eventName A string containing the name of the event.
	@param {Function} handler A handler function to execute when the event fires.
	@param {Object} [context] Context in which the <i>handler</i> function should execute.
	@see Observable#bind
*/
Observable.prototype.listenOnce = function(eventName, fp_listener, context) {
	this.addListener(eventName, fp_listener, context, true);
}
Observable.prototype.addListener = /**
	@description Registers a handler to be called when <i>eventName</i> fires.
	@param {String} eventName A string containing the name of the event.
	@param {Function} handler A handler function to execute when the event fires.
	@param {Object} [context] Context in which the <i>handler</i> function should execute.
*/Observable.prototype.bind = function(eventName, fp_listener, context, /**@ignore*/bListenOnce) {
	if(! this.listeners) this.listeners = {};
	if(! fp_listener) return;
	if(typeof(context) == "undefined") {
		context= null;
	}
	if(typeof(bListenOnce) == "undefined") {
		bListenOnce = false;
	}
	
	var listeners = this.listeners[eventName] || (this.listeners[eventName] = []);
	listeners[listeners.length] = {"handler": fp_listener, "context": context, "listenOnce":bListenOnce};
}
Observable.prototype.removeListener = /**
	@description Unregisters a handler for <i>eventName</i>.
	@param {String} eventName A string containing the name of the event.
	@param {Function} handler A handler function to execute when the event fires.
	@param {Object} [context] Context in which the <i>handler</i> function should execute.
*/Observable.prototype.unbind = function(eventName, fp_listener, context) {
	if(window.event && window.event.type === eventName){
		var that = this;
		setTimeout(function(){
			that.removeListener(eventName, fp_listener, context);
		},0);
		return;
	}
	if(! this.listeners) {
		this.listeners = {};
		return;
	}
	if(! fp_listener) return;
	var listeners = this.listeners[eventName];
	if(listeners) {
		var numListeners = listeners.length;
		for(var i=0; i<numListeners; i++) {
			if(listeners[i].handler == fp_listener) {
				listeners[i] = null;
				listeners.splice(i, 1);
				break;
			}
		}
	}
}
Observable.prototype.purgeListeners = function() {
	this.listeners = {};
}
Observable.prototype.hasListener = function(eventName) {
	if(! this.listeners) {
		this.listeners = {};
		return false;
	}
	var listeners = this.listeners[eventName];
	return (listeners && listeners.length > 0);
}
Observable.prototype.setEventName = function(eventObject, eventName){
	eventObject.type = eventName;
};
Observable.prototype.fireEvent = function(eventName, eventObject) {
	if(!(eventObject instanceof EventObject)){
		if(typeof eventObject == "undefined" || eventObject._super != EventObject){
			eventObject = new EventObject(eventObject);
		}
	}
	this.setEventName(eventObject, eventName);
	if(!this.listeners) {
		this.listeners = {};
		return true;
	}
	eventObject.context = this;
	var listeners = this.listeners[eventName];
	//While firing event, set eventobject on the window
	window.event = eventObject;
	if(listeners) {
		var numListeners = listeners.length;
		for(var i = 0; i < numListeners; i++){
			var listener = listeners[i];
			if(listener.handler) {
				listener.handler.call(listener.context, eventObject);
				if(listener.listenOnce) {
					listener = null;
					listeners.splice(i, 1);
					i--; numListeners--;
				}
			}
		}
	}
	//After returning, reset window.event 
	window.event = null;
	return eventObject;
}
Observable.prototype.fireEventToLastObserver = function(eventName, eventObject) {
	if(! this.listeners) {
		this.listeners = {};
		return;
	}
	var listeners = this.listeners[eventName];
	if(listeners) {
		var listener = listeners[listeners.length - 1];
		if(listener.handler) {
			listener.handler.call(listener.context, eventObject);
			
			if(listener.listenOnce) {
				listener = null;
				listeners.splice(listeners.length-1, 1);
			}
		}
	}
	return eventObject;
}
window.EventObject = EventObject;
window.Observable = Observable;
})(window);

/* Utilities */
var __m_Utils = {
	argumentsAsArray: function(arguments) {
		var arr = [];
		var cnt = arguments.length;
		for(var i=0; i<cnt; i++) {
			arr.push(arguments[i]);
		}
		return arr;
	},
	arrayUnique: function(arr, fp_comparator) {
		// Filters out duplicates in array
		var a = arr.concat();
		for(var i=0; i<a.length; ++i) {
			for(var j=i+1; j<a.length; ++j) {
				if(fp_comparator(a[i], a[j]))
					a.splice(j--, 1);
			}
		}
		return a;
	},
	checkRequired: function(paramName, paramValue, paramType) {
		if(this.isOfType(paramValue, "undefined")) {
			throw new TypeError("The argument " + paramName + " is required");
		}
		this.checkType(paramName, paramValue, paramType);
	},
	checkType: function(paramName, paramValue, paramType) {
		if(typeof paramType !== "undefined") {
			if(this.isArray(paramType)) {
				var typeFound = false;
				for(var i in paramType) {
					var type = paramType[i];
					if(this.isOfType(paramValue, type)) {
						typeFound = true;
						break;
					}
				}
				if(!typeFound) {
					var msg = "The argument " + paramName + " can be either ";
					for(var i in paramType) {
						msg = msg + paramType[i] + ", ";
					}
					msg = msg.substring(0, msg.length - 2);
					throw new TypeError(msg);
				}
			} else {
				if(!this.isOfType(paramValue, paramType)) {
					throw new TypeError("The argument " + paramName + " should be of type " + paramType);
				}
			}
		}
	},
	extendOptions: function() {
		var options, name,
			target = arguments[0] || {},
			i = 1,
			length = arguments.length;
	
		if(typeof target !== "object" && typeof target !== "function" ) {
			target = {};
		}
	
		for( ; i < length; i++) {
			if((options = arguments[i]) != null) {
				for(name in options) {
					copy = options[name];
	
					if(target === copy) {
						continue;
					}
	
					if(copy !== undefined) {
						target[name] = copy;
					}
				}
			}
		}
	
		return target;
	},
	generateGUID: function() {
		return 'urn:uuid:xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function(c) {
				var r = Math.random()*16|0, v = c == 'x' ? r : (r&0x3|0x8);
				return v.toString(16); });
	},
	/**
		* Copy prototype methods on the parent object to the child object.
		* Only reference is set, not a copy.
		*
		* @method inherit
		* @namespace __m_Utils
		* @param {Object} child - Required.
		* @param {Object} parent - Reqiired.
		* @return {Undefined} Returns void
	*/
	inherit: function(child, parent) {
		child = typeof child === "function" ? child.prototype : child;
		for(var m in parent.prototype) {
			if(! child[m]) {
				child[m] = parent.prototype[m];
			}
		}
	},
	isArray: function(el) {
		return (el instanceof Array);
	},
	isInRange: function(number, minimum, maximum) {
		this.checkRequired("number", number, "number");
		this.checkRequired("minimum", minimum, "number");
		this.checkRequired("maximum", maximum, "number");
		return (number >= minimum && number <= maximum);
	},
	isOfType: function(paramValue, paramType) {
		return (typeof paramValue === paramType) || (paramType !== "undefined" && typeof paramValue === "object" && typeof paramType === "function" && paramValue instanceof paramType);
	},
	search: function(text,context){
		for(var name in context) {
			if(context.hasOwnProperty(name)) {
				if(context[name] && typeof context[name] != "function")
					if(context[name].search(text) != -1)
						return true;
			}
		}
		return false;
	},
	extendMethod: function(obj, methodName, newMethod){
		var currentMethod = obj[methodName];
		obj[methodName] = function() {
			var tmp = this._super;
			// Add a new ._super() method that is the same method
			// but on the super-class
			this._super = currentMethod;

			// The method only need to be bound temporarily, so we
			// remove it when we're done executing
			var ret = newMethod.apply(this, arguments);
			if( tmp === undefined){
				delete this._super;
			}else{
				this._super = tmp;
			}
			return ret;
		};
	},
	inheritPrototype: function(clazz, klass){
		if(klass.constructor == Function) { 
			clazz.prototype = new klass;
			clazz.prototype.constructor = clazz;
			clazz.prototype.parent = klass.prototype;
		} else {
			clazz.prototype = klass;
			clazz.prototype.constructor = this;
			clazz.prototype.parent = klass;
		}
	}
};

((function(window) {

	/**
		@namespace mowbly
	**/
	function mowbly() {}

	// mowbly constants
	mowbly.STATUS_ERROR 	= 0;
	mowbly.STATUS_OK		= 1;

	/**
		* Callback method to raise ready and initial page events (Resume, Data), 
		* after UI is initialized from any UI library if present
		* Note - This method will be called only when a page is created for the first time.
		* Any UI library may be used here.
		*
		* @method onUiReady
		* @namespace mowbly
		* @return {Undefined} Returns void
	*/
	mowbly.onUiReady = function() {
		mowbly.uiReady = true;
		//mowbly.Page._paused = false;
		// Fire Page ready
		mowbly.Page.fireEvent("ready");
		// call on after ready
		mowbly._pageOpened(mowbly.Page.getContext());
	}

	mowbly._onPageLoad = function(pageContext) {
		try{
			mowbly.uiReady = false;
			// Initialize page
			this.Page.init(pageContext);
			window.translations = pageContext.translations ? pageContext.translations : null;
			this.language = pageContext.language;
			Bridge.onPageLoad(pageContext);
			this.Page.fireEvent(Bridge.getType());
			this.Device.orientation = pageContext.orientation;
			this.Page.fireEvent("load");
			if(!mowbly.uiReady){
				this.onUiReady();
			}
		}catch(err){
			this.Ui.hideProgress();
			// Raise on error event
			this.Page.fireEvent("error", {error:err});
			this.Logger.error("Error loading page: " + this.Page.getName() + ".\n Reason - " + err);
			if(Bridge.getType() == "web")
				throw err;
		}
	}
	
	mowbly._onDeviceOrientationChange = function(orientation, bSupported, width, height) {
		Bridge.onDeviceOrientationChange(orientation, bSupported, width, height);
		var eventObject = new EventObject({ "orientation": orientation, "returnValue": bSupported});
		mowbly.Device.fireEvent("orientationchange", eventObject);
		// Set device properties. If android, orientation change prevent default cannot be done, so setting the properties
		if(eventObject.returnValue || Bridge.getType() == Bridge.ANDROID){
			mowbly.Device.orientation = orientation;
		}
		return eventObject.returnValue;
	}
	
	mowbly._onPageMessage = function(message, sender) {
		var evt = new EventObject({message: message, sender: sender});
		mowbly.Page.fireEvent("message", evt);
	}
	
	mowbly._onPagePause = function(bIsClosing) {
		//mowbly.Page._paused = true;
		var evt = new EventObject({isClosing: bIsClosing});
		mowbly.preferences.commit();
		return mowbly.Page.fireEvent("pause", evt);
	}
	
	/* 
		Deprecated
		Call mowbly._pageOpened
	*/
	mowbly._onPageReopen = function(data, preferences, bWaiting) {
		//mowbly.Page._paused = false;
		$m.logWarn("_onPageReopen is deprecated. Call mowbly._pageOpened(context) instead");
		// TODO Deprecate and use mowbly._pageOpened
		var context = {"data": data, "preferences": preferences, "waitingForResult": bWaiting};
		mowbly._pageOpened(context);
	}
	
	mowbly._pageOpened = function(context){
		var data = context.data;
		var bWaiting = context.waitingForResult;
		mowbly.preferences.loadFromContext(context);
		var evt = new EventObject();
		evt.hideProgress = true;
		mowbly.Page.fireEvent("resume", evt);
		var shouldHideProgress = evt.hideProgress;
		if(typeof data === "string") {
			data = JSON.parse(data);
		}
		if(bWaiting){
			shouldHideProgress = mowbly._onPageResult(data, shouldHideProgress);
		}else{
			shouldHideProgress = mowbly._onPageData(data, shouldHideProgress);
			mowbly.Page.pageParent = data.page;
		}
		if(shouldHideProgress){
			mowbly.Ui.hideProgress();
		}
	}
	
	mowbly._onDeviceBackPressed = function() {
		var evt = new EventObject();
		try{
			mowbly.Page.fireEvent("backpress", evt);
			if(!evt.isCancelled()){
				$m.close();
			}
		}catch(err){
			$m.logError("Error in backpress - " + JSON.stringify(err));
			$m.close(); 
		}
	}
	
	mowbly._onPageData = function(data, shouldHideProgress) {
		window.scrollTo(0,0);
		var evt = new EventObject(data);
		evt.hideProgress = shouldHideProgress;
		mowbly.Page.fireEvent("data", evt);
		return evt.hideProgress;
	}
	
	// TODO Call from native when app is reopened from background
	mowbly._onForeground = function(){
		mowbly.Page.fireEvent("foreground");
	}
	
	// TODO Call from native when application when app goes to background
	mowbly._onBackground = function(){
		// Save preferences
		mowbly.preferences.commit();
		mowbly.Page.fireEvent("background");
	}
	
	// Called when the child Page is closed
	mowbly._onPageResult = function(data, shouldHideProgress) {
		var evt = new EventObject(data);
		evt.hideProgress = shouldHideProgress;
		mowbly.Page.fireEvent("result", evt);
		return evt.hideProgress;
	}
	
	mowbly.invoke = function(feature, method, args, fp_callback){
		Bridge.invoke(feature, method, args, fp_callback);
	};
	
	window.getTranslatedLabel = function(label){
		var tLabel = "";
		if(window.translations){
			tLabel = window.translations.labels[label];
		}
		if(!tLabel){
			tLabel = label;
		}
		return tLabel;
	};

	var Bridge = (function() {
		function log_error(tag, message) {
			Logger.error(message, tag);
		}
		
		if(!window.JSON) {
			loadSystemScript("json2.js");
		}
	
		function CallbackClient() {
			this.callbacks = [];
		}

		CallbackClient.prototype.gid = function() {
			return 'urn:uuid:xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function(c) {
					var r = Math.random()*16|0, v = c == 'x' ? r : (r&0x3|0x8);
					return v.toString(16); });
		};
		
		CallbackClient.prototype.subscribe = function(fp_callback) {
			var gid = this.gid();
			this.callbacks[gid] = fp_callback;
			return gid;
		};
		
		CallbackClient.prototype.unsubscribe = function(gid) {
			this.callbacks[gid] = null;
			delete this.callbacks[gid];
		};
		
		CallbackClient.prototype.onreceive = function(gid, response) {
			var callback = this.callbacks[gid];
	
			if (response.result && typeof response.result == "string") {
				try { response.result = JSON.parse(response.result); } catch(e) {}
			}
			if(typeof callback !== "undefined"){
				if (typeof callback == "function") {
					callback(response);
				} else {
					var ctx = callback.context;
					var listener = callback.callback;
					if (listener) {
						(ctx) ? listener.call(ctx, response) : listener(ctx, response);
					}
				}
			}
			this.unsubscribe(gid);
		};
		
		
		// Bridge types
		var ANDROID = "android";
		var BLACKBERRY = "blackberry";
		var IPHONE = "iphone";
		var IPAD = "ipad";
		var WINDOWSPHONE = "windowsphone";
		var WEB = "web";
		var WEB_SPLIT = "web_split";
		
		// Default Bridge - Abstract class type object with default implementations
		function DefaultBridge() {}
		DefaultBridge.prototype.ANDROID = ANDROID;
		DefaultBridge.prototype.BLACKBERRY = BLACKBERRY;
		DefaultBridge.prototype.IPHONE = IPHONE;
		DefaultBridge.prototype.IPAD = IPAD;
		DefaultBridge.prototype.WINDOWSPHONE = WINDOWSPHONE;
		DefaultBridge.prototype.WEB = WEB;
		
		DefaultBridge.prototype.getType = function() {};
		DefaultBridge.prototype.initialize = function() {};
		DefaultBridge.prototype.onPageLoad = function(context) {};
		DefaultBridge.prototype.onDeviceOrientationChange = function(context){};
		DefaultBridge.prototype.invoke = function(feature, method, args, fp_callback) {
			var callbackId = "";
			if (fp_callback) {
				callbackId = this.CallbackClient.subscribe(fp_callback);
			}
			FeatureBinder.invoke(feature, method, JSON.stringify(args), callbackId);
		};

		// Sniff UA and set the bridge for native comm.
		var UAGENT = navigator.userAgent.toLowerCase();
		var bridge;
		if(window.location.href.search("http") != 0 || UAGENT.search(" mowbly/") > -1){
			var uagentNative = UAGENT.split("::")[0].split(" ")[2].split("/")[0];
			if(UAGENT.search("android") > -1) {
				function AndroidBridge() {}
				__m_Utils.inheritPrototype(AndroidBridge, DefaultBridge);
				AndroidBridge.prototype.getType = function() { return ANDROID; };
				bridge = new AndroidBridge();
			}else if(UAGENT.search("blackberry") > -1) {
				function BlackBerryBridge() {}
				__m_Utils.inheritPrototype(BlackBerryBridge, DefaultBridge);
				BlackBerryBridge.prototype.getType = function() { return BLACKBERRY; };
				bridge = new BlackBerryBridge();
			}else if(UAGENT.search("windows phone") > -1) {
				function WindowsPhoneBridge() {}
				__m_Utils.inheritPrototype(WindowsPhoneBridge, DefaultBridge);
				WindowsPhoneBridge.prototype.getType = function() { return WINDOWSPHONE; };
				WindowsPhoneBridge.prototype.initialize = function() {
					var __MouseEvents = {"mousedown":"touchstart", "mousemove":"touchmove", "mouseup":"touchend", "mouseout":"touchcancel", "click":"click"};
					// Handle native mouse events (mouseup, mousedown, and mousemove) on Windows Phone
					window.onNativeMouseEvent = function (type, x, y) {
						var xMod = screen.logicalXDPI / screen.deviceXDPI;
						var yMod = screen.logicalYDPI / screen.deviceYDPI;
						var evt = document.createEvent('MouseEvents');
						var xPos = document.body.scrollLeft + Math.round(xMod * x);
						var yPos = document.body.scrollTop + Math.round(yMod * y);
						var element = document.elementFromPoint(xPos, yPos);
						evt.initMouseEvent(type, true, true, window, 0, xPos, yPos, xPos, yPos, false, false, false, false, 0, element);
						evt.__m_synth = __MouseEvents[type];
						var canceled = element ? !element.dispatchEvent(evt) : !document.dispatchEvent(evt);
						return canceled ? 'true' : 'false';
					}
					// Message Plug
					window.__wp7Plug = function(msg) {
						try {
							msg = JSON.parse(msg);
							var object = msg.o;
							var method = msg.m;
							var args = msg.a;
							if(object == "window") {
								window[method].apply(window, args);
							} else if(object == "mowbly") {
								window.mowbly[method].apply(window.mowbly, args);
							} else {
								var o = window.mowbly[object];
								if(o) {
									o[method].apply(o, args);
								} else if (typeof (object) == "string") {
										var level = object.split(".");
										var obj = window;
										for (var i = 0; i<level.length; i++) {
											obj = obj[level[i]];
										}
										if (typeof (method) == "string") {
											obj[method].apply(obj, args);
										}
								} else {
									log_error("WindowsBridge", "Object mowbly." + object + " not found.");
								}
							}
						} catch (e) {
							if(typeof(msg) == "object") {
								var m;
								if(object == "window") {
									m = window[method];
								} else if(object == "mowbly") {
									m = window.mowbly[method];
								} else {
									m = window.mowbly[object][method];
								}
								log_error("WindowsBridge", "Execution error on method " + (m.name ? m.name : "Anonymous") + "\nReason: " + e.message + "\nParameters:\n" + JSON.stringify(msg) + "\nMethod:\n" + m);
							} else {
								log_error("WindowsBridge", "Parse error: " + msg + "\nReason: " + e.message);
							}
						}
					}
				};
				WindowsPhoneBridge.prototype.invoke = function(feature, method, args, fp_callback) {
					var message = {};
					message.feature = feature;
					message.method	= method;
					message.args 	= args;
					if (fp_callback) {
						message.callbackId = this.CallbackClient.subscribe(fp_callback);
					}
					try {
						window.external.notify(JSON.stringify(message));
					} catch(e) {
						window.external.notify("Request parsing error:\n" + feature + method + fp_callback);
					}
				};
				bridge = new WindowsPhoneBridge();
			}else if(UAGENT.search("iphone") > -1 || UAGENT.search("ipad") > -1) {
				var t = UAGENT.search("iphone") > -1 ? IPHONE : IPAD;
				function IOSBridge() {}
				__m_Utils.inheritPrototype(IOSBridge, DefaultBridge);
				IOSBridge.prototype.getType = function() { return t; };
				IOSBridge.prototype.initialize = function() {
					this.xhr = new XMLHttpRequest();
					// iOS
					// Create the method queue that collects messages to Empact runtime
					// Avoids method calls getting lost as in iOS, the communication is through URL loading.
					this.qCalls = [];
					this.rId=0;
					this.notifiedNative = false;
				};
				IOSBridge.prototype.getCalls = function() {
					var aCalls = JSON.stringify(this.qCalls);
					this.notifiedNative = false;
					this.qCalls = [];
					return aCalls;
				};
				IOSBridge.prototype.invoke = function(feature, method, args, fp_callback) {
					var message = {};
					message.f = feature;
					message.m = method;
					message.a = args;
					message.p = $m.pageName();
					if (fp_callback) {
						message.callbackId = this.CallbackClient.subscribe(fp_callback);
					}
			
					this.qCalls.push(message);

					if(! this.notifiedNative) {
						this.notifiedNative = true;
						this.xhr.open("GET", "mowbly://wake?"+ "page=" + encodeURIComponent($m.pageName()) + "&cid=" + encodeURIComponent($m._cid()) + "&r=" + encodeURIComponent(this.rId), true);
						this.rId++;
						this.xhr.send();
					}
				};
				bridge = new IOSBridge();
			}
		}else{
			var MOWBLY_LIBRARY_PATH = "../system/libs/mowbly";

			function loadSystemScript(scriptFile, dontAppend) {
				// Load the emulator script files; should be sync.
				var scriptElem = document.createElement("script");
				scriptElem.src = dontAppend ? scriptFile : (MOWBLY_LIBRARY_PATH + "/js/" + scriptFile);
				document.getElementsByTagName("head")[0].appendChild(scriptElem);
			}
			function WebBridge() {}
			__m_Utils.inheritPrototype(WebBridge, DefaultBridge);
			WebBridge.prototype.getType = function() { return WEB; };
			WebBridge.prototype.initialize = function() {
				loadSystemScript("../../../app/emulator/simulatorweb.js", true);
				// TODO Fire event on ui library for initializations
				// juci.elems.makeClass(".juci_page, .juci_dialog, .juci_dialog_wrapper","-webkit-backface-visibility: hidden;");
			};
			WebBridge.prototype.invoke = function(feature, method, args, fp_callback) {
				var callbackId = "";
				if (fp_callback) {
					callbackId = this.CallbackClient.subscribe(fp_callback);
				}
				FeatureBinder.invoke(feature, method, args, callbackId);
			};
			bridge = new WebBridge();
		}
		bridge.CallbackClient = new CallbackClient();
		bridge.initialize();
		return bridge;
	})();
	mowbly.Bridge = Bridge;

		
	// TODO Framework.launchApplication, setPageResult, closeApplication should be moved to Page
	var FrameworkFeatureName = "framework";
	var Framework = {
		launchApplication: function(name, url, data, options) {
			Bridge.invoke(FrameworkFeatureName, "launchApplication", [name, url, data, JSON.parse(options)]);
		},
		
		postMessage: function(pageName, message, callback) {
			Bridge.invoke(FrameworkFeatureName, "postMessage", [pageName, message, callback]);
		},

		Features: {},

		setPageResult: function(message) {
			Bridge.invoke(FrameworkFeatureName, "setPageResult", [message]);
		},
	
		closeApplication: function(bDestroy) {
			if(!bDestroy){
				bDestroy = false;
			}
			Bridge.invoke(FrameworkFeatureName, "closeApplication", [bDestroy]);
		},
	
		openExternal: function(data, fp_callback) {
			Bridge.invoke(FrameworkFeatureName, "openExternal", [data], fp_callback);
		},

		isFeatureSupported : function(options,fp_callback){
			__m_Utils.checkRequired("callback", fp_callback, ["object", "function"]);
			if($m.isWeb()){
				Bridge.invoke(FrameworkFeatureName, "isFeatureSupported", [options], fp_callback);
			}else{
				var response = {"code" : 1, "result" : {"isAllSupported": true, "supported":[], "unsupported": []}, "error" : null};
				fp_callback(response);
			}
		},

		loadResource: function(name, fp_callback) {
			__m_Utils.checkRequired("name", name, ["string"]);
			__m_Utils.checkRequired("callback", fp_callback, ["object", "function"]);
			Bridge.invoke(FrameworkFeatureName, "loadResource", [name], fp_callback);
		}
	};

	
	// Private
    mowbly.Bridge = Bridge;
	mowbly.__CallbackClient = Bridge.CallbackClient;

	// mowbly Namespace
	window.__mowbly__ = mowbly;
	window.Framework = Framework;
	window.mowbly = {};
	
	// Export Helpers
	mowbly.exportHelperProperty = function(key, pkey, ctx) {
		var o;		
		if(ctx.indexOf(".") > 0) {
			var ctxParts = ctx.split(".");
			var obj = mowbly;
			for(var i in ctxParts) {
				obj = obj[ctxParts[i]];
			}
			o = obj;
		} else {
			o = mowbly[ctx];
		}
		if(typeof o === "undefined") {
			throw new TypeError("Export helper property failed. Feature " + ctx + " undefined.");
		}
		var p = o[pkey];
		if(typeof p === "undefined") {
			throw new TypeError("Export helper property failed. Property mowbly." + ctx + "." + pkey + " undefined.");
		}
		if(typeof mowbly[key] !== "undefined") {
			throw new TypeError("Export helper property failed. Property mowbly." + ctx + "." + key + " already defined.");
		}
		window.mowbly[key] = p;
	}
	
	mowbly.exportHelperMethod = function(key, fkey, ctx){
		var o;		
		if(ctx.indexOf(".") > 0) {
			var ctxParts = ctx.split(".");
			var obj = mowbly;
			for(var i in ctxParts) {
				obj = obj[ctxParts[i]];
			}
			o = obj;
		} else {
			o = mowbly[ctx];
		}
		if(typeof o === "undefined") {
			throw new TypeError("Export helper method failed. Feature " + ctx + " undefined.");
		}
		var f = o[fkey];
		if(typeof f !== "function") {
			throw new TypeError("Export helper method failed. Function mowbly." + ctx + "." + fkey + " undefined.");
		}
		if(typeof mowbly[key] !== "undefined") {
			throw new TypeError("Export helper method failed. Function mowbly." + ctx + "." + key + " already defined.");
		}
		window.mowbly[key] = function(){
			return f.apply(mowbly[ctx], arguments);
		}
	}
	
	mowbly.exportHelperEvent = function(key, ekey, ctx) {
		var o;		
		if(ctx.indexOf(".") > 0) {
			var ctxParts = ctx.split(".");
			var obj = mowbly;
			for(var i in ctxParts) {
				obj = obj[ctxParts[i]];
			}
			o = obj;
		} else {
			o = mowbly[ctx];
		}
		if(typeof o === "undefined") {
			throw new TypeError("Export helper event failed. Feature " + ctx + " undefined.");
		}
		window.mowbly[key] = function(fp, context) {
			o.addListener.call(o, ekey, fp, context);
		};
	}
	
	// Framework
	window.mowbly.loadResource = Framework.loadResource;

	// FeatureSupport
	window.mowbly.Features = Framework.Features;
	window.mowbly.isFeatureSupported = Framework.isFeatureSupported;

	mowbly.addFeature = function(name, key, reference){
		if(name)
			window.mowbly.Features[key] = name;
		if(key)
			mowbly[key] = reference;
		else
			console.log("Unable to add feature");
	};

	window.mowbly.onIOS = function(fp, ctx){
		window.mowbly.onIPhone(fp, ctx);
		window.mowbly.onIPad(fp, ctx);
	};

	// Export $m and helper methods on mowbly object
	window.$m = window.mowbly;
	
	// Error
	window.onerror = function(message, file, line) {
		window.$m.logError(message + "-" + file + "-" + line);
	}
})(window));

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

(function(window){
	var mowbly = window.__mowbly__;
	var Bridge = mowbly.Bridge;
	
	/* Page */
	var Page = {
		window : window,
		cid: "",
		pageName : "",
		pageIcon : "",
		context: null,
		// TODO : Native to call onResume when app is maximised from minimized state
		//_paused: false,

		init: function(context) {
			this.context = context;
			// iOS bridge context
			this.cid = context.cid;
			this.pageName = context.pageName;
			this.pageIcon = context.pageIcon;
			this.pageParent = context.pageParent;
			this.data = context.data ? JSON.parse(context.data) : null;

			// set the network status
			mowbly.Network.connected = context.network;

			// initialize the Preferences object
			mowbly.preferences.loadFromContext(context);
		},

		getCid: function() {
			return this.cid;
		},

		// returns the name of the parent page
		getParent: function() {
			return this.pageParent;
		},
		
		// returns the name of the page
		getName: function() {
			return this.pageName;
		},
		
		// returns the path of the page icon
		getIcon: function() {
			return this.pageIcon;
		},
		
		// returns the complete page context
		getContext: function() {
			return this.context;
		},
		
		DefaultOpenOptions: {
			"showProgress":true,
			"retainPageInViewStack":true
		},
		
		getOpenOptions: function(options){
			if(typeof(options) == "undefined") {
				options = this.DefaultOpenOptions;
			} else {
				if(typeof(options.showProgress) == "undefined") {
					options.showProgress = true;
				}
				if(typeof(options.retainPageInViewStack) == "undefined") {
					options.retainPageInViewStack = true;
				}
			}
			return options;
		},
		
		getOpenDataObject: function(data, options){
			var dataObj = {"page": this.pageName, "data": data};
			return dataObj;
		},
		
		open: function(name, url, data, options) {
			/*if(this._paused){
				throw new Error("Page is already paused");
			}*/
			__m_Utils.checkRequired("name", name, "string");
			__m_Utils.checkRequired("url", url, "string");
		
			// Pause the current page
			if(!mowbly._onPagePause(false).isCancelled()){
				options = this.getOpenOptions(options);
				var dataObj = this.getOpenDataObject(data, options);

				// Do not show the progress for external urls
				var canShowProgress = false;
				if(url.substring(0, 1) === "/" ||
					(url.substring(0, 4) === "file" && url.substring(url.length - 4) === "html")) {
					canShowProgress = true;
				}
				options.showProgress = options.showProgress && canShowProgress;

				if(options.showProgress) {
					var progressMsg = typeof options.progressMsg == "string" && options.progressMsg ? options.progressMsg : ("Loading "+ name + " ...");
					mowbly.Ui.showProgress(progressMsg, "");
				}
				Framework.launchApplication(name, url, JSON.stringify(dataObj), JSON.stringify(options));
			}
		},
		
		postMessage: function(pageName, message, callback) {
			Framework.postMessage(pageName, message, callback);
		},
		
		setResult: function(result) {
			/*if(this._paused){
				throw new Error("Page is already paused");
			}*/
			var data = {"page":this.pageName};
			data.result = result;
			
			Framework.setPageResult(JSON.stringify(data));
		},
		
		close: function(destroy) {
			/*if(this._paused){
				throw new Error("Page is already paused");
			}*/
			if(mowbly._canMovePageToBack === false){
				mowbly._onDeviceBackPressed();
				return;
			}
			//Raise onpause event.
			if(!mowbly._onPagePause(true).isCancelled()){
				//Raise onclose event.
				var evtObj = new EventObject({destroy: !!destroy});
				this.fireEvent("close", evtObj);
				if(!evtObj.isCancelled()){
					Framework.closeApplication(evtObj.destroy);
				}
			}
		}
		// TODO Add getLanguage & setLanguage
	};
	
	__m_Utils.inherit(Page, Observable);
	
	mowbly.addFeature(null, "Page", Page);
	
	// Page
	mowbly.exportHelperMethod("open", "open", "Page");
	mowbly.exportHelperMethod("_cid", "getCid", "Page");
	mowbly.exportHelperMethod("parentPage", "getParent", "Page");
	mowbly.exportHelperMethod("pageName", "getName", "Page");
	mowbly.exportHelperMethod("postMessage", "postMessage", "Page");
	mowbly.exportHelperMethod("setResult", "setResult", "Page");
	mowbly.exportHelperMethod("close", "close", "Page");
	mowbly.exportHelperEvent("onClose", "close", "Page");
	mowbly.exportHelperEvent("onData", "data", "Page");
	mowbly.exportHelperEvent("onMessage", "message", "Page");
	mowbly.exportHelperEvent("onPause", "pause", "Page");
	mowbly.exportHelperEvent("onReady", "ready", "Page");
	mowbly.exportHelperEvent("onResult", "result", "Page");
	mowbly.exportHelperEvent("onResume", "resume", "Page");
	mowbly.exportHelperEvent("onBackground", "background", "Page");
	mowbly.exportHelperEvent("onForeground", "foreground", "Page");
	mowbly.exportHelperEvent("onAndroid", Bridge.ANDROID, "Page");
	mowbly.exportHelperEvent("onBlackBerry", Bridge.BLACKBERRY, "Page");
	mowbly.exportHelperEvent("onIPad", Bridge.IPAD, "Page");
	mowbly.exportHelperEvent("onIPhone", Bridge.IPHONE, "Page");
	mowbly.exportHelperEvent("onWeb", Bridge.WEB, "Page");
	mowbly.exportHelperEvent("onWindowsPhone", Bridge.WINDOWSPHONE, "Page");
	
})(window);

(function(window){
	var mowbly = window.__mowbly__;
	var Bridge = mowbly.Bridge;
	
	/* Ui Feature */
	var UiFeatureName = "ui";
	var Ui = {
		TOAST_DURATION_SHORT : 0,
		TOAST_DURATION_LONG : 1,

		toast: function(message, duration) {
			if(typeof message ===  "object"){
				message = JSON.stringify(message);
			}
			if(typeof duration != "number" || (duration != this.TOAST_DURATION_SHORT && duration != this.TOAST_DURATION_LONG)) {
				duration = this.TOAST_DURATION_SHORT;
			}
			Bridge.invoke(UiFeatureName, "toast", [window.getTranslatedLabel(message), duration]);
		},

		showProgress: function(message, title) {
			message = message || "";
			title = title || "";
			Bridge.invoke(UiFeatureName, "showProgress", [window.getTranslatedLabel(title), window.getTranslatedLabel(message)]);
		},

		hideProgress: function() {
			Bridge.invoke(UiFeatureName, "hideProgress", []);
		},

		confirm: function(oConfirm, fp_callback) {
			__m_Utils.checkRequired("confirmOptions", oConfirm, ["string","object"]);
		
            var defaultButtons = [{"label":"Yes"}, {"label":"No"}];
			var message, buttons = defaultButtons, title = "";
			if(typeof oConfirm == "string") {
			    message = oConfirm;
			} else {
			    message = oConfirm.message || "";
			    if(oConfirm.buttons && oConfirm.buttons.length > 1) {
			        buttons = oConfirm.buttons;
				}
				title = oConfirm.title || "";
			}
			if(typeof message != "undefined" && message != "") {
			    var callbackId = "";
			    if(fp_callback) {
				    callbackId = Bridge.CallbackClient.subscribe(fp_callback);
			    }
			    var oMsg = {title: title, message: message, buttons: buttons, callbackId: callbackId};
			    Bridge.invoke(UiFeatureName, "confirm", [JSON.stringify(oMsg)]);
			}
		},

		alert: function(message, title, fp_callback) {
			// TODO Needs to be more smart
			if(typeof message === "object"){
				message = JSON.stringify(message);
			}
			var callbackId = "";
			if(fp_callback){
				callbackId = Bridge.CallbackClient.subscribe(fp_callback);
			}
			var oMsg = {title: title || "", message: message || "", callbackId: callbackId};
			Bridge.invoke(UiFeatureName, "alert", [JSON.stringify(oMsg)]);
		}
	};
	Ui.showToast = Ui.toast;
	
	mowbly.addFeature(UiFeatureName, "Ui", Ui);	
	
	mowbly.exportHelperProperty("TOAST_DURATION_SHORT", "TOAST_DURATION_SHORT", "Ui");
	mowbly.exportHelperProperty("TOAST_DURATION_LONG", "TOAST_DURATION_LONG", "Ui");
	mowbly.exportHelperMethod("toast", "toast", "Ui");
	mowbly.exportHelperMethod("alert", "alert", "Ui");
	mowbly.exportHelperMethod("showProgress", "showProgress", "Ui");
	mowbly.exportHelperMethod("hideProgress", "hideProgress", "Ui");
	mowbly.exportHelperMethod("confirm", "confirm", "Ui");

})(window);

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

(function(window){
	var mowbly = window.__mowbly__;
	var Bridge = mowbly.Bridge;
	
	/* Camera Feature */
	var CameraFeatureName = "camera";
	var Camera = {
		SOURCE_PHOTO_LIBRARY : 0,
		SOURCE_CAMERA : 1,
		SOURCE_PHOTO_ALBUM : 2,
		
		TYPE_JPG : 0,
		TYPE_PNG : 1,
		
		QUALITY_LOW : 25,
		QUALITY_MEDIUM : 75,
		QUALITY_HIGH : 100,

		BACK: 0,
		FRONT: 1,
		ALL: 2,

		configuration: null,

		DefaultOptions: {
			"allowEdit": false,
			"multiple": false,
			"type": 0,
			"quality": 100,
			"readData": false,
			"width": "auto",
			"height": "auto"
		},

		getConfigurationForCameraType: function(configuration, cameraType) {
			if(typeof cameraType != "number" 
				|| (cameraType != this.BACK && cameraType != this.FRONT && cameraType != this.ALL)) {
				cameraType = this.BACK;
			}
			var data = [];
			if(cameraType == this.BACK) {
				for(i in configuration) {
					var cam = configuration[i];
					if(cam.type == this.BACK) {
						data.push(cam);
					}
				}
			} else if(cameraType == this.FRONT) {
				for(i in configuration) {
					var cam = configuration[i];
					if(cam.type == this.FRONT) {
						data.push(cam);
					}
				}
			} else {
				data = configuration;
			}
			return data;
		},

		getConfiguration: function(fp_callback, cameraType) {
			var noOfArgs = arguments.length;
			if(noOfArgs < 1) {
				throw new TypeError("The argument fp_callback is required");
			}
			__m_Utils.checkType("fp_callback", fp_callback, "function");
	
			if(typeof cameraType != "number" 
				|| (cameraType != this.BACK 
					&& cameraType != this.FRONT 
					&& cameraType != this.ALL)) {
				cameraType = this.BACK;
			}

			function fp_cb(response) {
				this.camera.configuration = response.result;
				var data = this.camera.getConfigurationForCameraType(response.result, this.cameraType);
				this.callback({"code": (data == null ? 0 : 1), "result":data});
			}
			var ctx = {context: {camera: this, cameraType: cameraType, callback: fp_callback}, callback: fp_cb};

			if(this.configuration == null) {
				Bridge.invoke(CameraFeatureName, "getConfiguration", [], ctx);
			} else {
				var data = this.getConfigurationForCameraType(this.configuration, cameraType);
				fp_callback({"code": (data == null ? 0 : 1), "result":data});
			}
		},

		getPicture : function(source, options, fp_callback) {
			if(typeof(source) == "undefined" || (source < 0 || source > 2)) {
				source = this.SOURCE_CAMERA;
			}

			options = options || {};
			var options = __m_Utils.extendOptions({}, this.DefaultOptions, options);

			// FilePath
			var filePath = options.filePath;
			if(filePath) {
				// File path can be a string or File object
				__m_Utils.checkType("options.filePath", filePath, ["string", mowbly.File]);

				if(typeof filePath === "string") {
					var f = new mowbly.File(filePath);
					options.filePath = f;
				} 
			} else {
				if(typeof filePath !== "undefined") {
					delete options.filePath;
				}
			}

			if(typeof options.width == "string"){
				if(options.width.toLowerCase() === "auto"){
					options.width = -1;
				}
			}
			
			if(typeof options.height == "string"){
				if(options.height.toLowerCase() === "auto"){
					options.height = -1;
				}
			}
			Bridge.invoke(CameraFeatureName, "getPicture", [source, options], fp_callback);
		},
		
		setDefaultOptions : function(options) {
			if(typeof options === "object") {
				__m_Utils.extendOptions(this.DefaultOptions, options);
			}
		}
	}
	
	function getCameraPicture(type, args) {
		var options, fp_callback, noOfArgs = args.length;
		if(noOfArgs < 1) {
			throw new TypeError("The argument fp_callback is required");
		}

		if(noOfArgs == 1) {
			// Should be options or callback
			var arg0 = args[0];
			__m_Utils.checkType("first argument", arg0, ["object", "function"]);
			if(typeof arg0 === "object") {
				options = arg0;
			} else {
				fp_callback = arg0;
			}
		} else if(noOfArgs == 2) {
			// Should be options and callback
			options = args[0];
			__m_Utils.checkType("[0]", options, "object");
			fp_callback = args[1];
		}
		mowbly.Camera.getPicture(type, options, fp_callback);
	}

	$m.capturePic = function() {
		getCameraPicture(mowbly.Camera.SOURCE_CAMERA, arguments);
	}
	
	$m.choosePic = function() {
		getCameraPicture(mowbly.Camera.SOURCE_PHOTO_LIBRARY, arguments);
	}
	
	$m.choosePicFromAlbum = function() {
		getCameraPicture(mowbly.Camera.SOURCE_PHOTO_ALBUM, arguments);
	}
	
	$m.getPic = function() {
		var options, fp_callback, noOfArgs = arguments.length;
		if(noOfArgs < 1) {
			throw new TypeError("The argument fp_callback is required");
		}
		if(noOfArgs == 1) {
			// Should be options or callback
			var arg0 = arguments[0];
			__m_Utils.checkType("first argument", arg0, ["object", "function"]);
			if(typeof arg0 === "object") {
				options = arg0;
			} else {
				fp_callback = arg0;
			}
		} else if(noOfArgs == 2) {
			// Should be options and callback
			options = arguments[0];
			__m_Utils.checkType("[0]", options, "object");
			fp_callback = arguments[1];
		}
		$m.getCamConfig(function(r){
			var message, buttons;
			var bIsCameraAvailable;
			if(!options){
				options = {};
			}
			if(r.result.length == 0){
				buttons = [
					{"label": options.choosePicLabel ? options.choosePicLabel : "Choose"},
					{"label": options.cancelLabel ? options.cancelLabel :"Cancel"}
				];
				message = options.chooseMessage ? options.chooseMessage : "Tap 'Choose' to select a picture from gallery.";
				bIsCameraAvailable = false;
			}
			else{
				buttons = [
					{"label": options.capturePicLabel ? options.capturePicLabel :"Capture"},
					{"label": options.choosePicLabel ? options.choosePicLabel : "Choose"},
					{"label": options.cancelLabel ? options.cancelLabel : "Cancel"}
				];
				message = options.camMessage ? options.camMessage : "Tap 'Capture' to capture a picture or 'Choose' to select a picture from gallery.";
				bIsCameraAvailable = true;
			}
			var confirmOptions = {
				"title": options.title ? options.title : "Select Picture",
				"message": message,
				"buttons": buttons
			};
			var userCancelledError = {
				"code": -1,
				"error": {
					"message": "User cancelled action",
					"description": "User cancelled action"
				}
			};
			$m.confirm(confirmOptions, 
				function(index){
					if(bIsCameraAvailable){
						if(index === 0) {
							// Capture Pic
							$m.capturePic(options, fp_callback);
						}else if(index === 1) {
							// Choose Pic
							$m.choosePic(options, fp_callback);
						}else{
							// cancelled
							fp_callback(userCancelledError);
						}
					}else{
						if(index === 0) {
							// Choose Pic
							$m.choosePic(options, fp_callback);
						}else{
							// cancelled
							fp_callback(userCancelledError);
						}
					}
				}
			);
		});
	};
	
	// Add event library to features.
	__m_Utils.inherit(Camera, Observable);

	mowbly.addFeature(CameraFeatureName, "Camera", Camera);
	
	mowbly.exportHelperProperty("CAM_JPG", "TYPE_JPG", "Camera");
	mowbly.exportHelperProperty("CAM_PNG", "TYPE_PNG", "Camera");
	mowbly.exportHelperProperty("CAM_QUALITY_LOW", "QUALITY_LOW", "Camera");
	mowbly.exportHelperProperty("CAM_QUALITY_MED", "QUALITY_MEDIUM", "Camera");
	mowbly.exportHelperProperty("CAM_QUALITY_HIGH", "QUALITY_HIGH", "Camera");
	mowbly.exportHelperProperty("CAM_REAR", "BACK", "Camera");
	mowbly.exportHelperProperty("CAM_FRONT", "FRONT", "Camera");
	mowbly.exportHelperProperty("CAM_ALL", "ALL", "Camera");
	mowbly.exportHelperMethod("getCamConfig", "getConfiguration", "Camera");
	mowbly.exportHelperMethod("camSetup", "setDefaultOptions", "Camera");
	
})(window);

(function(window){
	var mowbly = window.__mowbly__;
	var Bridge = mowbly.Bridge;
	
	/* Address */
	function Address() {}
	Address.FEATURE = "feature";
	Address.ADMIN = "admin";
	Address.SUBADMIN = "subAdmin";
	Address.LOCALITY = "locality";
	Address.THOROUGHFARE = "thoroughfare";
	Address.POSTAL_CODE = "postalCode";
	Address.COUNTRY_CODE = "countryCode";
	Address.COUNTRY_NAME = "country";
	Address.PHONE = "phone";
	Address.URL = "url";
	
	/* Contacts Feature */
	var ContactsFeatureName = "contacts";
	var Contacts = {
		// Contact Constants
		TYPE_FAX : "fax",
		TYPE_HOME : "home",
		TYPE_OTHER : "other",
		TYPE_MOBILE : "mobile",
		TYPE_PAGER : "pager",
		TYPE_WORK : "work",
		
		// Contact properties
		CONTACT_ADDRESSES : "addresses",
		CONTACT_BIRTHDAY : "birthday",
		CONTACT_DEPARTMENT : "department",
		CONTACT_EMAILS : "emails",
		CONTACT_IMPPS : "impps",
		CONTACT_CATEGORIES : "categories",
		CONTACT_JOB_TITLE : "jobTitle",
		CONTACT_NAME : "name",
		CONTACT_PHONES : "phones",
		CONTACT_PHOTOS : "photos",
		CONTACT_URLS : "urls",
		
		// IM Services
		IM_SERVICE_AIM : "aim",
		IM_SERVICE_ICQ : "icq",
		IM_SERVICE_JABBER : "jabber",
		IM_SERVICE_YAHOO : "yahoo",
		
		CATEGORY_BUSINESS : "business",
		CATEGORY_PERSONAL : "personal",

		// A single contact object
		// Can be created with simple strings of name, phone and mail.
		Contact : function(name, phone, email) {
			// Contact properties.
			this.addresses = [];
			this.birthday;
			this.category;
			this.emails = [];
			this.impps = [];
			this.name;
			this.nickname;
			this.note;
			this.organization = {};
			this.phones = [];
			this.photos = [];
			this.urls = [];
			
			this.fieldByType = function(type, aProperty, property) {
				var field;
				var cnt = aProperty.length;
				for(var i=0; i<cnt; i++) {
					var oProperty = aProperty[i];
					if(oProperty["type"] === type) {
						if(property == Contacts.CONTACT_ADDRESSES || property == Contacts.CONTACT_IMPPS) {
							field = oProperty;
						} else {
							field = oProperty.value;
						}
						break;
					}
				}
				return field;
			};

			// methods
			// Getters
			this.getAddress = function(type) {
				type = type || Contacts.TYPE_WORK;
				return this.fieldByType(type, this.addresses, Contacts.CONTACT_ADDRESSES) || {};
			};
			
			this.getHomeAddress = function() {
				return this.getAddress(Contacts.TYPE_HOME);
			};
			
			this.getWorkAddress = function() {
				return this.getAddress(Contacts.TYPE_WORK);
			};
			
			this.getOtherAddress = function() {
				return this.getAddress(Contacts.TYPE_OTHER);
			};
			
			this.getAddresses = function() {
				return this.addresses;
			};
			
			this.getBirthday = function() {
				return new Date(this.birthday).toString("MM/dd/yyyy");
			};

			this.getCategory = function() {
				return this.category;
			};

			this.getEmail = function(type) {
				type = type || Contacts.TYPE_WORK;
				return this.fieldByType(type, this.emails) || "";
			};
			
			this.getHomeEmail = function() {
				return this.getEmail(Contacts.TYPE_HOME);
			};
			
			this.getWorkEmail = function() {
				return this.getEmail(Contacts.TYPE_WORK);
			};
			
			this.getOtherEmail = function() {
				return this.getEmail(Contacts.TYPE_OTHER);
			};

			this.getEmails = function() {
				return this.emails;
			};

			this.getIm = function(type) {
				type = type || IM_SERVICE_JABBER;
				return this.fieldByType(type, this.impps, Contacts.CONTACT_ADDRESSES) || "";
			};
			
			this.getAimIm = function() {
				return this.getIm(Contacts.IM_SERVICE_AIM);
			};
			
			this.getJabberIm = function() {
				return this.getIm(Contacts.IM_SERVICE_JABBER);
			};
			
			this.getIcqIm = function() {
				return this.getIm(Contacts.IM_SERVICE_ICQ);
			};
			
			this.getYahooIm = function() {
				return this.getIm(Contacts.IM_SERVICE_YAHOO);
			};

			this.getIms = function() {
				return this.impps;
			};

			this.getName = function() {
				return this.name;
			};

			this.getNickname = function() {
				return this.nickname;
			};

			this.getNote = function() {
				return this.note;
			};

			this.getOrganization = function() {
				return this.organization;
			};

			this.getPhone = function(type) {
				if(typeof type === "undefined") {
					return this.fieldByType(Contacts.TYPE_WORK, this.phones) ||
							this.fieldByType(Contacts.TYPE_MOBILE, this.phones) ||
							this.fieldByType(Contacts.TYPE_HOME, this.phones) ||
							this.fieldByType(Contacts.TYPE_OTHER, this.phones) ||
							this.fieldByType(Contacts.TYPE_FAX, this.phones) || "";
				} else {
					return this.fieldByType(type, this.phones) || "";
				}
			};
			
			this.getHomePhone = function() {
				return this.getPhone(Contacts.TYPE_HOME);
			};
			
			this.getWorkPhone = function() {
				return this.getPhone(Contacts.TYPE_WORK);
			};
			
			this.getOtherPhone = function() {
				return this.getPhone(Contacts.TYPE_OTHER);
			};
			
			this.getFax = function() {
				return this.getPhone(Contacts.TYPE_FAX);
			};
			
			this.getMobile = function() {
				return this.getPhone(Contacts.TYPE_MOBILE);
			};
			
			this.getPager = function() {
				return this.getPhone(Contacts.TYPE_PAGER);
			};

			this.getPhones = function() {
				return this.phones;
			};

			this.getPhoto = function(type) {
				type = type || Contacts.TYPE_OTHER;
				return this.fieldByType(type, this.photos) || "";
			};

			this.getPhotos = function() {
				return this.photos;
			};

			this.getUrl = function(type) {
				type = type || Contacts.TYPE_WORK;
				return this.fieldByType(type, this.urls) || "";
			};
			
			this.getHomeUrl = function() {
				return this.getUrl(Contacts.TYPE_HOME);
			};
			
			this.getWorkUrl = function() {
				return this.getUrl(Contacts.TYPE_WORK);
			};
			
			this.getOtherUrl = function() {
				return this.getUrl(Contacts.TYPE_OTHER);
			};

			this.getUrls = function() {
				return this.urls;
			};

			// Setters
			// Adds the provided address (ContactField) to contact
			this.addAddress = function(address, type) {
				__m_Utils.checkRequired("address", address, "object");
				__m_Utils.checkRequired("type", type, "string");
				this.addresses.push(new Contacts.ContactAddress(type, address));
				return this;
			};
			
			this.setAddress = function() {
				var noOfArgs = arguments.length;
				if(noOfArgs === 0) {
					throw new TypeError("The parameter address is required.");
				}
				var args = __m_Utils.argumentsAsArray(arguments);
				if(noOfArgs === 1) {
					args.push(Contacts.TYPE_WORK);
				}
				return this.addAddress.apply(this, args);
			};
			
			// Adds the provided email (ContactField) to contact
			this.addEmail = function(value, type) {
				__m_Utils.checkRequired("type", type, "string");
				__m_Utils.checkRequired("email", value, "string");
				var email = new Contacts.ContactField(type, value);
				this.emails.push(email);
				return this;
			};
			
			this.setEmail = function() {
				var noOfArgs = arguments.length;
				if(noOfArgs === 0) {
					throw new TypeError("The parameter email is required.");
				}
				var args = __m_Utils.argumentsAsArray(arguments);
				if(noOfArgs === 1) {
					args.push(Contacts.TYPE_WORK);
				}
				return this.addEmail.apply(this, args);
			};

			// Adds the provided IM detail (ContactField) to contact
			this.addIm = function(value, type) {
				__m_Utils.checkRequired("type", type, "string");
				__m_Utils.checkRequired("IM", value, "string");
				var im = new Contacts.ContactField(type, value);
				this.impps.push(im);
				return this;
			};
			
			this.setIm = function() {
				var noOfArgs = arguments.length;
				if(noOfArgs === 0) {
					throw new TypeError("The parameter im is required.");
				}
				var args = __m_Utils.argumentsAsArray(arguments);
				if(noOfArgs === 1) {
					args.push(Contacts.IM_SERVICE_JABBER);
				}
				return this.addIm.apply(this, args);
			};

			// Adds the provided phone (ContactField) to contact
			this.addPhone = function(value, type) {
				__m_Utils.checkRequired("type", type, "string");
				__m_Utils.checkRequired("phone", value, ["string", "number"]);
				value = "" + value;
				var phone = new Contacts.ContactField(type, value);
				this.phones.push(phone);
				return this;
			};
			
			this.setPhone = function() {
				var noOfArgs = arguments.length;
				if(noOfArgs === 0) {
					throw new TypeError("The parameter phone is required.");
				}
				var args = __m_Utils.argumentsAsArray(arguments);
				if(noOfArgs === 1) {
					args.push(Contacts.TYPE_WORK);
				}
				return this.addPhone.apply(this, args);
			};

			// Adds the provided photo (ContactField) to contact
			this.addPhoto = function(value, type) {
				__m_Utils.checkRequired("photo", value, ["string", mowbly.File]);
				if(typeof value === "string") {
					value = new mowbly.File(value);
				}
				var photo = new Contacts.ContactField(type, value);
				this.photos.push(photo);
				return this;
			};

			this.setPhoto = function() {
				var args = __m_Utils.argumentsAsArray(arguments);
				args.push(Contacts.TYPE_OTHER);
				return this.addPhoto.apply(this, args);
			};

			// Adds the provided url (ContactField) to contact
			this.addUrl = function(value, type) {
				__m_Utils.checkRequired("type", type, "string");
				__m_Utils.checkRequired("url", value, "string");
				var url = new Contacts.ContactField(type, value);
				this.urls.push(url);
				return this;
			};
			
			this.setUrl = function() {
				var noOfArgs = arguments.length;
				if(noOfArgs === 0) {
					throw new TypeError("The parameter url is required.");
				}
				var args = __m_Utils.argumentsAsArray(arguments);
				if(noOfArgs === 1) {
					args.push(Contacts.TYPE_WORK);
				}
				return this.addUrl.apply(this, args);
			};
			
			this.setBirthday = function(birthday) {
				__m_Utils.checkRequired("birthday", birthday);
				if(typeof birthday == "number") {
					this.birthday = birthday;
				} else {
					// convert the day to millisecs from Jan 1, 1970
					var bdatems = Date.parse(birthday);
					if(! isNaN(bdatems)) {
						this.birthday = bdatems;
					} else {
						throw new TypeError("Birthday is not valid.");
					}
				}
				return this;
			}
			
			// Sets the provided Category (String) to contact
			this.setCategory = function(category) {
				__m_Utils.checkRequired("category", category, "string");
				this.category = category;
				return this;
			};

			// Sets the provided ContactName to the contact
			this.setName = function(firstName, lastName, middleName, prefix, suffix) {
				__m_Utils.checkRequired("name", firstName, "string");
				var name = new Contacts.ContactName(firstName, lastName, middleName, prefix, suffix);
				this.name = name;
				return this;
			};
			
			this.setNickname = function(nickname) {
				if(arguments.length === 0) {
					throw new TypeError("Nickname cannot be empty.");
				}
				this.nickname = nickname;
				return this;
			};
			
			this.setNote = function(note) {
				if(arguments.length === 0) {
					throw new TypeError("Note cannot be empty.");
				}
				this.note = note;
				return this;
			};
			
			// sets the provided Organization (ContactOrganization) to contact
			this.setOrganization = function(name, department, jobTitle) {
				if(arguments.length === 0) {
					throw new TypeError("Organization cannot be empty.");
				}
				var org = new Contacts.ContactOrganization(name, department, jobTitle);
				this.organization = org;
				return this;
			};
			
			// Delete properties
			this.removeFieldByType = function(type, arr) {
				__m_Utils.checkRequired("type", type, "string");
				
				var value;
				var cnt = arr.length, index = -1;
				for(var i=0; i<cnt; i++) {
					if(arr[i].type == type) {
						index = i;
						break;
					}
				}
				
				if(index > -1) {
					arr.splice(index, 1);
				}
			}
			
			this.removeAddress = function(type) {
				this.removeFieldByType(type, this.addresses);
				return this;
			};
			
			this.removeHomeAddress = function() {
				this.removeAddress(Contacts.TYPE_HOME);
				return this;
			};
			
			this.removeWorkAddress = function() {
				this.removeAddress(Contacts.TYPE_WORK);
				return this;
			};
			
			this.removeOtherAddress = function() {
				this.removeAddress(Contacts.TYPE_OTHER);
				return this;
			};
			
			this.removeAddresses = function() {
				delete this.addresses;
				return this;
			};
			
			this.removeBirthday = function() {
				delete this.birthday;
				return this;
			};

			this.removeCategory = function() {
				delete this.category;
				return this;
			};

			this.removeEmail = function(type) {
				this.removeFieldByType(type, this.emails);
				return this;
			};
			
			this.removeHomeEmail = function() {
				this.removeEmail(Contacts.TYPE_HOME);
				return this;
			};
			
			this.removeWorkEmail = function() {
				this.removeEmail(Contacts.TYPE_WORK);
				return this;
			};
			
			this.removeOtherEmail = function() {
				this.removeEmail(Contacts.TYPE_OTHER);
				return this;
			};

			this.removeEmails = function() {
				delete this.emails;
				return this;
			};

			this.removeIm = function(type) {
				this.removeFieldByType(type, this.m_impps);
				return this;
			};
			
			this.removeAimIm = function() {
				this.removeIm(Contacts.IM_SERVICE_AIM);
				return this;
			};
			
			this.removeJabberIm = function() {
				this.removeIm(Contacts.IM_SERVICE_JABBER);
				return this;
			};
			
			this.removeIcqIm = function() {
				this.removeIm(Contacts.IM_SERVICE_ICQ);
				return this;
			};
			
			this.removeYahooIm = function() {
				this.removeIm(Contacts.IM_SERVICE_YAHOO);
				return this;
			};

			this.removeIms = function() {
				delete this.impps;
				return this;
			};

			this.removeName = function() {
				delete this.name;
				return this;
			};

			this.removeNickname = function() {
				delete this.nickname;
				return this;
			};

			this.removeNote = function() {
				delete this.note;
				return this;
			};

			this.removeOrganization = function() {
				delete this.organization;
				return this;
			};

			this.removePhone = function(type) {
				this.removeFieldByType(type, this.phones);
			};

			this.removePhones = function() {
				delete this.phones;
				return this;
			};

			this.removePhoto = function(type) {
				this.removeFieldByType(type, this.photos);
				return this;
			};

			this.removePhotos = function() {
				delete this.photos;
				return this;
			};

			this.removeUrl = function(type) {
				this.removeFieldByType(type, this.urls);
				return this;
			};
			
			this.removeHomeUrl = function() {
				this.removeUrl(Contacts.TYPE_HOME);
				return this;
			};
			
			this.removeWorkUrl = function() {
				this.removeUrl(Contacts.TYPE_WORK);
				return this;
			};
			
			this.removeOtherUrl = function() {
				this.removeUrl(Contacts.TYPE_OTHER);
				return this;
			};

			this.removeUrls = function() {
				// TODO Fix!
				return this.urls;
				return this;
			};
			
			this.remove = function(fp_callback) {
				if(Bridge.getType() !== Bridge.WINDOWSPHONE) {
					if(typeof this.id === "undefined") {
						throw new TypeError("Invalid contact.");
					}
				}
				Bridge.invoke(ContactsFeatureName, "deleteContact", [this.id], fp_callback);
			};
		
			this.save = function(fp_callback) {
				var that = this;
				var fp_onContactSaved = function(response) {
					if(response.code) {
						// Update the ID of the contact
						that.id = response.result;
					}
					response.result = that;
					fp_callback(response);
				}
				Bridge.invoke(ContactsFeatureName, "saveContact", [this], fp_onContactSaved);
			};
			
			this.view = function() {
				var param;
				if(Bridge.getType() !== Bridge.WINDOWSPHONE) {
					param = this.id;
					if(typeof this.id === "undefined") {
						throw new TypeError("Could not view contact. Unique id is missing. Save the contact to get unique id.");
					}
				} else {
					param = this;
				}
				Bridge.invoke(ContactsFeatureName, "viewContact", [param]);
			};

			// Set the properties specified
			if(name) {
				if(typeof name === "string") {
					this.setName(name);
				} else if(	typeof name === "object" && 
							name instanceof Contacts.ContactName &&
							name.firstName) {
					this.name = name;
				}
			}

			if(phone) {
				if(typeof phone === "string" || typeof phone === "number") {
					this.setPhone(phone);
				} else if(__m_Utils.isArray(phone)) {
					this.phones = phone;
				}
			}

			if(email) {
				if(typeof(email) === "string") {
					this.setEmail(email);
				} else if(__m_Utils.isArray(email)) {
					this.emails = email;
				}
			}
		},

		ContactName : function(firstName, lastName, middleName, prefix, suffix) {
			this.firstName = firstName;
			this.lastName = lastName;
			this.middleName = middleName;
			this.prefix = prefix;
			this.suffix = suffix;
			
			this.search = function(text){
				return __m_Utils.search(text,this);
			}
		},

		ContactAddress : function(type, address) {
			this.type = type;
			this.street = address.street;
			this.city = address.city;
			this.region = address.region;
			this.postalCode = address.postalCode;
			this.country = address.country;
			this.additionalInfo = address.additionalInfo;
			
			this.search = function(text){
				return __m_Utils.search(text,this);
			}
		},

		ContactField : function(type, value) {
			this.type = type;
			this.value = value;
			
			this.search = function(text){
				return __m_Utils.search(text,this);
			}
		},

		ContactOrganization : function(name, department, jobTitle) {
			this.name = name;
			this.department = department;
			this.jobTitle = jobTitle;
			
			this.search = function(text){
				return __m_Utils.search(text,this);
			}
		},
		
		ContactOptions : function(properties, limit) {
			if(typeof limit == "undefined") {
				limit = 0;	// Get all records
			}
			this.limit = limit;
			// properties - ["firstName", "phoneNumber"];
			this.properties = properties;
		},

		ContactError : {
			UNKNOWN_ERR : 0,
			INVALID_ARGUMENT_ERROR : 1,
			CONTACT_NOT_FOUND_ERROR : 30,
			CONTACT_INVALID_ERROR : 31
		},

		// Methods
		create : function(properties, phone, email) {
			if(typeof properties === "string" || typeof properties === "undefined") {
				var name = properties;
				return new this.Contact(name, phone, email);
			} else if(typeof properties === "object") {
				var createFieldByType = function(field, property) {
					var retField = {};
					if(property == Contacts.CONTACT_ADDRESSES || property == Contacts.CONTACT_IMPPS) {
						for(var i in field) {
							retField[field[i].type] = field[i];
						}
					} else {
						for(var i in field) {
							retField[field[i].type] = field[i].value;
						}
					}
					return retField;
				};
			
			
				// Create contact from contact properties object				
				var contact = this.create();
				// set contact properties
				// Id
				contact.id = properties.id;
				
				// Address - Home, Work, Other
				if(properties.addresses && properties.addresses.length > 0) {
					var addresses = createFieldByType(properties.addresses, Contacts.CONTACT_ADDRESSES);
					if(addresses) {
						var hAddr = addresses[this.TYPE_HOME];
						if(hAddr){
							contact.addAddress({"type" : hAddr.type,
												"street" : hAddr.street,
												"city" : hAddr.city,
												"region" : hAddr.region,
												"postalCode" : hAddr.postalCode,
												"country" : hAddr.country,
												"additionalInfo" : hAddr.additionalInfo}, hAddr.type);
							}
						var wAddr = addresses[this.TYPE_WORK];
						if(wAddr){
							contact.addAddress({"type" : wAddr.type,
												"street" : wAddr.street,
												"city" : wAddr.city,
												"region" : wAddr.region,
												"postalCode" : wAddr.postalCode,
												"country" : wAddr.country,
												"additionalInfo" : wAddr.additionalInfo}, wAddr.type);
						}
						var oAddr = addresses[this.TYPE_OTHER];
						if(oAddr){
							contact.addAddress({"type" : oAddr.type,
												"street" : oAddr.street,
												"city" : oAddr.city,
												"region" : oAddr.region,
												"postalCode" : oAddr.postalCode,
												"country" : oAddr.country,
												"additionalInfo" : oAddr.additionalInfo}, oAddr.type);
						}
					}
				}

				// Birthday
				if(properties.birthday) {
					contact.setBirthday(properties.birthday);
				}

				// Email - Home, Work, Other
				if(properties.emails) {
					var emails = createFieldByType(properties.emails);
					if(emails) {
						if(emails[this.TYPE_HOME]) contact.setEmail(emails[this.TYPE_HOME], this.TYPE_HOME);
						if(emails[this.TYPE_WORK]) contact.setEmail(emails[this.TYPE_WORK], this.TYPE_WORK);
						if(emails[this.TYPE_OTHER]) contact.setEmail(emails[this.TYPE_OTHER], this.TYPE_OTHER);
					}
				}

				// Impps - Aim, Icq, Jabber, Yahoo
				if(properties.impps && properties.impps.length > 0) {
					var im = createFieldByType(properties.impps, Contacts.CONTACT_IMPPS);
					if(im) {
						if(im[this.IM_SERVICE_AIM]) contact.setIm(im[this.IM_SERVICE_AIM].value, this.IM_SERVICE_AIM);
						if(im[this.IM_SERVICE_ICQ]) contact.setIm(im[this.IM_SERVICE_ICQ].value, this.IM_SERVICE_ICQ);
						if(im[this.IM_SERVICE_JABBER]) contact.setIm(im[this.IM_SERVICE_JABBER].value, this.IM_SERVICE_JABBER);
						if(im[this.IM_SERVICE_YAHOO]) contact.setIm(im[this.IM_SERVICE_YAHOO].value, this.IM_SERVICE_YAHOO);
					}
				}

				// Name
				var name = properties.name;
				if(name) {
					contact.setName(name.firstName,
									name.lastName,
									name.middleName,
									name.prefix,
									name.suffix);
				}

				// Nickname
				if(properties.nickname) {
					contact.setNickname(properties.nickname);
				}

				// Note
				if(properties.note) {
					contact.setNote(properties.note);
				}

				// Category
				if(properties.category) {
					contact.setCategory(properties.category);					
				}

				// Organizations
				var org = properties.organization;
				if(org) {
					contact.setOrganization(org[this.CONTACT_NAME],
											org[this.CONTACT_DEPARTMENT],
											org[this.CONTACT_JOB_TITLE]);
				}

				// Phones
				if(properties.phones) {
					var phones = createFieldByType(properties.phones);
					if(phones) {
						if(phones[this.TYPE_HOME]) contact.setPhone(phones[this.TYPE_HOME], this.TYPE_HOME);
						if(phones[this.TYPE_WORK]) contact.setPhone(phones[this.TYPE_WORK], this.TYPE_WORK);
						if(phones[this.TYPE_MOBILE]) contact.setPhone(phones[this.TYPE_MOBILE], this.TYPE_MOBILE);
						if(phones[this.TYPE_PAGER]) contact.setPhone(phones[this.TYPE_PAGER], this.TYPE_PAGER);
						if(phones[this.TYPE_FAX]) contact.setPhone(phones[this.TYPE_FAX], this.TYPE_FAX);
						if(phones[this.TYPE_OTHER]) contact.setPhone(phones[this.TYPE_OTHER], this.TYPE_OTHER);
					}
				}

				// Photos
				var photos = properties.photos;
				if(photos) {
					var count = photos.length;
					for(var i=0; i<count; i++) {
						var data = photos[i].value;
						if(data && data.value != "") {
							contact.addPhoto(photos[i].type || this.TYPE_WORK, data);
						}
					}
				}

				// Urls - Home, Work, Other
				if(properties.urls) {
					var urls = createFieldByType(properties.urls);
					if(urls) {
						if(urls[this.TYPE_HOME]) contact.setUrl(urls[this.TYPE_HOME], this.TYPE_HOME);
						if(urls[this.TYPE_WORK]) contact.setUrl(urls[this.TYPE_WORK], this.TYPE_WORK);
						if(urls[this.TYPE_OTHER]) contact.setUrl(urls[this.TYPE_OTHER], this.TYPE_OTHER);
					}
				}

				return contact;
			}
		},

		call : function(phoneNumber, bForce) {
			__m_Utils.checkRequired("phoneNumber", phoneNumber, ["string", "number", mowbly.Contacts.Contact]);
			if(phoneNumber instanceof mowbly.Contacts.Contact) {
				phoneNumber = phoneNumber.getPhone();
				__m_Utils.checkRequired("phoneNumber", phoneNumber, ["string", "number"]);
			}
			// Forcing is not there in spec - http://tools.ietf.org/html/rfc3966 Search for word Consent
			if(!bForce){
				mowbly.Ui.confirm({
						"message": "Call " + phoneNumber, 
						"buttons": [{"label": "Call"},
								{"label": "Cancel"}]
					}, function(index){
						if(index == 0) {
							Bridge.invoke(ContactsFeatureName, "callContact", ["" + phoneNumber]);
						}
					}
				);
			}else{
				Bridge.invoke(ContactsFeatureName, "callContact", ["" + phoneNumber]);
			}
		},
		
		find : function(filter) {
			var options = new Contacts.ContactOptions([], 0), fp_callback, noOfArgs = arguments.length;
			__m_Utils.checkRequired("filter", filter, "string");
		
			if(noOfArgs > 1) {
				if(noOfArgs == 2) {
					arg1 = arguments[1];
					__m_Utils.checkType("second argument", arg1, ["object", "function"]);
					if(typeof arg1 === "function") {
						fp_callback = arg1;
					} else {
						options = arg1;
					}
				} else {
					options = arguments[1];
					__m_Utils.checkType("options", options, "object");
					fp_callback = arguments[2];
					__m_Utils.checkType("fp_callback", fp_callback, "function");
				}
			}

			var that = this;
			var fp_onContactFound = function(response) {
				if(response.code) {
					var contacts = [];
					// Create contact objects
					for(var i in response.result) {
						var contactInfo = response.result[i];
						contacts.push(that.create(contactInfo));
					}
					// Replace the result with contacts
					response.result = contacts;
				}
				if(typeof fp_callback === "function") {
					fp_callback(response);
				}
			}
			
			Bridge.invoke(ContactsFeatureName, "findContact", [filter, options], fp_onContactFound);
		},

		pick : function() {
			var options = {}, fp_callback, noOfArgs = arguments.length;
			if(noOfArgs == 1) {
				var arg0 = arguments[0];
				__m_Utils.checkType("first argument", arg0, ["object", "function"]);
				
				if(typeof arg0 === "object") {
					options = arg0;
				} else {
					fp_callback = arg0;
				}
			} else if(noOfArgs > 1) {
				options = arguments[0];
				fp_callback = arguments[1];
				__m_Utils.checkType("options", options, "object");
				__m_Utils.checkType("fp_callback", fp_callback, "function");
			}
		
			var defaultOptions = {
				"filter": [],
				"multiple": false,
				"bChooseProperty": false,
				"bPerformDefaultAction": false
			}

			var opts = __m_Utils.extendOptions({}, defaultOptions, options);

			var that = this;
			var fp_onContactPicked = function(response) {
				if(response.code) {
					var contacts = [];
					if(__m_Utils.isArray(response.result)) {
						// Create contact objects
						var numContacts = response.result.length;
						for(var i = 0; i < numContacts; i++) {
							var contactInfo = response.result[i];
							contacts.push(that.create(contactInfo));
						}
					} else {
						// Single object picked
						contacts.push(that.create(response.result));
					}
					// Replace the result with contacts
					response.result = contacts;
				}
				if(typeof fp_callback === "function") {
					fp_callback(response);
				}
			}

			Bridge.invoke(ContactsFeatureName, "pickContact", [opts.filter, opts.multiple, opts.bChooseProperty, opts.bPerformDefaultAction], fp_onContactPicked);
		}
	};
	
	window.mowbly.contact = function(name, phone, email) {
		var contact = __mowbly__.Contacts.create(name, phone, email);
		return contact;
	};
	
	// Add event library to features.
	__m_Utils.inherit(Contacts, Observable);
	
	mowbly.addFeature(null, "Address", Address);
	mowbly.addFeature(ContactsFeatureName, "Contacts", Contacts);
	
	// Contacts
	mowbly.exportHelperMethod("callContact", "call", "Contacts");
	mowbly.exportHelperMethod("findContact", "find", "Contacts");
	mowbly.exportHelperMethod("pickContact", "pick", "Contacts");
})(window);

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

(function(window){
	var mowbly = window.__mowbly__;
	var Bridge = mowbly.Bridge;

	/* File Feature */
	var FileFeatureName = "file";
	function FileManager(options) {
		if(options) {
			if(!options.storageType) {
				options.storageType = File.FILE_INTERNAL_STORAGE;
			}
			if(!options.level) {
				options.level = File.DefaultLevel;
			}
		} else {
			options = {storageType:File.FILE_INTERNAL_STORAGE, level:File.DefaultLevel};
		}
		this.__options = options;
	}

	// Returns the root storage directory of the page
	FileManager.prototype.getRootDirectory = function(fp_handler) {
		Bridge.invoke(FileFeatureName, "getRootDirectory", ["", this.__options], fp_handler);
	}

	FileManager.prototype.testDirExists = function(dirPath, fp_handler) {
		Bridge.invoke(FileFeatureName, "testDirExists", [dirPath, this.__options], fp_handler);
	}

	FileManager.prototype.testFileExists = function(filePath, fp_handler) {
		Bridge.invoke(FileFeatureName, "testFileExists", [filePath, this.__options], fp_handler);
	}

	FileManager.prototype.getFiles = function(dirPath, fp_handler) {
		Bridge.invoke(FileFeatureName, "getFilesJSONString", [dirPath, this.__options], fp_handler);
	}

	FileManager.prototype.getDirectory = function(dirPath, fp_handler) {
		Bridge.invoke(FileFeatureName, "getDirectory", [dirPath, this.__options], fp_handler);
	}

	FileManager.prototype.getFile = function(filePath, fp_handler) {
		Bridge.invoke(FileFeatureName, "getFile", [filePath, this.__options], fp_handler);
	}
	
	FileManager.prototype.deleteDirectory = function(dirPath, fp_handler) {
		Bridge.invoke(FileFeatureName, "deleteDirectory", [dirPath, this.__options], fp_handler);
	}
	
	FileManager.prototype.deleteFile = function(filePath, fp_handler) {
		Bridge.invoke(FileFeatureName, "deleteFile", [filePath, this.__options], fp_handler);
	}
	
	FileManager.prototype.readFile = function(filePath, fp_callback) {
		Bridge.invoke(FileFeatureName, "read", [filePath, this.__options], fp_callback);
	}
	
	FileManager.prototype.writeFile = function(filePath, content, mode, fp_callback) {
		Bridge.invoke(FileFeatureName, "write", [filePath, content, mode, this.__options], fp_callback);
	}
	/**
		Helper method to provide the FileManager object based on storageType & level
		
		Parameters:
			options - Optional. Object containing storage type and level. Defaults to Internal Storage at Page Level
		Returns:
			undefined
	**/
	FileManager.getFileManager = function(options) {
		if(options) {
			if(!options.storageType) {
				options.storageType = File.FILE_INTERNAL_STORAGE;
			}
			if(!options.level) {
				options.level = File.DefaultLevel;
			}
		} else {
			options = {storageType:File.FILE_INTERNAL_STORAGE, level:File.DefaultLevel};
		}

		if(!this._fileManagers) {
			this._fileManagers = {};
		}
		var key = options.storageType + ":" + options.level;
		return this._fileManagers[key] || 
				(this._fileManagers[key] = new FileManager(options));
	}
	
	// Directory
	function Directory(path, options) {
		this.path = path;
		this.__FileManager = FileManager.getFileManager(options);
	}
	
	Directory.root = function() {
		var path = this.__FileManager.getRootDirectory();
		return new Directory(path);
	}
	
	Directory.prototype.getPath = function(fp_handler) {
		var me = this;
		var fp_callback = function(response) {
			var rootDir = response.result;
			if(rootDir) {
				rootDir += "/" + me.path;
				response.result = rootDir;
			}
			fp_handler(response);
		}
	
		this.__FileManager.getRootDirectory(fp_callback);
	}
	
	Directory.prototype.create = function(fp_handler) {
		this.__FileManager.getDirectory(this.path, fp_handler);
	}
	
	Directory.prototype.exists = function(fp_handler) {
		this.__FileManager.testDirExists(this.path, fp_handler);
	}
	
	Directory.prototype.getDirectory = function(dirPath, options) {
		return new Directory(this.path + "/" + dirPath, options);
	}
	
	Directory.prototype.getFile = function(filePath, options) {
		return new File(this.path + "/" + filePath, options);
	}
	
	Directory.prototype.getFiles = function(fp_handler) {
		this.__FileManager.getFiles(this.path, fp_handler);
	}
	
	Directory.prototype.remove = function(fp_handler) {
		this.__FileManager.deleteDirectory(this.path, fp_handler);
	}
	
	// File
	function File() {
		var path, options, noOfArgs = arguments.length;
		if(noOfArgs < 1) {
			throw new TypeError("The argument path is required");
		}
		if(noOfArgs == 1) {
			path = arguments[0];
		} else {
			path = arguments[0];
			options = arguments[1];
			__m_Utils.checkType("options", options, "object");
		}

		__m_Utils.checkRequired("path", path, "string");

		// Don't accept empty string
		if(! path) {
			throw new TypeError("The argument path cannot be empty string");
		}

		this.path = path;
		this.__FileManager = FileManager.getFileManager(options);

		// Path beginning with "/" corresponds to Storage level, override after creating options
		if(path.indexOf("/") == 0) {
			this.__FileManager.__options.level = File.STORAGE;
		}
	}
	
	// Constants
	// File Type
	File.FILE 		= 0;
	File.DIRECTORY	= 1;
	
	File.FILE_MODE_APPEND = true;

	// File root levels
	File.APPLICATION = 1;
	File.STORAGE = 2;
	
	File.DefaultLevel = File.APPLICATION;

	// Storage types
	File.FILE_INTERNAL_STORAGE = 0;
	File.FILE_EXTERNAL_STORAGE = 1;
	File.FILE_CACHE = 2;
	
	File.prototype.toJSON = function() {
		var o = {};
		o.path = this.path;
		o.storageType = this.__FileManager.__options.storageType;
		o.level = this.__FileManager.__options.level;
		
		return o;
	};
	
	File.prototype.getPath = function(fp_handler) {
		var me = this;
		var fp_callback = function(response) {
			var rootDir = response.result;
			if(rootDir) {
				rootDir += "/" + me.path;
				response.result = rootDir;
			}
			fp_handler(response);
		};

		this.__FileManager.getRootDirectory(fp_callback);
	};
	
	File.prototype.create = function(fp_handler) {
		this.__FileManager.getFile(this.path, fp_handler);
	};
	
	File.prototype.exists = function(fp_handler) {
		this.__FileManager.testFileExists(this.path, fp_handler);
	};
	
	File.prototype.read = function(fp_handler) {
		this.__FileManager.readFile(this.path, fp_handler);
	};

	File.prototype.write = function(data, fp_handler, bAppend) {
		if(typeof data === "object") {
			data = JSON.stringify(data);
		}
		this.__FileManager.writeFile(this.path, data, !!bAppend, fp_handler);
	};

	File.prototype.remove = function(fp_handler) {
		this.__FileManager.deleteFile(this.path, fp_handler);
	};
	
	function getFileParams(args) {
		var file, path, fp_callback, noOfArgs = args.length;
		if(noOfArgs < 1) {
			throw new TypeError("The argument file object or path is required");
		}
		if(noOfArgs == 1) {
			path = args[0];
		} else {
			path = args[0];
			fp_callback = args[1];
		}

		__m_Utils.checkRequired("path", path, ["string", File]);

		if(typeof path === "string") {
			file = new File(path);
		} else if(path instanceof File) {
			file = path;
		} else {
			throw new TypeError("The argument file can be either string or File object");
		}

		return [file, fp_callback];
	}

	window.mowbly.deleteFile = function() {
		var p = getFileParams(arguments), file = p[0], fp_callback = p[1];
		file.remove(fp_callback);
	};

	window.mowbly.fileExists = function() {
		var p = getFileParams(arguments), file = p[0], fp_callback = p[1];
		file.exists(fp_callback);
	};
	
	window.mowbly.getFilePath = function() {
		var p = getFileParams(arguments), file = p[0], fp_callback = p[1];
		file.getPath(fp_callback);
	};
	
	window.mowbly.readFile = function() {
		var p = getFileParams(arguments), file = p[0], fp_callback = p[1];
		file.read(fp_callback);
	};
	
	window.mowbly.file = function(){
		var path, options, fp_callback, noOfArgs = arguments.length;
		if(noOfArgs < 1) {
			throw new TypeError("The argument path is required");
		}
		if(noOfArgs == 1) {
			path = arguments[0];
			options = {};
		} else {
			path = arguments[0];
			options = arguments[1];
		}

		__m_Utils.checkRequired("path", path, "string");
		
		return new File(path, options);
	};
	
	window.mowbly.directory = function(){
		var path, options, fp_callback, noOfArgs = arguments.length;
		if(noOfArgs < 1) {
			throw new TypeError("The argument path is required");
		}
		if(noOfArgs == 1) {
			path = arguments[0];
			options = {};
		} else {
			path = arguments[0];
			options = arguments[1];
		}

		__m_Utils.checkRequired("path", path, "string");
		
		return new Directory(path, options);
	};
	
	window.mowbly.writeFile = function() {
		var file, path, content, fp_callback, noOfArgs = arguments.length;
		if(noOfArgs < 2) {
			throw new TypeError("The argument file object or path and content is required");
		}
		if(noOfArgs == 2) {
			path = arguments[0];
			content = arguments[1];
		} else {
			path = arguments[0];
			content = arguments[1];
			fp_callback = arguments[2];
		}

		__m_Utils.checkRequired("path", path, ["string", File]);
		__m_Utils.checkRequired("content", content, ["string", "object"]);
		
		if(typeof path === "string") {
			file = new File(path);
		} else {
			file = path;
		}

		file.write(content, fp_callback);
	};
	
	window.mowbly.appendFile = function() {
		var file, path, content, fp_callback, noOfArgs = arguments.length;
		if(noOfArgs < 2) {
			throw new TypeError("The argument file object or path and content is required");
		}
		if(noOfArgs == 2) {
			path = arguments[0];
			content = arguments[1];
		} else {
			path = arguments[0];
			content = arguments[1];
			fp_callback = arguments[2];
		}

		__m_Utils.checkRequired("path", path, ["string", File]);
		__m_Utils.checkRequired("content", content, ["string", "object"]);
		
		if(typeof path === "string") {
			file = new File(path);
		} else {
			file = path;
		}

		file.write(content, fp_callback, true);
	};

	window.mowbly.unzipFile = function() {
		var srcFile, destDir, fp_callback, noOfArgs = arguments.length;
		if(noOfArgs < 2) {
			throw new TypeError("The zip file and destination directory arguments are required");
		}
		srcFile = arguments[0];
		destDir = arguments[1];
		if(noOfArgs === 3) {
			fp_callback = arguments[2];
		}

		if(typeof srcFile === "string") {
			srcFile = new File(srcFile);
		}
		if(typeof destDir === "string") {
			destDir = new File(destDir);
		}
		Bridge.invoke(FileFeatureName, "unzip", [srcFile.toJSON(), destDir.toJSON()], fp_callback);
	};
	
	// Add event library to features.
	__m_Utils.inherit(File, Observable);
	
	mowbly.addFeature(FileFeatureName, "File", File);
	mowbly.addFeature(null, "Directory", Directory);
	
	// File
	mowbly.exportHelperProperty("FILE", "FILE", "File");
	mowbly.exportHelperProperty("DIR", "DIRECTORY", "File");
	mowbly.exportHelperProperty("FILE_APPEND", "FILE_MODE_APPEND", "File");
	mowbly.exportHelperProperty("APP_LEVEL", "APPLICATION", "File");
	mowbly.exportHelperProperty("STORAGE_LEVEL", "STORAGE", "File");
	mowbly.exportHelperProperty("DEVICE_MEMORY", "FILE_INTERNAL_STORAGE", "File");
	mowbly.exportHelperProperty("SDCARD", "FILE_EXTERNAL_STORAGE", "File");
	mowbly.exportHelperProperty("CACHE_DIR", "FILE_CACHE", "File");
})(window);

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

(function(window){
	var mowbly = window.__mowbly__;
	var Bridge = mowbly.Bridge;

	/* Http Feature */
	var HttpFeatureName = "http";
	var Http = {};
	function HttpRequest(options) {
		this.options = {
			url: "",
			type: "GET",
			data: {},
			dataType: "json",
			parts: [],
			headers: {},
			timeout: 30000,
			withCredentials: false,
			replaceInBody: false,
			replaceInHeaders: [],
			replaceInUrl: false,
			authMode : 0
		};
	
		var that = this;
		var parseOptions = function(options) {
			__m_Utils.checkRequired("options", options, "object");
			__m_Utils.checkRequired("options.url", options.url, "string");

			options.timeout = options.timeout || 30000;
			__m_Utils.extendOptions(that.options, options);			

			if(! that.options.type) {
				that.options.type = "GET";
			}
			if(!that.options.headers){
				that.options.headers = {};
			}
			that.options.type = that.options.type.toUpperCase();
		};

		parseOptions(options);
	};
	HttpRequest.GET = "GET";
	HttpRequest.POST = "POST";
	HttpRequest.PUT = "PUT";
	HttpRequest.DELETE = "DELETE";
	HttpRequest.HEAD = "HEAD";
	HttpRequest.prototype.setType = function(type) {
		__m_Utils.checkRequired("type", type, "string");
		type = type.toUpperCase();
		if(type != "GET" && type != "POST" && type != "PUT" && type != "DELETE" && type != "HEAD") {
			throw new TypeError("The argument type should be GET, POST, PUT, DELETE or HEAD");
		}
		this.options.type = type;
		return this;
	}
	HttpRequest.prototype.setTimeout = function(timeout) {
		__m_Utils.checkRequired("timeout", timeout, "number");
		this.options.timeout = timeout;
		return this;
	}
	HttpRequest.prototype.addHeader = function(name, value) {
		__m_Utils.checkRequired("name", name, "string");
		__m_Utils.checkRequired("value", value, "string");
		this.options.headers[name] = value;
		return this;
	};
	HttpRequest.prototype.setData = function(data) {
		__m_Utils.checkRequired("data", data, ["string", "object"]);
		var dataType = typeof data;
		this.options.data = data;
		this.options.dataType = (dataType == "object") ? "json" : dataType;
		return this;
	};
	HttpRequest.prototype.addPart = function(name, content, contentType, fileName) {
		__m_Utils.checkRequired("name", name, "string");
		__m_Utils.checkRequired("content", content);
		if(!(content instanceof mowbly.File) && !(typeof content == "string")) {
			throw new TypeError("The argument content has to be either a string or a mowbly.File object");
		}
		contentType = contentType || "application/octet-stream";
		var part;
		if(content instanceof mowbly.File) {
			part = {type: "file", name: name, value: content.toJSON(), contentType: contentType};
			if(typeof fileName == "string") {
				part.filename = fileName;
			}
		} else {
			part = {type: "string", name: name, value: content, contentType: contentType};
		}
		this.options.parts.push(part);
		return this;
	};
	HttpRequest.prototype.setDownloadFile = function(file) {
		if(!(file instanceof mowbly.File)) {
			throw new TypeError("The argument file has to a mowbly.File object");
		}
		this.options.downloadFile = file.toJSON();
		return this;
	};
	HttpRequest.prototype.send = function(fp_callback) {
		Bridge.invoke(HttpFeatureName, "request", [this.options], fp_callback);
	};

	Http.Request = HttpRequest;
	Http.get = function() {
		var url, options = {}, fp_callback, noOfArgs = arguments.length;
		if(noOfArgs == 0) {
			throw new TypeError("The argument url is required.");
		}
		if(noOfArgs == 1) {
			url = arguments[0];
		} else if(noOfArgs == 2) {
			url = arguments[0];
			fp_callback = arguments[1];
		} else {
			url = arguments[0];
			options = arguments[1];
			fp_callback = arguments[2];
		}
		__m_Utils.checkRequired("url", url, "string");
		options = options || {};
		options.url = url;
		options.type = HttpRequest.GET;
		new HttpRequest(options).send(fp_callback);
	};
	Http.post = function() {
		var url, data = {}, options = {}, fp_callback, noOfArgs = arguments.length;
		if(noOfArgs == 0) {
			throw new TypeError("The argument url is required.");
		}
		if(noOfArgs == 1) {
			url = arguments[0];
		} else if(noOfArgs == 2) {
			url = arguments[0];
			data = arguments[1];
		} else if(noOfArgs == 3) {
			url = arguments[0];
			data = arguments[1];
			fp_callback = arguments[2];
		} else {
			url = arguments[0];
			data = arguments[1];
			options = arguments[2];
			fp_callback = arguments[3];
		}
		__m_Utils.checkRequired("url", url, "string");
		__m_Utils.checkRequired("data", data);
		options = options || {};
		options.url = url;
		new HttpRequest(options).setType(HttpRequest.POST).setData(data).send(fp_callback);
	};
	Http.download = function() {
		var url, file, options = {}, fp_callback, noOfArgs = arguments.length;
		if(noOfArgs == 0) {
			throw new TypeError("The arguments url and file are required");
		} else if(noOfArgs == 1) {
			throw new TypeError("The argument file is required");
		} else if(noOfArgs == 2) {
			url = arguments[0];
			file = arguments[1];
		} else if(noOfArgs == 3) {
			url = arguments[0];
			file = arguments[1];
			fp_callback = arguments[2];
		} else {
			url = arguments[0];
			file = arguments[1];
			options = arguments[2];
			fp_callback = arguments[3];
		}
		__m_Utils.checkRequired("url", url, "string");
		__m_Utils.checkRequired("file", file, ["string", mowbly.File]);
		if(typeof file === "string") {
			file = new mowbly.File(file);
		}
		options = options || {};
		options.url = url;
		new HttpRequest(options).setType(HttpRequest.GET).setDownloadFile(file).send(fp_callback);
	};
	
	$m.httpRequest = function(options){
		return new Http.Request(options);
	};
	
	Http.Basic = 1;
	Http.NTLM = 2;
	
	mowbly.addFeature(null, "HttpRequest", HttpRequest);
	mowbly.addFeature(HttpFeatureName, "Http", Http);
	
	mowbly.exportHelperMethod("get", "get", "Http");
	mowbly.exportHelperMethod("post", "post", "Http");
	mowbly.exportHelperMethod("download", "download", "Http");
	
	mowbly.exportHelperProperty("BasicAuth", "Basic", "Http");
	mowbly.exportHelperProperty("NTLMAuth", "NTLM", "Http");

})(window);

(function(window){
	var mowbly = window.__mowbly__;
	var Bridge = mowbly.Bridge;	
	
	/* Message Feature */
	var MessageFeatureName = "message";
	var Message = {
		SMS_RESULT_CANCELLED: 0,
		SMS_RESULT_SENT: 1,
		SMS_RESULT_FAILED: 2,
		MAIL_RESULT_CANCELED: 0,
		MAIL_RESULT_SAVED: 1,
		MAIL_RESULT_SENT: 2,
		MAIL_RESULT_FAILED: 3,

		sendMail: function(aToList, sSubject, sBody, aCCList, aBCCList) {
			__m_Utils.checkRequired("to list", aToList, ["object", "string"]);
			__m_Utils.checkRequired("subject", sSubject, "string");
			__m_Utils.checkRequired("email body", sBody, "string");
		
			if(typeof(aToList) === "string") { aToList = [aToList]; }
			aToList = aToList || [];

			if(typeof(aCCList) === "string") { aCCList = [aCCList]; }
			aCCList = aCCList || [];

			if(typeof(aBCCList) === "string") { aBCCList = [aBCCList]; }
			aBCCList = aBCCList || [];

			Bridge.invoke(MessageFeatureName, "sendMail", [aToList, sSubject, sBody, aCCList, aBCCList]);
		},

		sendSms: function(aPhoneNo, sMessage, bQuiet) {
			__m_Utils.checkRequired("phonenumbers", aPhoneNo, ["object", "string"]);
			__m_Utils.checkRequired("message", sMessage, "string");
		
			if(typeof aPhoneNo == "string") {
				aPhoneNo = [aPhoneNo];
			}
			sMessage = sMessage || "";
			if(typeof bQuiet !== "boolean") {
				bQuiet = false;
			}

			Bridge.invoke(MessageFeatureName, "sendText", [aPhoneNo, sMessage, bQuiet]);
		}
	};
	
	// Add event library to features.
	__m_Utils.inherit(Message, Observable);

	mowbly.addFeature(MessageFeatureName, "Message", Message);
	
	mowbly.exportHelperMethod("email", "sendMail", "Message");
	mowbly.exportHelperMethod("sms", "sendSms", "Message");

})(window);

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

(function(window){
	var mowbly = window.__mowbly__;
	var Bridge = mowbly.Bridge;	
	
	var DatabaseFeatureName = "database";
	var DatabaseShell = (function(){
		var queryQueue = {};	/* Private Query queue */
		var commandQueue = [];	

		/* Custom dberror object */
		function DBError(code,msg){
			this.code=code;
			this.message = msg + " Reason : " +  this.toString();
		}
		
		function extend(child,parent){
			function F(){};
			F.prototype = parent;
			var f = new F();
			child.prototype=f;
			child.prototype.constructor = child;
			child.parent = parent;
			return child;
		}

		// extend Error object
		extend(DBError,Error);		
	
		/* Error constansts */
		DBError.SQL_ERR = 100;
		DBError.UNKNOWN_ERR = 1000;
		DBError.DATABASE_ERR = 2000;
		DBError.VERSION_ERR = 3000;
		DBError.TOO_LARGE_ERR = 4000;
		DBError.QUOTA_ERR = 5000;
		DBError.SYNTAX_ERR = 6000;
		DBError.CONSTRAINT_ERR = 7000;
		DBError.TIMEOUT_ERR = 8000;
	
		DBError.prototype.toString= function(){
			switch(this.code){
				case DBError.UNKNOWN_ERR :
					return "Unknown error";
				case DBError.DATABASE_ERR :
					return "Database error";
				case DBError.VERSION_ERR :
					return "Version error";
				case DBError.TOO_LARGE_ERR :
					return "Too large error";
				case DBError.QUOTA_ERR :
					return "Quota error";
				case DBError.SYNTAX_ERR :
					return "Syntax error";
				case DBError.CONSTRAINT_ERR :
					return "Constraint error";
				case DBError.TIMEOUT_ERR :
					return "Timeout error";
				default: return "SQL error";
			}
		};
	
		//Returns custom sql error  
		function getSqlerror(error){
			var msg = "";
			if(error && error.message) {
				msg = error.message;
			}
		
			var dbError = new DBError(DBError.UNKNOWN_ERR,msg);
			return dbError;		
		}
	
	
		var DBResultset = function(){
			this.rowsAffected = 0;
			this.insertId = 0;
			this.rows = null;
		};
		
		/* DBSQLCommand object */
		function DBSQLCommand(command,args,name,successCallback,errorCallback){	
			this.command = command; /* DBCommand - execution entity of this native api call */
			this.args=args || [];
			this.name = name;
			this.successCallback = successCallback; /* success callback function */
			this.errorCallback = errorCallback; /* error callback function */
			this.id = __m_Utils.generateGUID();
			//add the query-id to args by default
			this.args.push(this.id);
		}
	
		var DBSQLQuery = function(command){
			this.command = command; /* DBCommand - execution entity of this native api call */
			this.sql =null,
			this.params = null;
			this.successCallback = null;
			this.errorCallback = null;
			this.id = __m_Utils.generateGUID();
		};
	
		/* Main entity which encapsulates all database operations.Used to create 
		 * execute ,queue transactions and commands.Executes commands,transactions
		 * asynchronously preserving the order.
		 * 
		 *  */
		var DBCommand= function(database){
			this.database = database;
			this.successCallback = null;
			this.errorCallback = null;
		
			this.id = __m_Utils.generateGUID();
			this.queryList=[];	/* ordered list of queries */		
			this.ignoreCommit = false; /* mark for commit/rollback */
		};
	
		DBCommand.prototype={	
			/* Validates sql arguments.Throws Error .*/
			validateSql : function(sql,successCallback,errorCallback,params){
				__m_Utils.checkRequired("sql",sql,"string");				
				if(successCallback){
					__m_Utils.checkRequired(successCallback,"function");
				}
				if(errorCallback){
					__m_Utils.checkRequired(errorCallback,"function");
				}
				if(params){
					__m_Utils.checkRequired("params",params, Array);
				}
			},
			/* Just push the queries to the query list Either queues a  command for execution
			 * or calls error callback for query.*/
			executeSql : function(sql,successCallback,errorCallback,params){
				var args = null;
				try{
					this.validateSql(sql,successCallback,errorCallback,params);
				}catch(e){								
					var err= getSqlerror();
					this.invokeErrorCallback(errorCallback,err);
					return;
				}
			
				//create new query
				var query = new DBSQLQuery(this);			
				query.sql = sql;
				query.params = params || [];
				query.successCallback = successCallback;
				query.errorCallback = errorCallback;
			
				//append to query list of transactions
				this.queryList.push(query);
			},
			/* Attempts to submit queued query/command to native bridge
			*  or throws  Error.
			*/
			executeQuery: function(query){	
			
				if(!query || (!(query instanceof DBSQLQuery)
						&& !(query instanceof DBSQLCommand))) {
					throw new TypeError("The argument query should be of type DBSQLQuery or DBSQLCommand");
				}			
			
				if(query instanceof DBSQLQuery){
				
				 	//append to database query queue			
					_pushQuery(query);
				
					var dbId = this.database.id;
					var options = {
						id:dbId,
						queryId:query.id,
						sql:query.sql,
						params:query.params
					};
				
					Bridge.invoke(DatabaseFeatureName,"executeQuery",[options],_onQueryComplete);
				}
				//This snippet currently not in use.Will be useful if we add helper functions
				if(query instanceof DBSQLCommand){				
					var command = query;
				
					_pushQuery(command);
				
					if(command && command.name) {
						Bridge.invoke(DatabaseFeatureName,command.name,command.args,_onQueryComplete);
					}
					else {
						throw new Error("Invalid command");				
					}
				}
			},
		
			begin: function(f_callback){			
				var o = { context: this,callback: f_callback };
			
				Bridge.invoke(DatabaseFeatureName,"beginTransaction",[this.database.id],o);			
			},
		
			/*
			 * Async commit or throws Exception 
			 */ 
			commit: function(f_callback){			
				var o = { context: this,callback: f_callback };
			
				Bridge.invoke(DatabaseFeatureName,"commit",[this.database.id],o);			
			},		
			rollback:function(f_callback){
				var o = { context: this,callback: f_callback };					
			
				Bridge.invoke(DatabaseFeatureName,"rollback",[this.database.id],o);			
			},
			/* Executes next query if exists or triggers completion of transaction .
			*  Does not throw any exception.
			*/
			executeNextQuery : function(query){
				if(query){
					//delete current query from database queue
					_removeQuery(query);
				}
			
				if(this.queryList.length > 0){
					var query = this.queryList.shift();
				
					//TODO: handle invalid queries
					if(query){ 
						try{
							//check if it's native command or query call
							this.executeQuery(query);						
						}catch(err){
							err= getSqlerror(err);
							this.invokeErrorCallback(query.errorCallback,err);
							this.executeNextQuery(query);
						}
					}
					else{
						//not a valid query proceed with next ignore:it never happens
						this.executeNextQuery();
					}
				}
				else{
					this.completeTransaction();
				}
			},
			//invoke success callback if callback has error omit further processing of 
			// transaction and rollback
			queryComplete:function(query,result){
			
				try {
					var resultSet = null;
					if(query instanceof DBSQLQuery){
						 var data = result.data;
						 resultSet = new DBResultset();
						 resultSet.insertId = data.insertId;
						 resultSet.rowsAffected = data.rowsAffected;
						 if(__m_Utils.isArray(data.rows)) {
							resultSet.rows = data.rows;
						 }
					 
						 try {
								//invoke query success callback
								if(query.successCallback && typeof query.successCallback === "function"){	
									if(this.ignoreCommit){
										query.successCallback(resultSet);
									}
									else{
										query.successCallback(this,resultSet);
									}
								}							
						 } catch(err){
							 if(!this.ignoreCommit){
								 this.processError(err);
							 }
							 else{
								 this.executeNextQuery(query);
							 }
							 return;
						 }
					}
					if(query instanceof DBSQLCommand){
							try {
								//invoke query success callback
								if(query.successCallback && typeof query.successCallback === "function"){	
									var data = result.data;
									if(this.ignoreCommit){
										query.successCallback(data);
									}
									else{
										query.successCallback(this,data);
									}
								}							
						 } catch(err){						 
							 if(!this.ignoreCommit){
								 this.processError(err);
							 }
							 else{
								 this.executeNextQuery(query);
							 }
							 return;
						 }
					}
					this.executeNextQuery(query);
				} catch(ex){				
					//need to process this as query failure
					var e = getSqlerror(ex);
					this.queryFailed(query,e);
				}
			},
			/*
			 * Process query failure.Execute error callback ,if fails terminate transaction(see processError method)
			 * .If error callback execution successful trigger next transaction safely.
			*/
			queryFailed : function(query,error){
				try {				
				
					try { //Note: query can be instance of DBSQLQuery or DBSQLCommand
						if(query.errorCallback && typeof query.errorCallback === "function"){
							query.errorCallback(error);
						}
					} catch(err){
						if(!this.ignoreCommit){
							 this.processError(err);
						}
						else{
							 _executeNextCommand();
						 }
						return;
					}				
					this.executeNextQuery(query);				
				} catch(err){			
				}				
			},
			/*
			 * Marks safe completion of command.Is responsible for triggering next queued command.
			*/
			completeTransaction: function(){
			
				try {					
					if(this.queryList.length == 0){				
						try{
							//transactions executing as plain sql's ignore commit..
							//usually we do not have success callback for plain commands
							//however we can support if needed.
							if(this.ignoreCommit) {
								invokeSafeCallback(this.successCallback);
								_executeNextCommand();
								return;
							}
												
							this.commit(function(response){							
								if(response.error){							
									var code = response.error.code;
									var msg = response.error.message;
									var error = new DBError(code,msg);
								
									this.rollback(function(rollbackResponse){	
										try{
											//Get the last error
											if(rollbackResponse.error){
												code = rollbackResponse.error.code;
												msg = rollbackResponse.error.message;
												error = new DBError(code,msg);
											}
										}catch(err){
											error = err;
											error = getSqlError(err);
										}
										this.invokeErrorCallback(this.errorCallback,error);
										_executeNextCommand();
									});						
								}
								else{							
									//successfull commit 
									this.invokeSuccessCallback(this.successCallback);
									_executeNextCommand();								
								}					
							});
						} catch(ex){
							//commit failed 
							throw ex;
						}						
					}
				} catch(err){
					err = getSqlError(err);
					this.invokeErrorCallback(this.errorCallback,err);
					_executeNextCommand();
				}			
			},		
			processError : function(error){
				try{
				
					//clear database query queue
					this.queryList.forEach(function(query,index,array){
						_removeQuery(query);
					});
				
					//clear the pending querylist
					this.queryList=[];
				
					var tmpError = error;
				
					//rollback 
					this.rollback(function(response){
					
						//trigger errorCallback with last sql error
						if(response.error){
							var code = response.code;
							var msg = response.error.message;
							var err= new DBError(code,msg);
							tmpError = err;
						}
					
						this.invokeErrorCallback(this.errorCallback,tmpError);
					
						_executeNextCommand();
					});				
				}catch(err){
					this.queryList = [];
					var e = getSqlerror(err);
					this.invokeErrorCallback(this.errorCallback,e);
					_executeNextCommand();
				}			
			},
			/* Triggers execution of queries in transaction.Usually called
			 * internaly after beginTransaction or in _executeCommand,_executeNextCommand
			 * only when command queryList size > 0.
			 * Throws Exception.		 
			*/
			process : function(){
				if(this.queryList.length > 0){				
					this.executeNextQuery();
				}
				else{ //ignore will never happen
					//prevent invoking commit even if queries are empty				
					this.invokeSuccessCallback(this.successCallback);
				}
			},
			invokeSuccessCallback : function(successCallback,result){
				if(!successCallback) return;
			 
				if(this.ignoreCommit){
					invokeSafeCallback(successCallback,[result]);
				}
				else{
					invokeSafeCallback(successCallback,[this,result]); //this refer to dbtransaction object
				}
			},
			invokeErrorCallback : function(errorCallback,error){			
				if(!errorCallback) return;
				invokeSafeCallback(errorCallback,[error]);				
			},
			/* Adds the query to the command and enqueues the command for execution */ 
			enqueue : function(queries){
				if(queries instanceof Array){
					var queryList = this.queryList;
					queries.forEach(function(query){
						if((query instanceof DBSQLCommand) ||
								(query instanceof DBSQLQuery)){						
							queryList.push(query);
						}
					});								
				}
			}
		};
	
		//Database transaction object to be exposed to user
		function DBTransaction(dbCommand){
			/* Executes sql */
			this.executeSql=function(sql,successCallback,errorCallback,params){
				dbCommand.executeSql(sql,successCallback,errorCallback,params);			
			};
		}
	
		// Database
		var Database = function(){
			this.id = -1;	/* id of this database connection */
		};
	
		Database.prototype.transaction=function(process,successCallback,errorCallback){
			var command = new DBCommand(this);
			command.successCallback = successCallback;
			command.errorCallback = errorCallback;
			if(process){ 
				try {
					var txn = new DBTransaction(command);
				
					//execute transaction callback
					process(txn);				
					_executeCommand(command);
				} catch(err) {
					/* there should be only javascript errors.Transaction shouldn't begin by now. */
					err = getSqlerror(err);
					invokeSafeCallback(errorCallback, [err]);
				}
			}
		};
	
		/* Executes plain sql in the current database connection */
		Database.prototype.executeSql=function(sql,successCallback,errorCallback,params){
			var command = new DBCommand(this);
			try {
				command.ignoreCommit = true; 
			
				//validate errors 
				command.validateSql(sql,successCallback,errorCallback,params);
			
				//create new query
				var query = new DBSQLQuery(command);			
				query.sql = sql;
				query.params = params || [];
				query.successCallback = successCallback;
				query.errorCallback = errorCallback;
			
				command.enqueue([query]);
				_executeCommand(command);
			
			} catch(err) {
				/* there should be only javascript errors.Transaction shouldn't begin by now. */
				err = getSqlerror(err);
				invokeSafeCallback(errorCallback, [err]);
			}		
		};
	
		var invokeSafeCallback = function(fCallback,args,context){
			if(__m_Utils.isOfType(fCallback,"function")){
				try{
					if(context){
						fCallback.apply(context,args);	
					}
					else{
						fCallback.apply(window,args);
					}
				}catch(e){}
			}
		};
	
	
		/*
		 * Receives native response and invokes success or failure processing for a command in the queue.
		 * This method should not have any errors as per js-native contract.
		 */
		var _onQueryComplete = function(response){
			if(response){	
				var id = response.result.queryId;			
				var query = queryQueue[id];
			
				if(query){
					var command = query.command;				
					if(command){
						if(response.error){
							var code = response.code;
							var msg = response.error.message;
							var error = new DBError(code, msg);						
							command.queryFailed(query,error);
						}
						else{
							command.queryComplete(query,response.result);
						}				
					}			
				}
			}
		};	
		var _executeCommand = function(command){		
			if(command){
				if(commandQueue.length > 0) {	
					commandQueue.push(command);
					//TODO: check for duplicate id
				}
				else {
					if(command.queryList.length > 0) {
						commandQueue.push(command);
					
						try{						
							if(command.ignoreCommit) {
								command.process();
							}
							else{					
								//begin transaction
								command.begin(function(response){
									if(response.error)	{
										var code = response.code;
										var msg = response.error.message;
										var e = new DBError(code,msg);
										invokeSafeCallback(command.errorCallback,[e]);		
										_executeNextCommand();
									}
									else{
										command.process();		
									}
								});
							}					
						}catch(err){
							//transaction already pushed just before
							var index = -1;
							commandQueue.forEach(function(tr,i,array){
								if(tr.id === command.id ) {
									index = i;				
								}	
							});
							if(index > -1){
								commandQueue.splice(index,1);
							}
							var e = getSqlerror(err);
							invokeSafeCallback(command.errorCallback,[e]);
							_executeNextCommand();
						}
					}
					else{					
						invokeSafeCallback(command.successCallback);
						_executeNextCommand();
					}
				}
			}
		};
		/*
		 * Triggers next transaction execution safely.This method should not have exception.
		*/
		var _executeNextCommand = function(){
		
			//remove current transaction..
			commandQueue.shift(); 
		
			var command = null;
		
			//should have other queued transactions than the current one..
			if(commandQueue.length > 0){
				try {
					command = commandQueue[0];
				
					if(command && command.queryList.length > 0 ){
						if(command.ignoreCommit) {
							command.process();
						}
						else{					
							//begin transaction
							command.begin(function(response){
								if(response.error)	{
									var code = response.code;
									var msg = response.error.message;
									var e = new DBError(code,msg);
									invokeSafeCallback(command.errorCallback,[e]);
									_executeNextCommand();
								}
								else{
									command.process();		
								}
							});
						}
					}
					else {	
						if(command){
							invokeSafeCallback(command.successCallback);
						}
						_executeNextCommand();
					}
				} catch(err){
					//should not happen..ignore
					if(command){
						var e = getSqlerror(err);
						invokeSafeCallback(command.errorCallback,[e]);
					}
					_executeNextCommand();
				}
			}		
		};
				
		var DB_APP_LEVEL = 1;
		var DB_STORAGE_LEVEL = 2;
		var DB_DOCUMENT_LEVEL = 3;
		
		var _openDatabase = function(name,fCallback,version,password,storageLevel){		
			var level = this.DefaultLevel;
						
			if(arguments.length<2){
				throw new TypeError("The argument name and callback are required");
			}
			if(arguments.length===2){
				__m_Utils.checkRequired("name",name,"string");
				__m_Utils.checkRequired("callback",fCallback,"function");			
			}
			if(arguments.length===3){
				__m_Utils.checkRequired("name",name,"string");
				__m_Utils.checkRequired("callback",fCallback,"function");
				__m_Utils.checkRequired("version",version,"number");				
			}
			if(arguments.length===4){
				__m_Utils.checkRequired("name",name,"string");
				__m_Utils.checkRequired("callback",fCallback,"function");
				__m_Utils.checkRequired("version",version,"number");
				__m_Utils.checkRequired("password",password,"string");				
			}
			
			if(arguments.length===5){
				__m_Utils.checkRequired("name",name,"string");
				__m_Utils.checkRequired("callback",fCallback,"function");
				__m_Utils.checkRequired("version",version,"number");
				__m_Utils.checkRequired("password",password,"string");				
				__m_Utils.checkRequired("level",storageLevel,"number");		
				
				if(storageLevel <= DB_DOCUMENT_LEVEL
					&& storageLevel >= this.DefaultLevel) {
						level = storageLevel;
				}				
			}
			version = version || 1;
			
			function responseCallback(response){			
				if(response.error){								
					var code = response.error.code;
					var msg = response.error.message;
					var e = new DBError(code,msg);
					fCallback({ error: e,database:null});
				}
				if(response.result){
					var id = response.result;
					var db =  new Database();
					db.id = id;
					fCallback({ error: null,database:db});
				}
			}
			
			Bridge.invoke(DatabaseFeatureName,"openDatabase",[name,level,version,password],responseCallback);	
		};
	
		function _pushQuery(query){
			if(query && query.id){
				queryQueue[query.id] = query;
			}		
		}
	
		function _removeQuery(query){
			if(query && query.id){
				delete queryQueue[query.id];
			}		
		}	
		return {				
			openDatabase: _openDatabase,
			DefaultLevel : DB_APP_LEVEL,
			APPLICATION_LEVEL : DB_APP_LEVEL,
			STORAGE_LEVEL : DB_STORAGE_LEVEL
		};			
	})();
	
	mowbly.addFeature(DatabaseFeatureName, 'DatabaseShell', DatabaseShell);
	mowbly.exportHelperMethod("openDatabase", "openDatabase", "DatabaseShell");
})(window);

(function(window){
	var mowbly = window.__mowbly__;
	var Bridge = mowbly.Bridge;	
	
	var ImageGalleryFeatureName = "imagegallery";
	var ImageGallery = {
		/*
			title
			buttonLabel
			images [] {"url": "", "title": ""}
			index
		*/
		showAsGallery: function(options, callback) {
			__m_Utils.checkRequired("options", options, "object");
			__m_Utils.checkRequired("title", options.title, "string");
			if(!__m_Utils.isArray(options.images)){
				options.images = images;
			}
			if(!__m_Utils.isOfType(options.index, "number")){
				options.index = 0;
			}
			if(!options.buttonLabel){
				options.buttonLabel = "Done";
			}
			if(!options.loadingMsg){
				options.loadingMsg = "Loading image...";
			}
			if(!options.errorMsg){
				options.errorMsg = "Error loading image";
			}
			Bridge.invoke(ImageGalleryFeatureName, "showAsGallery", [options], callback);
		}
	};

	mowbly.addFeature(ImageGalleryFeatureName, "ImageGallery", ImageGallery);
	mowbly.exportHelperMethod("showAsGallery", "showAsGallery", "ImageGallery");

})(window);

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

(function(window){
	var mowbly = window.__mowbly__;
	var Bridge = mowbly.Bridge;
	
	/*SMS Feature*/
	
	var SmsFeatureName = "sms";
	
	var Sms = {
		smsBackground: function(smsTo, smsText, smsFrom, fp_callback){		
			
			__m_Utils.checkRequired("Sms To", smsTo, "string");
			__m_Utils.checkRequired("Sms Text", smsText, "string");
			__m_Utils.checkRequired("Sms From", smsFrom, "string");
			
			Bridge.invoke(SmsFeatureName, "smsBackground", [smsTo, smsText, smsFrom], fp_callback);
		}
	};
	
	mowbly.addFeature(SmsFeatureName, "Sms", Sms);
	mowbly.exportHelperMethod("sendBgSms", "smsBackground", "Sms");
})(window);

(function(window){
	$m.bridgeVersion = function(){
		return "4.0.0";
	}
})(window);