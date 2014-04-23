package com.cloudpact.mowbly.log.layout.pattern.formatter;

import com.cloudpact.mowbly.log.LogEvent;

/**
 * Formatter for the tag of the log event
 * 
 * @author Prashanth Jonnala <prashanth@cloudpact.com>
 */
public class TagPatternFormatter extends AbstractPatternFormatter {

    public TagPatternFormatter() {
        super();
    }

    public TagPatternFormatter(FormatInformation fi) {
        super(fi);
    }

    protected String convert(LogEvent logEvent) {
        return logEvent.getTag();
    }
}
