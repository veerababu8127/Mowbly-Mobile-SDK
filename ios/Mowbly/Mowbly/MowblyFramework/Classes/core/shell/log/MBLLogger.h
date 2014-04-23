//
//  MBLLogger.h
//  Mowbly
//
//  Created by Sathish on 04/09/12.
//  Copyright (c) 2011-2014 CloudPact. All rights reserved.
//

#import "MBLAbstractLogger.h"
#import "MBLLogLevel.h"

@interface MBLLogger : MBLAbstractLogger

+ (void)configureDefaultLoggers;

+ (MBLLogger *)loggerByName:(NSString*)type;

+ (void)log:(NSString*)message withTag:(NSString *)tag andLevel:(MBLLogLevel *)level;

+ (void) setThresholdForAllLoggers:(MBLLogLevel *) level;

+ (void)reset;

+ (void)setUp;

// Log level switches
#ifndef LOGGING_LEVEL_INFO
#	define LOGGING_LEVEL_INFO		1
#endif
#ifndef LOGGING_LEVEL_WARN
#	define LOGGING_LEVEL_WARN		1
#endif
#ifndef LOGGING_LEVEL_ERROR
#	define LOGGING_LEVEL_ERROR		1
#endif
#ifndef LOGGING_LEVEL_DEBUG
#	define LOGGING_LEVEL_DEBUG		1
#endif
#ifndef LOGGING_LEVEL_FATAL
#	define LOGGING_LEVEL_FATAL		1
#endif

#define MBL_LOG(__level__, __fmt__, ...) { \
  NSString *__info__ = [NSString stringWithFormat:@"%s[Line %d] ", __PRETTY_FUNCTION__, __LINE__]; \
  NSString *__str__ = [NSString stringWithFormat:(__fmt__), ##__VA_ARGS__]; \
  [MBLLogger log:[__info__ stringByAppendingString:__str__] withTag:@"System" andLevel:__level__]; \
}

// Info
#if defined(LOGGING_LEVEL_INFO) && LOGGING_LEVEL_INFO
#define LogInfo(fmt, ...) MBL_LOG(LEVEL_INFO, fmt, ##__VA_ARGS__)
#else
#define LogInfo(...)
#endif

// Warn
#if defined(LOGGING_LEVEL_WARN) && LOGGING_LEVEL_WARN
#define LogWarn(fmt, ...) MBL_LOG(LEVEL_WARN, fmt, ##__VA_ARGS__)
#else
#define LogWarn(...)
#endif

// Error
#if defined(LOGGING_LEVEL_ERROR) && LOGGING_LEVEL_ERROR
#define LogError(fmt, ...) MBL_LOG(LEVEL_ERROR, fmt, ##__VA_ARGS__)
#else
#define LogError(...)
#endif

// Debug
#if defined(LOGGING_LEVEL_DEBUG) && LOGGING_LEVEL_DEBUG
#define LogDebug(fmt, ...) MBL_LOG(LEVEL_DEBUG, fmt, ##__VA_ARGS__)
#else
#define LogDebug(...)
#endif

// Fatal
#if defined(LOGGING_LEVEL_FATAL) && LOGGING_LEVEL_FATAL
#define LogFatal(fmt, ...) MBL_LOG(LEVEL_FATAL, fmt, ##__VA_ARGS__)
#else
#define LogFatal(...)
#endif

@end