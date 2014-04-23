//
//  MBLLogEvent.m
//  Mowbly
//
//  Created by Sathish on 06/08/12.
//  Copyright (c) 2011-2014 CloudPact. All rights reserved.
//

#import "MBLLogEvent.h"
#import "MBLLogLevel.h"
#import "MBLUtils.h"
#import "QuartzCore/QuartzCore.h"

@implementation MBLLogEvent

@synthesize isHandled = _isHandled,
            level = _level,
            message = _message,
            tag = _tag,
            type = _type,
            timestamp = _timestamp;

- (id) initWithType:(NSString *)logType
                tag:(NSString *)logTag
              level:(MBLLogLevel *)logLevel
            message:(NSString *)logMessage{

    if ((self = [super init])) {

        [self setIsHandled:NO];
        [self setLevel:logLevel];
        [self setMessage:logMessage];
        [self setTag:logTag];
        [self setType:logType];
        [self setTimestamp:[NSString stringWithFormat:@"%lld", (long long)[[NSDate date] timeIntervalSince1970] *1000]];
    }

    return self;
} // initWithType:tag:level:message

- (NSString *)description {

    return [NSString stringWithFormat:@"%@ [%@] %@ - %@\n", [self timestamp], [self level],[self tag],[self message]];
} // description


@end