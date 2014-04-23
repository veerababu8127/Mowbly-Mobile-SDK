(function(window){
	var mowbly = window.__mowbly__;
	var Bridge = mowbly.Bridge;
	
	/*SMS Feature*/
	
	var SmsFeatureName = "sms";
	
	var Sms = {
		smsBackground: function(smsTo, smsText, smsFrom, fp_callback){		
			
			__m_Utils.checkRequired("Sms To", smsTo, "string");
			__m_Utils.checkRequired("Sms Text", smsText, "string");
			__m_Utils.checkRequired("Sms From", smsFrom, "string");
			
			Bridge.invoke(SmsFeatureName, "smsBackground", [smsTo, smsText, smsFrom], fp_callback);
		}
	};
	
	mowbly.addFeature(SmsFeatureName, "Sms", Sms);
	mowbly.exportHelperMethod("sendBgSms", "smsBackground", "Sms");
})(window);