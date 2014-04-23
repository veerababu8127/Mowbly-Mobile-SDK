
//
//  MBLPage.m
//  Mowbly
//
//  Created by Sathish on 30/03/12.
//  Copyright (c) 2011-2014 CloudPact. All rights reserved.
//

#import "CJSONDeserializer.h"
#import "CJSONSerializer.h"
#import "MBLApp.h"
#import "MBLAppDelegate.h"
#import "MBLConnectivity.h"
#import "MBLConstants.h"
#import "MBLJSMessage.h"
#import "MBLPage.h"
#import "MBLPageViewController.h"

@interface MBLPage (private)

- (MBLPageViewController *)viewController;

@end

@implementation MBLPage

@synthesize data = _data,
            parentPage = _parentPage,
            retainInViewStack = _bRetainInViewStack;

#pragma mark -
#pragma mark Private

- (MBLPageViewController*)viewController {
    
    MBLPageViewController *vc = nil;
    for (UIView* next = [self superview]; next; next = next.superview) {
        
        UIResponder* nextResponder = [next nextResponder];
        if ([nextResponder isKindOfClass:[MBLPageViewController class]]) {
            
            vc = (MBLPageViewController *)nextResponder;
        }
    }
    
    if(! vc) {
        
        UIViewController *topViewController = [[appDelegate navigationController] topViewController];
        if([topViewController isKindOfClass:[MBLPageViewController class]]) {
            
            vc = (MBLPageViewController *)topViewController;
        }
    }
    
    return vc;
} // viewController

#pragma mark -
#pragma mark Initialization

- (id)initWithPageName:(NSString *)pageName configuration:(NSDictionary *)configuration {
    
    if(self = [super initWithName:pageName configuration:configuration]) {
        
        [self drawView];
    }

    return self;
} // initWithPageName:url

- (void)drawView {

    CGFloat screenWidth, screenHeight;
    CGRect screenRect = [[UIScreen mainScreen] bounds];
    CGRect statusBarRect = [[UIApplication sharedApplication] statusBarFrame];

    if (UIDeviceOrientationIsPortrait([UIApplication sharedApplication].statusBarOrientation)) {

        screenWidth = screenRect.size.width;
        screenHeight = screenRect.size.height;
        screenHeight = (screenHeight - statusBarRect.size.height);
    } else {

        screenWidth = screenRect.size.height;
        screenHeight = screenRect.size.width;
        screenHeight = (screenHeight - statusBarRect.size.width);
    }

    CGFloat startX = 0.0;
    CGFloat startY = 0.0;
    CGFloat width = screenWidth;
    CGFloat height = screenHeight;
    [self setFrame:CGRectMake(startX, startY, width, height)];
} // drawView

#pragma mark -
#pragma mark Page methods

- (NSDictionary *)getViewContext {

    NSMutableDictionary *ctx = [[NSMutableDictionary alloc] init];

    // View Controller CID
    NSString *cid = [[self pvc] cid];
    if(! cid) {
        
        cid = [[self viewController] cid];
    }
    [ctx setValue:cid forKey:CID];

    // Page name
    [ctx setValue:[self name] forKey:PAGE_NAME];

    // Parent
    [ctx setValue:[self parentPage] forKey:PAGE_PARENT];
    
    // Network
    [ctx setValue:[NSNumber numberWithBool:[[MBLConnectivity getInstance] networkConnected]] forKey:NETWORK];

    // Orientation
    [ctx setValue:[NSNumber numberWithInt:[UIApplication sharedApplication].statusBarOrientation] forKey:ORIENTATION];
    
    NSError *error = nil;
   
    // Preferences - Merge page and global preferences
    NSDictionary *preferences = [NSDictionary dictionary];
    NSString *gPrefStr = [self getPreferences];
    if (gPrefStr) {

        NSDictionary *gPreferences = [[CJSONDeserializer deserializer] deserializeAsDictionary:[gPrefStr dataUsingEncoding:NSUTF8StringEncoding]
                                                                                         error:&error];
        if (gPreferences) {

            preferences = gPreferences;
        }
    }
    [ctx setValue:preferences forKey:PREFERENCES];
     preferences = nil;

    // Translations
    [ctx setValue:[MBLApp getLanguage] forKey:LANGUAGE];

    // Page data
    NSString *pageData = [self data];
    if(pageData && ![[NSNull null] isEqual:pageData]) {

        // Enclose within quotes if a string; not JSON object
        if (! [pageData hasPrefix:@"{"]) {

            pageData = [NSString stringWithFormat:@"'%@'", pageData];
        }
    } else {

        pageData = @"{}";
    }
    [ctx setValue:pageData forKey:DATA];

    NSDictionary *context = [NSDictionary dictionaryWithDictionary:ctx];
    ctx = nil;
    
    return context;
} // getPageContext

- (void)onPageLoad:(BOOL)bPoppedFromViewStack {

    // Send page result on resume
    NSString *result = [self data];
    if (result && ![[NSNull null] isEqual:result]) {
        
        // Enclose within quotes if a string; not JSON object
        if (! [result hasPrefix:@"{"]) {
            result = [NSString stringWithFormat:@"'%@'", result];
        }
    } else {
        
        result = @"{}";
    }
    
    int poppedStringValue = [[NSNumber numberWithBool:bPoppedFromViewStack] intValue];
    NSMutableDictionary *context = [[NSMutableDictionary alloc]initWithDictionary:[self getViewContext]];
    if(bPoppedFromViewStack)
        [context setObject:result forKey:@"result"];
    [context setObject:[NSNumber numberWithInt:poppedStringValue] forKey:@"waitingForResult"];
    
    NSError *err = nil;
   NSString *contextStr = [[NSString alloc] initWithData:[[CJSONSerializer serializer] serializeDictionary:context error:&err]
                                 encoding:NSUTF8StringEncoding];
    // Raise the onAfterLoad event
    [self pushJavascriptMessage:[NSString stringWithFormat:@"window.__mowbly__._pageOpened(%@);",contextStr]];

    // Reset the result data
    [self setData:nil];
} // onPageLoad


- (void)setResultForPage:(NSString *)result {

    MBLPage *parent = [[[appDelegate pageViewController] pages] objectForKey:[self parentPage]];
    [parent setData:result];
} // setResultForPage
                              
- (NSString *) getPreferences {
    
    NSUserDefaults *settings = [NSUserDefaults standardUserDefaults];
	return [settings objectForKey:MOWBLY_PREFERENCES];
} // getGlobalPreferences

#pragma mark -
#pragma mark UIWebViewDelegate methods

- (void)webViewDidFinishLoad:(UIWebView *)theWebView {
    
    [super webViewDidFinishLoad:theWebView];
    NSString *theUrl = [[[theWebView request] URL] absoluteString];	
	if(! [theUrl isEqualToString:@"about:blank"]) {

        // Clear data
        [self setData:nil];
    }
}


@end