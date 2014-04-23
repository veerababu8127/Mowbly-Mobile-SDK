package com.cloudpact.mowbly.log;

/**
 * LoggerFactory
 * 
 * @author Prashanth Jonnala <prashanth@cloudpact.com>
 */
public interface LoggerFactory {

    /**
     * Create a logger by the specified name
     * 
     * @param name The name of the logger to be created
     * @return The logger
     */
    Logger createLogger(String name);
}
