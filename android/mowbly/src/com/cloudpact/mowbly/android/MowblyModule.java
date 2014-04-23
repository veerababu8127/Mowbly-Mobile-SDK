package com.cloudpact.mowbly.android;

import android.content.Context;

import com.cloudpact.mowbly.android.log.AndroidLoggerFactory;
import com.cloudpact.mowbly.android.log.handler.DatabaseHandler;
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
import com.cloudpact.mowbly.log.LoggerFactory;
import com.cloudpact.mowbly.log.layout.Layout;
import com.cloudpact.mowbly.log.layout.pattern.PatternLayout;
import com.google.inject.AbstractModule;
import com.google.inject.Provides;
import com.google.inject.Singleton;
import com.google.inject.name.Named;
import com.google.inject.name.Names;

/**
 * Mowbly Module for guice. Provides and binds objects with guice injector
 * 
 * @author Aravind Baskaran <aravind@cloudpact.com>
 */
public class MowblyModule extends AbstractModule {

    protected final Context appContext;

    protected MowblyModule(Context context) {
        appContext = context;
    }

    @Override
    protected void configure() {
        bind(Context.class)
            .annotatedWith(Names.named("Application"))
            .toInstance(appContext);
        bindOthers();
    }
    
    protected void bindOthers(){
    	bind(FileService.class);
        bind(PreferenceService.class);
    }


    @Provides @Singleton
    public NetworkService provideNetworkService(@Named("Application") Context context) {
        return new NetworkService(context);
    }
    
    @Provides
    public Page providePage(@Named("Application") Context context){
    	return new Page();
    }

    /** Logger **/
    @Provides @Singleton
    public LoggerFactory provideLoggerFactory(@Named("Application") Context context){
    	return new AndroidLoggerFactory();
    }
    
    @Provides @Singleton
    public Layout provideLayout(@Named("Application") Context context){
    	return new PatternLayout();
    }
    
    @Provides @Singleton
    public DatabaseHandler provideDatabaseHandler(@Named("Application") Context context){
    	return new DatabaseHandler(context);
    }
    
    /** Services **/
    @Provides @Singleton
    public ContactsService provideContactsService(@Named("Application") Context context){
    	return new ContactsService();
    }

    @Provides @Singleton
    public MowblyFileServiceUtil provideMowblyFileServiceUtil(@Named("Application") Context context){
    	return new MowblyFileServiceUtil();
    }
    
    /** File **/
    @Provides
    public MowblyFile provideMowblyFile(@Named("Application") Context context){
    	return new MowblyFile();
    }
    
    /** Page **/
    @Provides
    public PageFragment providePageFragment(@Named("Application") Context context){
    	return new PageFragment();
    }
    
    @Provides
    public PageView providePageView(@Named("Application") Context context){
    	return new PageView(context);
    }
    
    @Provides @Singleton
    public GsonUtils provideGsonUtils(@Named("Application") Context context){
    	return new GsonUtils();
    }
}
