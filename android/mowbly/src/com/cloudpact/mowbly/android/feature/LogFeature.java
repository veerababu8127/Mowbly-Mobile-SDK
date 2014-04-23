package com.cloudpact.mowbly.android.feature;

import com.cloudpact.mowbly.feature.BaseFeature;
import com.cloudpact.mowbly.feature.Response;
import com.cloudpact.mowbly.log.Level;
import com.cloudpact.mowbly.log.Logger;

/**
 * Javascript interface for the Log feature
 * 
 * @author Prashanth Jonnala <prashanth@cloudpact.com>
 */
public class LogFeature extends BaseFeature {

    /** Exposed name of the LogFeature */
    public static final String NAME = "logger";
    

    protected static final String loggerName = "user";
    protected static Logger logger = Logger.getLogger(loggerName);

    public LogFeature() {
        super(NAME);
    }

    /**
     * Log a message from javascript
     * 
     * @param message
     * @param tag
     * @param priority
     * @return Response
     */
    @Method(async = false, args = {
        @Argument(name = "message", type = String.class),
        @Argument(name = "tag", type = String.class),
        @Argument(name = "priority", type = int.class)
    })
    public Response log(String message, String tag, int priority) {
        Level level = Level.DEBUG;
        if (priority == Level.DEBUG.getPriority()) {
            level = Level.DEBUG;
        } else if (priority == Level.INFO.getPriority()) {
            level = Level.INFO;
        } else if (priority == Level.WARN.getPriority()) {
            level = Level.WARN;
        } else if (priority == Level.ERROR.getPriority()) {
            level = Level.ERROR;
        } else if (priority == Level.FATAL.getPriority()) {
            level = Level.FATAL;
        }

        logger.log(tag, level, message);

        return new Response();
    } 
}
