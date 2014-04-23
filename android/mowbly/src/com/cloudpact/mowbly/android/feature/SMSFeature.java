package com.cloudpact.mowbly.android.feature;

import android.app.Activity;
import android.app.PendingIntent;
import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.content.IntentFilter;
import android.os.Bundle;
import android.telephony.SmsMessage;

import com.cloudpact.mowbly.android.ui.PageActivity;
import com.cloudpact.mowbly.android.ui.PageView;
import com.cloudpact.mowbly.android.util.GsonUtils;
import com.cloudpact.mowbly.feature.BaseFeature;
import com.cloudpact.mowbly.feature.Error;
import com.cloudpact.mowbly.feature.Response;
import com.cloudpact.mowbly.log.Logger;

public class SMSFeature extends BaseFeature  {
	public static final String NAME = "sms";
	public static final String ACTION_SMS_SENT = "SMS_SENT";
	public static final String ACTION_SMS_RECEIVED = "android.provider.Telephony.SMS_RECEIVED";
	
	private static final Logger logger = Logger.getLogger();
	
	GsonUtils gsonUtils = new GsonUtils();

	public SMSFeature() {
		super(NAME);
	}

	@Method(async = true, args = {
	        @Argument(name = "to", type = String.class),
	        @Argument(name = "message", type = String.class),
	        @Argument(name = "from", type = String.class)
	    }, callback = true)
	public  Response smsBackground(String to, String message, String from, String callbackId) {
		final PageActivity context = ((FeatureBinder) binder).getActivity();
		final PageView view = ((FeatureBinder) binder).getView();
		new SentReceiver(view, callbackId, from, context);
		Intent smsIntent = new Intent(ACTION_SMS_SENT);
        PendingIntent sentPi = PendingIntent.getBroadcast(context, 0, smsIntent, 0);

        android.telephony.SmsManager sms = android.telephony.SmsManager.getDefault();
        sms.sendTextMessage(to, null, message, sentPi, null);
		return null;
	}
	
	
	protected class SentReceiver extends BroadcastReceiver {
        private Context context;
        private PageView view;
        private String callbackId;
        private String from;
        private boolean isRegistered = false;

        public SentReceiver(PageView view, String callbackId, String from, Context context) {
            this.view = view;
            this.context = context;
            this.callbackId = callbackId;
            this.from = from;

            ((PageActivity)this.context).addReceiver(this, new IntentFilter(ACTION_SMS_SENT));
            isRegistered = true;
        }

        @Override
        public void onReceive(Context context, Intent intent) {
            String message;
            int resultCode = getResultCode();
            switch (resultCode) {
            case Activity.RESULT_OK:
                message = "Message sent!";
                // Register SMS Receiver
                new ResponseSmsReceiver(view, callbackId, from, context);
                break;
            case android.telephony.SmsManager.RESULT_ERROR_GENERIC_FAILURE:
                message = "Generic failure";
                break;
            case android.telephony.SmsManager.RESULT_ERROR_NO_SERVICE:
                message = "No service";
                break;
            case android.telephony.SmsManager.RESULT_ERROR_NULL_PDU:
                message = "Null PDU";
                break;
            case android.telephony.SmsManager.RESULT_ERROR_RADIO_OFF:
                message = "Radio off";
                break;
            default:
                message = "Failed to send SMS. Error code - " + resultCode;
            }

            logger.debug("Message", "SMS delivery status, Code: " + resultCode + ", Result: " + message);
            if(resultCode != Activity.RESULT_OK){
            	final Response response = new Response();
                response.setCode(0);
                response.setError(new Error(message, "Code - " + resultCode));
                view.post(new Runnable() {
	                @Override
	                public void run() {
	                	view.getEventHandler().onCallbackReceive(callbackId, gsonUtils.getGson().toJson(response));
	                }
	            });
            }
            if(isRegistered){
            	((PageActivity)context).removeReceiver(this);
            	isRegistered = false;
            }
        }
    }
	
	protected class ResponseSmsReceiver extends BroadcastReceiver{

		private Context context;
        private PageView view;
        private String callbackId;
        private String from;
        private boolean isRegistered = false;

        public ResponseSmsReceiver(PageView view, String callbackId, String from, Context context) {
            this.view = view;
            this.context = context;
            this.callbackId = callbackId;
            this.from = from;

            ((PageActivity)this.context).addReceiver(this, new IntentFilter(ACTION_SMS_RECEIVED));
            isRegistered = true;
        }
		
		@Override
		public void onReceive(Context context, Intent intent) {
			Bundle bundle = intent.getExtras();
			StringBuilder sb = new StringBuilder();
			boolean received = false;
			
			if(bundle != null){
				Object[] pdus = (Object[]) bundle.get("pdus");
				SmsMessage[] messages = new SmsMessage[pdus.length];
	            // TODO Verify compatibility - Telephony.Sms.Intents.getMessagesFromIntent(intent);
				for (int i = 0; i < messages.length; i++) {
					SmsMessage currentMessage = messages[i] = SmsMessage.createFromPdu((byte[])pdus[i]);
					if(currentMessage.getOriginatingAddress().equals(from)){
						received = true;
						sb.append(currentMessage.getMessageBody());
					}
				}
			}
			if(received){
				final Response response = new Response();
	            response.setCode(1);
	            response.setResult(sb.toString());
	            view.post(new Runnable() {
	                @Override
	                public void run() {
	                	view.getEventHandler().onCallbackReceive(callbackId, gsonUtils.getGson().toJson(response));
	                }
	            });
	            if(isRegistered){
	            	((PageActivity)context).removeReceiver(this);
	            	isRegistered = false;
	            }
			}
		}
	}
}

