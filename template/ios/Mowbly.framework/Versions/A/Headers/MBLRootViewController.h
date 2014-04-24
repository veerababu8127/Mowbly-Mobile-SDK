//
//  MBLRootViewController.h
//  Mowbly
//
//  Created by Sathish on 07/06/11.
//  Copyright (c) 2011-2014 CloudPact. All rights reserved.
//
//  Root view controller of Mowbly app.
//  Custom app should have their root view controller extended from this.

#import <UIKit/UIKit.h>
#import "MBLConnectivity.h"
#import "MBLPageViewController.h"

@class MBLAppDelegate;

@interface MBLRootViewController : UIViewController <MBLConnectivityDelegate> {
    
    UIAlertView *alertDlg;                              // Confirm dialog for updates
    MBLAppDelegate *appDelegate;                        // App delegate for global context
}

@property (nonatomic,retain) UIActivityIndicatorView *progressBar;
@property (nonatomic,retain) UITextView *progressText;
@property (nonatomic, retain) UIImageView *splashScreenView;


- (void)launchAppWithName:(NSString *)name url:(NSString *)url;

- (void)launchApp;

- (void)launchHomeApp;

- (void)bootstrap;

- (NSString *)installationFailedMessagewithError:(NSError *)error;

- (void)launchDefaultApp:(BOOL)reload;

- (void)setInstallationProgress:(NSString *)message;

- (void)showSplashScreen;

- (void)stopProgressBar;

- (UIStatusBarStyle)preferredStatusBarStyle;

- (void)hideSplashScreen;

-(UIColor *)colorFromHexString:(NSString *)hexString;

@end