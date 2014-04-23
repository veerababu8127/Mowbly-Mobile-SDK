package com.cloudpact.mowbly.android.service;

import android.annotation.SuppressLint;
import android.content.Context;
import android.content.SharedPreferences;
import android.preference.PreferenceManager;

import com.google.gson.JsonArray;
import com.google.gson.JsonObject;
import com.google.gson.JsonParser;
import com.google.inject.Inject;
import com.google.inject.name.Named;

/**
 * Preferences service provider
 * 
 * @author Prashanth Jonnala <prashanth@cloudpact.com>
 */
public class PreferenceService {

    public static final String APPLICATION_PREFERENCES = "Application";
    public static final String FRAMEWORK_PREFERENCES = "Framework";

    public static final String KEY_CAMERA_CONFIGURATION = "__mowbly_camera_configuration";
    public static final String KEY_MOWBLY_PREFERENCES = "__mowbly_global";

    protected Context context;
    

    @Inject
    public PreferenceService(@Named("Application") Context context) {
        this.context = context;
    }

    public SharedPreferences getDefaultPreferences() {
        return PreferenceManager.getDefaultSharedPreferences(context);
    }

    public SharedPreferences getApplicationPreferences() {
        return context.getSharedPreferences(APPLICATION_PREFERENCES, Context.MODE_PRIVATE);
    }

    public SharedPreferences getFrameworkPreferences() {
        return context.getSharedPreferences(FRAMEWORK_PREFERENCES, Context.MODE_PRIVATE);
    }

    public JsonArray getCameraConfiguration() {
        String config = getFrameworkPreferences().getString(KEY_CAMERA_CONFIGURATION, null);
        if (config != null) {
            return new JsonParser().parse(config).getAsJsonArray();
        }
        return null;
    }

    public void setCameraConfiguration(JsonArray config) {
        SharedPreferences.Editor editor = getFrameworkPreferences().edit();
        editor.putString(KEY_CAMERA_CONFIGURATION, config.toString());
        editor.commit();
    }

    protected void setOrRemovePreference(SharedPreferences.Editor editor, String key, String value) {
        if (value != null) {
            editor.putString(key, value);
        } else {
            editor.remove(key);
        }
        editor.commit();
    }

    @SuppressLint("CommitPrefEdits")
	protected void setOrRemovePreference(String key, String value) {
        SharedPreferences.Editor editor = getApplicationPreferences().edit();
        setOrRemovePreference(editor, key, value);
    }

    public JsonObject getMowblyPreferences(){
    	String prefs = getApplicationPreferences().getString(KEY_MOWBLY_PREFERENCES, null);
    	if (prefs != null) {
            return new JsonParser().parse(prefs).getAsJsonObject();
        }
        return new JsonObject();
    }
    public void setMowblyPreferences(String prefs){
    	setOrRemovePreference(KEY_MOWBLY_PREFERENCES, prefs);
    }
}
