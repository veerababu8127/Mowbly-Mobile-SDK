//
//  MBLApp.m
//  Mowbly
//
//  Created by Sathish on 19/10/11.
//  Copyright (c) 2011-2014 CloudPact. All rights reserved.
//

#import "MBLApp.h"
#import "MBLAppDelegate.h"
#import "MBLConnectivity.h"
#import "MBLConstants.h"
#import "MBLExternalPageViewController.h"
#import "MBLFileManager.h"
#import "MBLLogger.h"
#import "MBLProgress.h"
#import "MBLPage.h"
#import "MBLPageViewController.h"
#import "OpenUDID.h"
#import "Reachability.h"
#import "MBLUtils.h"

@implementation MBLApp

static NSDictionary* properties;
static NSString* strings;
static NSString* translations;
static NSMutableDictionary* _viewControllers;

+ (void)initWithPropertiesFile:(NSString *)plistFile andStringsFile:(NSString *)slistFile {

    NSMutableDictionary *dProperties = [[NSDictionary dictionaryWithContentsOfFile:[[MBLAppDelegate frameworkBundle] pathForResource:@"MBLProperties" ofType:@"plist"]] mutableCopy];
    NSString *pListFilePath = [[NSBundle mainBundle] pathForResource:plistFile ofType:@"plist"];
    if(pListFilePath != nil) {

        [dProperties addEntriesFromDictionary:[NSDictionary dictionaryWithContentsOfFile:pListFilePath]];
    } else {

        LogError(@"Properties file %@ not found.", plistFile);
    }

    // Read config from Info dictionary and set on App properties
    NSDictionary* infoDict = [NSDictionary dictionaryWithContentsOfFile:[[MBLAppDelegate frameworkBundle] pathForResource:@"Mowbly-Info" ofType:@"plist"]];
    [dProperties setObject:[infoDict objectForKey:@"MBLAppName"] forKey:@"APP_NAME"];
    [dProperties setObject:[infoDict objectForKey:@"MBLHomePageTitle"] forKey:@"HOME_PAGE_TITLE"];
    [dProperties setObject:[infoDict objectForKey:@"MBLHomePageUrl"] forKey:@"HOME_PAGE_URL"];
    // Set the app directory
    NSString *libraryDirPath = [NSSearchPathForDirectoriesInDomains(NSApplicationSupportDirectory, NSUserDomainMask, YES) objectAtIndex:0];
    NSString *appDir = [libraryDirPath stringByAppendingPathComponent:[dProperties valueForKey:@"APP_NAME"]];
    [dProperties setObject:appDir forKey:@"APP_DIRECTORY"];

    properties = [NSDictionary dictionaryWithDictionary:dProperties];

    // Set the strings file property
    strings = [slistFile copy];

    // Initialize the viewcontrollers array
    _viewControllers = [[NSMutableDictionary alloc] init];
} // initWithPropertiesFile:stringsFile

#pragma  mark -
#pragma mark View Controllers

+ (void)addViewController:(MBLPageViewController *)pvc {
    
    [_viewControllers setObject:pvc forKey:[pvc cid]];
} // addViewControllerByID:

+ (void)removeViewController:(MBLPageViewController *)pvc {
    
    [_viewControllers removeObjectForKey:[pvc cid]];
} // removeViewControllerByID:

+ (MBLPageViewController *)viewControllerByID:(NSString *)cid {
    
    return [_viewControllers objectForKey:cid];
} // viewControllerByID

+ (NSDictionary *)viewControllers {
    
    return _viewControllers;
} // viewControllers

#pragma mark -
#pragma mark Properties and strings

+ (NSString *)localizedString:(NSString *)key {

    NSString* value = NSLocalizedStringFromTable(key, strings, nil);
    if([value isEqualToString:key]) {

        value = NSLocalizedStringFromTable(key, @"MBLStrings", @"");
    }
    return value;
} // localizedString

+ (id)property:(NSString *)name {

    id value = [properties objectForKey:name];
    if(! value) {

        LogError(@"Missing property %@.", name);
    }
    return value;
} // property

#pragma mark -
#pragma mark App management

+ (NSString *)appName {

    return MBLProperty(@"APP_NAME");
} // appName

+ (void)cleanup {

    properties = nil;
    strings = nil;
    translations = nil;
    _viewControllers = nil;
    
} // cleanup

+ (NSString *)deviceId {

    NSString *dId = [self getPreference:DEVICE_ID];
    if(! dId) {

        dId = [OpenUDID value];
        [self setValue:dId forPreference:DEVICE_ID];
    }

    return dId;
}

#pragma mark -
#pragma mark Page management

+ (void)deleteCachedPages {

    MBLAppDelegate *appDelegate = (MBLAppDelegate *)[[UIApplication sharedApplication] delegate];
    [[[appDelegate appViewController] pages] removeAllObjects];
} // deleteCachedPages

+ (void)openExternalUrl:(NSURL *)url withTitle:(NSString *)title{

    MBLAppDelegate *appDelegate = (MBLAppDelegate *)[[UIApplication sharedApplication] delegate];
    UINavigationController *navigationController =[[[[appDelegate navigationController] viewControllers] objectAtIndex:0] navigationController];
    MBLExternalPageViewController *epvc = [[MBLExternalPageViewController alloc] initWithUrl:url.absoluteString title:title];
    // Push the controller in our navigation stack
    [navigationController pushViewController:epvc animated:YES];

} // openExternalUrl:

#pragma mark -
#pragma mark Directory management

+ (NSString *)appDirectory {

    NSString *aPath = MBLProperty(@"APP_DIRECTORY");
    NSFileManager *fileMgr = [[NSFileManager alloc] init];
    BOOL isDir;
    if(! ([fileMgr fileExistsAtPath:aPath isDirectory:&isDir] && isDir)) {

        NSError *error = nil;
        if(! [fileMgr createDirectoryAtPath:aPath withIntermediateDirectories:YES attributes:nil error:&error]) {

            LogError(@"Failed to create app dir. Reason - %@", [error localizedDescription]);
        }
    }

    return aPath;
} // appDirectory

+ (NSString *)cacheDirectory {

    NSString *cachesDirPath = [NSSearchPathForDirectoriesInDomains(NSCachesDirectory, NSUserDomainMask, YES) objectAtIndex:0];
    NSString *cPath = [cachesDirPath stringByAppendingPathComponent:[MBLApp appName]];

    NSFileManager *fileMgr = [[NSFileManager alloc] init];
    BOOL isDir;
    if(! ([fileMgr fileExistsAtPath:cPath isDirectory:&isDir] && isDir)) {

        NSError *error = nil;
        if(! [fileMgr createDirectoryAtPath:cPath withIntermediateDirectories:YES attributes:nil error:&error]) {

            LogError(@"Failed to create cache dir. Reason - %@", [error localizedDescription]);
        }
    }
    return cPath;
} // cacheDirectory

+ (NSString *)documentDirectory {

    NSString *documentDirPath = [NSSearchPathForDirectoriesInDomains(NSDocumentDirectory, NSUserDomainMask, YES) objectAtIndex:0];
    NSString *dPath = [documentDirPath stringByAppendingPathComponent:[MBLApp appName]];

    NSFileManager *fileMgr = [[NSFileManager alloc] init];
    BOOL isDir;
    if(! ([fileMgr fileExistsAtPath:dPath isDirectory:&isDir] && isDir)) {

        NSError *error = nil;
        if(! [fileMgr createDirectoryAtPath:dPath withIntermediateDirectories:YES attributes:nil error:&error]) {

            LogError(@"Failed to create documents dir. Reason - %@", [error localizedDescription]);
        }
    }
    
    return dPath;
} // documentsDirectory

+ (NSString *)logFilePath {

    return [[MBLApp documentDirectory] stringByAppendingPathComponent:@"__logs/mowbly.log"];
} // logFilePath

+ (NSString *)tmpDirectory {

    NSString *tPath = [NSTemporaryDirectory() stringByAppendingPathComponent:[MBLApp appName]];

    NSFileManager *fileMgr = [[NSFileManager alloc] init];
    BOOL isDir;
    if(! ([fileMgr fileExistsAtPath:tPath isDirectory:&isDir] && isDir)) {
        
        NSError *error = nil;
        if(! [fileMgr createDirectoryAtPath:tPath withIntermediateDirectories:YES attributes:nil error:&error]) {
            
            LogError(@"Failed to create tmp dir. Reason - %@", [error localizedDescription]);
        }
    }
    
    return tPath;
} // tmpDirectory

#pragma mark -
#pragma mark Preferences management

+ (void)clearPreference:(NSString *)preference {

    NSUserDefaults *settings = [NSUserDefaults standardUserDefaults];
    [settings removeObjectForKey:preference];
    [settings synchronize];
} // clearPreference:

+ (id)getPreference:(NSString *)preference {

    return [[NSUserDefaults standardUserDefaults] objectForKey:preference];
} // getPreference:

+ (void)setValue:(id)value forPreference:(NSString *)preference {
    
    NSUserDefaults *settings = [NSUserDefaults standardUserDefaults];
    [settings setObject:value forKey:preference];
    [settings synchronize];
} // setValue:forPreference

#pragma mark -
#pragma mark Internationalization

+ (NSString *)getLanguage {

    NSString *language = [self getPreference:@"APP_LANGUAGE"];
    if (!language) {

        language = @"en-US";
    }
    return language;
} // getLanguage

+ (void)setLanguage:(NSString *)langCode {

    if (! [langCode isEqualToString:[self getLanguage]]) {
        
        [self setValue:langCode forPreference:@"APP_LANGUAGE"];
        translations = nil;
    }
} // setLanguage

@end