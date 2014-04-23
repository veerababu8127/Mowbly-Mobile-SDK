//
//  MBLApp.h
//  Mowbly
//
//  Created by Sathish on 19/10/11.
//  Copyright (c) 2011-2014 CloudPact. All rights reserved.
//
//  Class that helps manage pages preferences and settings.

#import <Foundation/Foundation.h>

@class MBLPageViewController;
@class MBLReachability;

@interface MBLApp : NSObject

+ (void)initWithPropertiesFile:(NSString *)plistFile andStringsFile:(NSString *)slistFile;

#pragma  mark -
#pragma mark View Controllers

+ (void)addViewController:(MBLPageViewController *)pvc;

+ (void)removeViewController:(MBLPageViewController *)pvc;

+ (MBLPageViewController *)viewControllerByID:(NSString *)cid;

+ (NSDictionary *)viewControllers;

#pragma mark -
#pragma mark Properties and strings

+ (NSString *)localizedString:(NSString *)key;

+ (id)property:(NSString *)name;

#pragma mark -
#pragma mark App management

+ (NSString *)appName;

+ (void)cleanup;

+ (NSString *)deviceId;
#pragma mark -
#pragma mark Page management

+ (void)openExternalUrl:(NSURL *)url;

#pragma mark -
#pragma mark Directory management

+ (NSString *)appDirectory;

+ (NSString *)cacheDirectory;

+ (NSString *)documentDirectory;

+ (NSString *)logFilePath;

+ (NSString *)tmpDirectory;

#pragma mark -
#pragma mark Preferences management

+ (void)clearPreference:(NSString *)preference;

+ (id)getPreference:(NSString *)preference;

+ (void)setValue:(id)value forPreference:(NSString *)preference;

#pragma mark -
#pragma mark Internationalization

+ (NSString *)getLanguage;

+ (NSString *)getTranslations;

+ (void)setLanguage:(NSString *)langCode;

#pragma mark -
#pragma mark Macros

#define MBLLocalizedString(key) [MBLApp localizedString:key]
#define MBLProperty(name) [MBLApp property:name]

@end
