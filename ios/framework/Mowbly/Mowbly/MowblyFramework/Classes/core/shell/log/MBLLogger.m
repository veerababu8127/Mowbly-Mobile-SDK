//
//  MBLLogger.m
//  Mowbly
//
//  Created by Sathish on 04/09/12.
//  Copyright (c) 2011-2014 CloudPact. All rights reserved.
//

#import "CJSONDeserializer.h"
#import "MBLApp.h"
#import "MBLConsoleLogHandler.h"
#import "MBLConstants.h"
#import "MBLDatabaseLogHandler.h"
#import "MBLHttpClient.h"
#import "MBLLogger.h"
#import "MBLRotatingFileLogHandler.h"

@implementation MBLLogger

static NSMutableDictionary* _loggers;

#pragma mark -
#pragma mark Singleton

+ (void)configureDefaultLoggers {

    [MBLLogLevel initialize];
    
    _loggers = [NSMutableDictionary dictionaryWithCapacity:2];

    MBLLogger *systemLogger = [self loggerByName:SYSTEM];
    MBLLogger *userLogger = [self loggerByName:USER];

    MBLDatabaseLogHandler *dbHandler = [[MBLDatabaseLogHandler alloc] init];
    [systemLogger addHandler:dbHandler];
    [userLogger addHandler:dbHandler];
    
    NSString *logFile = [MBLApp logFilePath];
    MBLRotatingFileLogHandler *fileHandler = [[MBLRotatingFileLogHandler alloc] initWithFile:logFile];
    [systemLogger addHandler:fileHandler];
    [userLogger addHandler:fileHandler];

#ifdef DEBUG

    // Enable console log handler for debug
    MBLConsoleLogHandler *consoleHandler = [[MBLConsoleLogHandler alloc] init];
    [systemLogger addHandler:consoleHandler];
    [userLogger addHandler:consoleHandler];

#endif
} // configureLoggers

+ (MBLLogger *)loggerByName:(NSString*)name {

    MBLLogger *logger = nil;
    if(! _loggers) {

        _loggers = [[NSMutableDictionary alloc] init];
    }
    logger = [_loggers objectForKey:name];
    if(! logger) {

        logger = [[self alloc] initWithName:name];
        [_loggers setObject:logger forKey:name];
    }

    return logger;
} //loggerByName:

+ (void)log:(NSString*)message withTag:(NSString *)tag andLevel:(MBLLogLevel *)level {

    [[self loggerByName:SYSTEM] log:message
                            withTag:tag
                              level:level];
} // log:withTag:andLevel

+ (void)reset {

    [_loggers enumerateKeysAndObjectsUsingBlock:^(id key, id obj, BOOL *stop) {

        MBLLogger *logger = (MBLLogger *)obj;
        [logger reset];
    }];
} // reset


+ (void)setUp {

    [_loggers enumerateKeysAndObjectsUsingBlock:^(id key, id obj, BOOL *stop) {
        
        MBLLogger *logger = (MBLLogger *)obj;
        [logger setUp];
    }];
} // setUp

+ (void) setThresholdForAllLoggers:(MBLLogLevel *) level {
    
    // Iterating through all the loggers and setting the threshold for each of them
    [_loggers enumerateKeysAndObjectsUsingBlock:^(id key, id obj, BOOL *stop) {
        
        MBLLogger *logger = (MBLLogger *)obj;
        [logger setThreshold:level];
    }];
}


@end