package com.cloudpact.mowbly.android.log.handler;

import java.io.File;
import java.io.FileNotFoundException;
import java.io.FileOutputStream;

import com.cloudpact.mowbly.log.handler.StreamHandler;

/**
 * Use this handler to write to a file
 * 
 * @author Prashanth Jonnala <prashanth@cloudpact.com>
 */
public class FileHandler extends StreamHandler {

    protected String filename;

    protected boolean append = true;

    public FileHandler(String filename) throws FileNotFoundException {
        this(filename, true);
    }

    public FileHandler(String filename, boolean append) throws FileNotFoundException {
        this.filename = filename;
        this.append = append;
        setFile(filename, append);
    }

    public FileOutputStream getFile(String filename, boolean append) throws FileNotFoundException {
        FileOutputStream ostream = null;
        try {
            ostream = new FileOutputStream(filename, append);
        } catch (FileNotFoundException e) {
            String parentName = new File(filename).getParent();
            if (parentName != null) {
                File parentDir = new File(parentName);
                if (!parentDir.exists() && parentDir.mkdirs()) {
                    ostream = new FileOutputStream(filename, append);
                } else {
                    throw e;
                }
            } else {
                throw e;
            }
        }
        return ostream;
    }

    public void setFile(String filename, boolean append) throws FileNotFoundException {
        setWriter(getFile(filename, append));
    }

    public void shutdown() {
        super.shutdown();
    }
}
