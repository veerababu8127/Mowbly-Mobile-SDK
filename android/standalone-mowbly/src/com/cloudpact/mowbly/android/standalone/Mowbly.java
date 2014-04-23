package com.cloudpact.mowbly.android.standalone;

import com.cloudpact.mowbly.standalone.R;


/**
 * Mowbly - Application Context for apps build using Open Mowbly SDK
 * 
 * @author Aravind Baskaran <aravind@cloudpact.com>
 */
public class Mowbly extends com.cloudpact.mowbly.android.Mowbly {
	protected static boolean shouldShowSplash = true;
    public static boolean shouldShowSplash() {
		return shouldShowSplash;
	}
	public static void setShouldShowSplash(boolean shouldShowSplash) {
		Mowbly.shouldShowSplash = shouldShowSplash;
	}
	@Override
    protected void configureApplication() {
    	super.configureApplication();
    	basePagePath = "file:///android_asset"+ applicationContext.getResources().getString(R.string.bundle_namespace);
    }
    
}
