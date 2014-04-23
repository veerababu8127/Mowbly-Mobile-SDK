//
//  MBLFramework.m
//  Mowbly
//
//  Created by Sathish on 09/06/11.
//  Copyright (c) 2011-2014 CloudPact. All rights reserved.
//

#import "MBLApp.h"
#import "MBLAppDelegate.h"
#import "MBLFeatureBinder.h"
#import "MBLFeatureResult.h"
#import "MBLFileManager.h"
#import "MBLFramework.h"
#import "MBLJSMessage.h"
#import "MBLPage.h"
#import "MBLPageViewController.h"
#import "MBLLogger.h"
#import "MBLUtils.h"

@implementation MBLFramework

- (void) invoke:(MBLJSMessage *)message
{
    if([message.method isEqualToString:@"broadcastMessage"])
    {
        id messageObj = [message.args objectAtIndex:0];

        [[[[self appDelegate] appViewController] pages] enumerateKeysAndObjectsUsingBlock:^(id key, id obj, BOOL *stop) {

            MBLPage *page=(MBLPage *)obj;
            
            if (![page isActive]) {
                [(MBLPage *)obj onMessage:messageObj];
                //Sending message to a page if it is not active
            }
        }];
    }
    else if ([message.method isEqualToString:@"closeApplication"])
    {
        bool bReleasePage = [(NSNumber *)[message.args objectAtIndex:0] boolValue];
        [[[self appDelegate] appViewController] navigateBack:bReleasePage];
	}
    else if ([message.method isEqualToString:@"launchApplication"])
    {
        // Ignore launch call if arrived after another page opened. Happens when 2 tasks are tapped in Home page.
        if ([[[MBLFeatureBinder defaultBinder] view] isActive])
        {
            NSString *pageName = [message.args objectAtIndex:0];
            NSString *url = [message.args objectAtIndex:1];
            NSString *pageData = [message.args objectAtIndex:2];
            NSDictionary *options = [message.args objectAtIndex:3];

            NSURL *theUrl = [NSURL URLWithString:url];
            // Complete Url is provided; Could be an external url or a document like pdf in file etc.
            if ([theUrl scheme])
            {
                [MBLApp openExternalUrl:theUrl];
            }
            else
            {
                BOOL bRetainPageInViewStack = [[options valueForKey:@"retainPageInViewStack"] boolValue];
                [[[self appDelegate] 
                  appViewController] openPageWithName:pageName 
                                                            url:url 
                                                           data:pageData 
                                                  configuration:options 
                                          retainPageInViewStack:bRetainPageInViewStack
                                                      direction:ANIM_IN];
            }
        }
	}
    else if([message.method isEqualToString:@"loadResource"])
    {
        NSString *resourcePath = (NSString *)[message.args objectAtIndex:0];
        // TODO : Get the resource path correctly
        resourcePath = [[[MBLApp appDirectory]
                         stringByAppendingPathComponent:@"resources"]
                         stringByAppendingPathComponent:resourcePath];
        MBLFileManager *fileMgr = [MBLFileManager defaultManager];
        NSError *error = nil;
        NSString *content = [fileMgr read:resourcePath error:&error];
        if(! error) {

            [self pushResponseToJavascript:content
                                  withCode:RESPONSE_CODE_OK
                             andCallBackId:[message callbackId]];
        } else {

            [self pushErrorToJavascript:error
                               withCode:RESPONSE_CODE_ERROR
                          andCallBackId:[message callbackId]];
        }
    }
    else if([message.method isEqualToString:@"openExternal"])
    {
        BOOL response = YES;
        NSString *errorMsg = nil;
        NSDictionary *options = (NSDictionary *)[message.args objectAtIndex:0];
        NSURL *url = [NSURL URLWithString:(NSString *)[options objectForKey:@"scheme"]];
        if(! [[UIApplication sharedApplication] canOpenURL:url]) {

            response = NO;
            errorMsg = [MBLLocalizedString(@"NO_APP_TO_HANDLE_URL") stringByAppendingString:[url absoluteString]];
        } else if(! [[UIApplication sharedApplication] openURL:url]) {

            response = NO;
            errorMsg = MBLLocalizedString(@"ERROR_OPENING_APP");
        }
        if(response) {

            [self pushResponseToJavascript:[NSNumber numberWithBool:response]
                                  withCode:RESPONSE_CODE_OK
                             andCallBackId:[message callbackId]];
        } else {

            NSError *error = [MBLUtils errorWithCode:RESPONSE_CODE_ERROR description:errorMsg];
            [self pushErrorToJavascript:error
                               withCode:RESPONSE_CODE_ERROR
                          andCallBackId:[message callbackId]];
        }
    }
    else if([message.method isEqualToString:@"postMessage"])
    {
        NSString *viewName = (NSString *)[message.args objectAtIndex:0];
        id messageObj = [message.args objectAtIndex:1];

        MBLPage *theView = ([viewName isEqualToString:@""]) ?
            [[[self appDelegate] appViewController] activePage] :
            [[[self appDelegate] appViewController] getPageByName:viewName];
        if(theView)
        {
            [theView onMessage:messageObj];
        }
    }
    else if ([message.method isEqualToString:@"setLanguage"])
    {
        NSString *langCode = (NSString *)[message.args objectAtIndex:0];
        [MBLApp setLanguage:langCode];
    }
    else if ([message.method isEqualToString:@"setPageResult"])
    {
		NSString *result = [message.args objectAtIndex:0];
		[[[MBLFeatureBinder defaultBinder] view] setResultForPage:result];
	}
    else
    {
		// Not a supported method
		LogError(@"Feature %@ does not support method %@.", message.feature, message.method);
	}
} // invoke

@end