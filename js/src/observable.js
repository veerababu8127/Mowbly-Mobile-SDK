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