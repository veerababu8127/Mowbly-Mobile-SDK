//
//  MBLFeature.h
//  Mowbly
//
//  Created by Sathish on 08/06/11.
//  Copyright (c) 2011-2014 CloudPact. All rights reserved.
//
//	Class that represents a feature supported by Mowbly.
//	All feature interfaces extend from this class.

#import "MBLAppDelegate.h"
#import "MBLLogger.h"

@class MBLFeatureResult;
@class MBLJSMessage;
@class MBLPage;

@interface MBLFeature : NSObject {
	
	NSNumber *RESPONSE_CODE_OK;         // Success status for method invocation result
	NSNumber *RESPONSE_CODE_ERROR;      // Error status for method invocation result
}

@property (nonatomic, readonly) MBLAppDelegate *appDelegate;
@property (nonatomic, copy) NSNumber *RESPONSE_CODE_OK;
@property (nonatomic, copy) NSNumber *RESPONSE_CODE_ERROR;

- (void)invoke:(MBLJSMessage *)message;

- (void)messageJS:(NSString *)message;

- (void) pushResponseToJavascript:(id)response withCode:(NSNumber *)code andCallBackId:(NSString *)callbackId;

- (void) pushResponseToJavascript:(id)response withCode:(NSNumber *)code error:(NSError *)error andCallBackId:(NSString *)callbackId;

- (void) pushErrorToJavascript:(id)error withCode:(NSNumber *)code andCallBackId:(NSString *)callbackId;

- (void)pushJavascriptMessage:(NSString *)jsMessage;

- (void)pushJavascriptMessage:(NSString *)jsMessage afterDelay:(float)delay;

- (void)pushJavascriptMessageWithResult:(MBLFeatureResult *)result;

- (void)onAppPause;

- (void)onAppResume;

@end