package com.cloudpact.mowbly.log.handler;

import java.io.OutputStream;
import java.io.OutputStreamWriter;
import java.io.UnsupportedEncodingException;
import java.io.Writer;

import com.cloudpact.mowbly.log.LogEvent;
import com.cloudpact.mowbly.log.helpers.QuietWriter;

/**
 * Base handler class for all Streams
 * 
 * @author Prashanth Jonnala <prashanth@cloudpact.com>
 *
 */
public class StreamHandler extends AbstractHandler {

    protected String encoding;

    protected QuietWriter writer;

    public StreamHandler() {        
    }

    public StreamHandler(Writer writer) {
        setWriter(writer);
    }

    public StreamHandler(OutputStream out) {
        this(out, null);
    }

    public StreamHandler(OutputStream out, String encoding) {
        setEncoding(encoding);
        setWriter(out);
    }

    public void setEncoding(String encoding) {
        this.encoding = encoding;
    }

    public void setWriter(QuietWriter qw) {
        shutdown();
        writer = qw;
    }

    public void setWriter(Writer w) {
        setWriter(new QuietWriter(w));
    }

    public void setWriter(OutputStream out) {
        setWriter(getWriter(out));
    }

    public Writer getWriter(OutputStream out) {
        Writer writer = null;
        if (encoding != null) {
            try {
                writer = new OutputStreamWriter(out, encoding);
            } catch (UnsupportedEncodingException e) {
                writer = new OutputStreamWriter(out);
            }
        } else {
            writer = new OutputStreamWriter(out);
        }
        return writer;
    }

    public synchronized void handle(LogEvent logEvent) {
        writer.write(layout.format(logEvent));
        writer.flush();
    }

    public void start() {
        String header = layout.getHeader();
        if (header != null && writer != null) {
            writer.write(header);
        }
    }

    public void shutdown() {
        if (writer != null && layout != null) {
            String footer = layout.getFooter();
            if (footer != null) {
                writer.write(footer);
            }

            writer.close();
            writer = null;
        }
    }
}
