package com.cloudpact.mowbly.feature;

import com.cloudpact.mowbly.feature.Feature;
import com.cloudpact.mowbly.feature.LifeCycle;

/**
 * Interface for binding the features
 * 
 * @author Prashanth Jonnala <prashanth@cloudpact.com>
 */
public interface Binder extends LifeCycle {

    /**
     * Get the name associated with the binder
     * 
     * @return The name of the binder
     */
    public String getName();

    /**
     * Set the name associated with the binder
     * 
     * @param binderName The name of the binder
     */
    public void setName(String binderName);

    /**
     * Binds the feature to make it available
     * 
     * @param feature The feature to be bound
     */
    public void bind(Feature feature);

    /**
     * Returns the feature bound by the provided name
     * 
     * @param featureName Name of the feature
     * @return Feature The requested feature
     */
    public Feature get(String featureName);

    /**
     * Invoke a method on the feature
     * 
     * @param featureName The name of the feature
     * @param method The method in the feature to be invoked
     * @param jsonArgs The string representation of json arguments
     * @param callbackId The callback
     * @return The string representation of the result returned by the feature method
     */
    public String invoke(String featureName, String method, String jsonArgs, String callbackId);
}
