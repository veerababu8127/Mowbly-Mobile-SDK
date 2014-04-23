//
//  MBLOfflineHttpHelper.h
//  Mowbly
//
//  Created by Sathish on 01/11/13.
//  Copyright (c) 2011-2014 CloudPact. All rights reserved.
//

#import <Foundation/Foundation.h>

@class MBLHttpHelper;
@class MBLJSMessage;
@class ASIHTTPRequest;
@class MBLHttpClient;

@interface MBLHttpHelper : NSObject

- (void)invoke:(MBLJSMessage *)message
       forFeature:(MBLFeature *)feature
     withBlock:(void(^)(NSDictionary* response, NSNumber* statusCode, NSError *error))onResponse;

- (NSString *)getValue:(NSString *)value
           withOptions:(NSDictionary *)options;

- (ASIHTTPRequest *)requestWithAuth:(NSDictionary *)options
                            request:(ASIHTTPRequest *)asiRequest;

- (NSString *)defaultUserName;

- (NSString *)defaultPassword;

@end