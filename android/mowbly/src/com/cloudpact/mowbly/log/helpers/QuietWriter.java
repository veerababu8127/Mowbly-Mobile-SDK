package com.cloudpact.mowbly.log.helpers;

import java.io.IOException;
import java.io.InterruptedIOException;
import java.io.Writer;

/**
 * Utility Writer which gracefully supresses the exceptions 
 * occured while doing IO on the stream
 * 
 * @author Prashanth Jonnala <prashanth@cloudpact.com>
 */
public class QuietWriter extends Writer {

    protected Writer out;

    public QuietWriter(Writer out) {
        super(out);
        this.out = out;
    }

    public void write(String message) {
        try {
            out.write(message);
        } catch (IOException e) {
            System.err.println("Could not write to the stream, " + e.getMessage());
        }
    }

    public void write(char[] cbuf, int off, int len) {
        try {
            out.write(cbuf, off, len);
        } catch (IOException e) {
            System.err.println("Could not write to the stream, " + e.getMessage());
        }
    }

    public void write(String str, int off, int len) {
        try {
            out.write(str, off, len);
        } catch (IOException e) {
            System.err.println("Could not write to the stream, " + e.getMessage());
        }
    }

    public void flush() {
        try {
            out.flush();
        } catch (IOException e) {
            System.err.println("Could not flush the stream, " + e.getMessage());
        }
    }

    public void close() {
        try {
            out.close();
        } catch (IOException e) {
            if (e instanceof InterruptedIOException) {
                Thread.currentThread().interrupt();
            }
        }
    }
}