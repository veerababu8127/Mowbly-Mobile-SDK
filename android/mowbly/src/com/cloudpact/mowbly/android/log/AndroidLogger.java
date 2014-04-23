package com.cloudpact.mowbly.android.log;

import com.cloudpact.mowbly.log.LogEvent;
import com.cloudpact.mowbly.log.Logger;

/**
 * AndroidLogger
 * 
 * @author Prashanth Jonnala <prashanth@cloudpact.com>
 */
public class AndroidLogger extends Logger {

    /**
     * Constructor
     * 
     * @param name The name of the logger
     */
    protected AndroidLogger(String name) {
        super(name);
    }

    /* (non-Javadoc)
     * @see com.cloudpact.mowbly.log.Logger#log(com.cloudpact.mowbly.log.LogEvent)
     */
    @Override
    public void log(LogEvent event) {
    	// Nothing to do here right now!
        super.log(event);
    }
}
