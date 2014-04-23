package com.cloudpact.mowbly.android.feature;

import android.content.Intent;
import android.net.Uri;

import com.cloudpact.mowbly.feature.BaseFeature;
import com.cloudpact.mowbly.feature.Response;
import com.cloudpact.mowbly.android.feature.FeatureBinder;
import com.cloudpact.mowbly.android.page.Page;
import com.cloudpact.mowbly.android.ui.PageActivity;
import com.google.gson.JsonObject;

/**
 * Javascript interface for the Page feature
 * 
 * @author Prashanth Jonnala <prashanth@cloudpact.com>
 */
public class PageFeature extends BaseFeature {

    /** Exposed name of the UIFeature */
    public static final String NAME = "page";

    public PageFeature() {
        super(NAME);
    }

    /**
     * Opens a {@link Page} with a name, url, data and configuration passed.
     * @param name
     * @param url
     * @param data
     * @param configuration
     * @return Response
     */
    @Method(async = false, args = {
        @Argument(name = "name", type = String.class),
        @Argument(name = "url", type = String.class),
        @Argument(name = "data", type = String.class),
        @Argument(name = "configuration", type = JsonObject.class)
    })
    public Response open(final String name, final String url, final String data, final JsonObject configuration) {
        final PageActivity activity = ((FeatureBinder) binder).getActivity();
        if (url.startsWith("/") || (url.startsWith("file") && url.endsWith("html"))) {
            activity.runOnUiThread(new Runnable() {
                @Override
                public void run() {
                    activity.open(name, url, data, configuration.toString());
                }
            });
        } else {
            Intent intent = new Intent(Intent.ACTION_VIEW);
            Uri uri = Uri.parse(url);
            if (configuration != null && configuration.get("type") != null) {
                String type = configuration.get("type").getAsString();
                intent.setDataAndType(uri, type);
            } else {
                intent.setData(uri);
            }

            if (activity.getPackageManager().resolveActivity(intent, 0) != null) {
                activity.startActivity(intent);
            } else {
                Response response = new Response();
                response.setCode(0);
                response.setError("No application found to open " + url);
                return response;
            }
        }
        return new Response();
    }

    /**
     * Close the current {@link Page}
     * @param destroy Destroys the web view object for the {@link Page}
     * @return Response
     */
    @Method(async = false, args = {
        @Argument(name = "destroy", type = boolean.class)
    })
    public Response close(final boolean destroy) {
        final PageActivity activity = ((FeatureBinder) binder).getActivity();
        activity.runOnUiThread(new Runnable() {
            @Override
            public void run() {
                activity.close(destroy);
            }
        });
        return new Response();
    }

    /**
     * Set result to be returned to the parent {@link Page} when {@link Page} is closed
     * @param result
     * @return Response
     */
    @Method(async = false, args = {
        @Argument(name = "result", type = String.class)
    })
    public Response setResult(final String result) {
        final PageActivity activity = ((FeatureBinder) binder).getActivity();
        activity.runOnUiThread(new Runnable() {
            @Override
            public void run() {
                activity.setPageResult(result);
            }
        });
        return new Response();
    }
}
