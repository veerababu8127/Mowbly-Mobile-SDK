//
//  MBLPage.h
//  Mowbly
//
//  Created by Sathish on 30/03/12.
//  Copyright (c) 2011-2014 CloudPact. All rights reserved.
//

#import "MBLWebView.h"
@class MBLPageViewController;

@interface MBLPage : MBLWebView <UIWebViewDelegate> {
    
    NSString *_data;                     // Holds the data passed to the page
    NSString *_parentPage;               // Name of the parent page
    MBLPageViewController *_pvc;         // Parent view
	BOOL _bRetainInViewStack;            // Tells if the page is retained in the view stack
    NSArray *_supportedOrientations;     // Supported orientations
}

@property (nonatomic, copy) NSString *data;
@property (nonatomic, copy) NSString *parentPage;
@property (nonatomic, strong) MBLPageViewController *pvc;
@property (nonatomic) BOOL retainInViewStack;
    
- (id)initWithPageName:(NSString *)pageName configuration:(NSDictionary *)configuration;

- (void)onPageLoad:(BOOL)bPoppedFromViewStack;

- (void)setResultForPage:(NSString *)result;

- (void)drawView;

- (NSString *) getPreferences;

@end