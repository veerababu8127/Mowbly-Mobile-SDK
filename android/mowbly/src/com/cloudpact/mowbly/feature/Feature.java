package com.cloudpact.mowbly.feature;

import com.cloudpact.mowbly.feature.Binder;

/**
 * Interface for Feature
 * 
 * @author Prashanth Jonnala <prashanth@cloudpact.com>
 */
public interface Feature extends LifeCycle {

    /**
     * Get the name of the feature;
     * 
     * @return The name of the feature
     */
    public String getName();

    /**
     * Set the name of the feature
     * 
     * @param featureName The name of the feature
     */
    public void setName(String featureName);

    /**
     * Get the feature binder associated with the feature
     * 
     * @return The feature binder
     */
    public Binder getBinder();

    /**
     * Set the feature binder associated with the feature
     * 
     * @param binder The feature binder to which the feature is bound
     */
    public void setBinder(Binder binder);
}
