package com.cloudpact.mowbly.log.handler;

import com.cloudpact.mowbly.log.LogEvent;

/**
 * No operation handler. Does nothing
 * 
 * @author Prashanth Jonnala <prashanth@cloudpact.com>
 */
public class NOPHandler extends AbstractHandler {

    public void handle(LogEvent logEvent) {
    }
}
