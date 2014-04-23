package com.cloudpact.mowbly.log.layout.pattern.formatter;

import com.cloudpact.mowbly.log.LogEvent;

/**
 * TypePatternFormatter
 * 
 * @author Prashanth Jonnala <prashanth@cloudpact.com>
 */
public class TypePatternFormatter extends AbstractPatternFormatter {

    public TypePatternFormatter() {
        super();
    }

    public TypePatternFormatter(FormatInformation fi) {
        super(fi);
    }

    protected String convert(LogEvent logEvent) {
        return logEvent.getType();
    }
}
