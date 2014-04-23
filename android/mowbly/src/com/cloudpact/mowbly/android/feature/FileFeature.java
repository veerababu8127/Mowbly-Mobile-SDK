package com.cloudpact.mowbly.android.feature;

import java.io.File;
import java.io.IOException;

import com.cloudpact.mowbly.android.Mowbly;
import com.cloudpact.mowbly.android.service.FileService;
import com.cloudpact.mowbly.android.util.ZipUtils;
import com.cloudpact.mowbly.feature.BaseFeature;
import com.cloudpact.mowbly.feature.Response;
import com.google.gson.JsonArray;
import com.google.gson.JsonObject;

/**
 * Javascript interface for the File feature
 * 
 * @author Prashanth Jonnala <prashanth@cloudpact.com>
 */
public class FileFeature extends BaseFeature {

    /** Exposed name of the FrameworkFeature */
    public static final String NAME = "file";

    private static final int STORAGE_NOT_READY_CODE = -1;
    private static final String STORAGE_NOT_READY_ERROR = "External storage is not ready";

    private FileService fileService = null;

    public FileFeature() {
        super(NAME);
    }

    protected FileService getFileService() {
        if (fileService == null) {
            fileService = Mowbly.getFileService();
        }
        return fileService;
    }

    private class StorageNotReadyException extends Exception {
        private static final long serialVersionUID = -1687009569583329961L;
    }

    protected File getAbsoluteFile(String path, JsonObject options) throws StorageNotReadyException {
        options.addProperty("path", path);
        return Mowbly.getMowblyFileServiceUtil().getFileForOptions(options);
    }

    protected String getAbsolutePath(String path, JsonObject options) throws StorageNotReadyException {
        return getAbsoluteFile(path, options).getAbsolutePath();
    }

    @Method(async = true, args = {
        @Argument(name = "path", type = String.class),
        @Argument(name = "options", type = JsonObject.class)
    })
    public Response getRootDirectory(String path, JsonObject options) {
        Response response = new Response();
        try {
            path = getAbsolutePath(path, options);
            response.setCode(1);
            response.setResult(path);
        } catch (StorageNotReadyException e) {
            response.setCode(STORAGE_NOT_READY_CODE);
            response.setError(STORAGE_NOT_READY_ERROR);
        }
        return response;
    }

    @Method(async = true, args = {
        @Argument(name = "path", type = String.class),
        @Argument(name = "options", type = JsonObject.class)
    })
    public Response testDirExists(String path, JsonObject options) {
        Response response = new Response();
        try {
            path = getAbsolutePath(path, options);
            boolean dirExists = getFileService().testDirExists(path);
            response.setCode(1);
            response.setResult(dirExists);
        } catch (StorageNotReadyException e) {
            response.setCode(STORAGE_NOT_READY_CODE);
            response.setError(STORAGE_NOT_READY_ERROR);
        }
        return response;
    }

    @Method(async = true, args = {
        @Argument(name = "path", type = String.class),
        @Argument(name = "options", type = JsonObject.class)
    })
    public Response testFileExists(String path, JsonObject options) {
        Response response = new Response();
        try {
            path = getAbsolutePath(path, options);
            boolean fileExists = getFileService().testFileExists(path);
            response.setCode(1);
            response.setResult(fileExists);
        } catch (StorageNotReadyException e) {
            response.setCode(STORAGE_NOT_READY_CODE);
            response.setError(STORAGE_NOT_READY_ERROR);
        }
        return response;
    }

    @Method(async = true, args = {
        @Argument(name = "path", type = String.class),
        @Argument(name = "options", type = JsonObject.class)
    })
    public Response getDirectory(String path, JsonObject options) {
        Response response = new Response();
        try {
            path = getAbsolutePath(path, options);
            File dir = getFileService().getDirectory(path);
            response.setCode(1);
            response.setResult(dir.getAbsolutePath());
        } catch (StorageNotReadyException e) {
            response.setCode(STORAGE_NOT_READY_CODE);
            response.setError(STORAGE_NOT_READY_ERROR);
        }
        return response;
    }

    @Method(async = true, args = {
        @Argument(name = "path", type = String.class),
        @Argument(name = "options", type = JsonObject.class)
    })
    public Response getFile(String path, JsonObject options) {
        Response response = new Response();
        try {
            path = getAbsolutePath(path, options);
            File file = getFileService().getFile(path);
            response.setCode(1);
            response.setResult(file.getAbsolutePath());
        } catch (StorageNotReadyException e) {
            response.setCode(0);
            response.setError(STORAGE_NOT_READY_ERROR);
        } catch (IOException e) {
            response.setCode(STORAGE_NOT_READY_CODE);
            response.setError("Error retrieving file");
        }
        return response;
    }

    @Method(async = true, args = {
        @Argument(name = "path", type = String.class),
        @Argument(name = "options", type = JsonObject.class)
    })
    public Response deleteDirectory(String path, JsonObject options) {
        Response response = new Response();
        try {
            path = getAbsolutePath(path, options);
            boolean status = getFileService().delete(path);
            response.setCode(1);
            response.setResult(status);
        } catch (StorageNotReadyException e) {
            response.setCode(STORAGE_NOT_READY_CODE);
            response.setError(STORAGE_NOT_READY_ERROR);
        }
        return response;
    }

    @Method(async = true, args = {
        @Argument(name = "path", type = String.class),
        @Argument(name = "options", type = JsonObject.class)
    })
    public Response deleteFile(String path, JsonObject options) {
        Response response = new Response();
        try {
            path = getAbsolutePath(path, options);
            boolean status = getFileService().delete(path);
            response.setCode(1);
            response.setResult(status);
        } catch (StorageNotReadyException e) {
            response.setCode(STORAGE_NOT_READY_CODE);
            response.setError(STORAGE_NOT_READY_ERROR);
        }
        return response;
    }

    @Method(async = true, args = {
        @Argument(name = "path", type = String.class),
        @Argument(name = "options", type = JsonObject.class)
    })
    public Response read(String path, JsonObject options) {
        Response response = new Response();
        try {
            path = getAbsolutePath(path, options);
            if (getFileService().testFileExists(path)) {
                String data = getFileService().read(path);
                response.setCode(1);
                response.setResult(data);
            } else {
                response.setCode(0);
                response.setError("File doesn't exist");
            }
        } catch (StorageNotReadyException e) {
            response.setCode(0);
            response.setError(STORAGE_NOT_READY_ERROR);
        } catch (IOException e) {
            response.setCode(0);
            response.setError("", e.getMessage());
        }
        return response;
    }

    @Method(async = true, args = {
        @Argument(name = "path", type = String.class),
        @Argument(name = "options", type = JsonObject.class)
    })
    public Response readData(String path, JsonObject options) {
        Response response = new Response();
        try {
            path = getAbsolutePath(path, options);
            String data = getFileService().readData(path);
            response.setCode(1);
            response.setResult(data);
        } catch (StorageNotReadyException e) {
            response.setCode(STORAGE_NOT_READY_CODE);
            response.setError(STORAGE_NOT_READY_ERROR);
        } catch (IOException e) {
            response.setCode(0);
            response.setError("");
        }
        return response;
    }

    @Method(async = true, args = {
        @Argument(name = "path", type = String.class),
        @Argument(name = "fileContent", type = String.class),
        @Argument(name = "mode", type = boolean.class),
        @Argument(name = "options", type = JsonObject.class)
    })
    public Response write(String path, String fileContent, boolean mode, JsonObject options) {
        Response response = new Response();
        try {
            path = getAbsolutePath(path, options);
            boolean status = getFileService().write(path, fileContent, mode);
            response.setCode(1);
            response.setResult(status);
        } catch (StorageNotReadyException e) {
            response.setCode(STORAGE_NOT_READY_CODE);
            response.setError(STORAGE_NOT_READY_ERROR);
        } catch (IOException e) {
            response.setCode(0);
            response.setError("Error in writing to file", e.getMessage());
        }
        return response;
    }

    @Method(async = true, args = {
        @Argument(name = "path", type = String.class),
        @Argument(name = "fileContent", type = String.class),
        @Argument(name = "mode", type = boolean.class),
        @Argument(name = "options", type = JsonObject.class)
    })
    public Response writeData(String path, String fileContent, boolean mode , JsonObject options) {
        Response response = new Response();
        try {
            path = getAbsolutePath(path, options);
            boolean status = getFileService().writeData(path, fileContent, mode);
            response.setCode(1);
            response.setResult(status);
        } catch (StorageNotReadyException e) {
            response.setCode(STORAGE_NOT_READY_CODE);
            response.setError(STORAGE_NOT_READY_ERROR);
        } catch (IOException e) {
            response.setCode(0);
            response.setError(e.getMessage());
        }
        return response;
    }

    /**
     * Returns the list of files in the specified directory as Json.
     * 
     * @param path The path of the directory with name
     * @return Json containing the list of directories and files
     */
    @Method(async = true, args = {
        @Argument(name = "path", type = String.class),
        @Argument(name = "options", type = JsonObject.class)
    })
    public Response getFilesJSONString(String path, JsonObject options) {
        Response response = new Response();
        try {
            path = getAbsolutePath(path, options);
            JsonArray fileList = new JsonArray();

            java.io.File[] files = getFileService().getFiles(path);
            if (files != null) {
                for (int i = 0, length = files.length; i < length; i++) {
                    JsonObject file = new JsonObject();
                    file.addProperty("name", files[i].getName());
                    file.addProperty("type", files[i].isDirectory() ? FileService.DIRECTORY : FileService.FILE);
                    fileList.add(file);
                }
            }

            response.setCode(1);
            response.setResult(fileList);
        } catch (StorageNotReadyException e) {
            response.setCode(STORAGE_NOT_READY_CODE);
            response.setError(STORAGE_NOT_READY_ERROR);
        }
        return response;
    }

    @Method(async = true, args = {
        @Argument(name = "srcOptions", type = JsonObject.class),
        @Argument(name = "destOptions", type = JsonObject.class)
    })
    public Response unzip(JsonObject srcOptions, JsonObject destOptions) {
        Response response = new Response();
        try {
            String srcPath = srcOptions.get("path").getAsString();
            String destPath = destOptions.get("path").getAsString();
            File src = getAbsoluteFile(srcPath, srcOptions);
            File dest = getAbsoluteFile(destPath, destOptions);

            ZipUtils.unzip(src, dest);

            response.setCode(1);
        } catch (StorageNotReadyException e) {
            response.setCode(STORAGE_NOT_READY_CODE);
            response.setError(STORAGE_NOT_READY_ERROR);
        } catch (IOException e) {
            response.setCode(0);
            response.setError("Error occured while extracting file", e.getMessage());
        }
        return response;
    }
}