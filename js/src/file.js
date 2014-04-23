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