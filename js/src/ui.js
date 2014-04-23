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