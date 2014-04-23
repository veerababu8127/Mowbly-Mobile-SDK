package com.cloudpact.mowbly.log.handler;

import com.cloudpact.mowbly.log.Level;
import com.cloudpact.mowbly.log.layout.Layout;

/**
 * Abstract implementation of the {@link Handler}
 * 
 * @author Prashanth Jonnala <prashanth@cloudpact.com>
 */
abstract public class AbstractHandler implements Handler {

    /** The name of the handler */
    protected String name;

    /** The threshold {@link Level} of the handler */
    protected Level threshold;

    protected Layout layout;

    /** Whether the handler allows propagation */
    protected boolean isPropagationAllowed = true;

    public AbstractHandler() {
        threshold = Level.DEBUG;
    }

    public String getName() {
        return name;
    }

    public void setName(String name) {
        this.name = name;
    }

    public Level getThreshold() {
        return threshold;
    }

    public void setThreshold(Level threshold) {
        this.threshold = threshold;
    }

    public Layout getLayout() {
        return layout;
    }

    public void setLayout(Layout layout) {
        this.layout = layout;
    }

    public boolean isPropagationAllowed() {
        return isPropagationAllowed;
    }

    public void start() {
    }

    public void shutdown() {
    }
}
