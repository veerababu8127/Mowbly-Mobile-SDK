//
//  MBLExternalTaskViewController.m
//  Mowbly
//
//  Created by Sathish on 02/11/11.
//  Copyright (c) 2011-2014 CloudPact. All rights reserved.
//

#import "MBLExternalPageViewController.h"
#import "MBLLogger.h"
#import "MBLProgress.h"

@implementation MBLExternalPageViewController

@synthesize url, webView;

#pragma mark -
#pragma mark App lifecycle methods

// Initializer
- (id)initWithUrl:(NSString *)theUrl {
    
    self = [super initWithNibName:@"MBLExternalPageViewController" bundle:nil];
    if (self) {

		// Set the url
		self.url = theUrl;
    }
    return self;
}// initWithUrl

- (void)viewDidLoad {

    //Adding webview to the view controller
    CGRect screenRect = [[UIScreen mainScreen] bounds];
    self.webView = [[UIWebView alloc]initWithFrame:screenRect];
    
    if([self respondsToSelector:@selector(edgesForExtendedLayout)])
        self.edgesForExtendedLayout = UIRectEdgeNone;

    // Add progress bar and title to navigation bar
    [self addProgressAndTitleToNavigationBar];
    
    // Get the webview
    self.webView = (UIWebView *)[[self view] viewWithTag:1];
    [self.webView setDelegate:self];
    
	// Load the url in the WebView
	NSURLRequest *request = [NSURLRequest requestWithURL:[NSURL URLWithString:self.url]];
    
    [self.webView loadRequest:request];
} // viewDidLoad

- (void)viewDidLayoutSubviews {

    if ([self respondsToSelector:@selector(topLayoutGuide)]) {

        CGRect viewBounds = self.view.bounds;
        CGFloat topBarOffset = self.topLayoutGuide.length;
        viewBounds.origin.y = topBarOffset * -1;
        self.view.bounds = viewBounds;
    }
} // viewDidLayoutSubviews

- (void)viewWillAppear:(BOOL)animated {
    
    [self.navigationController setNavigationBarHidden:NO];
} // viewWillAppear

- (void)viewDidDisappear:(BOOL)animated {

    [self.navigationController setNavigationBarHidden:YES];
} // viewDidDisappear

- (void)didRotateFromInterfaceOrientation:(UIInterfaceOrientation)fromInterfaceOrientation {

    [self setFrameForProgressAndTitle];
} // didRotateFromInterfaceOrientation

- (BOOL)shouldAutorotateToInterfaceOrientation:(UIInterfaceOrientation)interfaceOrientation {

    return YES;
} // shouldAutorotateToInterfaceOrientation

- (void)didReceiveMemoryWarning {

    [super didReceiveMemoryWarning];

    // Clear webview cache
    NSURLRequest *request = [NSURLRequest requestWithURL:[NSURL URLWithString:self.url]];
    [[NSURLCache sharedURLCache] removeCachedResponseForRequest:request];
} // didReceiveMemoryWarning

- (void)addProgressAndTitleToNavigationBar {

    // Add progress bar to navigation bar
    progressBar = [[UIActivityIndicatorView alloc] initWithActivityIndicatorStyle:UIActivityIndicatorViewStyleGray];
    [progressBar startAnimating];
    [self.navigationController.navigationBar addSubview:progressBar];

    // Set title
    title = [[UILabel alloc] init];
    [title setBackgroundColor:[UIColor clearColor]];
    [title setText:[[NSURL URLWithString:self.url] host]];
    [title setFont:[UIFont systemFontOfSize:16]];
    [title setTextColor:[UIColor whiteColor]];
    [title setTextAlignment:NSTextAlignmentCenter];
    [title setLineBreakMode:NSLineBreakByTruncatingTail];
    [self.navigationController.navigationBar addSubview:title];

    [self setFrameForProgressAndTitle];
} // addProgressAndTitleToNavigationBar

- (void)removeProgressAndTitleFromNavigationBar {

    // Remove and release progressbar
    if (progressBar) {

        [progressBar removeFromSuperview];
    }
    // Remove and release the title
    if (title) {

        [title removeFromSuperview];
    }
} // removeProgressAndTitleFromNavigationBar

- (void)setFrameForProgressAndTitle {

    // Get the width of the device, to postion the title
    CGRect screenBounds = [[UIScreen mainScreen] bounds];
    CGSize screenSize = CGSizeMake(screenBounds.size.width, screenBounds.size.height);
    float width, pbarMargin, titleMargin;

    int orientation = [self interfaceOrientation];
    if ((UIInterfaceOrientationIsPortrait(orientation))) {

        width = screenSize.width;
        pbarMargin = 10;
        titleMargin = 5;
    } else {

        width = screenSize.height;
        pbarMargin = 5;
        titleMargin = 0;
    }
    
    if (progressBar) {

        [progressBar setFrame:CGRectMake(width - 40, pbarMargin, 20, 20)];
    }
    
    if (title) {

        [title setFrame:CGRectMake(60, titleMargin, width-100, 30)];
    }
} // setFrameForProgressAndTitle

#pragma mark -
#pragma mark UIWebView delegate methods

- (void)webViewDidFinishLoad:(UIWebView *)theWebView {

    if (progressBar) {

        [progressBar stopAnimating];
    }
} // webViewDidFinishLoad

- (void)webView:(UIWebView *)theWebView didFailLoadWithError:(NSError *)error {

    LogError(@"External Page View failed to load url %@", [[theWebView request] URL]);
} // webView:didFailLoadWithError:

#pragma mark -
#pragma mark Memory management

- (void)dealloc {

    [self removeProgressAndTitleFromNavigationBar];
    [webView loadHTMLString:@"" baseURL:nil];
    [[NSURLCache sharedURLCache] removeAllCachedResponses];

} // dealloc

@end
