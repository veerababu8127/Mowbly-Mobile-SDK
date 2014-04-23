package com.cloudpact.mowbly.log.layout.pattern;

import java.util.Vector;

import com.cloudpact.mowbly.log.LogEvent;
import com.cloudpact.mowbly.log.layout.Layout;
import com.cloudpact.mowbly.log.layout.SimpleLayout;
import com.cloudpact.mowbly.log.layout.pattern.formatter.PatternFormatter;

/**
 * Pattern implementation of the {@link Layout}
 * 
 * @author Prashanth Jonnala <prashanth@cloudpact.com>
 */
public class PatternLayout extends SimpleLayout {

    final static private String DEFAULT_PATTERN = "%d %w [%l] %t - %m%n";

    final static private int BUFFER_SIZE = 256;

    protected String pattern;

    protected Vector<PatternFormatter> formatters;

    public PatternLayout() {
        this(DEFAULT_PATTERN);
    }

    public PatternLayout(String pattern) {
        super(BUFFER_SIZE);
        setPattern(pattern);
    }

    public String getPattern() {
        return pattern;
    }

    public void setPattern(String pattern) {
        this.pattern = (pattern == null) ? DEFAULT_PATTERN : pattern;
        setFormatters();
    }
    
    protected void setFormatters(){
    	this.formatters = new PatternParser(this.pattern).parse();
    }

    public String format(LogEvent logEvent) {
        if (buffer.capacity() > DEFAULT_MAX_BUFFER_CAPACITY) {
            buffer = new StringBuffer(BUFFER_SIZE);
        } else {
            buffer.setLength(0);
        }

        if (formatters != null) {
            int size = formatters.size();
            for (int i = 0; i < size; i++) {
                PatternFormatter formatter = (PatternFormatter) formatters.elementAt(i);
                formatter.format(buffer, logEvent);
            }
        }

        return buffer.toString();
    }
}
