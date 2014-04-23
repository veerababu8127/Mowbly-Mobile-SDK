//
//  MBLFeature.m
//  Mowbly
//
//  Created by Sathish on 08/06/11.
//  Copyright (c) 2011-2014 CloudPact. All rights reserved.
//

#import "MBLAppDelegate.h"
#import "MBLFeature.h"
#import "MBLFeatureBinder.h"
#import "MBLFeatureResult.h"
#import "MBLJSMessage.h"
#import "MBLPage.h"
#import "MBLPageViewController.h"

@implementation MBLFeature

@synthesize RESPONSE_CODE_OK, RESPONSE_CODE_ERROR;

- (id)init {

    if((self = [super init])) {

        // initialize the status messages
		[self setRESPONSE_CODE_OK:[NSNumber numberWithInt:1]];
		[self setRESPONSE_CODE_ERROR:[NSNumber numberWithInt:0]];
    }

    return self;
} // init

- (MBLAppDelegate *)appDelegate {

    return (MBLAppDelegate *)[[UIApplication sharedApplication] delegate];
} // appDelegate

- (void) invoke:(MBLJSMessage *)message {
} // invoke

- (void) pushResponseToJavascript:(id)response withCode:(NSNumber *)code andCallBackId:(NSString *)callbackId {
	
	[self pushResponseToJavascript:response withCode:code error:nil andCallBackId:callbackId];
} // pushResponseToJavascript:withStatus:andCallbackId

- (void) pushResponseToJavascript:(id)response withCode:(NSNumber *)code error:(NSError *)error andCallBackId:(NSString *)callbackId {

    MBLFeatureResult *result = nil;
    if(callbackId && ![callbackId isEqualToString:@""]) {
        
        result = [MBLFeatureResult resultWithCode:code result:response error:error];
        [self pushJavascriptMessage:[result toCallbackString:callbackId]];
	}
} // pushResponseToJavascript:withCode:error:andCallbackId

- (void) pushErrorToJavascript:(id)error withCode:(NSNumber *)code andCallBackId:(NSString *)callbackId {
	
	[self pushResponseToJavascript:nil withCode:code error:error andCallBackId:callbackId];
} // pushResponseToJavascript:withStatus:andCallbackId

- (void)pushJavascriptMessage:(NSString *)jsMessage {

    [self pushJavascriptMessage:jsMessage afterDelay:0.01];
} // pushJavascriptMessage

- (void)pushJavascriptMessage:(NSString *)jsMessage afterDelay:(float)delay {

    if(! [jsMessage isEqualToString:@""]) {
        
        [self performSelector:@selector(messageJS:) 
                   withObject:jsMessage
                   afterDelay:delay];
    }
}

- (void)pushJavascriptMessageWithResult:(MBLFeatureResult *)result {

    [self pushJavascriptMessage:[result toJSONString] afterDelay:0.01];
} // pushJavascriptMessageWithResult

- (void)messageJS:(NSString *)message {

    //LogDebug(@"Message to JS layer - %@", message);
    [[[MBLFeatureBinder defaultBinder] view] stringByEvaluatingJavaScriptFromString:message];
} // messageJS

- (void)onAppPause {
} // onAppPause

- (void)onAppResume {
} // onAppResume



@end