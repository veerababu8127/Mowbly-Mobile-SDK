package com.cloudpact.mowbly.android.page;

/**
 * No operation Page event Handler
 * 
 * @author Prashanth Jonnala <prashanth@cloudpact.com>
 */
public class NOPPageEventHandler implements PageEventHandler {

    @Override
    public void onPageLoad() {
    }

    @Override
    public void onDeviceBackPressed() {
    }

    @Override
    public void onPageReopen(boolean poppedFromStack) {
    }

    @Override
    public void onCallbackReceive(String callbackId, String result) {
    }

    @Override
    public void onReceivedError(int errorCode, String description, String failingUrl) {
    }
}
