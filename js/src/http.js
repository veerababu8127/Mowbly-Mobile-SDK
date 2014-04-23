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