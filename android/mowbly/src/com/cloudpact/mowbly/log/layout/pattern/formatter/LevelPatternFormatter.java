package com.cloudpact.mowbly.log.layout.pattern.formatter;

import com.cloudpact.mowbly.log.LogEvent;

/**
 * Formatter for the {@link com.cloudpact.mowbly.log.Level Level} of the log record
 *  
 * @author Prashanth Jonnala <prashanth@cloudpact.com>
 */
public class LevelPatternFormatter extends AbstractPatternFormatter {

    public LevelPatternFormatter() {
        super();
    }

    public LevelPatternFormatter(FormatInformation fi) {
        super(fi);
    }

    protected String convert(LogEvent logEvent) {
        return logEvent.getLevel().toString();
    }
}
