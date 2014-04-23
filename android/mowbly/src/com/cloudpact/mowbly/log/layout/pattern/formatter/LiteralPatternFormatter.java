package com.cloudpact.mowbly.log.layout.pattern.formatter;

import com.cloudpact.mowbly.log.LogEvent;

/**
 * Formats the given literal string
 * 
 * @author Prashanth Jonnala <prashanth@cloudpact.com>
 */
public class LiteralPatternFormatter implements PatternFormatter {

    private String literal;

    public LiteralPatternFormatter(String literal) {
        this.literal = literal;
    }

    public void format(StringBuffer buffer, LogEvent logEvent) {
        buffer.append(literal);
    }
}
