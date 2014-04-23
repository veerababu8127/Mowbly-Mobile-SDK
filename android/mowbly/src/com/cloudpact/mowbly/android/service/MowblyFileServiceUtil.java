package com.cloudpact.mowbly.android.service;

import java.io.File;

import com.cloudpact.mowbly.android.Mowbly;
import com.google.gson.JsonObject;

/**
 * Utilities for creating {@link MowblyFile} references 
 * 
 * @author Aravind Baskaran <aravind@cloudpact.com>
 */
public class MowblyFileServiceUtil {
	protected static final int DEFAULT_STORAGE_TYPE = FileService.INTERNAL_STORAGE;
    protected static final int DEFAULT_STORAGE_LEVEL = FileService.LEVEL_APPLICATION;

    public MowblyFile fromFileOptions(JsonObject options) {
        MowblyFile file = Mowbly.createMowblyFile();
        if (options.get("storageType") != null) {
            file.setStorageType(options.get("storageType").getAsInt());
        }
        if (options.get("level") != null) {
            file.setStorageLevel(options.get("level").getAsInt());
        }
        if (options.get("path") != null) {
            file.setPath(options.get("path").getAsString());
        }
        return file;
    }
    
    public File getFileForOptions(JsonObject options) {
    	return fromFileOptions(options)
                .getAbsoluteFile();
    }
    public String getPathForFileOptions(JsonObject options) {
        return fromFileOptions(options)
                .getAbsolutePath();
    }
}
