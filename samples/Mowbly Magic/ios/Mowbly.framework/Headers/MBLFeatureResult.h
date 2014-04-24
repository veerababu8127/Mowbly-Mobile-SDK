//
//  MBLFeatureResult.h
//  Mowbly
//
//  Created by Sathish on 08/06/11.
//  Copyright (c) 2011-2014 CloudPact. All rights reserved.
//

#import <Foundation/Foundation.h>

@interface MBLFeatureResult : NSObject {
    
	NSNumber *code;         // Code status of the feature method invocation
	NSError *error;         // Error returned by the feature method invocation
	id result;              // Result object of the feature method invocation
}

- (MBLFeatureResult *)initWithCode:(NSNumber *)theCode result:(id)theResult error:(NSError *)err;

+ (MBLFeatureResult *)resultWithCode:(NSNumber *)theCode;
+ (MBLFeatureResult *)resultWithCode:(NSNumber *)theCode result:(id)theResult;
+ (MBLFeatureResult *)resultWithCode:(NSNumber *)theCode error:(NSError *)err;
+ (MBLFeatureResult *)resultWithCode:(NSNumber *)theCode result:(id)theResult error:(NSError *)err;

- (NSString *)toJSONString;

- (NSString *)toCallbackString:(NSString *)callbackId;

@end