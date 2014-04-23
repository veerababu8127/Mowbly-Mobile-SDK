package com.cloudpact.mowbly.android.util;

import java.io.BufferedInputStream;
import java.io.BufferedOutputStream;
import java.io.File;
import java.io.FileOutputStream;
import java.io.IOException;
import java.util.Enumeration;
import java.util.zip.ZipEntry;
import java.util.zip.ZipFile;

/**
 * Zip utilities
 * 
 * @author Prashanth Jonnala <prashanth@cloudpact.com>
 */
public class ZipUtils {

    public static void unzip(File file, File outputFolder) throws IOException {
        int b;
        byte[] buffer = new byte[1024];

        if (!outputFolder.exists()) {
            outputFolder.mkdir();
        }

        ZipFile zipFile = new ZipFile(file);
        Enumeration<? extends ZipEntry> e = zipFile.entries();
        while (e.hasMoreElements()) {
            ZipEntry entry = e.nextElement();
            File newFile = new File(outputFolder, entry.getName());
            new File(newFile.getParent()).mkdirs();
            if (!entry.isDirectory()) {
                BufferedInputStream bis = new BufferedInputStream(zipFile.getInputStream(entry), 8);
                FileOutputStream fos = new FileOutputStream(newFile);
                BufferedOutputStream bos = new BufferedOutputStream(fos, 1024);
                while ((b = bis.read(buffer, 0, 1024)) != -1) {
                    bos.write(buffer, 0, b);
                }
                bos.flush();
                bos.close();
                bis.close();
            }
        }
        zipFile.close();
    }

    public static void unzip(String zipFile, String outputFolder) throws IOException {
        unzip(new File(zipFile), new File(outputFolder));
    }
}
