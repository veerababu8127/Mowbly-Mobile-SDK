package com.cloudpact.mowbly.log.layout.pattern.formatter;

import com.cloudpact.mowbly.log.LogEvent;

/**
 * Interface for all pattern formatters
 * 
 * @author Prashanth Jonnala <prashanth@cloudpact.com>
 */
public interface PatternFormatter {

    /**
     * Formats the parameter in pattern according to this formatter
     * 
     * @param buffer The buffer to which the formatted content should be appended
     * @param logEvent The log event which has to be formatted
     */
    public void format(StringBuffer buffer, LogEvent logEvent);
}
