package com.cloudpact.mowbly.android.util;

import java.util.UUID;

import android.content.Context;
import android.content.SharedPreferences;
import android.provider.Settings;
import android.telephony.TelephonyManager;

import com.cloudpact.mowbly.android.service.PreferenceService;

/**
 * Android Utilities.
 * 
 * @author Prashanth Jonnala <prashanth@cloudpact.com>
 */
public class AndroidUtil {

    private static final String PREF_DEVICE_UNIQUE_ID = "__mowbly_device_unique_id";

    private static String generateDeviceUniqueIdentifier(Context context) {
        final TelephonyManager tm = (TelephonyManager) context.getSystemService(Context.TELEPHONY_SERVICE);

        final String tmDevice = "" + tm.getDeviceId();
        final String tmSerial = "" + tm.getSimSerialNumber();
        final String androidId = "" + Settings.Secure.getString(context.getContentResolver(), Settings.Secure.ANDROID_ID);
        final String packageBasedAndroidId = context.getPackageName() + androidId;

        UUID deviceUuid = new UUID(packageBasedAndroidId.hashCode(), ((long)tmDevice.hashCode() << 32) | tmSerial.hashCode());
        return deviceUuid.toString();
    }

    public static String getDeviceUniqueIdentifier(Context context) {
        SharedPreferences prefs = context.getSharedPreferences(PreferenceService.FRAMEWORK_PREFERENCES, Context.MODE_PRIVATE);
        String uid = prefs.getString(PREF_DEVICE_UNIQUE_ID, null);
        if (uid == null) {
            uid = generateDeviceUniqueIdentifier(context);
            prefs.edit().putString(PREF_DEVICE_UNIQUE_ID, uid).commit();
        }
        return uid;
    }
}
