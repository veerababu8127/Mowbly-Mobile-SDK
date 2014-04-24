//
//  MBLAppForegroundTasksManager.h
//  Mowbly
//
//  Created by Sathish on 01/10/13.
//  Copyright (c) 2011-2014 CloudPact. All rights reserved.

#import <Foundation/Foundation.h>

@class MBLAppDelegate;

@interface MBLAppForegroundTasksManager : NSObject

@property (readonly)MBLAppDelegate *appDelegate;

+ (MBLAppForegroundTasksManager *)defaultManager;

- (void)runTasks;


@end