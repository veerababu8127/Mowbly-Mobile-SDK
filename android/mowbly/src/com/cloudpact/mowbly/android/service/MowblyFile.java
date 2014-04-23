package com.cloudpact.mowbly.android.service;

import java.io.File;

import com.cloudpact.mowbly.android.Mowbly;

/**
 * MowblyFile - File object for managing access within Open Mowbly
 * 
 * @author Prashanth Jonnala <prashanth@cloudpact.com>
 */
public class MowblyFile {

	protected static final FileService fileService = Mowbly.getFileService();
	
    protected int storageType = MowblyFileServiceUtil.DEFAULT_STORAGE_TYPE;
    protected int storageLevel = MowblyFileServiceUtil.DEFAULT_STORAGE_LEVEL;
    private String path = "/";

    public MowblyFile() {
        this(null);
    }

    public MowblyFile(String path) {
        setPath(path);
    }

    public int getStorageType() {
        return storageType;
    }

    public MowblyFile setStorageType(int storageType) {
        this.storageType = storageType;
        return this;
    }

    public int getStorageLevel() {
        return storageLevel;
    }

    public MowblyFile setStorageLevel(int storageLevel) {
        this.storageLevel = storageLevel;
        return this;
    }

    public String getPath() {
        return path;
    }

    public MowblyFile setPath(String path) {
        path = (path != null) ? path : "";
        if (path.startsWith("file:///")) {
            path = path.substring(7);
        }
        if (!path.startsWith("/")) {
            path = "/" + path;
        }
        this.path = path;
        return this;
    }

    public File getAbsoluteFile() {
        File dir = null;
        if (storageLevel == FileService.LEVEL_STORAGE) {
            return new File(path);
        } else/* if (storageLevel == FileService.LEVEL_DOCUMENT) */{
            dir = fileService.getDocumentsDir();
        }

        if (path == null || path.equals("/")) {
            return dir;
        }

        return new File(dir, path);
    }

    public String getAbsolutePath() {
        return getAbsoluteFile().getAbsolutePath();
    }
}
