//
//  MBLAbstractLogger.h
//  Mowbly
//
//  Created by Sathish on 05/01/13.
//  Copyright (c) 2011-2014 CloudPact. All rights reserved.

#import "MBLLogHandler.h"

@protocol MBLLoggerDelegate <NSObject>

- (void)addHandler:(id<MBLLogHandler>)handler;

- (NSArray *)getAllHandlers;

- (id<MBLLogHandler>)getHandler:(NSString *)name;

- (void)removeHandler:(NSString *)name;

- (void)removeAllHandlers;

- (void)reset;

- (void)setUp;

- (void) setThreshold:(MBLLogLevel *)level;

@end

@interface MBLAbstractLogger : NSObject <MBLLoggerDelegate> {

    NSString *_name;                    // Name of the logger
    NSMutableArray *_delegates;         // List of handler delegates
}

- (id)initWithName:(NSString *)name;

- (void)log:(NSString *)message
    withTag:(NSString *)tag
      level:(MBLLogLevel *)level;

- (void)debug:(NSString *)sMessage
      withTag:(NSString *)sTag;

- (void)info:(NSString *)sMessage
     withTag:(NSString *)sTag;

- (void)warn:(NSString *)sMessage
     withTag:(NSString *)sTag;

- (void)error:(NSString *)sMessage
      withTag:(NSString *)sTag;

- (void)fatal:(NSString *)sMessage
      withTag:(NSString *)sTag;

@end