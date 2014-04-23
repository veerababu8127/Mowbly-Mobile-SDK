//
//  MBLConsoleHandler.m
//  Mowbly
//
//  Created by Sathish on 05/09/12.
//  Copyright (c) 2011-2014 CloudPact. All rights reserved.
//

#import "MBLLogger.h"
#import "MBLConsoleLogHandler.h"
#import "MBLLogEvent.h"

@implementation MBLConsoleLogHandler

- (id)init {
    
    if((self = [super init])) {
        
        [self setName:@"__console_log_handler"];
    }
    
    return self;
} // init

- (void)handle:(MBLLogEvent *)logEvent {

    NSLog(@"[%@] %@", [logEvent level], [logEvent message]);
} // handle:

@end