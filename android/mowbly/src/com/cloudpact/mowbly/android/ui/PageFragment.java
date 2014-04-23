package com.cloudpact.mowbly.android.ui;

import java.util.HashMap;

import android.app.Activity;
import android.content.Intent;
import android.os.Bundle;
import android.support.v4.app.Fragment;
import android.support.v4.app.FragmentManager;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;

import com.cloudpact.mowbly.android.Mowbly;
import com.cloudpact.mowbly.android.feature.FeatureBinder;
import com.cloudpact.mowbly.android.page.Page;
import com.cloudpact.mowbly.android.page.PageEventHandler;
import com.cloudpact.mowbly.android.service.NetworkService;
import com.cloudpact.mowbly.android.service.PreferenceService;
import com.google.gson.JsonObject;

/**
 * PageFragment
 * 
 * @author Prashanth Jonnala <prashanth@cloudpact.com>
 */
public class PageFragment extends Fragment implements PageEventHandler {

    protected Page page;
    protected String finalUrl;
    protected PageActivity activity;
    protected PageView pageView;
    protected PreferenceService preferenceService;
    protected NetworkService networkService;

    public PageFragment() {
        preferenceService = Mowbly.getPreferenceService();
        networkService = Mowbly.getNetworkService();
    }

    public Page getPage() {
        return page;
    }

    public void setPage(Page p) {
        page = p;
    }

    public PageActivity getPageActivity() {
        return activity;
    }

    public PageView getPageView() {
        return pageView;
    }

    protected JsonObject getPreferences(){
    	return preferenceService.getMowblyPreferences();
    }
    
    protected JsonObject getPageContext(){
    	JsonObject context = new JsonObject();
        context.addProperty("pageName", page.getName());
        context.add("preferences", getPreferences());
        context.addProperty("network", networkService.isConnected());
        String pageParent = page.getParent();
        context.addProperty("pageParent", pageParent);
        return context;
    }
    
    protected JsonObject getPageContextForLoad() {
    	JsonObject context = getPageContext();
    	context.addProperty("data", page.getData());
    	return context;
    }

    protected JsonObject getPageContextForReOpen(boolean poppedFromStack, String data){
    	JsonObject context = getPageContext();
    	context.addProperty("data", data);
    	context.addProperty("waitingForResult", poppedFromStack);
    	return context;
    }

    @Override
    public void onAttach(Activity activity) {
        super.onAttach(activity);

        this.activity = (PageActivity) activity;
    }

    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);

        Bundle args = getArguments();
        page = args.getParcelable("page");

        String url = page.getUrl();
        finalUrl = Mowbly.getBasePagePath() + url;
    }

    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
        if (page.isRetainedInViewStack()) {
            if (pageView == null) {
            	PageActivity activity = (PageActivity)getActivity();
                if (activity.pageViewsCache == null) {
                    activity.pageViewsCache = new HashMap<String, PageView>();
                }
                pageView = activity.pageViewsCache.get(page.getName());
                if (pageView == null) {
                    pageView = Mowbly.createPageView();
                    pageView.setEventHandler(this);
                    activity.pageViewsCache.put(page.getName(), pageView);
                }
            }
        } else {
            //pageView = (PageView) inflater.inflate(R.layout.pageview, null, false);
            pageView = Mowbly.createPageView();
            pageView.setEventHandler(this);
        }
        return pageView;
    }

    @Override
    public void onActivityCreated(Bundle savedInstanceState) {
        super.onActivityCreated(savedInstanceState);

        FeatureBinder binder = ((PageActivity) getActivity()).getBinder();
        binder.setFragment(this);
        pageView.addJavascriptInterface(binder, binder.getName());
    }

    @Override
    public void onStart() {
        super.onStart();

        FragmentManager fragmentManager = activity.getSupportFragmentManager();
        if (fragmentManager != null && pageView != null) {
            fragmentManager.putFragment(activity.fragmentsBundle, page.getName(), this);
        }
    }

    @Override
    public void onResume() {
        super.onResume();

        pageView.load(finalUrl);
    }

    @Override
    public void onActivityResult(int requestCode, int resultCode, Intent data) {
        super.onActivityResult(requestCode, resultCode, data);
    }

    @Override
    public void onDestroyView() {
        super.onDestroyView();

        ViewGroup parent = (ViewGroup) pageView.getParent();
        parent.removeView(pageView);
    }

    @Override
    public void onPageLoad() {
        pageView.loadJavascript("__mowbly__._onPageLoad(" + getPageContextForLoad().toString() + ")");
    }

    @Override
    public void onDeviceBackPressed() {
        pageView.loadJavascript("__mowbly__._onDeviceBackPressed();");
    }

    @Override
    public void onPageReopen(boolean poppedFromStack) {
        FeatureBinder binder = ((PageActivity) getActivity()).getBinder();
        binder.setFragment(this);
        pageView.addJavascriptInterface(binder, binder.getName());

        String data = null;
        if(poppedFromStack){
        	data = page.getResult();
        	page.setResult(null);
        }else{
        	data = page.getData();
        	page.setData(null);
        }
        if (data == null) {
            data = "{}";
        } else if (!data.startsWith("{")) {
            data = "'" + data + "'";
        }
        pageView.loadJavascript(
        	"__mowbly__._pageOpened("+ 
        		getPageContextForReOpen(poppedFromStack, data).toString()
        	+");" 
        );
    }

    @Override
    public void onCallbackReceive(String callbackId, String result) {
        if (callbackId != null) {
            pageView.loadJavascript(
                "__mowbly__.__CallbackClient.onreceive('"+callbackId+"', "+result+");"
            );
        } else {
            pageView.loadJavascript(result);
        }
    }

    @Override
    public void onReceivedError(int errorCode, String description, String failingUrl) {
        int l = failingUrl.lastIndexOf("/");
        int e = failingUrl.lastIndexOf("/", l-1);
        String failedPageUrl = failingUrl.substring(e, failingUrl.length());

        String appDir = Mowbly.getFileService().getAppDir().getAbsolutePath();
        pageView.loadUrl(String.format(pageView.getPNFUrl() + "&code="+errorCode+"&desc="+description, appDir, failedPageUrl));
    }
}