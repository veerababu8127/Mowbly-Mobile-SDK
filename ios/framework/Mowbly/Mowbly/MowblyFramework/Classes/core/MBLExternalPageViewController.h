//
//  MBLExternalTaskViewController.h
//  Mowbly
//
//  Created by Sathish on 02/11/11.
//  Copyright (c) 2011-2014 CloudPact. All rights reserved.
//

#import <UIKit/UIKit.h>
#import "MBLPageViewController.h"

@interface MBLExternalPageViewController : MBLPageViewController <UIWebViewDelegate> {

    UIActivityIndicatorView *progressBar;   // Progress bar to display until content is loaded
    UILabel *title;                         // Custom title view
	NSString *url;                          // the url to load in the WebView
}

@property (nonatomic, retain) UIWebView *webView;
@property (nonatomic, copy) NSString* url;

#pragma mark -
#pragma mark App lifecycle methods

- (id)initWithUrl:(NSString *)theUrl;

- (void)addProgressAndTitleToNavigationBar;

- (void)removeProgressAndTitleFromNavigationBar;

- (void)setFrameForProgressAndTitle;

@end