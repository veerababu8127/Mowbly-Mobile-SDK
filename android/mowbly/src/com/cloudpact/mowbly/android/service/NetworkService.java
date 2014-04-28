package com.cloudpact.mowbly.android.service;

import android.content.Context;
import android.net.ConnectivityManager;
import android.net.NetworkInfo;
import android.telephony.TelephonyManager;

import com.cloudpact.mowbly.log.Logger;
import com.google.inject.Inject;
import com.google.inject.name.Named;

/**
 * NetworkService
 * 
 * @author Prashanth Jonnala <prashanth@cloudpact.com>
 */
public class NetworkService {

    public static final int NETWORK_NONE = 0;
    public static final int NETWORK_WIFI = 1;
    public static final int NETWORK_2G = 2;
    public static final int NETWORK_3G = 3;

    private Context context;

    private ConnectivityManager connectivityManager;
    
    private static final String TAG = "NetworkService";
    private static Logger logger = Logger.getLogger();;

    @Inject
    public NetworkService(@Named("Application") Context context) {
        this.context = context;
    }

    private ConnectivityManager getConnectivityManager() {
        if (connectivityManager == null) {
            connectivityManager = (ConnectivityManager) context.getSystemService(Context.CONNECTIVITY_SERVICE);
        }
        return connectivityManager;
    }

    public boolean isConnected() {
        boolean isConnected = false;
        if(getActiveNetwork() != 0){
        	NetworkInfo wifi = getConnectivityManager().getNetworkInfo(ConnectivityManager.TYPE_WIFI);
        	NetworkInfo mobile = getConnectivityManager().getNetworkInfo(ConnectivityManager.TYPE_MOBILE);
	        if ((wifi != null && wifi.isConnected()) || (mobile != null && mobile.isConnected())) {
	            isConnected = true;
	        }
    	}
        return isConnected;
    }

    public int getActiveNetwork() {
        int code = NETWORK_NONE;
        NetworkInfo info = null;
        try{
        	 info = getConnectivityManager().getActiveNetworkInfo();
        }catch(SecurityException e){
        	logger.error(TAG, "Network permission not provided");
        }
        if (info != null) {
            int netType = info.getType();
            int subnetType = info.getSubtype();
            if (netType == ConnectivityManager.TYPE_WIFI) {
                code = NETWORK_WIFI;
            } else if (netType == ConnectivityManager.TYPE_MOBILE) {
                if (subnetType == TelephonyManager.NETWORK_TYPE_CDMA ||
                        subnetType == TelephonyManager.NETWORK_TYPE_EDGE ||
                        subnetType == TelephonyManager.NETWORK_TYPE_GPRS ||
                        subnetType == TelephonyManager.NETWORK_TYPE_IDEN) {

                    code = NETWORK_2G;
                } else if (subnetType == TelephonyManager.NETWORK_TYPE_1xRTT ||
                        subnetType == TelephonyManager.NETWORK_TYPE_EHRPD ||
                        subnetType == TelephonyManager.NETWORK_TYPE_EVDO_0 ||
                        subnetType == TelephonyManager.NETWORK_TYPE_EVDO_A ||
                        subnetType == TelephonyManager.NETWORK_TYPE_EVDO_B ||
                        subnetType == TelephonyManager.NETWORK_TYPE_HSDPA ||
                        subnetType == TelephonyManager.NETWORK_TYPE_HSPA ||
                        subnetType == TelephonyManager.NETWORK_TYPE_HSUPA ||
                        subnetType == TelephonyManager.NETWORK_TYPE_LTE ||
                        subnetType == TelephonyManager.NETWORK_TYPE_UMTS) {

                    code = NETWORK_3G;
                } else {

                    code = NETWORK_2G;
                }
            }
        }
        return code;
    }

    public boolean isRoaming() {
        TelephonyManager manager = (TelephonyManager) context.getSystemService(Context.TELEPHONY_SERVICE);
        return manager.isNetworkRoaming();
    }
}
