//
//  MBLLogLevel.h
//  Mowbly
//
//  Created by Sathish on 04/09/12.
//  Copyright (c) 2011-2014 CloudPact. All rights reserved.
//

#import "MBLLogPriority.h"

typedef enum _Level {

    _DEBUG = 10000,
    INFO = 20000,
    WARN = 30000,
    ERROR = 40000,
    FATAL = 50000
} Level;

@interface MBLLogLevel : MBLLogPriority

+ (MBLLogLevel *)logLevelWithPriorityValue:(NSNumber *)priority andPriorityStr:(NSString *)priorityStr;

extern MBLLogLevel* LEVEL_DEBUG;
extern MBLLogLevel* LEVEL_INFO;
extern MBLLogLevel* LEVEL_WARN;
extern MBLLogLevel* LEVEL_ERROR;
extern MBLLogLevel* LEVEL_FATAL;

+ (MBLLogLevel *)levelFrom:(int)levelValue;

@end