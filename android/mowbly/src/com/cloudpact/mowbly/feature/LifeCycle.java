package com.cloudpact.mowbly.feature;

/**
 * LifeCycle interface
 * 
 * @author Prashanth Jonnala <prashanth@cloudpact.com>
 */
public interface LifeCycle {

    /** Life cycle listener for create */
    public void onCreate();

    /** Life cycle listener for start */
    public void onStart();

    /**  Life cycle listener for resume */
    public void onResume();

    /** Life cycle listener for pause */
    public void onPause();

    /** Life cycle listener for stop */
    public void onStop();

    /** Life cycle listener for destroy */
    public void onDestroy();
}
