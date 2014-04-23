//
//  MBLWebView.m
//  Mowbly
//
//  Created by Sathish on 06/04/12.
//  Copyright (c) 2011-2014 CloudPact. All rights reserved.

#import <QuartzCore/QuartzCore.h>
#import "CJSONDeserializer.h"
#import "CJSONSerializer.h"
#import "MBLApp.h"
#import "MBLAppDelegate.h"
#import "MBLFeatureBinder.h"
#import "MBLJSMessage.h"
#import "MBLPageViewController.h"
#import "MBLWebView.h"
#import "MBLLogger.h"

@implementation MBLWebView

@synthesize appDelegate, name, url, isActive = _isActive, bReload = _reload;

- (id)init {

    if((self = [super init])) {

        [self setDelegate:self];

        // Initialize objects
        appDelegate = (MBLAppDelegate *)[[UIApplication sharedApplication] delegate];
    }

    return self;
} // init

- (id)initWithName:(NSString *)viewName {

    if((self = [self init])) {

        [self setName:viewName];
    }

    return self;
} // initWithName

- (id)initWithName:(NSString *)viewName configuration:(NSDictionary *)configuration {

    if(self = [self initWithName:viewName]) {
        
    }

    return self;
} // initWithName:configuration

- (id)initWithName:(NSString *)viewName frame:(CGRect)frame {

    if ((self = [self initWithName:viewName])) {

        [self setFrame:frame];
    }
    return self;
} // initWithName:frame

#pragma mark -
#pragma mark View methods

- (void)loadUrl:(NSString *)theUrl {
    
    // Load the url
    [self setUrl:theUrl];
    NSURLRequest *request = [NSURLRequest requestWithURL:[NSURL URLWithString:self.url]];
    [self loadRequest:request];
} // loadUrl

// Overridden by page.
- (NSDictionary *)getViewContext {

    return [NSDictionary dictionary];
} // getViewContext

- (void)messageJS:(NSString *)message {
    
    [self stringByEvaluatingJavaScriptFromString:message];
} // messageJS

- (void)pushJavascriptMessage:(NSString *)message {
    
    [self performSelector:@selector(messageJS:) withObject:message afterDelay:0.1];
} // pushJavascriptMessage

- (void)onMessage:(id)message {

    NSString *messageStr;
    NSError *err = nil;
    
    messageStr = [[NSString alloc] initWithData:[[CJSONSerializer serializer] serializeObject:message error:&err] 
                                           encoding:NSUTF8StringEncoding];
    
    [self pushJavascriptMessage:[NSString stringWithFormat:@"window.__mowbly__._onPageMessage(%@, %@)", messageStr, name]];
} // onMessage

#pragma mark -
#pragma mark UIWebViewDelegate methods

- (void)webViewDidFinishLoad:(UIWebView *)webView {
    
    NSString *theUrl = [[[webView request] URL] absoluteString];
	if(! [theUrl isEqualToString:@"about:blank"]) {
        
        // Get the context for the page
        NSDictionary *context = [self getViewContext];
        NSError *err = nil;
        NSString *contextStr = [[NSString alloc] initWithData:[[CJSONSerializer serializer] serializeDictionary:context error:&err] 
                                                     encoding:NSUTF8StringEncoding];
        
        // Fire __mowbly__._onPageLoad with page context and page data
        [self pushJavascriptMessage:[NSString stringWithFormat:@"if(document.readyState == 'complete') { window.__mowbly__._onPageLoad(%@); } else { window.onload=function() { window.__mowbly__._onPageLoad(%@); } }", contextStr, contextStr]];

        
        // set webview loaded
        bWebViewLoaded = YES;
    }
}

- (BOOL) webView:(UIWebView *)theWebView 
shouldStartLoadWithRequest:(NSURLRequest *)request 
  navigationType:(UIWebViewNavigationType)navigationType {	
    
	NSURL *theUrl = [request URL];
    if ([[theUrl scheme] isEqualToString:@"about"])
    {
		return NO;
	}
    else if(bWebViewLoaded)
    {
        if (([[theUrl scheme] isEqualToString:@"file"] &&
             ([theUrl.pathExtension isEqualToString:@"html"] || [theUrl.pathExtension isEqualToString:@"htm"])) ||
            [[theUrl scheme] isEqualToString:@"tel"])
        {
            return YES;
        }
        else
        {
            // Link launched from the page. Another webpage or different mimetype.
            [MBLApp openExternalUrl:theUrl withTitle:[[request URL] host]];
            return NO;
        }
    }
    return YES;
} // webView:shouldStartLoadWithRequest:navigationType

- (void)webView:(UIWebView *)webView didFailLoadWithError:(NSError *)error{

    if([error code] == -1100){
        
        NSString *callingURL = [@"/system/pnf.html" stringByAddingPercentEscapesUsingEncoding:NSASCIIStringEncoding];
        
        NSURLRequest *req = [NSURLRequest requestWithURL:[NSURL fileURLWithPath:[[MBLApp appDirectory] stringByAppendingString:callingURL]                                 
                                                                    isDirectory:NO]];

        [self loadRequest:req];
    }
} // webView:didFailLoadWithError

#pragma mark -
#pragma mark Feature Binder Invoke

- (void) invoke:(MBLJSMessage *)jsMessage {

    // convey the message to FeatureBinder
    [[MBLFeatureBinder defaultBinder] invoke:jsMessage];
} // invoke:


@end