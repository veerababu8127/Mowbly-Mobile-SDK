package com.cloudpact.mowbly.android.log.handler;

import android.util.Log;

import com.cloudpact.mowbly.log.Level;
import com.cloudpact.mowbly.log.LogEvent;
import com.cloudpact.mowbly.log.handler.AbstractHandler;

/**
 * Log Handler for LogCat
 * 
 * @author Prashanth Jonnala <prashanth@cloudpact.com>
 */
public class LogCatHandler extends AbstractHandler {

    @Override
    public void handle(LogEvent event) {
        String tag = event.getTag();
        Level level = event.getLevel();
        String message = event.getMessage();

        if (level.equals(Level.DEBUG)) {
            Log.d(tag, message);
        } else if (level.equals(Level.INFO)) {
            Log.i(tag, message);
        } else if (level.equals(Level.WARN)) {
            Log.w(tag, message);
        } else if (level.equals(Level.ERROR)) {
            Log.e(tag, message);
        } else if (level.equals(Level.FATAL)) {
            Log.e(tag, message);
        } 
    }
}
