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