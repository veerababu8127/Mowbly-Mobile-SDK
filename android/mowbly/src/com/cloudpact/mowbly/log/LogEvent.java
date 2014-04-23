package com.cloudpact.mowbly.log;

/**
 * LogEvent
 * 
 * @author Prashanth Jonnala <prashanth@cloudpact.com>
 */
public class LogEvent {

    private String type;

    private String tag;

    private Level level;

    private String message;

    private long timestamp;

    private boolean isHandled = false;

    public LogEvent(String type, String tag, Level level, String message) {
        this.type = type;
        this.tag = tag;
        this.level = level;
        this.message = message;
        this.timestamp = System.currentTimeMillis();
    }

    /**
     * Get the type of the log
     * 
     * @return The type of the log event
     */
    public String getType() {
        return type;
    }

    /**
     * Get the tag of the log
     * 
     * @return The tag of the log event
     */
    public String getTag() {
        return tag;
    }

    /**
     * Get the log level
     * 
     * @return The log level
     */
    public Level getLevel() {
        return level;
    }

    /**
     * Get the message of the log
     * 
     * @return The message of the log event
     */
    public String getMessage() {
        return message;
    }

    /**
     * Get the timestamp when the log was created
     * 
     * @return The timestamp of the log event
     */
    public long getTimestamp() {
        return timestamp;
    }

    /**
     * Retrieve whether the log event has been handled or not
     * 
     * If the log event has been handled, it will not be passed on
     * to the next handler in the stack
     * 
     * @return true if log event has been handled else false
     */
    public boolean isHandled() {
        return isHandled;
    }

    /**
     * Set the log event as handled.
     * 
     * This log event will not propagate further in the handler stack
     */
    public void setHandled() {
        isHandled = true;
    }
}
