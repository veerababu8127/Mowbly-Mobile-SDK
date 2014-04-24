//
//  MBLWebView.h
//  Mowbly
//
//  Created by Sathish on 06/04/12.
//  Copyright (c) 2011-2014 CloudPact. All rights reserved.

#import <UIKit/UIKit.h>

@class MBLAppDelegate;
@class MBLFeatureBinder;
@class MBLJSMessage;

@interface MBLWebView : UIWebView <UIWebViewDelegate> {
    
    MBLAppDelegate *appDelegate;        // App delegate for global app context

    NSString *name;                     // the unique name of the view
	NSString *url;						// the url to load in the WebView
    BOOL _reload;                       // Tells if the page needs to be reloaded
    BOOL _isActive;                     // Tells if the browser is in active view

    BOOL bWebViewLoaded;				// Tells if the webview has loaded
}

@property (readonly)MBLAppDelegate* appDelegate;
@property (nonatomic, copy) NSString* name;
@property (nonatomic, copy) NSString* url;
@property BOOL isActive;
@property BOOL bReload;

- (id)initWithName:(NSString *)viewName;

- (id)initWithName:(NSString *)viewName configuration:(NSDictionary *)configuration;

- (id)initWithName:(NSString *)viewName frame:(CGRect)frame;

- (NSDictionary *)getViewContext;

- (void)loadUrl:(NSString *)theUrl;

- (void)messageJS:(NSString *)message;

- (void)pushJavascriptMessage:(NSString *)message;

- (void)onMessage:(NSString *)message;

- (void)invoke:(MBLJSMessage *)jsMessage;

@end