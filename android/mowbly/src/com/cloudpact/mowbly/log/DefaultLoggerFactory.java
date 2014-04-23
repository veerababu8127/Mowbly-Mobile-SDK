package com.cloudpact.mowbly.log;

/**
 * DefaultLoggerFactory
 * 
 * @author Prashanth Jonnala <prashanth@cloudpact.com>
 */
public class DefaultLoggerFactory implements LoggerFactory {

    /* (non-Javadoc)
     * @see com.cloudpact.mowbly.log.LoggerFactory#createLogger(java.lang.String)
     */
    public Logger createLogger(String name) {
        return new Logger(name);
    }
}
