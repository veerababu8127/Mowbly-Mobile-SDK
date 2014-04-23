package com.cloudpact.mowbly.android.log.handler;

import java.io.File;
import java.io.FileNotFoundException;
import java.io.IOException;
import java.io.InterruptedIOException;
import java.io.Writer;

import com.cloudpact.mowbly.log.LogEvent;
import com.cloudpact.mowbly.log.helpers.CountingQuietWriter;

/**
 * This handler is same like the {@link FileHandler} but rotates the log files
 * 
 * @author Prashanth Jonnala <prashanth@cloudpact.com>
 */
public class RotatingFileHandler extends FileHandler {

    /** Maximum file size. Defaults to 10MB */
    protected long maxFileSize = 10 * 1024 * 1024;

    /** Number of backup files */
    protected int maxBackupFiles = 1;

    private long nextRollover = 0;

    public RotatingFileHandler(String filename) throws FileNotFoundException {
        this(filename, true);
    }

    public RotatingFileHandler(String filename, boolean append) throws FileNotFoundException {
        super(filename, append);

        setFile(filename, append);
    }

    public long getMaxFileSize() {
        return maxFileSize;
    }

    public void setMaxFileSize(long maxFileSize) {
        this.maxFileSize = maxFileSize;
    }

    public int getMaxBackupFiles() {
        return maxBackupFiles;
    }

    public void setMaxBackupFiles(int maxBackupFiles) {
        this.maxBackupFiles = maxBackupFiles;
    }

    public void setFile(String filename, boolean append) throws FileNotFoundException {
        Writer w = getWriter(getFile(filename, append));
        setWriter(new CountingQuietWriter(w));

        if (append) {
            File f = new File(filename);
            ((CountingQuietWriter) writer).setCount(f.length());
        }
    }

    private void rotate() {
        if (writer != null) {
            long size = ((CountingQuietWriter) writer).getCount();
            nextRollover = size + maxFileSize;
            writer.close();
        }

        boolean renameSucceeded = true;
        if (maxBackupFiles > 0) {
            File target;
            File file;

            file = new File(filename + '.' + maxBackupFiles);
            if (file.exists()) {
                renameSucceeded = file.delete();
            }

            for (int i = maxBackupFiles - 1; i >= 0 && renameSucceeded; i--) {
                file = (i > 0) ? new File(filename + "." + i) : new File(filename);
                if (file.exists()) {
                    target = new File(filename + '.' + (i + 1));
                    renameSucceeded = file.renameTo(target);
                }
            }

            if (!renameSucceeded) {
                try {
                  setFile(filename, true);
                } catch(IOException e) {
                    if (e instanceof InterruptedIOException) {
                        Thread.currentThread().interrupt();
                    }
                }
            }
        }

        if (renameSucceeded) {
            try {
                setFile(filename, false);
                nextRollover = 0;
            } catch(IOException e) {
                if (e instanceof InterruptedIOException) {
                    Thread.currentThread().interrupt();
                }
            }
        }
    }

    public void handle(LogEvent logEvent) {
        super.handle(logEvent);
        if (writer != null) {
            long size = ((CountingQuietWriter) writer).getCount();
            if (size >= maxFileSize && size >= nextRollover) {
                rotate();
            }
        }
    }
}
