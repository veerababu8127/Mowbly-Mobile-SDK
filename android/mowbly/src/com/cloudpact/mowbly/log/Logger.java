package com.cloudpact.mowbly.log;

import java.util.Enumeration;
import java.util.Vector;

import com.cloudpact.mowbly.log.handler.Handler;
import com.cloudpact.mowbly.log.handler.HandlerAttachable;

/**
 * Logger
 * 
 * @author Prashanth Jonnala <prashanth@cloudpact.com>
 */
public class Logger implements HandlerAttachable {

    /** The default logger name */
    static private final String DEFAULT_LOGGER_NAME = "system";

    /** The loggers */
    static private Vector<Logger> loggers = new Vector<Logger>();

    /** The logger factory */
    static private LoggerFactory loggerFactory = new DefaultLoggerFactory();

    /** The name of the logger */
    protected String name;

    /** The list of handlers attached to this logger */
    protected Vector<Handler> handlers;

    protected Logger(String name) {
        this.name = name;
    }

    /**
     * Set the logger factory to be used to create loggers
     * 
     * @param newLoggerFactory The logger factory
     */
    static public void setLoggerFactory(LoggerFactory newLoggerFactory) {
        loggerFactory = newLoggerFactory;
        loggers = new Vector<Logger>();
    }

    /**
     * Get the system {@link Logger}
     * 
     * @return The system logger
     */
    static public Logger getLogger() {
        return getLogger(DEFAULT_LOGGER_NAME);
    }

    /**
     * Get the {@link Logger} associated by name
     * 
     * @return The logger associated by name
     */
    static public Logger getLogger(String name) {
        Logger logger = null;

        // Enumerate over all the loggers in the repository to see if
        // any logger already exists by this name
        Enumeration<Logger> enumeration = loggers.elements();
        while (enumeration.hasMoreElements()) {
            logger = (Logger) enumeration.nextElement();

            if (logger.getName().equals(name)) {
                // A logger already exists in the repository by this name
                // So we directly return this logger
                return logger;
            }
        }

        // We are sure there is no logger in the repository by this name
        // So we create a new logger by this name and add it to repository
        logger = loggerFactory.createLogger(name);
        //logger = new Logger(name);
        loggers.addElement(logger);

        // Return the newly created logger
        return logger;
    }

    public void attachHandler(Handler handler) {
        if (handler == null) {
            return;
        }

        if (handlers == null) {
            handlers = new Vector<Handler>();
        }

        if (!handlers.contains(handler)) {
            handlers.addElement(handler);
        }
    }

    public void detachHandler(Handler handler) {
        if (handler == null || handlers == null) {
            return;
        }

        handlers.removeElement(handler);
    }

    public void detachAllHandlers() {
        if (handlers == null) {
            return;
        }

        handlers.removeAllElements();
        handlers = null;
    }

    public Enumeration<Handler> getAttachedHandlers() {
        if (handlers == null) {
            return null;
        }
        return handlers.elements();
    }

    public Handler getAttachedHandler(String name) {
        Handler handler = null;
        if (handlers != null) {
            int size = handlers.size();
            for (int i = 0; i < size; i++) {
                handler = (Handler) handlers.elementAt(i);
                if (name.equals(handler.getName())) {
                    break;
                }
            }
        }
        return handler;
    }

    /**
     * Get the name of the logger
     * 
     * @return The name of the logger
     */
    public String getName() {
        return name;
    }

    /**
     * Log a message with the {@link Level#DEBUG DEBUG} level
     * 
     * @param tag The tag of the log
     * @param message The message of the log
     */
    public void debug(String tag, String message) {
        log(tag, Level.DEBUG, message);   
    }

    /**
     * Log a message with the {@link Level#INFO INFO} level
     * 
     * @param tag The tag of the log
     * @param message The message of the log
     */
    public void info(String tag, String message) {
        log(tag, Level.INFO, message);
    }

    /**
     * Log a message with the {@link Level#WARN WARN} level
     * 
     * @param tag The tag of the log
     * @param message The message of the log
     */
    public void warn(String tag, String message) {
        log(tag, Level.WARN, message);
    }

    /**
     * Log a message with the {@link Level#ERROR ERROR} level
     * 
     * @param tag The tag of the log
     * @param message The message of the log
     */
    public void error(String tag, String message) {
        log(tag, Level.ERROR, message);
    }

    /**
     * Log a message with the {@link Level#FATAL FATAL} level
     * 
     * @param tag The tag of the log
     * @param message The message of the log
     */
    public void fatal(String tag, String message) {
        log(tag, Level.FATAL, message);
    }

    /**
     * Low level API for logging a message
     * 
     * @param tag The tag of the log
     * @param level The level of the log
     * @param message The message of the log
     */
    public void log(String tag, Level level, String message) {
    	message = (message != null) ? message : "";
        log(new LogEvent(name, tag, level, message));
    }

    /**
     * Log an event
     * 
     * @param logEvent The log event
     */
    public void log(LogEvent logEvent) {
        if (handlers != null) {
            int size = handlers.size();
            for (int i = 0; i < size; i++) {
                Handler handler = (Handler) handlers.elementAt(i);
                if (logEvent.getLevel().isGreaterOrEqualTo(handler.getThreshold())) {
                    handler.handle(logEvent);
                    if (!handler.isPropagationAllowed() || logEvent.isHandled()) {
                        break;
                    }
                }
            }
        }
    }

    public String toString() {
        return "Logger (" + name + ")";
    }
}
