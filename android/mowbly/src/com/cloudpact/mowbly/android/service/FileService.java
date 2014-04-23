package com.cloudpact.mowbly.android.service;

import java.io.BufferedReader;
import java.io.ByteArrayOutputStream;
import java.io.File;
import java.io.FileInputStream;
import java.io.FileOutputStream;
import java.io.FileWriter;
import java.io.IOException;
import java.io.InputStream;
import java.io.InputStreamReader;
import java.io.OutputStream;
import java.io.OutputStreamWriter;
import java.io.Writer;
import java.util.Date;

import android.content.Context;
import android.os.Environment;
import android.text.format.DateUtils;
import android.util.Base64;

import com.google.inject.Inject;
import com.google.inject.name.Named;

/**
 * FileService - To maintain files and directories for the Open Mowbly
 * 
 * @author Prashanth Jonnala <prashanth@cloudpact.com>
 */
public class FileService {

    public static final int INTERNAL_STORAGE = 0;
    public static final int EXTERNAL_STORAGE = 1;
    public static final int CACHE = 2;

    public static final int LEVEL_APPLICATION = 1;
    public static final int LEVEL_STORAGE = 2;
    public static final int LEVEL_DOCUMENT = 3;

    public static final int FILE = 0;
    public static final int DIRECTORY = 1;

    public static final String APPLICATION = "Application";
    public static final String DOCUMENTS = "Documents";
    public static final String LOGS = "__logs";

    protected Context context;

    @Inject
    public FileService(@Named("Application") Context context) {
        this.context = context;
    }
    
    public FileService() {}

    public void cleanup() {
        delete(getInternalDocumentsDir());
        delete(getExternalDocumentsDir());
    }

    public File createDirectoryIfNotExists(File dir) {
        if (!dir.exists()) {
            dir.mkdirs();
        }
        return dir;
    }

    public File getAppDir() {
        return createDirectoryIfNotExists(new File(context.getFilesDir(), APPLICATION));
    }

    public File getCacheDir() {
        return context.getCacheDir();
    }

    public File getLogsDir() {
        return createDirectoryIfNotExists(new File(getDocumentsDir(), LOGS));
    }

    public File getLogsFile() {
        return new File(getLogsDir(), "Mowbly.log");
    }


    private File getInternalDocumentsDir() {
        return createDirectoryIfNotExists(new File(context.getFilesDir(), DOCUMENTS));
    }

    private File getExternalDocumentsDir() {
        String sdDir = "/Android/data/" + context.getPackageName();
        return createDirectoryIfNotExists(new File(Environment.getExternalStorageDirectory(), sdDir));
    }

    protected File getDocumentsDir(int storageType) {
        if (storageType == INTERNAL_STORAGE) {
            return getInternalDocumentsDir();
        } else if (storageType == EXTERNAL_STORAGE) {
            return getExternalDocumentsDir();
        } else {
            return getCacheDir();
        }
    }

    public File getDocumentsDir() {
        return getDocumentsDir(INTERNAL_STORAGE);
    }

    public boolean delete(String path) {
        return delete(new File(path));
    }

    public boolean delete(File f) {
        if (!f.exists()) {
            return true;
        }

        if (f.isDirectory()) {
            for (File child : f.listFiles()) {
                delete(child);
            }
        }
        return f.delete();
    }

    public void truncate(File f) throws IOException {
        Writer writer = new FileWriter(f);
        writer.close();
    }

    public boolean isSDCardReady() {
        return Environment.getExternalStorageState().equals(Environment.MEDIA_MOUNTED);
    }

    public void copy(InputStream in, OutputStream out) throws IOException {
        byte[] buffer = new byte[1024];
        int read;
        while ((read = in.read(buffer)) != -1) {
            out.write(buffer, 0, read);
        }
        in.close();
        out.flush();
    }

    public File getDirectory(String path) {
        File file = new File(path);
        if (!testDirExists(path)) {
            file.mkdirs();
        }
        return file;
    }

    public File getFile(String path) throws IOException {
        File file = new File(path);
        if (!testFileExists(path)) {
            file.getParentFile().mkdirs();
            if (!file.createNewFile()) {
                throw new IOException("Cant create file. Directory by the same name already exists.");
            }
        }
        return file;
    }

    public File[] getFiles(String path) {
        File storagePath = getDirectory(path);
        return (storagePath != null) ? storagePath.listFiles() : null;
    }

    public boolean exists(String path) {
        return exists(new File(path));
    }

    public boolean exists(File path) {
        return path.exists();
    }

    public boolean testFileExists(String path) {
        return testFileExists(new File(path));
    }

    public boolean testFileExists(File path) {
        return (exists(path) && path.isFile());
    }

    public boolean testDirExists(String path) {
        return testDirExists(new File(path));
    }

    public boolean testDirExists(File path) {
        return (exists(path) && path.isDirectory());
    }

    /**
     * Reads the contents of the file as plain text.
     * 
     * @param path The path of the file including the name
     * @return The text content of the file
     */
    public String read(String path) throws IOException {
        FileInputStream fis;
        File f = getFile(path);
        fis = new FileInputStream(f);
        InputStreamReader isr = new InputStreamReader(fis, "UTF-8");
        BufferedReader in = new BufferedReader(isr, 8096);
        StringBuffer strBuf = new StringBuffer("");
        String line;
        while( (line = in.readLine()) != null)
            strBuf.append(line);

        in.close();
        isr.close();
        fis.close();

        return strBuf.toString();
    }

    /**
     * Reads the contents of the file as data.
     * 
     * @param path The name of the file to write
     * @return The data content of the file (Base 64)
     */
    public String readData(String path) throws IOException{
        String fileContent = "";
        ByteArrayOutputStream baos = new ByteArrayOutputStream();
        FileInputStream fis;
        File f = getFile(path);
        fis = new FileInputStream(f);
        byte[] buffer = new byte[1024];
        int numRead = 0;
        while((numRead = fis.read(buffer, 0, buffer.length)) != -1) {
            baos.write(buffer, 0, numRead);
        }
        fileContent = Base64.encodeToString(baos.toByteArray(), 0);
        baos.close();
        fis.close();

        return fileContent;
    }
    /**
     * Writes the provided text content into the specified file.
     * 
     * @param path The name of the file to write
     * @param fileContent The content to write
     * @return status of the write operation
     */
    public boolean write(String path, String fileContent, boolean append) throws IOException {
        FileOutputStream fos;
        fos = new FileOutputStream(getFile(path), append);
        OutputStreamWriter osw = new OutputStreamWriter(fos, "UTF-8");
        osw.write(fileContent);
        osw.close();
        fos.close();

        return true;
    }
    
    /**
     * Writes the provided byte content into the specified file.
     * 
     * @param path The name of the file to write
     * @param fileContent The base64 data to write
     * @return status of the write operation
     */
    public boolean writeData(String path, String fileContent, boolean append) throws IOException {
        FileOutputStream fos;
        fos = new FileOutputStream(getFile(path), append);
        byte[] data = Base64.decode(fileContent.getBytes("UTF-8"), 0);
        fos.write(data);
        fos.flush();
        fos.close();
        return true;
    }

    public int clearCacheDir(File dir, final int numDays) {
        if (dir == null) {
            dir = getCacheDir();
        }

        int deletedFiles = 0;
        if (dir!= null && dir.isDirectory()) {
            for (File child:dir.listFiles()) {
                //first delete subdirectories recursively
                if (child.isDirectory()) {
                    deletedFiles += clearCacheDir(child, numDays);
                }

                //then delete the files and subdirectories in this dir
                //only empty directories can be deleted, so subdirs have been done first
                if (child.lastModified() < new Date().getTime() - numDays * DateUtils.DAY_IN_MILLIS) {
                    if (child.delete()) {
                        deletedFiles++;
                    }
                }
            }
        }
        return deletedFiles;
    }

    public long folderSize(File directory) {
        if (directory == null) {
            return 0;
        }

        File[] files = directory.listFiles();
        if (files == null || files.length == 0) {
            return 0;
        }

        long length = 0;
        for (File file : files) {
            if (file.isFile()) {
                length += file.length();
            } else {
                length += folderSize(file);
            }
        }

        return length;
    }

    public void clearCacheFiles() {
        File dir = context.getCacheDir();
        for (File child : dir.listFiles()) {
            delete(child);
        }
    }

    public void wipeData(boolean includingPrefs) {
        File cache = context.getCacheDir();
        File appDir = new File(cache.getParent());
        if (appDir.exists()) {
            String[] children = appDir.list();
            for (String s : children){
                if (s.equals("lib")) {
                    continue;
                }

                if (s.equals("shared_prefs") && !includingPrefs) {
                    continue;
                }

                deleteDir(new File(appDir, s));
            }
        }
    }

    public boolean deleteDir(File dir) {
        if (dir != null && dir.isDirectory()) {
            String[] children = dir.list();
            for (int i = 0; i < children.length; i++) {
                boolean success = deleteDir(new File(dir, children[i]));
                if (!success) {
                    return false;
                }
            }
        }

        return dir.delete();
    }
}
