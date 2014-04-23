package com.cloudpact.mowbly.log.helpers;

import java.io.IOException;
import java.io.Writer;

public class CountingQuietWriter extends QuietWriter {

    protected long count;

    public CountingQuietWriter(Writer writer) {
        super(writer);
    }

    public long getCount() {
        return count;
    }

    public void setCount(long count) {
        this.count = count;
    }

    public void write(String string) {
        try {
            out.write(string);
            count += string.length();
        } catch (IOException e) {
            System.err.println("Could not write to the stream, " + e.getMessage());
        }
    }
}
