//
//  MBLAppBackgroundTasksManager.m
//  Mowbly
//
//  Created by Veerababu on 20/03/14.
//  Copyright (c) 2014 CloudPact. All rights reserved.
//

#import "MBLAppBackgroundTasksManager.h"
#import "MBLConnectivity.h"

@implementation MBLAppBackgroundTasksManager


static MBLAppBackgroundTasksManager *_tasksManager;  // static instance

#pragma mark -
#pragma mark Singleton

// Returns the singleton instance of MBLAppBackgroundTasksManager class
+ (MBLAppBackgroundTasksManager *)defaultManager {
    
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

- (void)suspendTasks {
    [self stopConnectivityCheck];
} // suspendTasks

- (void)stopConnectivityCheck {
    
    MBLConnectivity *connectivity = [MBLConnectivity getInstance];
    [connectivity setDelegate:nil];
    [connectivity stopReachabilityCheck];
}

@end