//
//  MBLProgress.h
//  XPLCIMS MobiE
//
//  Created by Sathish on 24/10/11.
//  Copyright (c) 2011-2014 CloudPact. All rights reserved.
//

#import <Foundation/Foundation.h>

@interface MBLProgress : NSObject

#pragma mark -
#pragma mark Progress methods

+ (void)hideProgress;

+ (bool)isVisible;

+ (void)showProgressWithTitle:(NSString *)title;

+ (void)showProgressWithTitle:(NSString *)title message:(NSString *)message;

@end