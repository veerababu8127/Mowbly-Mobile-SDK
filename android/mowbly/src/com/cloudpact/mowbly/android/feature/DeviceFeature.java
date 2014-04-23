package com.cloudpact.mowbly.android.feature;

import java.io.File;

import android.os.Environment;
import android.os.StatFs;

import com.cloudpact.mowbly.feature.BaseFeature;
import com.cloudpact.mowbly.feature.Response;
import com.cloudpact.mowbly.android.Mowbly;
import com.cloudpact.mowbly.android.ui.PageActivity;
import com.cloudpact.mowbly.android.util.AndroidUtil;
import com.google.gson.JsonObject;

/**
 * Javascript interface for the Device feature
 * 
 * @author Prashanth Jonnala <prashanth@cloudpact.com>
 */
@SuppressWarnings("deprecation")
public class DeviceFeature extends BaseFeature {

    /** Exposed name of the DeviceFeature */
    public static final String NAME = "device";

    private static final StatFs stat = new StatFs(Environment.getExternalStorageDirectory().getPath());

    public DeviceFeature() {
        super(NAME);
    }

    @Method(async = true, args={})
    public Response getDeviceId() {
        final PageActivity activity = ((FeatureBinder) binder).getActivity();
        String deviceId = AndroidUtil.getDeviceUniqueIdentifier(activity);

        Response response = new Response();
        response.setResult(deviceId);
        return response;
    }

    @Method(async = true, args={})
    public Response getMemoryStatus() {
        Response response = new Response();
        response.setResult(getMemoryStats());
        return response;
    }

    private boolean externalMemoryAvailable() {
        return Environment.getExternalStorageState().equals(android.os.Environment.MEDIA_MOUNTED);
    }

    private long getAvailableInternalMemorySize() {
        File path = Environment.getDataDirectory();
        stat.restat(path.getAbsolutePath());
		long blockSize = stat.getBlockSize();
        long availableBlocks = stat.getAvailableBlocks();
        return availableBlocks * blockSize;
    }

    private long getTotalInternalMemorySize() {
        File path = Environment.getDataDirectory();
        stat.restat(path.getAbsolutePath());
        long blockSize = stat.getBlockSize();
        long totalBlocks = stat.getBlockCount();
        return totalBlocks * blockSize;
    }

    private long getAvailableExternalMemorySize() {
        if (externalMemoryAvailable()) {
            File path = Environment.getExternalStorageDirectory();
            stat.restat(path.getAbsolutePath());
            long blockSize = stat.getBlockSize();
            long availableBlocks = stat.getAvailableBlocks();
            return availableBlocks * blockSize;
        } else {
            return -1;
        }
    }

    private long getTotalExternalMemorySize() {
        if (externalMemoryAvailable()) {
            File path = Environment.getExternalStorageDirectory();
            stat.restat(path.getAbsolutePath());
            long blockSize = stat.getBlockSize();
            long totalBlocks = stat.getBlockCount();
            return totalBlocks * blockSize;
        } else {
            return -1;
        }
    }

    private long getApplicationMemory() {
        return folderSize(Mowbly.getFileService().getAppDir());
        /*
        String appDir = Mowbly.getFileService().getAppDir().getAbsolutePath();
        stat.restat(appDir);
        long blockSize = stat.getBlockSize();
        long totalBlocks = stat.getBlockCount();
        long availBlocks = stat.getAvailableBlocks();
        return (totalBlocks - availBlocks) * blockSize;
        */
    }

    private String formatSize(long size) {
        String suffix = "MB";

        if (size >= 1024) {
            suffix = "KB";
            size /= 1024;
            if (size >= 1024) {
                suffix = "MB";
                size /= 1024;
                if (size >= 1024) {
                    suffix = "GB";
                    size /= 1024;
                }
            }
        }
        StringBuilder resultBuffer = new StringBuilder(Long.toString(size));
        resultBuffer.append(suffix);
        return resultBuffer.toString();
    }

    private long folderSize(File directory) {
        long length = 0;
        for (File file : directory.listFiles()) {
            if (file.isFile())
                length += file.length();
            else
                length += folderSize(file);
        }
        return length;
    }

    private JsonObject getMemoryStats() {
        JsonObject ms = new JsonObject();
        ms.addProperty("applicationMemory", formatSize(getApplicationMemory()));
        ms.addProperty("availableExternalMemory", formatSize(getAvailableExternalMemorySize()));
        ms.addProperty("availableInternalMemory", formatSize(getAvailableInternalMemorySize()));
        ms.addProperty("totalExternalMemory", formatSize(getTotalExternalMemorySize()));
        ms.addProperty("totalInternalMemory", formatSize(getTotalInternalMemorySize()));
        return ms;
    }
}
