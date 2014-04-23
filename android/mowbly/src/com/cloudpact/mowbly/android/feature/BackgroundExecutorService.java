package com.cloudpact.mowbly.android.feature;

import java.util.concurrent.ExecutorService;
import java.util.concurrent.Executors;

import android.os.HandlerThread;

/**
 * BackgroundExecutorService.
 * 
 * @author Prashanth Jonnala <prashanth@cloudpact.com>
 */
public class BackgroundExecutorService {

    private static final HandlerThread mHandlerThread;
    private static final ExecutorService mExecutor;
    static {
        mHandlerThread = new HandlerThread("BackgroundHandler", 10);
        mHandlerThread.start();
        mExecutor = Executors.newCachedThreadPool();
    }

    public static ExecutorService getExecutor() {
        return mExecutor;
    }

    public static void execute(Runnable task) {
        mExecutor.execute(task);
    }
}
