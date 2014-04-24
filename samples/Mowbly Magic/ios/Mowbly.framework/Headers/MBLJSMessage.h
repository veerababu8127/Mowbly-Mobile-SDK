//
//  MBLJSMessage.h
//  Mowbly
//
//  Created by Sathish on 08/06/11.
//  Copyright (c) 2011-2014 CloudPact. All rights reserved.
//
//	Class that represents the message from Mowbly js layer.

#import <Foundation/Foundation.h>

@interface MBLJSMessage : NSObject {
    
	NSArray *args;              // Array of arguments for the method
	NSString *callbackId;       // Identifier for the callback method in JS layer
	NSString *feature;          // Name of the feature
	NSString *method;           // Name of the method to invoke in the feature
    NSString *page;             // Name of the page
}

@property (nonatomic, strong) NSArray *args;
@property (nonatomic, copy) NSString *callbackId;
@property (nonatomic, copy) NSString *feature;
@property (nonatomic, copy) NSString *method;
@property (nonatomic, copy) NSString *page;

- (id)initWithMessage:(NSDictionary *)message;

- (NSDictionary *)asDictionary;

@end