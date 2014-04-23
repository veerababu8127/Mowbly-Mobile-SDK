//
//  MBLUtils.m
//  Mowbly
//
//  Created by Sathish on 18/10/11.
//  Copyright (c) 2011-2014 CloudPact. All rights reserved.
//

#import "MBLApp.h"
#import "MBLConstants.h"
#import "MBLUtils.h"
#import <UIKit/UIKit.h>

@implementation MBLUtils

+ (NSString*)GUID {
    
    NSString *guid = 0;
    if ([[[UIDevice currentDevice] systemVersion] floatValue]>= 6.0) {
        
        guid = [[NSUUID UUID] UUIDString];
    } else {
        
        CFUUIDRef uuid = CFUUIDCreate(kCFAllocatorDefault);
        CFStringRef uuidStr = CFUUIDCreateString(kCFAllocatorDefault, uuid);
        guid = CFBridgingRelease(uuidStr);
        CFRelease(uuid);
    }
    return([guid lowercaseString]);
}

+ (NSError *)errorWithCode:(NSNumber *)code description:(NSString *)description {

    if (description) {

        //replace occurances of " with \"
        description = [description stringByReplacingOccurrencesOfString:@"\"" withString:@"\\\""];
    } else {

        description = MBLLocalizedString(@"UNKNOWN_ERROR");
    }

	NSMutableDictionary *userInfo = [NSMutableDictionary dictionaryWithObjects:[NSArray arrayWithObjects:description, nil] 
																	   forKeys:[NSArray arrayWithObjects:NSLocalizedDescriptionKey, nil]];
	return (NSError *)[NSError errorWithDomain:APP_ERROR_DOMAIN code:[code integerValue] userInfo:(NSDictionary *)userInfo];
} // errorWithCode:description

+ (NSString *)timestampToDate:(long long)timestampInMilliseconds {

    NSDate *date = [NSDate dateWithTimeIntervalSince1970:(timestampInMilliseconds/1000)];
    NSDateFormatter *formatter=[[NSDateFormatter alloc]init];
    [formatter setDateFormat:@"yyyy-MM-dd HH:mm:ss.SSS"];
    NSString *dateStr = [formatter stringFromDate:date];

    return dateStr;
} // timestampToDate:

@end