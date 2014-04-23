package com.cloudpact.mowbly.log.layout.pattern.formatter;

import com.cloudpact.mowbly.log.LogEvent;

/**
 * Formatter for the message of the log event
 * 
 * @author Prashanth Jonnala <prashanth@cloudpact.com>
 */
public class MessagePatternFormatter extends AbstractPatternFormatter {

    public MessagePatternFormatter() {
        super();
    }

    public MessagePatternFormatter(FormatInformation fi) {
        super(fi);
    }

    protected String convert(LogEvent logEvent) {
        return logEvent.getMessage();
    }
}
