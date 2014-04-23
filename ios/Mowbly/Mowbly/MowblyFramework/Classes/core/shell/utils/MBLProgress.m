//
//  MBLProgress.m
//  XPLCIMS MobiE
//
//  Created by Sathish on 24/10/11.
//  Copyright (c) 2011-2014 CloudPact. All rights reserved.
//

#import "MBLAppDelegate.h"
#import "MBLProgress.h"
#import "MBProgressHUD.h"

static MBProgressHUD *progress;     // Progress
static bool bVisible;               // Tells if the progress bar is visible.

@implementation MBLProgress

+ (void)hideProgress {

    if (progress) {

     	[progress hide:YES];
        [progress removeFromSuperview];
        progress=nil;
    }   
    bVisible = NO;
} // hideProgress

+ (bool)isVisible {

    return bVisible;
} // isVisible

+ (void)showProgressWithTitle:(NSString *)title {
	
	[self showProgressWithTitle:title message:@""];
} // showProgressWithMessage

+ (void)showProgressWithTitle:(NSString *)title message:(NSString *)message {

	if(progress == nil) {
        
        MBLAppDelegate *appDelegate = (MBLAppDelegate *)[[UIApplication sharedApplication] delegate];
		progress = [[MBProgressHUD alloc] initWithWindow:appDelegate.window];
		[appDelegate.window addSubview:progress];
		[progress setDimBackground:YES];
       	[progress show:YES];
	}
	
	[progress setLabelFont:[UIFont fontWithName:@"HelveticaNeue" size:14]];
	if(! [title isEqual:[NSNull null]]) {

        [progress setLabelText:title];
    }
    if(! [message isEqual:[NSNull null]]) {

        [progress setDetailsLabelText:message];
    }
    bVisible = YES;
} // showProgressWithTitle:message

@end