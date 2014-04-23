package com.cloudpact.mowbly.log.handler;

import com.cloudpact.mowbly.log.Level;
import com.cloudpact.mowbly.log.LogEvent;
import com.cloudpact.mowbly.log.layout.Layout;

/**
 * Handler
 * 
 * @author Prashanth Jonnala <prashanth@cloudpact.com>
 */
public interface Handler {

    /**
     * Get the name of the handler
     * 
     * @return The name of the handler
     */
    public String getName();

    /**
     * Set the name of the handler
     * 
     * @param name The name to be set for the handler
     */
    public void setName(String name);

    /**
     * Retrieve the threshold {@link Level} for this handler.
     * 
     * The log events with level less than the threshold will not be 
     * handled by this handler.
     * 
     * @return The assigned Level
     */
    public Level getThreshold();

    /**
     * Set the threshold {@link Level} for this handler
     * 
     * @param level The threshold level for this handler
     */
    public void setThreshold(Level level);

    /**
     * Get the {@link Layout} to be used by this handler to format the message
     * 
     * @return The layout used by this handler to format the message
     */
    public Layout getLayout();

    /**
     * Set the {@link Layout} to be used by this handler to format the message
     * 
     * @param layout The layout used by this handler to format the message
     */
    public void setLayout(Layout layout);

    /**
     * Whether the handler allows propagation or not.
     * 
     * If the handler allows propagation the log record is passed
     * on to the next handler in the stack
     * 
     * @return true if the handler allows propagation else false
     */
    public boolean isPropagationAllowed();

    /**
     * Start the handler
     */
    public void start();

    /**
     * Handle a {@link LogEvent}
     * 
     * @param logEvent The log event to handle
     */
    public void handle(LogEvent logEvent);

    /**
     * Shutdown the handler.
     * 
     * Close all the resources opened by this handler
     */
    public void shutdown();
}
