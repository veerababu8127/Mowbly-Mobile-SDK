package com.cloudpact.mowbly.log.layout.pattern.formatter;

import com.cloudpact.mowbly.log.LogEvent;

/**
 * Abstract implementation of the {@link PatternFormatter}
 * 
 * @author Prashanth Jonnala <prashanth@cloudpact.com>
 */
public abstract class AbstractPatternFormatter implements PatternFormatter {

    static public String[] SPACES = { " ", "  ", "    ", "        ",
            "                ", "                                " };

    private FormatInformation fi;

    public AbstractPatternFormatter() {
        this(new FormatInformation());
    }

    public AbstractPatternFormatter(FormatInformation fi) {
        this.fi = fi;
    }

    protected void padSpaces(StringBuffer buffer, int length) {
        while (length >= 32) {
            buffer.append(SPACES[5]);
            length -= 32;
        }

        for (int i = 4; i >= 0; i--) {
            if ((length & (1 << i)) != 0) {
                buffer.append(SPACES[i]);
            }
        }
    }

    /**
     * Get the converted string format of the pattern parameter
     * 
     * @param logEvent
     *            The log event for which the pattern parameter is to converted
     * @return The string format of the pattern parameter
     */
    abstract protected String convert(LogEvent logEvent);

    public void format(StringBuffer buffer, LogEvent logEvent) {
        String s = convert(logEvent);

        if (s == null) {
            if (fi.getMin() > 0) {
                padSpaces(buffer, fi.getMin());
            }
            return;
        }

        int len = s.length();
        if (len > fi.getMax()) {
            buffer.append(s.substring(len - fi.getMax()));
        } else if (len < fi.getMin()) {
            if (fi.isLeftAlign()) {
                buffer.append(s);
                padSpaces(buffer, fi.getMin() - len);
            } else {
                padSpaces(buffer, fi.getMin() - len);
                buffer.append(s);
            }
        } else {
            buffer.append(s);
        }
    }
}
