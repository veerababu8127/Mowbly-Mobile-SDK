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