//
//  MBLJSMessage.m
//  Mowbly
//
//  Created by Sathish on 08/06/11.
//  Copyright (c) 2011-2014 CloudPact. All rights reserved.
//

#import "CJSONDeserializer.h"
#import "CJSONSerializer.h"
#import "MBLJSMessage.h"
#import "MBLLogger.h"

@implementation MBLJSMessage

@synthesize args, callbackId, feature, method, page;

- (id)initWithMessage:(NSDictionary *)message {
    
    if((self = [super init])) {

        self.feature = [message objectForKey:@"f"];
        self.method = [message objectForKey:@"m"];
        self.args = [message objectForKey:@"a"];
        self.callbackId = [message objectForKey:@"callbackId"];
        self.page = [message objectForKey:@"p"];
    }
    
    return self;
} // initWithMessage

- (NSDictionary *)asDictionary {

    return [NSDictionary dictionaryWithObjectsAndKeys:
            [self feature], @"f",
            [self method], @"m",
            [self args], @"a",
            [self callbackId], @"callbackId",
            [self page], @"p", nil];
} // asDictionary

- (NSString *)description {

	NSError *error = nil;
	NSString *argList = [[NSString alloc] initWithData:[[CJSONSerializer serializer] serializeArray:self.args error:&error]
												encoding:NSUTF8StringEncoding];
	NSString *description = [NSString stringWithFormat:@"Feature - %@, Method - %@, Page - %@, CallbackId - %@,\narguments - %@",self.feature, self.method, self.page, self.callbackId, argList];
	return description;
} // description

- (BOOL)isEqual:(id)otherMessage {

    if(otherMessage == self)
        return YES;

    if(!otherMessage || ![otherMessage isKindOfClass:[self class]])
        return NO;

    return [self isEqualToJSMessage:otherMessage];
} // isEqual:

- (BOOL)isEqualToJSMessage:(MBLJSMessage *)otherMessage {
    
    return ([[otherMessage feature] isEqualToString:[self feature]] &&
            [[otherMessage method] isEqualToString:[self method]] &&
            [[otherMessage page] isEqualToString:[self page]] &&
            [[otherMessage callbackId] isEqualToString:[self callbackId]] &&
            [[otherMessage args] isEqualToArray:[self args]]);
} // isEqualToJSMessage:

- (NSUInteger)hash { // hash needs to be overridden if isEqual: is.

    return [[NSString stringWithFormat:@"%@_%@_%@_%@", [self feature], [self method], [self page], [self callbackId]] hash];
} // hash


@end