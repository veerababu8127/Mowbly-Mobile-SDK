//
//  MBLLogPriority.h
//  Mowbly
//
//  Created by Tushar Ranjan Sahoo on 06/08/12.
//  Copyright (c) 2011-2014 CloudPact. All rights reserved.
//

#import <UIKit/UIKit.h>

@interface MBLLogPriority : NSObject {

    NSNumber* _priority;
    NSString* _priorityStr;
}

@property (nonatomic, retain) NSNumber* priority;
@property (nonatomic, copy) NSString* priorityStr;

- (id)initWithPriorityValue:(NSNumber *)priorityValue andPriorityStr:(NSString *)priorityString;

- (BOOL)isGreaterOrEqualTo:(MBLLogPriority *) counterPriority;

@end