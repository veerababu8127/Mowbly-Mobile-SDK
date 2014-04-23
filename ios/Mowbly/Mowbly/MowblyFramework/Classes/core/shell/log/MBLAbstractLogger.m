//
//  MBLAbstractLogger.m
//  Mowbly
//
//  Created by Sathish on 05/01/13.
//  Copyright (c) 2011-2014 CloudPact. All rights reserved.

#import "MBLAbstractLogger.h"
#import "MBLApp.h"

@implementation MBLAbstractLogger

- (id)init {

    if((self = [super init])) {

        _delegates = [NSMutableArray array];
    }

    return self;
} // init

- (id)initWithName:(NSString *)name {

    if((self = [self init])) {

        _name = [name copy];
    }

    return self;
} // initWithName:

#pragma mark -
#pragma mark MBLLogger methods

- (void)log:(NSString *)message
    withTag:(NSString *)tag
      level:(MBLLogLevel *)level {

    MBLLogEvent *logEvent = [[MBLLogEvent alloc] initWithType:_name
                                                          tag:tag
                                                        level:level
                                                      message:message];
    NSArray* handlers = [self getAllHandlers];
    for (id<MBLLogHandler> handler in handlers) {
        
        if ([level isGreaterOrEqualTo:[handler threshold]]) {
            
            [handler handle:logEvent];
            
            if (![handler isPropagationAllowed] || [logEvent isHandled]) {
                
                break;
            }
        }
    }
    
} // log:withTag:level:username:space

- (void)debug:(NSString *)sMessage
      withTag:(NSString *)sTag {
    
    [self log:sMessage withTag:sTag level:LEVEL_DEBUG];
} // debug:withTag:username:space:

- (void)info:(NSString *)sMessage
     withTag:(NSString *)sTag{
    
    [self log:sMessage withTag:sTag level:LEVEL_INFO];
} // info:withTag:username:space:

- (void)warn:(NSString *)sMessage
     withTag:(NSString *)sTag{
    
    [self log:sMessage withTag:sTag level:LEVEL_WARN];
} // warn:withTag:username:space:

- (void)error:(NSString *)sMessage
      withTag:(NSString *)sTag{
    
    [self log:sMessage withTag:sTag level:LEVEL_ERROR];
} // error:withTag:username:space:

- (void)fatal:(NSString *)sMessage
      withTag:(NSString *)sTag{
    
    [self log:sMessage withTag:sTag level:LEVEL_FATAL];
} // fatal:withTag:username:space:

#pragma mark -
#pragma marl MBLLoggerDelegate methods

- (void)addHandler:(id<MBLLogHandler>)handler {
    
    // Add handler, only if there is one with the same name is not available.
    if([_delegates indexOfObjectPassingTest:^BOOL(id obj, NSUInteger idx, BOOL *stop) {
        
        id<MBLLogHandler> logHandler = (id<MBLLogHandler>)obj;
        if([[logHandler name] isEqualToString:[handler name]]) {
            
            *stop = YES;
            return YES;
        }
        return NO;
    }] == NSNotFound) {
        
        [_delegates addObject:handler];
    }
} // addHandler

- (NSArray *)getAllHandlers {
    
    return [NSArray arrayWithArray:_delegates];
} // getAllHandlers

- (id<MBLLogHandler>)getHandler:(NSString *)name {
    
    __block id<MBLLogHandler> handler = nil;
    [_delegates enumerateObjectsUsingBlock:^(id obj, NSUInteger idx, BOOL *stop) {
        
        id<MBLLogHandler> _handler = (id<MBLLogHandler>)obj;
        if([[_handler name] isEqualToString:name]) {
            
            handler = _handler;
            *stop = YES;
        }
    }];
    
    return handler;
} // getHandler:

- (void)removeHandler:(NSString *)name {
    
    id<MBLLogHandler> handler = [self getHandler:name];
    if(handler != nil) {
        
        [_delegates removeObject:handler];
    }
} // removeHandler:

- (void)removeAllHandlers {
    
    [_delegates removeAllObjects];
} // removeAllHandlers

- (void)reset {

    NSArray* handlers = [self getAllHandlers];
    [handlers enumerateObjectsUsingBlock:^(id obj, NSUInteger idx, BOOL *stop) {

        id<MBLLogHandler> handler = (id<MBLLogHandler>)obj;
        [handler reset];
    }];
} // reset

- (void)setUp {
    
    NSArray* handlers = [self getAllHandlers];
    [handlers enumerateObjectsUsingBlock:^(id obj, NSUInteger idx, BOOL *stop) {
        
        id<MBLLogHandler> handler = (id<MBLLogHandler>)obj;
        [handler setUp];
    }];
} // setUp
- (void) setThreshold:(MBLLogLevel *)level {
    
    NSArray *loggerHandlers = [self getAllHandlers];
    [loggerHandlers enumerateObjectsUsingBlock:^(id obj, NSUInteger idx, BOOL *stop) {
        
        id<MBLLogHandler> handler = (id<MBLLogHandler>)obj;
        [handler setThreshold:level];
    }];
    
} // setting threshold values for all the handlers

@end