package com.cloudpact.mowbly.android;

import android.app.Activity;
import android.content.Context;

/**
 * Intents utility to get intent strings and request codes based on resources
 * 
 * @author Aravind Baskaran <aravind@cloudpact.com>
 */
public class Intents {
	public static int getRequestCode(Activity activity, int resId){
		return getRequestCode(activity.getApplicationContext(), resId);
	}
	
	public static int getRequestCode(Context context, int resId){
		String r = context.getResources().getString(resId);
		return Integer.parseInt(r, 10);
	}
	
	public static String getIntentString(Activity activity, int resId){
		return getIntentString(activity.getApplicationContext(), resId);
	}
	
	public static String getIntentString(Context context, int resId){
		return context.getResources().getString(resId);
	}
}
