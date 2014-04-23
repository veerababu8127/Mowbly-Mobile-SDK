package com.cloudpact.mowbly.android.feature;

import android.app.Activity;
import android.app.AlertDialog;
import android.app.ProgressDialog;
import android.content.DialogInterface;
import android.text.TextUtils;
import android.widget.Toast;

import com.cloudpact.mowbly.android.Mowbly;
import com.cloudpact.mowbly.android.ui.PageView;
import com.cloudpact.mowbly.feature.BaseFeature;
import com.cloudpact.mowbly.feature.Response;
import com.google.gson.JsonArray;
import com.google.gson.JsonObject;

/**
 * Javascript interface for the UI feature
 * 
 * @author Prashanth Jonnala <prashanth@cloudpact.com>
 */
public class UIFeature extends BaseFeature {

    /** Exposed name of the UIFeature */
    public static final String NAME = "ui";

    /** Whether any confirm modal box is running or not */
    private static boolean isModalRunning = false;

    /** ProgressDialog */
    private static ProgressDialog dialog;

    public UIFeature() {
        super(NAME);
    }

    /**
     * Shows a toast
     * @param message
     * @param duration - {@link Toast.LENGTH_SHORT} or {@link Toast.LENGTH_LONG}
     * @return Response
     */
    @Method(async = false, args = {
        @Argument(name = "message", type = String.class),
        @Argument(name = "duration", type = int.class)
    })
    public Response toast(final String message, final int duration) {
        Response response = new Response();
        if (TextUtils.isEmpty(message)) {
            response.setError("message cannot be empty");
        } else {
            Activity activity = ((FeatureBinder) binder).getActivity();
            Toast toast = Toast.makeText(activity, message, duration);
            
            toast.show();
        }
        return response;
    }

    /**
     * Shows a blocking progress bar
     * @param title
     * @param message
     * @return Response
     */
    @Method(async = false, args = {
        @Argument(name = "title", type = String.class),
        @Argument(name = "message", type = String.class)
    })
    public Response showProgress(final String title, final String message) {
        if (!isModalRunning) {
            if (dialog == null) {
                final Activity activity = ((FeatureBinder) binder).getActivity();
                dialog = ProgressDialog.show(activity, title, message, true, false);
            } else {
                dialog.setTitle(title);
                dialog.setMessage(message);
                if (!dialog.isShowing()) {
                    dialog.show();
                }
            }
        }
        return new Response();
    }

    /**
     * Hides progress bar if visible
     * @return Response
     */
    @Method(async = false, args = {})
    public Response hideProgress() {
        if (dialog != null && dialog.isShowing()) {
            dialog.dismiss();
            dialog = null;
        }
        return new Response();
    }

    /**
     * Shows a confirm prompt with a title, message and a maximum of three buttons
     * @param options
     * @return Response
     */
    @Method(async = true, args = {
        @Argument(name = "options", type = JsonObject.class)
    })
    public Response confirm(JsonObject options) {
        Response response = new Response();

        if (isModalRunning) {
            response.setError("confirm box already running");
            return response;
        }

        final String title = options.get("title").getAsString();
        final String message = options.get("message").getAsString();
        final String callbackId = options.get("callbackId").getAsString();
        final boolean cancelable = (options.get("cancelable") != null) ? options.get("cancelable").getAsBoolean() : false;
        JsonArray buttons = options.get("buttons").getAsJsonArray();
        JsonObject positiveButton = null;
        JsonObject neutralButton = null;
        JsonObject negativeButton = null;
        if (buttons != null && buttons.size() >= 2) {
            positiveButton = buttons.get(0).getAsJsonObject();
            neutralButton = buttons.get(1).getAsJsonObject();
            if (buttons.size() == 3) {
                negativeButton = buttons.get(2).getAsJsonObject();
            }
        }

        final PageView view = ((FeatureBinder) binder).getView();
        final Activity activity = ((FeatureBinder) binder).getActivity();
        final DialogInterface.OnClickListener listener = new DialogInterface.OnClickListener() {
            @Override
            public void onClick(DialogInterface dialog, int which) {
                Response response = new Response();
                switch (which) {
                case DialogInterface.BUTTON_POSITIVE:
                    response.setResult(0);
                    break;
                case DialogInterface.BUTTON_NEUTRAL:
                    response.setResult(1);
                    break;
                case DialogInterface.BUTTON_NEGATIVE:
                default:
                    response.setResult(2);
                    break;
                }
                dialog.dismiss();
                isModalRunning = false;
                view.getEventHandler().onCallbackReceive(callbackId, String.valueOf(response.getResult()));
            }
        };

        final AlertDialog.Builder builder = new AlertDialog.Builder(activity);
        builder.setTitle(title);
        builder.setMessage(message);
        builder.setCancelable(cancelable);
        builder.setPositiveButton(positiveButton.get("label").getAsString(), listener);
        builder.setNeutralButton(neutralButton.get("label").getAsString(), listener);
        if (negativeButton != null) {
            builder.setNegativeButton(negativeButton.get("label").getAsString(), listener);
        }

        activity.runOnUiThread(new Runnable() {
            @Override
            public void run() {
                builder.show();
            }
        });
        
        isModalRunning = true;
        return response;
    }

    /**
     * Alerts a message
     * @param options
     * @return Response
     */
    @Method(async = true, args = {
        @Argument(name = "options", type = JsonObject.class)
    })
    public Response alert(JsonObject options) {
        final Response response = new Response();

        if (isModalRunning) {
            response.setError("alert box already running");
            return response;
        }

        final String title = options.get("title").getAsString();
        final String message = options.get("message").getAsString();
        final String callbackId = options.get("callbackId").getAsString();
        final Activity activity = ((FeatureBinder) binder).getActivity();
        final PageView view = ((FeatureBinder) binder).getView();

        DialogInterface.OnClickListener listener = new DialogInterface.OnClickListener() {
            @Override
            public void onClick(DialogInterface dialog, int which) {
                dialog.dismiss();
                isModalRunning = false;
                view.getEventHandler().onCallbackReceive(callbackId, Mowbly.getGson().toJson(response));
            }
        };
        final AlertDialog.Builder builder = new AlertDialog.Builder(activity);
        builder.setTitle(title);
        builder.setMessage(message);
        builder.setCancelable(false);        
        builder.setPositiveButton("Ok", listener);
        activity.runOnUiThread(new Runnable() {
            @Override
            public void run() {
                builder.show();
            }
        });
        isModalRunning = true;
        return response;
    }
}
