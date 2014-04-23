//
//  MBLHttp.m
//  Mowbly
//
//  Created by Sathish on 13/06/11.
//  Copyright (c) 2011-2014 CloudPact. All rights reserved.
//

#import "MBLConnectivity.h"
#import "MBLConstants.h"
#import "MBLHttp.h"
#import "MBLHttpHelper.h"
#import "MBLJSMessage.h"
#import "MBLPage.h"

@implementation MBLHttp

- (id) init {

    if((self = [super init])) {

        _httpHelper = [[MBLHttpHelper alloc] init];
    }

    return self;
} // init

- (void)invoke:(MBLJSMessage *)message {

	if([message.method isEqualToString:@"request"]) {

        if([[MBLConnectivity getInstance] networkConnected]) {

            __block NSString *callbackId = [[message callbackId] copy];
            [_httpHelper invoke:message
                        forFeature:self
                      withBlock:^void(NSDictionary *response, NSNumber *statusCode, NSError *error) {

                if(error) {
                    
                    [self pushErrorToJavascript:error
                                       withCode:statusCode
                                  andCallBackId:callbackId];
                } else {

                    [self pushResponseToJavascript:response
                                          withCode:statusCode
                                             error:error
                                     andCallBackId:callbackId];
                }
            }];
        }
    }
} // invoke



@end