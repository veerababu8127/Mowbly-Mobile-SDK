package com.cloudpact.mowbly.log.layout;

import com.cloudpact.mowbly.log.LogEvent;

/**
 * Layout
 * 
 * @author Prashanth Jonnala <prashanth@cloudpact.com>
 */
public interface Layout {

    /**
     * Get the content type of the layout
     *
     * @return The content type
     */
    public String getContentType();

    /**
     * Get the header section for the layout
     * 
     * @return The header
     */
    public String getHeader();

    /**
     * Get the footer section for the layout
     * 
     * @return The footer
     */
    public String getFooter();

    /**
     * Format the log event according to this layout
     * 
     * @param logEvent The log event to format
     * @return The formatted output for the log event
     */
    public String format(LogEvent logEvent);
}
