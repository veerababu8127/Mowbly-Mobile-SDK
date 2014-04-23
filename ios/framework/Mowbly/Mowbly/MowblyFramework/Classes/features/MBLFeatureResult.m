//
//  MBLFeatureResult.m
//  Mowbly
//
//  Created by Sathish on 08/06/11.
//  Copyright (c) 2011-2014 CloudPact. All rights reserved.
//

#import "CJSONSerializer.h"
#import "MBLFeatureResult.h"

@implementation MBLFeatureResult

- (MBLFeatureResult *)initWithCode:(NSNumber *)theCode result:(id)theResult error:(NSError *)err
{
	if(self = [super init])
    {
		code = theCode;
		result = theResult;
		error = err;
	}
	
	return self;
} // initWithCode:result:error

+ (MBLFeatureResult *)resultWithCode:(NSNumber *)theCode {
    
	return [[self alloc] initWithCode:theCode result:nil error:nil];
} // initWithCode

+ (MBLFeatureResult *)resultWithCode:(NSNumber *)theCode result:(id)theResult {

	return [[self alloc] initWithCode:theCode result:theResult error:nil];
} // intiWithCode:result

+ (MBLFeatureResult *)resultWithCode:(NSNumber *)theCode error:(NSError *) err {

	return [[self alloc] initWithCode:theCode result:nil error:err];
} // initWithCode:error

+ (MBLFeatureResult *)resultWithCode:(NSNumber *)theCode result:(id)theResult error:(NSError *)err {

    return [[self alloc] initWithCode:theCode result:theResult error:err];
} // initWithCode:result:error

- (NSString *)toJSONString
{
    int iCode = code ? [code intValue] : 1;
    NSString *sResult = nil;
    NSString *sError = nil;

	// create message with result
	if(result)
    {
        NSString *r = nil;
        NSError *err = nil;
		// Check for the different types of result objects and convert them to string
		if([result isKindOfClass:[NSString class]]) {

            r = [[NSString alloc] initWithData:[[CJSONSerializer serializer] serializeString:result error:&err]
                                      encoding:NSUTF8StringEncoding];
            sResult = [NSString stringWithString:r];
		} else if([result isKindOfClass:[NSNumber class]]) {

			if(strcmp([result objCType], @encode(BOOL)) == 0) {

				sResult = ([result boolValue]) ? @"true" : @"false";
			} else {

				sResult = [NSString stringWithFormat:@"%d", [result intValue]];
			}
		} else if([result isKindOfClass:[NSDictionary class]]) {

			r = [[NSString alloc] initWithData:[[CJSONSerializer serializer] serializeDictionary:result error:&err]
                                      encoding:NSUTF8StringEncoding];
            sResult = [NSString stringWithString:r];
		} else if([result isKindOfClass:[NSArray class]]) {

			r = [[NSString alloc] initWithData:[[CJSONSerializer serializer] serializeArray:result error:&err]
                                      encoding:NSUTF8StringEncoding];
            sResult = [NSString stringWithString:r];
		}
	}

    if(error) {

        NSString *msg = [error localizedDescription];
        if (! msg) {

            msg = @"";
        }

        NSString *description = [error localizedFailureReason];
        if (! description) {

            description = @"";
        }
        
        sError = [NSString stringWithFormat:@"{\"message\":\"%@\",\"description\":\"%@\"}", msg, description];
    }

	return [NSString stringWithFormat:@"{\"code\":%d,\"result\":%@,\"error\":%@}", iCode, sResult, sError];
} // toJSONString

- (NSString *)toCallbackString:(NSString *)callbackId {

	NSString *message = [[[NSString stringWithFormat:@"window.__mowbly__.__CallbackClient.onreceive(\"%@\", %@)", callbackId, [self toJSONString]]
                          componentsSeparatedByCharactersInSet:[NSCharacterSet newlineCharacterSet]]
                         componentsJoinedByString:@""];
	
	return message;
} // toCallbackString

@end