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