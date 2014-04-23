package com.cloudpact.mowbly.log;

/**
 * Log Level
 * 
 * @author Prashanth Jonnala <prashanth@cloudpact.com>
 */
public class Level extends Priority {

    final static public Level DEBUG = new Level(DEBUG_PRIORITY, "DEBUG");

    final static public Level INFO = new Level(INFO_PRIORITY, "INFO");

    final static public Level WARN = new Level(WARN_PRIORITY, "WARN");

    final static public Level ERROR = new Level(ERROR_PRIORITY, "ERROR");

    final static public Level FATAL = new Level(FATAL_PRIORITY, "FATAL");

    private Level(int priority, String priorityStr) {
        super(priority, priorityStr);
    }
    
    public static Level from(int levelValue){
    	if(levelValue == 10000){
    		return Level.DEBUG;
    	}else if(levelValue == 20000){
    		return Level.INFO;
    	}else if(levelValue == 30000){
    		return Level.WARN;
    	}else if(levelValue == 40000){
    		return Level.ERROR;
    	}else if(levelValue == 50000){
    		return Level.FATAL;
    	}
		return Level.DEBUG;     	
    }
}
