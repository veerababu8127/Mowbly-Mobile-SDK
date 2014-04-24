//
//  MBLFeatureBinder.h
//  Mowbly
//
//  Created by Sathish on 08/06/11.
//  Copyright (c) 2011-2014 CloudPact. All rights reserved.
//
//	Class that manages the features that are supported by Mowbly framework.

#import <Foundation/Foundation.h>

@class MBLFeature;
@class MBLJSMessage;
@class MBLPage;

@interface MBLFeatureBinder : NSObject {

	NSMutableDictionary *features;           // Holds the feature objects
}

@property (nonatomic, strong) NSMutableDictionary *features;

+ (MBLFeatureBinder *)defaultBinder;

+ (id)create;

- (void)invoke:(MBLJSMessage *)message;

- (void)onAppPause;

- (void)onAppResume;

- (MBLPage *) view;

@end