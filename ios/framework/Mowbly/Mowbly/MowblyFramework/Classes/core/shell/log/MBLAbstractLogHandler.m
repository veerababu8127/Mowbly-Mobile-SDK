//
//  MBLAbstractLogHandler.m
//  Mowbly
//
//  Created by Sathish on 05/09/12.
//  Copyright (c) 2011-2014 CloudPact. All rights reserved.
//

#import "MBLAbstractLogHandler.h"
#import "MBLLogEvent.h"
#import "MBLLogLevel.h"
#import "MBLSimpleLogLayout.h"

@implementation MBLAbstractLogHandler

@synthesize isPropagationAllowed = _isPropagationAllowed,
            layout = _layout,
            name = _name,
            threshold = _threshold;

- (id)init {

    if((self = [super init])) {

        [self setIsPropagationAllowed:YES];

        MBLSimpleLogLayout *l = [[MBLSimpleLogLayout alloc] init];
        [self setLayout:l];

        [self setName:@"__log"];
        [self setThreshold:[MBLLogLevel levelFrom:[[[[NSBundle mainBundle] infoDictionary] valueForKey:@"MBLLogLevel"] intValue]]];
    }
    
    return self;
} // init

#pragma mark -
#pragma mark MBLLogHandler methods

- (void)handle:(MBLLogEvent *)logEvent {

    NSLog(@"%@", [[self layout] format:logEvent]);
} // handle:

- (void)reset {

    
} // reset

- (void)setUp {

    
} // setUp

- (void)start {

    
} // start

- (void)stop {

    
} // stop



@end