package com.cloudpact.mowbly.log.handler;

/**
 * Use this handler to write to the console
 * 
 * @author Prashanth Jonnala <prashanth@cloudpact.com>
 */
public class ConsoleHandler extends StreamHandler {

    public ConsoleHandler() {
        super(System.out);
    }
}
