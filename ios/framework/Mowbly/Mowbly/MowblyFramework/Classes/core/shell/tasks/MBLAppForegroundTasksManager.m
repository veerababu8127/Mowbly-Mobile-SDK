//
//  MBLAppForegroundTasksManager.m
//  Mowbly
//
//  Created by Sathish on 01/10/13.
//  Copyright (c) 2011-2014 CloudPact. All rights reserved.

#import "MBLAppDelegate.h"
#import "MBLAppForegroundTasksManager.h"
#import "MBLRootViewController.h"

@implementation MBLAppForegroundTasksManager

static MBLAppForegroundTasksManager *_tasksManager;  // static instance

#pragma mark -
#pragma mark Singleton

// Returns the singleton instance of MBLAppForegroundTasksManager class
+ (MBLAppForegroundTasksManager *)defaultManager {
    
	if (_tasksManager == nil) {
        
		_tasksManager = [[super alloc] init];
    }
    return _tasksManager;
} // defaultManager

- (id)init {
    
    if ((self = [super init])) {
        
	}
    
    return self;
} // init

- (id)copyWithZone:(NSZone *)zone {
    return self;
} // copyWithZone


#pragma mark -
#pragma mark MBLAppForegroundTasksManager methods

- (MBLAppDelegate *)appDelegate {

    return (MBLAppDelegate *)[[UIApplication sharedApplication] delegate];
} // appDelegate

- (void)runTasks {
    [self startConnectivityCheck];
} // runTasks

- (void)startConnectivityCheck {
    
    MBLConnectivity *connectivity = [MBLConnectivity getInstance];
    MBLRootViewController *rvc = [[[[self appDelegate] navigationController] viewControllers] objectAtIndex:0];
    [connectivity setDelegate:rvc];
    [connectivity startReachabilityCheck];
} // startConnectivityCheck



@end