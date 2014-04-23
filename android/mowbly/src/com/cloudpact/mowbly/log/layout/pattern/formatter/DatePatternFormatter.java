package com.cloudpact.mowbly.log.layout.pattern.formatter;

import java.util.Calendar;
import java.util.Date;

import com.cloudpact.mowbly.log.LogEvent;

/**
 * Formatter for the formatting the timestamp of the log event
 * 
 * @author Prashanth Jonnala <prashanth@cloudpact.com>
 */
public class DatePatternFormatter extends AbstractPatternFormatter {

    final static private String DEFAULT_DATE_FORMAT_STRING = "yyyy-MM-dd HH:mm:ss.S";

    private Calendar cal = Calendar.getInstance();

    private String format;

    public DatePatternFormatter() {
        super();
        format = DEFAULT_DATE_FORMAT_STRING;
    }

    public DatePatternFormatter(FormatInformation fi) {
        this(fi, DEFAULT_DATE_FORMAT_STRING);
    }

    public DatePatternFormatter(FormatInformation fi, String dateFormatString) {
        super(fi);

        format = (dateFormatString == null) ? DEFAULT_DATE_FORMAT_STRING
                : dateFormatString;
    }

    public String getFormat() {
        return format;
    }

    protected String convert(LogEvent logEvent) {
        Date date = new Date();
        date.setTime(logEvent.getTimestamp());
        cal.setTime(date);

        StringBuffer buf = new StringBuffer(256);
        int year = cal.get(Calendar.YEAR);
        buf.append(year);
        buf.append("-");
        int month = cal.get(Calendar.MONTH) + 1;
        if (month < 10) {
            buf.append("0");
        }
        buf.append(month);
        buf.append("-");
        int day = cal.get(Calendar.DATE);
        if (day < 10) {
            buf.append("0");
        }
        buf.append(day);
        buf.append(" ");
        int hours = cal.get(Calendar.HOUR_OF_DAY);
        if (hours < 10) {
            buf.append("0");
        }
        buf.append(hours);
        buf.append(":");
        int minutes = cal.get(Calendar.MINUTE);
        if (minutes < 10) {
            buf.append("0");
        }
        buf.append(minutes);
        buf.append(":");
        int secs = cal.get(Calendar.SECOND);
        if (secs < 10) {
            buf.append('0');
        }
        buf.append(secs);
        buf.append('.');
        int millis = cal.get(Calendar.MILLISECOND);
        if (millis < 100) {
            buf.append('0');
        }
        if (millis < 10) {
            buf.append('0');
        }
        buf.append(millis);

        return buf.toString();
    }
}
