package com.cloudpact.mowbly.android.feature;

import java.lang.reflect.InvocationTargetException;
import java.util.ArrayList;
import java.util.List;

import android.util.Log;
import android.webkit.JavascriptInterface;

import com.cloudpact.mowbly.android.Mowbly;
import com.cloudpact.mowbly.android.page.Page;
import com.cloudpact.mowbly.android.ui.PageActivity;
import com.cloudpact.mowbly.android.ui.PageFragment;
import com.cloudpact.mowbly.android.ui.PageView;
import com.cloudpact.mowbly.feature.AbstractBinder;
import com.cloudpact.mowbly.feature.Feature;
import com.cloudpact.mowbly.feature.Response;
import com.google.gson.JsonArray;
import com.google.gson.JsonElement;
import com.google.gson.JsonObject;
import com.google.gson.JsonParseException;
import com.google.gson.JsonParser;
import com.google.gson.JsonPrimitive;

/**
 * Concrete implementation of the FeatureBinder
 * 
 * @author Prashanth Jonnala <prashanth@cloudpact.com>
 */
public class FeatureBinder extends AbstractBinder {

    protected static final String NAME = "FeatureBinder";

    protected static final String METHOD_ARGUMENTS_ERROR_MESSAGE = "error occured while reading the arguments";

    protected static final String METHOD_DEFINITION_ERROR_MESSAGE = "error in the method definition";

    protected static final String METHOD_INVOCATION_ERROR_MESSAGE = "error occured while calling the function";

    protected PageFragment fragment;

    public FeatureBinder() {
        super(NAME);
    }

    public void setFragment(PageFragment fragment) {
        this.fragment = fragment;
    }

    public PageFragment getPageFragment() {
        return fragment;
    }

    public Page getPage() {
        return (fragment != null) ? fragment.getPage() : null;
    }

    public PageView getView() {
        return fragment.getPageView();
    }

    public PageActivity getActivity() {
        return fragment.getPageActivity();
    }

    protected List<Object> parseFeatureMethodArgs(Method methodDefinition, JsonArray args) {
        List<Object> finalArgs = new ArrayList<Object>();

        Argument[] featureMethodArgs = methodDefinition.args();
        for (int i = 0; i < featureMethodArgs.length; i++) {
            Class<?> argType = featureMethodArgs[i].type();
            JsonElement arg = args.get(i);

            if (arg.isJsonNull()) {
                finalArgs.add(null);
            } else {
                if (argType.equals(JsonArray.class)) {
                    if (arg.isJsonPrimitive()) {
                        JsonPrimitive primitive = arg.getAsJsonPrimitive();
                        if (primitive.isString()) {
                            finalArgs.add(new JsonParser().parse(primitive.getAsString()).getAsJsonArray());
                        }
                    } else {
                        finalArgs.add(arg.getAsJsonArray());
                    }
                } else if (argType.equals(JsonObject.class)) {
                    if (arg.isJsonPrimitive()) {
                        JsonPrimitive primitive = arg.getAsJsonPrimitive();
                        if (primitive.isString()) {
                            finalArgs.add(new JsonParser().parse(primitive.getAsString()).getAsJsonObject());
                        }
                    } else {
                        finalArgs.add(arg.getAsJsonObject());
                    }
                } else if (argType.equals(String.class)) {
                    finalArgs.add(arg.getAsString());
                } else if (argType.equals(int.class)) {
                    finalArgs.add(arg.getAsInt());
                } else if (argType.equals(long.class)) {
                    finalArgs.add(arg.getAsLong());
                } else if (argType.equals(boolean.class)) {
                    finalArgs.add(arg.getAsBoolean());
                } else if (argType.equals(double.class)) {
                    finalArgs.add(arg.getAsDouble());
                }
            }
        }

        return finalArgs;
    }

    protected java.lang.reflect.Method getFeatureMethod(Feature feature, String methodName, int argsLength) {
        java.lang.reflect.Method[] methods = feature.getClass().getMethods();
        for (java.lang.reflect.Method classMethod : methods) {
            if (classMethod.getName().equals(methodName) 
                    && classMethod.isAnnotationPresent(Method.class)) {
            	if(argsLength == classMethod.getAnnotation(Method.class).args().length){
            		return classMethod;
            	}
            }
        }
        return null;
    }

    @Override
    @JavascriptInterface
    public String invoke(String featureName, String methodName, String methodArgs, String callbackId) {
        Response response = new Response();

        Feature feature = get(featureName);
        if (feature == null) {
            response.setError("No feature by the name " + featureName);
        } else {
        	JsonArray jargs = new JsonParser().parse(methodArgs).getAsJsonArray();
            java.lang.reflect.Method classMethod = getFeatureMethod(feature, methodName, jargs.size());
            if (classMethod == null) {
                response.setError("No method by the name " + methodName + " in feature " + featureName);
            } else {
                try {
                    Method methodDefinition = classMethod.getAnnotation(Method.class);
                    List<Object> pargs = parseFeatureMethodArgs(methodDefinition, jargs);
                    
                    
                    if(methodDefinition.callback()){
                    	pargs.add(callbackId);
                    }
                    
                    Object[] args = pargs.toArray();
                    boolean async = methodDefinition.async();
                    if (async) {
                        Runnable task = new BridgeRequest(feature, classMethod, args, callbackId);
                        BackgroundExecutorService.execute(task);
                        response = new Response();
                    } else {
                        response = (Response) classMethod.invoke(feature, args);
                        if (callbackId != null && !callbackId.equals("")) {
                            onAsyncMethodResult(callbackId, response);
                        }
                    }
                } catch (JsonParseException e) {
                    Log.e(NAME, METHOD_ARGUMENTS_ERROR_MESSAGE, e);

                    response.setError(METHOD_ARGUMENTS_ERROR_MESSAGE, e.getMessage());
                } catch (ClassCastException e) {
                    Log.e(NAME, METHOD_ARGUMENTS_ERROR_MESSAGE, e);

                    response.setError(METHOD_ARGUMENTS_ERROR_MESSAGE, e.getMessage());
                } catch (IllegalArgumentException e) {
                    Log.e(NAME, METHOD_ARGUMENTS_ERROR_MESSAGE, e);

                    response.setError(METHOD_ARGUMENTS_ERROR_MESSAGE, e.getMessage());
                } catch (IllegalAccessException e) {
                    Log.e(NAME, METHOD_ARGUMENTS_ERROR_MESSAGE, e);

                    response.setError(METHOD_DEFINITION_ERROR_MESSAGE, e.getMessage());
                } catch (InvocationTargetException e) {
                    Log.e(NAME, METHOD_ARGUMENTS_ERROR_MESSAGE, e);

                    response.setError(METHOD_INVOCATION_ERROR_MESSAGE, e.getMessage());
                }
            }
        }

        String res = Mowbly.getGson().toJson(response);
        return res;
    }

    public void onAsyncMethodResult(final String callbackId, final Response response) {
        final String res = getResponseString(response);
        if (res != null) {
            final PageView view = getView();
            view.post(new Runnable() {
                @Override
                public void run() {
                    view.getEventHandler().onCallbackReceive(callbackId, res);
                }
            });
        }
    }

    public String getResponseString(Response response) {
        if (response != null) {
            String res = null;

            try {
                if (response.getResult() instanceof JsonElement) {
                    JsonElement result = (JsonElement) response.getResult();                    
                    JsonObject r = new JsonObject();
                    r.addProperty("code", response.getCode());
                    r.add("result", result);

                    if (response.getError() != null) {
                        JsonObject error = new JsonObject();
                        error.addProperty("message", response.getError().getMessage());
                        error.addProperty("description", response.getError().getDescription());

                        r.add("error", error);
                    }

                    res = r.toString();
                } else {
                    res = Mowbly.getGson().toJson(response);
                }
            } catch (OutOfMemoryError e) {
                response.setCode(0);
                response.setResult(null);
                response.setError("Response too big to return");

                res = Mowbly.getGson().toJson(response);
            }

            return res;
        }

        return null;
    }

    private class BridgeRequest implements Runnable {

        private Feature feature;

        private java.lang.reflect.Method method;

        private Object[] args;

        private String callbackId;

        public BridgeRequest(Feature feature, java.lang.reflect.Method method, Object[] args, String callbackId) {
            this.feature = feature;
            this.method = method;
            this.args = args;
            this.callbackId = callbackId;
        }

        @Override
        public void run() {
            Response response = new Response();
            try {
                response = (Response) method.invoke(feature, args);
            } catch (IllegalArgumentException e) {
                Log.e(NAME, METHOD_ARGUMENTS_ERROR_MESSAGE, e);

                response.setError(METHOD_ARGUMENTS_ERROR_MESSAGE, e.getMessage());
            } catch (IllegalAccessException e) {
                Log.e(NAME, METHOD_ARGUMENTS_ERROR_MESSAGE, e);

                response.setError(METHOD_DEFINITION_ERROR_MESSAGE, e.getMessage());
            } catch (InvocationTargetException e) {
                Log.e(NAME, METHOD_ARGUMENTS_ERROR_MESSAGE, e);

                response.setError(METHOD_INVOCATION_ERROR_MESSAGE, e.getMessage());
            }

            onAsyncMethodResult(callbackId, response);
        }
    }
}
