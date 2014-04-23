package com.cloudpact.mowbly.android;

import java.io.FileNotFoundException;
import java.io.IOException;
import java.util.Map.Entry;
import java.util.Enumeration;
import java.util.Properties;
import java.util.Vector;

import android.app.Application;
import android.content.Context;
import android.content.res.Resources.NotFoundException;

import com.cloudpact.mowbly.R;
import com.cloudpact.mowbly.android.feature.FeatureBinder;
import com.cloudpact.mowbly.android.log.handler.DatabaseHandler;
import com.cloudpact.mowbly.android.log.handler.FileHandler;
import com.cloudpact.mowbly.android.log.handler.LogCatHandler;
import com.cloudpact.mowbly.android.page.Page;
import com.cloudpact.mowbly.android.service.ContactsService;
import com.cloudpact.mowbly.android.service.FileService;
import com.cloudpact.mowbly.android.service.MowblyFile;
import com.cloudpact.mowbly.android.service.MowblyFileServiceUtil;
import com.cloudpact.mowbly.android.service.NetworkService;
import com.cloudpact.mowbly.android.service.PreferenceService;
import com.cloudpact.mowbly.android.ui.PageFragment;
import com.cloudpact.mowbly.android.ui.PageView;
import com.cloudpact.mowbly.android.util.GsonUtils;
import com.cloudpact.mowbly.feature.Feature;
import com.cloudpact.mowbly.log.Level;
import com.cloudpact.mowbly.log.Logger;
import com.cloudpact.mowbly.log.LoggerFactory;
import com.cloudpact.mowbly.log.handler.Handler;
import com.cloudpact.mowbly.log.layout.Layout;
import com.google.gson.Gson;
import com.google.inject.Guice;
import com.google.inject.Injector;
import com.google.inject.Module;

/**
 * Mowbly - Application context
 * 
 * @author Aravind Baskaran <aravind@cloudpact.com>
 */
public class Mowbly extends Application {
    protected static Vector<Feature> features = null;

    protected static FeatureBinder featureBinder = null;

    protected static Injector injector;

    protected static CrashLogger crashLogger;

    protected static Context applicationContext;
    
    protected static String basePagePath = "";
    
    protected static Logger logger = Logger.getLogger();
    protected static final String TAG = "Mowbly";
    
    protected static int log_level;

    @Override
    public void onCreate() {
        super.onCreate();
        configure();
    }

    /*
     * Starting point of Mowbly
     */
    private void configure() {
        crashLogger = CrashLogger.getInstance();
        crashLogger.init(getApplicationContext());

        configureApplication();

        configureLogger();
        
        configureFeatures();
    }

    protected void configureApplication() {
    	log_level = Integer.parseInt(getResources().getString(R.string.log_level));
        applicationContext = getApplicationContext();
        createInjector();
    }

    protected void createInjector() {
        injector = null;
        Module module = new MowblyModule(applicationContext);
        injector = Guice.createInjector(module);
    }

    protected void configureLogger() {
        // Initiate logger
        Logger.setLoggerFactory(Mowbly.getLoggerFactory());
        Logger systemLogger = Logger.getLogger();
        logger = systemLogger;
        systemLogger.detachAllHandlers();
        Logger userLogger = Logger.getLogger("user");
        userLogger.detachAllHandlers();

        // Create the Logcat handler
        Handler lch = new LogCatHandler();
        lch.setName("lch");

        // Create the Database handler
        Handler dbh = createDatabaseLogHandler();
        dbh.setName("dbh");

        // Attach the handlers to system and user loggers
        systemLogger.attachHandler(lch);
        systemLogger.attachHandler(dbh);
        userLogger.attachHandler(lch);
        userLogger.attachHandler(dbh);

        // Attach a File Handler
        try {
            String logFile = getFileService().getLogsFile().getAbsolutePath();
            Handler fh = new FileHandler(logFile);
            fh.setName("fh");

            Layout layout = Mowbly.createLayout();
            fh.setLayout(layout);

            systemLogger.attachHandler(fh);
            userLogger.attachHandler(fh);
        } catch (FileNotFoundException e) {
        }
        updateLogLevel();
    }

    protected Vector<Feature> getAvailableFeatures() {
    	Properties features = new Properties();
    	try {
			features.load(getApplicationContext().getResources().openRawResource(R.raw.mowblyfeatures));
		} catch (NotFoundException e) {
			logger.error(TAG, "Features property list not found - " + e.getLocalizedMessage());
		} catch (IOException e) {
			logger.error(TAG, "Error reading features property list - " + e.getLocalizedMessage());
		}
		Vector<Feature> featuresList = new Vector<Feature>();
		for(Entry<Object, Object> entry : features.entrySet()) {
			Object featureClassName = entry.getValue();
			if(featureClassName != null){
				try {
					@SuppressWarnings("unchecked")
					Class<Feature> featureClazz = (Class<Feature>)Class.forName(featureClassName.toString());
					featuresList.add((Feature)featureClazz.newInstance());
				} catch (InstantiationException e) {
					logger.error(TAG, "Error instantiating feature " + featureClassName + " - " + e.getLocalizedMessage());
				} catch (IllegalAccessException e) {
					logger.error(TAG, "Error accessing feature " + featureClassName + " - " + e.getLocalizedMessage());
				} catch (ClassNotFoundException e) {
					logger.error(TAG, "Feature class not found " + featureClassName + " - " + e.getLocalizedMessage());
				}
			}else{
				logger.error(TAG, "Error instantiating feature - Feature name not defined");
			}
        }
		return featuresList;
    }

    public static String getBasePagePath(){
    	return basePagePath;
    }
    
    protected void configureFeatures() {
        features = getAvailableFeatures();
        featureBinder = getJsNativeInterface();
        featureBinder.bind(features);
    }
    
    public void updateLogLevel(){
    	Level logLevel = getLogLevel();
    	Logger userLogger = Logger.getLogger("user");
    	Logger systemLogger = Logger.getLogger();
    	Enumeration<Handler> userLoggerhandlers = userLogger.getAttachedHandlers();
    	Enumeration<Handler> systemLoggerhandlers = systemLogger.getAttachedHandlers();
    	while (userLoggerhandlers.hasMoreElements()) {
            Handler handler = (Handler) userLoggerhandlers.nextElement();
            handler.setThreshold(logLevel);
    	}
    	while (systemLoggerhandlers.hasMoreElements()) {
            Handler handler = (Handler) systemLoggerhandlers.nextElement();
            handler.setThreshold(logLevel);
    	}
    }
    
    protected Level getLogLevel(){
    	return Level.from(log_level);
    }
    
    
    public FeatureBinder getJsNativeInterface() {
    	return new FeatureBinder();
    }
    
    public static Vector<Feature> getFeatures() {
        return features;
    }

    public static FeatureBinder getFeatureBinder() {
        return featureBinder;
    }

    public static <V> V getService(Class<V> klass) {
        return injector.getInstance(klass);
    }

    public static PreferenceService getPreferenceService() {
        return injector.getInstance(PreferenceService.class);
    }

    public static FileService getFileService() {
        return injector.getInstance(FileService.class);
    }

    public static NetworkService getNetworkService() {
        return injector.getInstance(NetworkService.class);
    }
    
    public static Page createPage(){
    	return injector.getInstance(Page.class);
    }
    
    protected static LoggerFactory getLoggerFactory(){
    	return injector.getInstance(LoggerFactory.class);
    }
    
    protected static Layout createLayout(){
    	return injector.getInstance(Layout.class);
    }
    
    protected static DatabaseHandler createDatabaseLogHandler(){
    	return injector.getInstance(DatabaseHandler.class);
    }
    
    public static ContactsService getContactsService() {
        return injector.getInstance(ContactsService.class);
    }
    
    public static MowblyFileServiceUtil getMowblyFileServiceUtil(){
    	return injector.getInstance(MowblyFileServiceUtil.class);
    }
    
    public static MowblyFile createMowblyFile(){
    	return injector.getInstance(MowblyFile.class);
    }
    
    public static PageFragment createPageFragment(){
    	return injector.getInstance(PageFragment.class);
    }
    
    public static PageView createPageView(){
    	return injector.getInstance(PageView.class);
    }
    
    public static GsonUtils getGsonUtils(){
    	return injector.getInstance(GsonUtils.class);
    }

	public static Gson getGson() {
		return Mowbly.getGsonUtils().getGson();
	}
}