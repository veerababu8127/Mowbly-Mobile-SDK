//
//  MBLURLProtocol.m
//  Mowbly
//
//  Created by Sathish on 27/11/12.
//  Copyright (c) 2011-2014 CloudPact. All rights reserved.

#import "CJSONDeserializer.h"
#import "MBLLogger.h"
#import "MBLApp.h"
#import "MBLAppDelegate.h"
#import "MBLURLProtocol.h"
#import "MBLFeatureBinder.h"
#import "MBLJSMessage.h"
#import "MBLPageViewController.h"
#import "MBLPage.h"

static NSURL *theUrl;

@implementation MBLURLProtocol

#pragma mark -
#pragma mark NSURLProtocol

+ (BOOL)canInitWithRequest:(NSURLRequest*)theRequest {
    
    // request scheme is mowbly
    if(theRequest && ![theRequest isEqual:[NSNull null]] && [[[theRequest URL] scheme] isEqualToString:@"mowbly"]) {
        
        if (!theUrl || (![theUrl isEqual:[NSNull null]] && ![theUrl isEqual:[theRequest URL]])) {

            if (theUrl) {
                theUrl = nil;
            }
            
            theUrl = [theRequest URL];
            MBLURLProtocol *ptr = [[MBLURLProtocol alloc] init];
            [ptr performSelectorOnMainThread:@selector(invoke:)
                                  withObject:theUrl
                               waitUntilDone:NO];
        }
    }
    return NO;
} // canInitWithRequest:

+ (NSURLRequest*)canonicalRequestForRequest:(NSURLRequest*)theRequest {
    
    return theRequest;
} // canonicalRequestForRequest:

- (void)startLoading {
    
} // startLoading

- (void)invoke:(NSURL *)url {
    
    NSString *myUrl = [url absoluteString];
    int prefixLength = [[url scheme] length] + [@"://" length] + [[url host] length] + 1; // mowbly://wake?
    NSString *queryStr = [myUrl substringWithRange:NSMakeRange(prefixLength, [myUrl length] - prefixLength)];
    NSArray *qParts = [queryStr componentsSeparatedByString:@"&"];
    
    NSString *pageName = [[[(NSString *)[qParts objectAtIndex:0]
                            stringByReplacingOccurrencesOfString:@"page=" withString:@""]
                           stringByReplacingOccurrencesOfString:@"," withString:@""]
                          stringByReplacingPercentEscapesUsingEncoding:NSUTF8StringEncoding];
    NSString *cid = [[(NSString *)[qParts objectAtIndex:1]
                      stringByReplacingOccurrencesOfString:@"cid=" withString:@""]
                     stringByReplacingOccurrencesOfString:@"," withString:@""];
    
    MBLPageViewController *vc = [MBLApp viewControllerByID:cid];
    if(vc) {
        
        MBLPage *page = [vc getPageByName:pageName];
        if(page) {
            
            NSString *str = [page stringByEvaluatingJavaScriptFromString:[NSString stringWithFormat:@"__mowbly__.Bridge.getCalls()"]];
            NSError *error = nil;
            // CJSON will take care of any space or %20 in string if present
            NSArray *calls = [[CJSONDeserializer deserializer] deserializeAsArray:[str dataUsingEncoding:NSUTF8StringEncoding]error:&error];
            //LogDebug(@"Calls from page %@:\n%@", pageName, calls);
            
            [calls enumerateObjectsUsingBlock:^(id obj, NSUInteger idx, BOOL *stop) {
                
                NSDictionary *jsMsg = (NSDictionary *)obj;
                MBLJSMessage *jsMessage = [[MBLJSMessage alloc] initWithMessage:jsMsg];
                [page invoke:jsMessage];
                
            }];
        } else {
            LogError(@"BRIDGE FAILURE: Page %@ not found in controller %@", pageName, [[vc class] description]);
        }
    } else {
        
        LogError(@"BRIDGE FAILURE: View controller %@ not found.", [[vc class] description]);
    }
} // invoke

- (void)stopLoading
{
    
} // stopLoading

#pragma mark -
#pragma mark MBLURLProtocol

+ (void)reset {

    if (theUrl) {

        theUrl = nil;
    }
} // setTheUrlToNull

@end