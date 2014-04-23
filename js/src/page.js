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