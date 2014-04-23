//
//  MBLLog.m
//  Mowbly
//
//  Created by Sathish on 10/09/12.
//  Copyright (c) 2011-2014 CloudPact. All rights reserved.
//

#import "MBLApp.h"
#import "MDBufferedInputStream.h"
#import "MBLJSMessage.h"
#import "MBLLog.h"
#import "MBLLogger.h"
#import "MBLLogLevel.h"
#import "MBLPage.h"
#import "MBLUtils.h"

@implementation MBLLog

@synthesize logger;

- (id)init {

    if((self = [super init])) {

        logger = [MBLLogger loggerByName:@"user"];
    }

    return self;
} // init

- (void)invoke:(MBLJSMessage *)message {

	if ([message.method isEqualToString:@"log"]) {

        NSString *logMessage = (NSString *)[message.args objectAtIndex:0];
        NSString *logTag = (NSString *)[message.args objectAtIndex:1];
        int severity = [[message.args objectAtIndex:2] intValue];

       switch (severity) {

            case _DEBUG:
                [logger debug:logMessage withTag:logTag];
                break;

            case INFO:
                [logger info:logMessage withTag:logTag];
                break;

            case WARN:
                [logger warn:logMessage withTag:logTag];
                break;

            case ERROR:
                [logger error:logMessage withTag:logTag];
                break;

            case FATAL:
                [logger fatal:logMessage withTag:logTag];
                break;

            default:
                NSLog(@"%@", logMessage);
                break;
        }
	} else if ([message.method isEqualToString:@"readLogs"]) {

        callbackId = message.callbackId;

        NSInteger start = [[message.args objectAtIndex:0] intValue];
        NSInteger numLines = [[message.args objectAtIndex:1] intValue];
        NSInteger next = 0;
        NSInteger counter = 0;

        NSError *error = nil;

        NSMutableArray *lines = [[NSMutableArray alloc] init];

        NSString *logFile = [[MBLApp documentDirectory] stringByAppendingPathComponent:[NSString stringWithFormat:@"__logs/%@", [MBLApp appName]]];

        NSFileManager *fileManager = [[NSFileManager alloc] init];
        BOOL doesLogFileExist = [fileManager fileExistsAtPath:logFile];

        if(doesLogFileExist) {

            NSString *line = nil;
            NSInputStream *iStream = [NSInputStream inputStreamWithFileAtPath:logFile];

            MDBufferedInputStream *bufstream = [[MDBufferedInputStream alloc] initWithInputStream:iStream bufferSize:8046 encoding:NSUTF8StringEncoding];

            [bufstream open];

            while ((line = [bufstream readLine])) {
                next = next + 1;
                
                if (next <= start) {

                    continue;
                }
                [lines addObject:line];
                counter = counter + 1;

                if (counter == numLines) {

                    break;
                }
            }
            [bufstream close];

            NSDictionary *result = [NSDictionary dictionaryWithObjectsAndKeys:lines,@"data",[NSNumber numberWithInt:next],@"next",nil];
            [self pushResponseToJavascript:result withCode:RESPONSE_CODE_OK andCallBackId:callbackId];
        } else {

            if (!error) {

                error = [MBLUtils errorWithCode:RESPONSE_CODE_ERROR description:@"File not Found"];
            }
            [self pushResponseToJavascript:nil withCode:RESPONSE_CODE_ERROR error:error andCallBackId:callbackId];
        }
    } 
    else {

		// Not a supported method
		LogError(@"Feature %@ does not support method %@.", message.feature, message.method);
	}
} // invoke

@end