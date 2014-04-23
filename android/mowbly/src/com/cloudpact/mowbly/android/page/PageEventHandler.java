package com.cloudpact.mowbly.android.page;

/**
 * PageEventHandler
 * 
 * @author Prashanth Jonnala <prashanth@cloudpact.com>
 */
public interface PageEventHandler {

    /**
     * Event fired when the page is loaded for the first time
     */
    public void onPageLoad();

    /**
     * Event fired when the user presses back button
     */
    public void onDeviceBackPressed();

    /**
     * Event fired when the child sends the result back to parent
     */
    public void onPageReopen(boolean poppedFromStack);

    /**
     * Invoked when an Async Feature method returns
     */
    public void onCallbackReceive(String callbackId, String result);

    /**
     * Invoked when the PageViewClient encountered an error opening the page
     * 
     * @param errorCode The error code
     * @param description The error description
     * @param failingUrl The failing Url
     */
    public void onReceivedError(int errorCode, String description, String failingUrl);
}
