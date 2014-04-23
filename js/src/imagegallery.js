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