//
//  MBLLogPriority.m
//  Mowbly
//
//  Created by Tushar Ranjan Sahoo on 06/08/12.
//  Copyright (c) 2011-2014 CloudPact. All rights reserved.
//

#import "MBLLogPriority.h"

@interface MBLLogPriority(private)

- (NSComparisonResult)compare:(MBLLogPriority *)otherPriority;

@end

@implementation MBLLogPriority

@synthesize priority = _priority,
            priorityStr = _priorityStr;

- (id)initWithPriorityValue:(NSNumber *)priorityValue andPriorityStr:(NSString *)priorityString {

    if(self = [super init]){

        [self setPriority:priorityValue];
        [self setPriorityStr:priorityString];
    }

    return self;
} // initWithPropertyValue:andPriorityStr

- (BOOL) isGreaterOrEqualTo:(MBLLogPriority *)otherPriority {

    return ([self compare:otherPriority] == NSOrderedDescending || [self compare:otherPriority] == NSOrderedSame);
} // isGreaterOrEqualTo

- (NSComparisonResult)compare:(MBLLogPriority *)otherPriority {

    NSNumber* iPriority = [otherPriority priority];
    return [_priority compare:iPriority];
} // compare:


@end