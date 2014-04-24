//
//  MBLHandler.h
//  Mowbly
//
//  Created by Sathish on 28/08/12.
//  Copyright (c) 2011-2014 CloudPact. All rights reserved.
//

#import "MBLLogLevel.h"
#import "MBLLogEvent.h"
#import "MBLLogLayout.h"

@protocol MBLLogHandler

@property (nonatomic, assign) BOOL isPropagationAllowed;
@property (nonatomic, retain) id<MBLLogLayout> layout;
@property (nonatomic, copy) NSString* name;
@property (nonatomic, retain) MBLLogLevel* threshold;

@required

- (void)setUp;

- (void)start;

- (void) handle:(MBLLogEvent *) logEvent;

- (void)reset;

- (void)stop;

@end