package com.cloudpact.mowbly.android.feature;

import android.app.Activity;
import android.app.PendingIntent;
import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.content.IntentFilter;
import android.net.Uri;

import com.cloudpact.mowbly.android.Mowbly;
import com.cloudpact.mowbly.android.ui.PageView;
import com.cloudpact.mowbly.feature.BaseFeature;
import com.cloudpact.mowbly.feature.Response;
import com.cloudpact.mowbly.log.Logger;
import com.google.gson.JsonArray;

/**
 * Javascript interface for Message feature
 * 
 * @author Prashanth Jonnala <prashanth@cloudpact.com>
 */
public class MessageFeature extends BaseFeature {

    /** Exposed name of the MessageFeature */
    final static public String NAME = "message";

    private static final String ACTION_SMS_SENT = "SMS_SENT";
    protected static final Logger logger = Logger.getLogger();

    public MessageFeature() {
        super(NAME);
    }

    /**
     * Opens the default mail client or intent chooser for email
     * @param to
     * @param subject
     * @param body
     * @param cc
     * @param bcc
     * @return Response
     */
    @Method(async = true, args = {
        @Argument(name = "to", type = JsonArray.class),
        @Argument(name = "subject", type = String.class),
        @Argument(name = "body", type = String.class),
        @Argument(name = "cc", type = JsonArray.class),
        @Argument(name = "bcc", type = JsonArray.class)
    })
    public Response sendMail(JsonArray to, String subject, String body, JsonArray cc, JsonArray bcc) {
        int count = to.size();
        String[] toList = new String[count];
        for (int i = 0; i < count; i++) {
            toList[i] = to.get(i).getAsString();
        }

        count = cc.size();
        String[] ccList = new String[count];
        for (int i = 0; i < count; i++) {
            ccList[i] = cc.get(i).getAsString();
        }

        count = bcc.size();
        String[] bccList = new String[count];
        for (int i = 0; i < count; i++) {
            bccList[i] = bcc.get(i).getAsString();
        }

        Intent emailIntent = new Intent(android.content.Intent.ACTION_SEND);
        emailIntent.setType("text/html");
        emailIntent.putExtra(android.content.Intent.EXTRA_EMAIL, toList);
        emailIntent.putExtra(android.content.Intent.EXTRA_SUBJECT, subject);
        emailIntent.putExtra(android.content.Intent.EXTRA_TEXT, body);
        emailIntent.putExtra(android.content.Intent.EXTRA_CC, ccList);
        emailIntent.putExtra(android.content.Intent.EXTRA_BCC, bccList);
        ((FeatureBinder) binder).getActivity().startActivity(emailIntent);

        return new Response();
    }

    /**
     * Opens SMS client with passed arguments
     * @param to
     * @param message
     * @param quiet
     * @param callbackId
     * @return Response
     */
    @Method(async = true, args = {
        @Argument(name = "to", type = JsonArray.class),
        @Argument(name = "message", type = String.class),
        @Argument(name = "quiet", type = boolean.class)
    })
    public Response sendText(JsonArray to, String message, boolean quiet, String callbackId) {
        final PageView view = ((FeatureBinder) binder).getView();
        final Context context = ((FeatureBinder) binder).getActivity();

        // Register and start the SMSListener
        if (callbackId != null && callbackId != "") {
            new SMSListener(view, callbackId);
        }

        if (!quiet) {
            StringBuilder sb = new StringBuilder();
            int count = to.size();
            for (int i = 0; i < count; i++) {
                sb.append(to.get(i).getAsString());
                sb.append(";");
            }
            Intent smsIntent = new Intent(Intent.ACTION_VIEW);
            smsIntent.setData(Uri.parse("sms:" + sb.toString()));
            smsIntent.putExtra("sms_body", message);
            context.startActivity(smsIntent);
        } else {
            // Send message
            int count = to.size();
            if (count > 0) {
                logger.debug("Message", "Sending SMS");
            }
            for (int i = 0; i < count; i++) {
                Intent smsIntent = new Intent(ACTION_SMS_SENT);
                PendingIntent smsPi = PendingIntent.getBroadcast(context, 0, smsIntent, 0);

                android.telephony.SmsManager sms = android.telephony.SmsManager.getDefault();
                sms.sendTextMessage(to.get(i).getAsString(), null, message, smsPi, null);
            }
        }

        return null;
    }

    private class SMSListener extends BroadcastReceiver {
        private Context context;
        private PageView view;
        private String callbackId;

        public SMSListener(PageView view, String callbackId) {
            this.view = view;
            this.context = view.getContext();
            this.callbackId = callbackId;

            context.registerReceiver(this, new IntentFilter(ACTION_SMS_SENT));
        }

        @Override
        public void onReceive(Context context, Intent intent) {
            String message;
            int resultCode = getResultCode();
            switch (getResultCode()) {
            case Activity.RESULT_OK:
                message = "Message sent!";
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
            Response response = new Response();
            response.setCode(resultCode);
            response.setResult(message);
            view.getEventHandler().onCallbackReceive(callbackId, Mowbly.getGson().toJson(response));

            context.unregisterReceiver(this);
        }
    }
}
