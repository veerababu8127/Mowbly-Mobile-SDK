//
//  MBLLogEvent.h
//  Mowbly
//
//  Created by Sathish on 06/08/12.
//  Copyright (c) 2011-2014 CloudPact. All rights reserved.
//

#import <UIKit/UIKit.h>

@class MBLLogLevel;

@interface MBLLogEvent : NSObject {

    BOOL _isHandled;
    MBLLogLevel* _level;
    NSString* _message;
    NSString* _tag;
    NSString* _timestamp;
    NSString* _type;
}

@property (nonatomic,assign) BOOL isHandled;
@property (nonatomic,retain) MBLLogLevel* level;
@property (nonatomic,copy) NSString* message;
@property (nonatomic,copy) NSString *tag;
@property (nonatomic,copy) NSString* timestamp;
@property (nonatomic,copy) NSString* type;

- (id) initWithType:(NSString *)logType
                tag:(NSString *)logTag
              level:(MBLLogLevel *)logLevel
            message:(NSString *)logMessage;

@end