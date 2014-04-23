//
//  MBLLogLevel.m
//  Mowbly
//
//  Created by Sathish on 04/09/12.
//  Copyright (c) 2012 CloudPact. All rights reserved.
//

#import "MBLLogLevel.h"

@implementation MBLLogLevel

MBLLogLevel* LEVEL_DEBUG = nil;
MBLLogLevel* LEVEL_INFO = nil;
MBLLogLevel* LEVEL_WARN = nil;
MBLLogLevel* LEVEL_ERROR = nil;
MBLLogLevel* LEVEL_FATAL = nil;

+ (void)initialize {

    LEVEL_DEBUG = [MBLLogLevel logLevelWithPriorityValue:[NSNumber numberWithInt:_DEBUG] andPriorityStr:@"DEBUG"];
    LEVEL_INFO = [MBLLogLevel logLevelWithPriorityValue:[NSNumber numberWithInt:INFO] andPriorityStr:@"INFO"];
    LEVEL_WARN = [MBLLogLevel logLevelWithPriorityValue:[NSNumber numberWithInt:WARN] andPriorityStr:@"WARN"];
    LEVEL_ERROR = [MBLLogLevel logLevelWithPriorityValue:[NSNumber numberWithInt:ERROR] andPriorityStr:@"ERROR"] ;
    LEVEL_FATAL = [MBLLogLevel logLevelWithPriorityValue:[NSNumber numberWithInt:FATAL] andPriorityStr:@"FATAL"];
} // initialize

+ (MBLLogLevel *)logLevelWithPriorityValue:(NSNumber *)priority andPriorityStr:(NSString *)priorityStr {

    return [[MBLLogLevel alloc] initWithPriorityValue:priority andPriorityStr:priorityStr];
} // logLevelWithPriorityValue:andPriorityStr

- (NSString *)description {

    return [self priorityStr];
} // description

+ (MBLLogLevel *)levelFrom:(int)levelValue{
    
    if(levelValue == 10000){
        return LEVEL_DEBUG;
    } else if(levelValue == 20000){
        return LEVEL_INFO;
    } else if(levelValue ==30000){
        return LEVEL_WARN;
    } else if(levelValue ==40000){
        return LEVEL_ERROR;
    } else if(levelValue ==50000){
        return LEVEL_FATAL;
    }
    return LEVEL_DEBUG;
}

@end