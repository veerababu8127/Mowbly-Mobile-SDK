//
//  MBLSimpleLayout.m
//  Mowbly
//
//  Created by Sathish on 28/08/12.
//  Copyright (c) 2011-2014 CloudPact. All rights reserved.
//

#import "MBLSimpleLogLayout.h"
#import "MBLLogEvent.h"
#import "MBLConstants.h"

@implementation MBLSimpleLogLayout

@synthesize contentType = _contentType,
            footer = _footer,
            header = _header;

- (id)init {

    if((self = [super init])) {

        [self setContentType:@"text/plain"];
        [self setFooter:nil];
        [self setHeader:nil];
    }
    
    return self;
} // init

#pragma mark -
#pragma mark MBLLayout methods

- (NSString *)format:(MBLLogEvent *)logEvent {
    
    NSMutableDictionary *logDict = [[NSMutableDictionary alloc]init];
    [logDict setObject:[logEvent type] forKey:LOGS_TYPE];
    [logDict setObject:[NSString stringWithFormat:@"%@",[logEvent level]] forKey:LOGS_LEVEL];
    [logDict setObject:[logEvent tag] forKey:LOGS_TAG];
    [logDict setObject:[logEvent message] forKey:LOGS_MESSAGE];
    [logDict setObject:[logEvent timestamp] forKey:LOGS_TIMESTAMP];
    NSData *jsonData = [NSJSONSerialization dataWithJSONObject:logDict
                                                       options:NSJSONWritingPrettyPrinted
                                                         error:Nil];
    
    return [NSString stringWithFormat:@"%@,",[[NSString alloc] initWithData:jsonData encoding:NSUTF8StringEncoding]];
} // format:


@end