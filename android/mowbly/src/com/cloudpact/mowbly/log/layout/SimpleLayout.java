package com.cloudpact.mowbly.log.layout;

import com.cloudpact.mowbly.log.LogEvent;

/**
 * Default implementation of the {@link Layout}
 * 
 * @author Prashanth Jonnala <prashanth@cloudpact.com>
 */
public class SimpleLayout implements Layout {

    //final static public String LINE_SEPARATOR = System.getProperty("line.separator");
    final static public String LINE_SEPARATOR = "\r\n";

    final static public int LINE_SEPARATOR_LENGTH  = LINE_SEPARATOR.length();

    final static protected int DEFAULT_BUFFER_SIZE = 128;

    final static protected int DEFAULT_MAX_BUFFER_CAPACITY = 1024;

    protected StringBuffer buffer;

    public SimpleLayout() {
        this(DEFAULT_BUFFER_SIZE);
    }

    public SimpleLayout(int bufferSize) {
        buffer = new StringBuffer(bufferSize);
    }

    public String getContentType() {
        return "text/plain";
    }

    public String getHeader() {
        return null;
    }

    public String getFooter() {
        return null;
    }

    public String format(LogEvent logEvent) {
        buffer.setLength(0);
        buffer.append(logEvent.getTag());
        buffer.append(" - ");
        buffer.append(logEvent.getLevel().toString());
        buffer.append(" - ");
        buffer.append(logEvent.getMessage());
        buffer.append(LINE_SEPARATOR);
        return buffer.toString();
    }
}
