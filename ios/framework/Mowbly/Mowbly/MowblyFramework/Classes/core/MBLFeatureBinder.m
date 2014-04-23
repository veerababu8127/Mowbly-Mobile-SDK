//
//  MBLFeatureBinder.m
//  Mowbly
//
//  Created by Sathish on 08/06/11.
//  Copyright (c) 2011-2014 CloudPact. All rights reserved.
//

#import "MBLApp.h"
#import "MBLAppDelegate.h"
#import "MBLFeature.h"
#import "MBLFeatureBinder.h"
#import "MBLJSMessage.h"
#import "MBLWebView.h"
#import "MBLUi.h"
#import "MBLCamera.h"
#import "MBLContacts.h"
#import "MBLDatabase.h"
#import "MBLDevice.h"
#import "MBLFile.h"
#import "MBLFramework.h"
#import "MBLGeoLocation.h"
#import "MBLLog.h"
#import "MBLMessageUI.h"
#import "MBLNetwork.h"
#import "MBLNotification.h"
#import "MBLPageViewController.h"
#import "MBLPreferences.h"
#import "MBLHttp.h"
#import "MBLImageGallery.h"


@implementation MBLFeatureBinder

@synthesize features;

static MBLFeatureBinder *_featureBinder;  // static instance

#pragma mark -
#pragma mark Singleton

// Returns the singleton instance of MBLFeatureBinder class
+ (MBLFeatureBinder *)defaultBinder {
    
	if (_featureBinder == nil) {
        
		_featureBinder = [self create];
    }
    return _featureBinder;
} // defaultManager

+ (id)create {
    
    return [[self alloc] init];
    
} // create

- (id)copyWithZone:(NSZone *)zone {
    return self;
} // copyWithZone


#pragma mark -
#pragma mark MBLFeatureBinder methods

- (void)invoke:(MBLJSMessage *)message {

	MBLFeature *f;
    NSString *feature = [message feature];
    
    if (!self.features) {
        self.features = [[NSMutableDictionary alloc]init];
    }
    
    f = [self.features objectForKey:feature];
    if (!f) {
        if ([feature isEqualToString:@"ui"]) {
            f = [[MBLUi alloc]init];
            [self.features setObject:f forKey:@"ui"];
        } else if([feature isEqualToString:@"camera"]){
            f = [[MBLCamera alloc]init];
            [self.features setObject:f forKey:@"camera"];
        } else if([feature isEqualToString:@"contacts"]){
            f = [[MBLContacts alloc]init];
            [self.features setObject:f forKey:@"contacts"];
        } else if([feature isEqualToString:@"database"]){
            f = [[MBLDatabase alloc]init];
            [self.features setObject:f forKey:@"database"];
        } else if([feature isEqualToString:@"device"]){
            f = [[MBLDevice alloc]init];
            [self.features setObject:f forKey:@"device"];
        } else if([feature isEqualToString:@"file"]){
            f = [[MBLFile alloc]init];
            [self.features setObject:f forKey:@"file"];
        } else if([feature isEqualToString:@"framework"]){
            f = [[MBLFramework alloc]init];
            [self.features setObject:f forKey:@"framework"];
        } else if([feature isEqualToString:@"geolocation"]){
            f = [[MBLGeoLocation alloc]init];
            [self.features setObject:f forKey:@"geolocation"];
        } else if([feature isEqualToString:@"http"]){
            f = [[MBLHttp alloc]init];
            [self.features setObject:f forKey:@"http"];
        } else if([feature isEqualToString:@"imagegallery"]){
            f = [[MBLImageGallery alloc]init];
            [self.features setObject:f forKey:@"imagegallery"];
        } else if([feature isEqualToString:@"logger"]){
            f = [[MBLLog alloc]init];
            [self.features setObject:f forKey:@"logger"];
        } else if([feature isEqualToString:@"message"]){
            f = [[MBLMessageUI alloc]init];
            [self.features setObject:f forKey:@"message"];
        } else if([feature isEqualToString:@"network"]){
            f = [[MBLNetwork alloc]init];
            [self.features setObject:f forKey:@"network"];
        } else if([feature isEqualToString:@"notification"]){
            f = [[MBLNotification alloc]init];
            [self.features setObject:f forKey:@"notification"];
        } else if([feature isEqualToString:@"preferences"]){
            f = [[MBLPreferences alloc]init];
            [self.features setObject:f forKey:@"preferences"];
        }
    }
    
    if (f) {

		[f invoke:message];
	} else {

		// Feature not supported.
		LogError(@"Feature %@ not supported.", [message feature]);
	}

} // invoke

- (void) onAppPause {

	// Call onAppPause on all features
	[self.features enumerateKeysAndObjectsUsingBlock:^(id key, id obj, BOOL *stop) {

		[((MBLFeature *)obj) onAppPause];
	}];
} // onAppPause

- (void) onAppResume {

	// Call onAppResume on all features
	[self.features enumerateKeysAndObjectsUsingBlock:^(id key, id obj, BOOL *stop) {

		[((MBLFeature *)obj) onAppResume];
	}];
} //onAppResume

- (MBLPage *)view {
    
    MBLAppDelegate *appDelegate = (MBLAppDelegate *)[[UIApplication sharedApplication] delegate];
    return [[appDelegate pageViewController] activePage];
} // view

@end