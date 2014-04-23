package com.cloudpact.mowbly.android.log;

import com.cloudpact.mowbly.log.Logger;
import com.cloudpact.mowbly.log.LoggerFactory;

/**
 * AndroidLoggerFactory
 * 
 * @author Prashanth Jonnala <prashanth@cloudpact.com>
 */
public class AndroidLoggerFactory implements LoggerFactory {

    /* (non-Javadoc)
     * @see com.cloudpact.mowbly.log.LoggerFactory#createLogger(java.lang.String)
     */
    @Override
    public Logger createLogger(String name) {
        return new AndroidLogger(name);
    }
}
