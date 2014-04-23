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