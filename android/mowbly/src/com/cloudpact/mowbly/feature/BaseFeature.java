package com.cloudpact.mowbly.feature;

import com.cloudpact.mowbly.feature.Binder;

/**
 * Abstact implementation of Feature
 * 
 * @author Prashanth Jonnala <prashanth@cloudpact.com>
 */
public class BaseFeature implements Feature {

    /** The name of the feature */
    protected String name;

    protected Binder binder;

    public BaseFeature(String name) {
        setName(name);
    }

    public String getName() {
        return name;
    }

    public void setName(String featureName) {
        name = featureName;
    }

    public Binder getBinder() {
        return binder;
    }

    public void setBinder(Binder binder) {
        this.binder = binder;
    }

    public void onCreate() {
    }

    public void onStart() {
    }

    public void onResume() {
    }

    public void onPause() {
    }

    public void onStop() {
    }

    public void onDestroy() {
    }
}
