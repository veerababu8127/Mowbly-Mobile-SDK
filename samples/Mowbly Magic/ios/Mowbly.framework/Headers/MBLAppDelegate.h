//
//  MBLAppDelegate.h
//  Mowbly
//
//  Created by Sathish on 08/10/11.
//  Copyright (c) 2011-2014 CloudPact. All rights reserved.
//

#import <UIKit/UIKit.h>

@class MBLAppForegroundTasksManager;
@class MBLPageViewController;
@class MBLRootViewController;

@interface MBLAppDelegate : UIResponder <UIApplicationDelegate, UINavigationControllerDelegate> {

    MBLPageViewController *pageViewController;  // Page view controller
    MBLPageViewController *_topViewController;  // Top view controller
}

#pragma mark -
#pragma mark Application UI outlets

@property (readonly)MBLAppForegroundTasksManager *appForegroundTasksManager;
@property (readonly)MBLPageViewController *appViewController;
@property (nonatomic, retain) UINavigationController *navigationController;
@property (nonatomic, retain) MBLPageViewController *pageViewController;
@property (nonatomic, retain) UIWindow *window;

#pragma mark -
#pragma mark App context

- (void) initApp;

+ (NSBundle *)frameworkBundle;

- (MBLRootViewController *) allocRVC;

- (void) configLoggers ;

@end