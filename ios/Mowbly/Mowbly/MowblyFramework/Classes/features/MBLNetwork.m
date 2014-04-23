//
//  MBLNetwork.m
//  Mowbly
//
//  Created by Sathish on 13/06/11.
//  Copyright (c) 2011-2014 CloudPact. All rights reserved.
//

#import "MBLApp.h"
#import "MBLConnectivity.h"
#import "MBLHttpClient.h"
#import "MBLJSMessage.h"
#import "MBLNetwork.h"
#import "MBLUtils.h"

@implementation MBLNetwork

- (void)invoke:(MBLJSMessage *)message {

	if ([message.method isEqualToString:@"getActiveNetwork"])
    {
        int nw = [[MBLConnectivity getInstance] networkStatus];
        NSNumber *code = [NSNumber numberWithInt:nw];
        NSString *response = [[MBLConnectivity getInstance] networkType];
        
        [self pushResponseToJavascript:response withCode:code andCallBackId:message.callbackId];
    }
    else if ([message.method isEqualToString:@"isHostReachable"])
    {
        if([[MBLConnectivity getInstance] networkConnected])
        {
            NSString *url = (NSString *)[message.args objectAtIndex:0];
            NSTimeInterval timeout = ([[message.args objectAtIndex:1] doubleValue]/1000.0);
            MBLHttpRequestBlock httpRequestBlock = ^(ASIHTTPRequest* request) {
                
                // Set timeout on request
                [request setTimeOutSeconds:timeout];
            };
            MBLHttpResponseBlock httpResponseBlock = ^(ASIHTTPRequest* request, NSError *error) {

                // send response to JS layer
                NSNumber *result = [NSNumber numberWithBool:(error) ? NO : YES];
                [self pushResponseToJavascript:result withCode:[NSNumber numberWithInt:[request responseStatusCode]] error:error andCallBackId:message.callbackId];
            };

            MBLHttpClient *httpClient = [[MBLHttpClient alloc] init];
            [httpClient get:url withHeaders:nil onRequest:httpRequestBlock onResponse:httpResponseBlock];
        }
        else
        {
            // Send error to JS layer
            NSNumber *result = [NSNumber numberWithBool:NO];
            NSError *error = [MBLUtils errorWithCode:RESPONSE_CODE_ERROR description:MBLLocalizedString(@"NO_NETWORK")];
            [self pushResponseToJavascript:result withCode:RESPONSE_CODE_ERROR error:error andCallBackId:message.callbackId];
        }
	}
    else
    {
		// Not a supported method
		LogError(@"Feature %@ does not support method %@.", message.feature, message.method);
	}
} // invoke

@end