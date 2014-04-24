//
//  MBLUtils.h
//  Mowbly
//
//  Created by Sathish on 18/10/11.
//  Copyright (c) 2011-2014 CloudPact. All rights reserved.
//

#import <Foundation/Foundation.h>

@interface MBLUtils : NSObject

+ (NSString*)GUID;

+ (NSError *)errorWithCode:(NSNumber *)code description:(NSString *)description;

+ (NSString *)timestampToDate:(long long)timestampInMilliseconds;

@end