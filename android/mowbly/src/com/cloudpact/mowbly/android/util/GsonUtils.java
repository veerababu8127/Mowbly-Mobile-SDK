package com.cloudpact.mowbly.android.util;

import com.google.gson.Gson;
import com.google.gson.GsonBuilder;
import com.google.gson.JsonElement;

/**
 * GsonUtils
 * 
 * @author Prashanth Jonnala <prashanth@cloudpact.com>
 */
public class GsonUtils {

    private final Gson GSON = createGson(true);

    private final Gson GSON_NO_NULLS = createGson(false);

    public GsonUtils() {
    }

    public final Gson createGson() {
        return createGson(true);
    }

    /**
     * Register your gson adapters here
     * @param serializeNulls
     * @return Gson
     */
    public final Gson createGson(boolean serializeNulls) {
    	final GsonBuilder builder = getGsonBuilder();
    	if (serializeNulls) {
    		builder.serializeNulls();
    	}
        return builder.create();
    }

    protected GsonBuilder getGsonBuilder(){
    	GsonBuilder builder = new GsonBuilder();
    	return builder;
    }
    
    public final Gson getGson() {
        return GSON;
    }

    public final Gson getGson(boolean serializeNulls) {
        return (serializeNulls) ? GSON : GSON_NO_NULLS;
    }
    
    public JsonElement getJsonElement(Object o){
    	return getGson().toJsonTree(o);
    }
}
