package com.cloudpact.mowbly.android.ui;

import android.content.Intent;
import android.os.Bundle;

import com.cloudpact.mowbly.android.standalone.Mowbly;

/**
 * OpenPageActivity - Starting activity of the Open Mowbly
 * 
 * @author Prashanth Jonnala <prashanth@cloudpact.com>, Aravind Baskaran <aravind@cloudpact.com>
 */
public class OpenPageActivity extends PageActivity {
	
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
    }

    @Override
    protected boolean shouldOpenSplash() {
    	boolean shouldShowSplash = Mowbly.shouldShowSplash();
    	Mowbly.setShouldShowSplash(false);
        return shouldShowSplash;
    }

    @Override
    protected Intent getSplashIntent() {
        return new Intent(OpenPageActivity.this, OpenSplashActivity.class);
    }

    @Override
    protected void enterTheDragon() {
        if (activeFragment == null) {
            openHomePage();
        }
    }
}
