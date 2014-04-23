package com.cloudpact.mowbly.android.ui;

import android.annotation.SuppressLint;
import android.annotation.TargetApi;
import android.content.Context;
import android.content.Intent;
import android.graphics.Canvas;
import android.net.Uri;
import android.os.Build;
import android.util.AttributeSet;
import android.view.MotionEvent;
import android.view.View;
import android.webkit.WebChromeClient;
import android.webkit.WebView;
import android.webkit.WebViewClient;
import android.widget.RelativeLayout;

import com.cloudpact.mowbly.R;
import com.cloudpact.mowbly.android.Mowbly;
import com.cloudpact.mowbly.android.page.NOPPageEventHandler;
import com.cloudpact.mowbly.android.page.PageEventHandler;

/**
 * PageView
 * 
 * @author Prashanth Jonnala <prashanth@cloudpact.com>
 */
public class PageView extends WebView {

    private boolean mBackgroundRemoved = false;

    private PageEventHandler eventHandler = new NOPPageEventHandler();

    private boolean isAlreadyLoaded = false;

    public PageView(Context context) {
        super(context);
        init();
    }

    public PageView(Context context, AttributeSet attrs, int defStyle) {
        super(context, attrs, defStyle);
        init();
    }

    public PageView(Context context, AttributeSet attrs) {
        super(context, attrs);
        init();
    }

    @SuppressLint({ "SetJavaScriptEnabled"})
    private void init() {
    	// TODO Assets alert not working - WebChromeClient issue
        setWebChromeClient(new WebChromeClient());
        setWebViewClient(new PageViewClient());
        setScrollBarStyle(View.SCROLLBARS_OUTSIDE_OVERLAY);
        setScrollbarFadingEnabled(true);
        setupHardwareAcceleration();
        
        getSettings().setJavaScriptEnabled(true);
        getSettings().setNeedInitialFocus(true);
        getSettings().setUserAgentString(PageActivity.userAgent);
        getSettings().setDomStorageEnabled(true);

        requestFocus(View.FOCUS_DOWN);
        setOnTouchListener(new View.OnTouchListener() {
            @Override
            public boolean onTouch(View v, MotionEvent event) {
                switch (event.getAction()) {
                    case MotionEvent.ACTION_DOWN:
                    case MotionEvent.ACTION_UP:
                        v.clearFocus();
                        if (!v.hasFocus()) {
                            v.requestFocus();
                        }
                        break;
                }
                return false;
            }
        });

        RelativeLayout.LayoutParams layoutParams = new RelativeLayout.LayoutParams(
                RelativeLayout.LayoutParams.MATCH_PARENT,
                RelativeLayout.LayoutParams.MATCH_PARENT);
        setLayoutParams(layoutParams);
    }

    @Override
    protected void onDraw(Canvas c) {
        super.onDraw(c);
        if (!mBackgroundRemoved && getRootView().getBackground() != null) {
            mBackgroundRemoved = true;
            post(new Runnable() {
                @SuppressWarnings("deprecation")
				public void run() {
                    getRootView().setBackgroundDrawable(null);
                }
            });
        }
    }

    @TargetApi(11)
    private void setupHardwareAcceleration() {
        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.HONEYCOMB) {
            if (isHardwareAccelerated()) {
                setLayerType(View.LAYER_TYPE_NONE, null);
            } else {
                setLayerType(View.LAYER_TYPE_SOFTWARE, null);
            }
        }
    }

    public void setEventHandler(PageEventHandler handler) {
        if (handler == null) {
            handler = new NOPPageEventHandler();
        }
        eventHandler = handler;
    }

    public PageEventHandler getEventHandler() {
        return eventHandler;
    }

    public void loadJavascript(String script) {
        loadUrl("javascript:try{"+script+"}catch(e){throw e}");
    }

    public void load(String url) {
        if (!isAlreadyLoaded) {
            loadUrl(getAbsoluteUrl(url));
        }
    }

    private boolean isExternalUrl(String url) {
        if (url.startsWith("/") || (url.startsWith("file") && url.endsWith("html"))) {
            return false;
        }
        return true;
    }

    protected String getAbsoluteUrl(String url) {
        if (isExternalUrl(url)) {
            return url;
        }

        if (url.startsWith("/")) {
            String appDir = Mowbly.getFileService().getAppDir().getAbsolutePath();
            url = "file://" + appDir + url;
        }
        return url;
    }
    
    protected String getPNFUrl(){
    	return getAbsoluteUrl(Mowbly.getBasePagePath() + getResources().getString(R.string.pnf_url) + "?pageUrl=%s");
    }

    private class PageViewClient extends WebViewClient {

        @Override
        public boolean shouldOverrideUrlLoading(WebView view, String url) {
            if (!url.startsWith("file")) {
                Intent intent = new Intent(Intent.ACTION_VIEW);
                intent.setData(Uri.parse(url));
                view.getContext().startActivity(intent);
                return true;
            }
            return false;
        }

        @Override
        public void onPageFinished(WebView view, String url) {
            if (!isAlreadyLoaded) {
                isAlreadyLoaded = true;
                eventHandler.onPageLoad();
            }
        }

        @Override
        public void onReceivedError(WebView view, int errorCode, String description, String failingUrl) {
            eventHandler.onReceivedError(errorCode, description, failingUrl);
        }
    }
}
