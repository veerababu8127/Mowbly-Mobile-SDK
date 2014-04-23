package com.cloudpact.mowbly.android.feature;

import java.util.List;
import java.util.Map.Entry;

import android.content.Intent;
import android.content.pm.PackageManager;
import android.content.pm.ResolveInfo;
import android.net.Uri;

import com.cloudpact.mowbly.android.ui.PageActivity;
import com.cloudpact.mowbly.android.ui.PageFragment;
import com.cloudpact.mowbly.feature.BaseFeature;
import com.cloudpact.mowbly.feature.Response;
import com.google.gson.JsonElement;
import com.google.gson.JsonObject;
import com.google.gson.JsonPrimitive;

/**
 * Javascript interface for the Framework feature
 * 
 * @author Prashanth Jonnala <prashanth@cloudpact.com>
 */
public class FrameworkFeature extends BaseFeature {

    /** Exposed name of the FrameworkFeature */
    public static final String NAME = "framework";

    public FrameworkFeature() {
        super(NAME);
    }

    @Deprecated
    @Method(async = false, args = {
        @Argument(name = "name", type = String.class),
        @Argument(name = "url", type = String.class),
        @Argument(name = "data", type = String.class),
        @Argument(name = "configuration", type = JsonObject.class)
    })
    public Response launchApplication(final String name, final String url, final String data, final JsonObject configuration) {
        return ((PageFeature) binder.get(PageFeature.NAME)).open(name, url, data, configuration);
    }

    @Deprecated
    @Method(async = false, args = {
        @Argument(name = "result", type = String.class)
    })
    public Response setPageResult(String result) {
        return ((PageFeature) binder.get(PageFeature.NAME)).setResult(result);
    }

    @Deprecated
    @Method(async = false, args = {
        @Argument(name = "destroy", type = boolean.class)
    })
    public Response closeApplication(boolean destroy) {
        return ((PageFeature) binder.get(PageFeature.NAME)).close(destroy);
    }

    @Method(async = true, args = {
        @Argument(name = "options", type = JsonObject.class)
    })
    public Response openExternal(final JsonObject options) {
        JsonElement el = null;
        Response response = new Response();

        el = options.get("action");
        String action = el != null ? el.getAsString() : android.content.Intent.ACTION_DEFAULT;

        Intent intent = new Intent(action);

        String scheme = null;
        el = options.get("scheme");
        if (el != null) {
            scheme = el.getAsString();
        } 

        if (scheme != null) {
            Uri data = Uri.parse(scheme);
            intent.setData(data);
        }

        el = options.get("mimeType");
        if (el != null) {
            String mimeType = el.getAsString();
            intent.setType(mimeType);
        }

        String className = null;
        String packageName = null;
        el = options.get("className");
        if (el != null) {
            className = el.getAsString();

            el = options.get("packageName");
            if (el == null) {
                response.setCode(0);
                response.setError("packageName is required along with className");
                return response;
            }

            packageName = el.getAsString();
        }

        if (packageName == null && className == null) {
            if (scheme == null) {
                response.setCode(0);
                response.setError("scheme or packageName/className is required");
                return response;
            }
        } else {
            intent.setClassName(packageName, className);
        }

        el = options.get("extras");
        if (el != null) {
            JsonObject extras = el.getAsJsonObject();
            for (Entry<String, JsonElement> entry : extras.entrySet()) {
                String key = entry.getKey();
                JsonElement val = entry.getValue();
                if (val.isJsonNull() || !val.isJsonPrimitive()) {
                    continue;
                }

                JsonPrimitive primitive = val.getAsJsonPrimitive();
                if (primitive.isBoolean()) {
                    intent.putExtra(key, primitive.getAsBoolean());
                } else if (primitive.isNumber()) {
                    intent.putExtra(key, primitive.getAsNumber());
                } else {
                    intent.putExtra(key, primitive.getAsString());
                }
            }
        }

        PageActivity activity = ((FeatureBinder) binder).getActivity();

        PackageManager manager = activity.getPackageManager();
        List<ResolveInfo> list = manager.queryIntentActivities(intent, 0);
        if (list == null || list.size() == 0) {
            response.setCode(0);
            response.setError("No application found which can handle this request");
        } else {
            activity.startActivity(intent);
        }

        return response;
    }

    @Method(async = true, args = {
            @Argument(name = "pageName", type = String.class),
            @Argument(name = "message", type = JsonObject.class)
        })
    public Response postMessage(final String pageName, final JsonObject message) {
    	Response response = new Response();
    	boolean success = true;
    	final PageActivity activity = ((FeatureBinder) binder).getActivity();
        activity.runOnUiThread(new Runnable() {
            @Override
            public void run() {
            	PageFragment activeFragment = activity.getActiveFragment();
            	PageFragment fragment = 
            			(PageFragment)activeFragment.getFragmentManager().findFragmentByTag(pageName);
            	if(fragment != null) {
            		String sender = activeFragment.getPage().getName();
            		fragment.getPageView().loadJavascript("__mowbly__._onPageMessage(" + message + ",'" + sender + "');");
            	}	
            }
        });

    	if(success) {
    		response.setCode(1);
    	} else {
    		response.setCode(0);
    		response.setError("Error posting message. Page " + pageName + " not found.");
    	}

    	return response;
    }
}