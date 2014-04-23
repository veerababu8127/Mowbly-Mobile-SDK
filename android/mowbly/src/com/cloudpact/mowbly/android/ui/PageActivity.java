package com.cloudpact.mowbly.android.ui;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.Iterator;

import android.annotation.SuppressLint;
import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.content.IntentFilter;
import android.content.pm.PackageInfo;
import android.content.pm.PackageManager;
import android.content.pm.PackageManager.NameNotFoundException;
import android.content.res.Configuration;
import android.net.ConnectivityManager;
import android.os.Build;
import android.os.Bundle;
import android.support.v4.app.FragmentActivity;
import android.support.v4.app.FragmentManager;
import android.support.v4.app.FragmentManager.OnBackStackChangedListener;
import android.support.v4.app.FragmentTransaction;
import android.util.DisplayMetrics;
import android.view.Window;
import android.webkit.WebView;

import com.cloudpact.mowbly.R;
import com.cloudpact.mowbly.android.Intents;
import com.cloudpact.mowbly.android.Mowbly;
import com.cloudpact.mowbly.android.feature.CameraFeature;
import com.cloudpact.mowbly.android.feature.ContactsFeature;
import com.cloudpact.mowbly.android.feature.FeatureBinder;
import com.cloudpact.mowbly.android.feature.NetworkFeature;
import com.cloudpact.mowbly.android.page.Page;
import com.cloudpact.mowbly.feature.Response;
import com.cloudpact.mowbly.log.Logger;

/**
 * PageActivity
 * 
 * @author Prashanth Jonnala <prashanth@cloudpact.com>
 */
public abstract class PageActivity extends FragmentActivity {

    public enum Navigation {
        NEW,
        FORWARD,
        BACKWARD
    };

    /*user agent declarations*/
	public static String applicationName = "";
	public static String  packageName = "";
	public static String  versionName = "";
	public static int  versionCode;
	public static String PhoneModel = android.os.Build.MODEL;
	public static String AndroidVersion = android.os.Build.VERSION.RELEASE;
	public static String appUserAgent = "";
	public static String nativeUserAgent = "";
	public static String userAgent;
    
    private static final String TAG = "UserAgent";
    protected FeatureBinder binder;

    private boolean bIsNewInstance = true;

    private boolean bIsCommitAllowed = true;

    static Navigation navDirection;

    FragmentManager fragmentManager;

    Bundle fragmentsBundle;

    protected HashMap<String, PageView> pageViewsCache;

    protected PageFragment activeFragment;

    private NetworkBroadcastReceiver networkBroadcastReceiver;

    private BackStackChangedListener backStackChangedListener;
    
    private ArrayList<BroadcastReceiver> receivers =  new ArrayList<BroadcastReceiver>();

    private static final Logger logger = Logger.getLogger(); 

    private class BackStackChangedListener implements OnBackStackChangedListener {

        @Override
        public void onBackStackChanged() {
            if (navDirection != Navigation.NEW && fragmentManager.getBackStackEntryCount() > 0) {
                boolean bWaiting = (navDirection == Navigation.BACKWARD);
                String fragmentName = fragmentManager.getBackStackEntryAt(fragmentManager.getBackStackEntryCount() - 1).getName();
                activeFragment = (PageFragment) fragmentManager.findFragmentByTag(fragmentName);
                activeFragment.onPageReopen(bWaiting);
            }
        }        
    }

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setUserAgent();
        getWindow().requestFeature(Window.FEATURE_NO_TITLE);
        //getWindow().setBackgroundDrawableResource(R.drawable.splash);
        setContentView(R.layout.activity_page);

        binder = Mowbly.getFeatureBinder();

        backStackChangedListener = new BackStackChangedListener();

        fragmentsBundle = new Bundle();
    }
    
    protected void setUserAgent(){
    	PackageManager manager = this.getPackageManager();
    	try {
    		PackageInfo pm = manager.getPackageInfo(this.getPackageName(), 0);
        	applicationName = (String) manager.getApplicationLabel(manager.getApplicationInfo( this.getPackageName(), 0));
			packageName = pm.packageName;
			versionCode = pm.versionCode;
			versionName = pm.versionName;
			appUserAgent = applicationName+ "/" + versionName + " "+ getResources().getString(R.string.framework_name)+ "/" + getResources().getString(R.string.framework_version)+ " " +  "android/" + AndroidVersion + " "+ PhoneModel + "::" ;
			WebView wb = new WebView(this);
			nativeUserAgent = wb.getSettings().getUserAgentString();
			userAgent = appUserAgent + nativeUserAgent;
			wb.destroy();
       } catch (NameNotFoundException e) {
    	   logger.warn(TAG, "Name Not Found");
       }
    }

    @Override
    protected void onStart() {
        if (pageViewsCache == null) {
            pageViewsCache = new HashMap<String, PageView>();
        }

        super.onStart();

        if (bIsNewInstance) {
            fragmentManager = getSupportFragmentManager();
            fragmentManager.addOnBackStackChangedListener(backStackChangedListener);

            bIsNewInstance = false;
        }
    }

    @Override
    protected void onResume() {
        super.onResume();

        bIsCommitAllowed = true;

        registerReceivers();

        loadJavascriptOnCurrentPage("__mowbly__._onForeground();");

        if (shouldOpenSplash()) {
            clearPageViewsCache();
            showSplash();
        } else {
            enterTheDragon();
        }
    }
    
    protected void showSplash(){
    	startActivityForResult(getSplashIntent(), Intents.getRequestCode(this, R.string.action_ACTIVITY_SPLASH));
    }

    @Override
    public void onConfigurationChanged(Configuration config) {
        super.onConfigurationChanged(config);

        DisplayMetrics metrics = new DisplayMetrics();
        getWindowManager().getDefaultDisplay().getMetrics(metrics);
        loadJavascriptOnCurrentPage(
            String.format(
                "__mowbly__._onDeviceOrientationChange(%d, %s, %d, %d);",
                config.orientation,
                true, 
                metrics.widthPixels,
                metrics.heightPixels
            )
        );
    }

    @Override
    protected void onActivityResult(int requestCode, int resultCode, Intent intent) {
        if (requestCode == Intents.getRequestCode(this, R.string.action_CAMERA_TAKE_PICTURE) || requestCode == Intents.getRequestCode(this, R.string.action_CAMERA_CHOOSE_PICTURE)) {
            ((CameraFeature) binder.get(CameraFeature.NAME)).onActivityResult(requestCode, resultCode, intent);
        } else if (requestCode == Intents.getRequestCode(this, R.string.action_CONTACTS_PICK_CONTACT)) {
            ((ContactsFeature) binder.get(ContactsFeature.NAME)).onActivityResult(requestCode, resultCode, intent);
        } else {
            super.onActivityResult(requestCode, resultCode, intent);
        }
    }

    @Override
    protected void onPause() {
        super.onPause();

        bIsCommitAllowed = false;

        loadJavascriptOnCurrentPage("__mowbly__._onBackground();");
    }

    @Override
    protected void onStop() {
        super.onStop();

        unregisterReceivers();
    }

    @Override
    public void onBackPressed() {
        if (activeFragment != null) {
            activeFragment.onDeviceBackPressed();
        }
    }

    @Override
    protected void onSaveInstanceState(Bundle outState) {
        //first saving my state, so the bundle wont be empty.
        //http://code.google.com/p/android/issues/detail?id=19917
        //outState.putString("WORKAROUND_FOR_BUG_19917_KEY", "WORKAROUND_FOR_BUG_19917_VALUE");
        //super.onSaveInstanceState(outState);
    	logger.debug("PageActivity", "Save instance state called");
    }
    @SuppressLint("NewApi")
	public void restart() {
        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.HONEYCOMB) {
            recreate();
        } else {
            Intent intent = getIntent();
            overridePendingTransition(0, 0);
            intent.addFlags(Intent.FLAG_ACTIVITY_NO_ANIMATION);
            finish();

            overridePendingTransition(0, 0);
            startActivity(intent);
        }
    }

    protected abstract boolean shouldOpenSplash();

    protected abstract Intent getSplashIntent();

    protected abstract void enterTheDragon();

    public HashMap<String, PageView> getPageViewsCache() {
    	return pageViewsCache;
    }
    
    public void clearPageViewsCache() {
        if (pageViewsCache != null && pageViewsCache.size() > 0) {
            Iterator<String> iterator = pageViewsCache.keySet().iterator();
            PageView view = pageViewsCache.get(iterator.next());
            view.clearCache(true);

            pageViewsCache.clear();
        } else {
            pageViewsCache = new HashMap<String, PageView>();
        }
    }

    public PageFragment getActiveFragment() {
        return activeFragment;
    }

    public FeatureBinder getBinder() {
        return binder;
    }
    
    public void openHomePage() {
        String configuration = null;
        String pageName = getResources().getString(R.string.page_home_name);
        String pageUrl = getResources().getString(R.string.page_home_url);
        open(pageName, pageUrl, null, configuration);        
    }

    public void setPageResult(String result) {
        String parentPage = activeFragment.getPage().getParent();
        if (parentPage != null) {
            PageFragment fragment = (PageFragment) fragmentManager.findFragmentByTag(parentPage);
            if (fragment != null) {
                fragment.getPage().setResult(result);
            }
        }
    }

    public void close(boolean destroy) {
        if (destroy) {
            pageViewsCache.remove(activeFragment.getPage().getName());
        }
        goBack();
    }

    protected Page createPage(String name, String url, String data, String configuration, Page parentPage) {
        data = (data == null) ? "{}" : data;
        Page page = Mowbly.createPage().setName(name)
        					.setUrl(url)
                            .setData(data)
                            .setConfiguration(configuration)
                            .setParent(parentPage);
        return page;
    }

    protected Page createPage(String name, String url, String data, String configuration) {
        return createPage(name, url, data, configuration, null);
    }

    protected PageFragment createPageFragment(Page page) {
        Bundle args = new Bundle();
        args.putParcelable("page", page);
        
        PageFragment fragment = Mowbly.createPageFragment();
        fragment.setArguments(args);

        return fragment;
    }

    private void addOrReplacePageFragment(PageFragment fragment, String name, String type) {
        FragmentTransaction fragmentTransaction = fragmentManager.beginTransaction();
        fragmentTransaction.setCustomAnimations(R.anim.slide_in_right, R.anim.slide_out_left, R.anim.slide_in_left, R.anim.slide_out_right);            

        if (type.equals("add")) {
            fragmentTransaction.add(R.id.fragment_container, fragment, name);
        } else if (type.equals("replace")) {
            fragmentTransaction.replace(R.id.fragment_container, fragment, name);
        }

        fragmentTransaction.addToBackStack(name);
        fragmentTransaction.commit();
    }

    protected void addPageFragmentToBackStack(PageFragment fragment, String name) {
        addOrReplacePageFragment(fragment, name, "add");
    }

    protected void replacePageFragmentInBackStack(PageFragment fragment, String name) {
        addOrReplacePageFragment(fragment, name, "replace");
    }

    public void open(String name, String url, String data, String configuration) {
        if (!bIsCommitAllowed) {
            return;
        }

        PageFragment frag = activeFragment;
        while (frag != null && !frag.getPage().isRetainedInViewStack()) {
            frag = (PageFragment) fragmentManager.findFragmentByTag(frag.getPage().getParent());
        }
        Page parentPage = (frag != null) ? frag.getPage() : null; 

        PageFragment fragment = (PageFragment) fragmentManager.findFragmentByTag(name);
        if (fragment == null) {
            navDirection = (pageViewsCache.containsKey(name)) ? Navigation.FORWARD : Navigation.NEW;
            fragment = createPageFragment(createPage(name, url, data, configuration, parentPage));
            addPageFragmentToBackStack(fragment, name);
        } else {
            navDirection = Navigation.FORWARD;
            fragmentManager.popBackStack(name, 0);
            fragment.getPage().setData(data);
        }

        activeFragment = fragment;
    }

    protected boolean popAllAndOpen(String name, String url, String data, String configuration) {
        clearPageViewsCache();
        if (!bIsCommitAllowed) {
        	logger.debug("PageActivity", "Failed popAllAndOpen - " + name);
            return false;
        }

        PageFragment fragment = (PageFragment) fragmentManager.findFragmentByTag(name);
        if (fragment != null) {
            fragmentManager.popBackStackImmediate(name, 0);

            setFragmentVisibility(fragment, false);

            navDirection = (pageViewsCache.containsKey(name)) ? Navigation.FORWARD : Navigation.NEW;
            PageFragment newFragment = createPageFragment(createPage(name, url, data, configuration));
            replacePageFragmentInBackStack(newFragment, name);
            activeFragment = newFragment;
        } else {
            activeFragment = null;
            fragmentManager.popBackStackImmediate(null, FragmentManager.POP_BACK_STACK_INCLUSIVE);
            open(name, url, data, configuration);
        }
        return true;
    }

    public void goBack() {
        fragmentsBundle.remove(activeFragment.getPage().getName());

        String parentPage = activeFragment.getPage().getParent();
        if(parentPage != null) {
            navDirection = Navigation.BACKWARD;

            fragmentManager.popBackStack(parentPage, 0);
        } else {
            moveTaskToBack(true);
        }
    }

    public void setFragmentVisibility(PageFragment f, boolean bVisible) {
        FragmentTransaction ft = fragmentManager.beginTransaction();
        if (bVisible) {
            ft.show(f);
        } else {
            ft.hide(f);
        }

        if (bIsCommitAllowed) {
            ft.commit();
        } else {
            ft.commitAllowingStateLoss();
        }
        fragmentManager.executePendingTransactions();
    }

    private void registerReceivers() {
        if (networkBroadcastReceiver == null) {
            networkBroadcastReceiver = new NetworkBroadcastReceiver();
        }
        registerReceiver(networkBroadcastReceiver, new IntentFilter(ConnectivityManager.CONNECTIVITY_ACTION));
    }

    private void unregisterReceivers() {
        if (networkBroadcastReceiver != null) {
            unregisterReceiver(networkBroadcastReceiver);
            networkBroadcastReceiver = null;
        }
        for (BroadcastReceiver b : receivers) {
			unregisterReceiver(b);
		}
        receivers.removeAll(receivers);
    }
    
    
    public Intent addReceiver(BroadcastReceiver receiver,
    		IntentFilter filter) {
    	receivers.add(receiver);
    	return registerReceiver(receiver, filter);
    }
    
    public void removeReceiver(BroadcastReceiver receiver){
    	unregisterReceiver(receiver);
    	receivers.remove(receiver);
    }

    public void loadJavascriptOnAllPages(final String javascript) {
        Iterator<PageView> iterator = pageViewsCache.values().iterator();
        while (iterator.hasNext()) {
            PageView view = iterator.next();
            view.loadJavascript(javascript);
        }
    }

    public void loadJavascriptOnCurrentPage(final String javascript) {
        if (activeFragment != null && activeFragment.getPageView() != null) {
            activeFragment.getPageView().loadJavascript(javascript);
        }
    }

    private class NetworkBroadcastReceiver extends BroadcastReceiver {

        private void loadJavascript(final String javascript) {
            final PageActivity activity = PageActivity.this;
            activity.runOnUiThread(new Runnable() {
                @Override
                public void run() {
                    activity.loadJavascriptOnAllPages(javascript);
                }
            });
        }

        private void onNetworkConnected() {
            logger.info("NetworkManager", "Network connected");

            NetworkFeature network = (NetworkFeature) binder.get(NetworkFeature.NAME);
            Response resp = network.getActiveNetwork();
            String res = Mowbly.getGson().toJson(resp);
            loadJavascript("__mowbly__.Network.onConnect("+ res +");");
        }

        private void onNetworkDisconnected() {
            logger.info("NetworkManager", "Network disconnected");

            loadJavascript("__mowbly__.Network.onDisconnect();");
        }

        @Override
        public void onReceive(Context context, Intent intent) {            
            if (!intent.getAction().equals(ConnectivityManager.CONNECTIVITY_ACTION)) {
                return;
            }

            boolean noConnectivity = intent.getBooleanExtra(ConnectivityManager.EXTRA_NO_CONNECTIVITY, false);
            if (noConnectivity) {
                onNetworkDisconnected();
            } else {
                onNetworkConnected();
            }
        }
    }
}
