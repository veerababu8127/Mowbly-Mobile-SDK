//
//  MBLAppBackgroundTasksManager.h
//  Mowbly
//
//  Created by Veerababu on 20/03/14.
//  Copyright (c) 2014 CloudPact. All rights reserved.
//


#import <Foundation/Foundation.h>
#import "MBLAppDelegate.h"

@interface MBLAppBackgroundTasksManager : NSObject

@property (readonly)MBLAppDelegate *appDelegate;

+ (MBLAppBackgroundTasksManager *)defaultManager;

- (void)suspendTasks;

@end