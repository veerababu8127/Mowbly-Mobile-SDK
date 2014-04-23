//
//  MBLHttpClient.m
//  Mowbly
//
//  Created by Sathish on 03/06/11.
//  Copyright (c) 2011-2014 CloudPact. All rights reserved.
//

#import "ASIFormDataRequest.h"
#import "ASIDownloadCache.h"
#import "MBLHttpClient.h"
#import "MBLHttpPart.h"
#import "MBLHTTPResponseDelegate.h"

@interface MBLHttpClient (private)

- (void) setHeaders:(NSDictionary *)headers
        andDelegate:(MBLHttpResponseBlock)httpResponseBlock
         forRequest:(ASIHTTPRequest *)request;

@end

@implementation MBLHttpClient

+ (NSArray *)getPartsForPostParameters:(NSDictionary *)partsDict {

    NSMutableArray *aParts = [NSMutableArray arrayWithCapacity:[partsDict count]];
    [partsDict enumerateKeysAndObjectsUsingBlock:^(id key, id obj, BOOL *stop) {

        MBLHttpPart *part = [[MBLHttpPart alloc] initWithName:(NSString *)key
                                                     andValue:obj];
        [aParts addObject:part];
    }];

    return aParts;
} // getPartsForPostParameters:

+ (BOOL)isResponseForRequest:(ASIHTTPRequest *)request ofContentType:(NSString *)type {

    NSRange range = [[[request responseHeaders] objectForKey:@"Content-Type"] rangeOfString:type options:NSCaseInsensitiveSearch];
    return (range.length != 0);
} // isResponseForRequest:ofContentType:

- (void)    get:(NSString *)url
    withHeaders:(NSDictionary *)headers
      onRequest:(MBLHttpRequestBlock)httpRequestBlock
     onResponse:(MBLHttpResponseBlock)httpResponseBlock {
	
	// Parse the url and create the request object
	ASIHTTPRequest *request = [ASIHTTPRequest requestWithURL:[NSURL URLWithString:url]];
    [request setAllowCompressedResponse:YES];
    //Set caching parameters 
    [request setDownloadCache:[ASIDownloadCache sharedCache]];
    [[ASIDownloadCache sharedCache] setShouldRespectCacheControlHeaders:YES];
    [request setCacheStoragePolicy:(ASICacheStoragePolicy)ASIDoNotReadFromCacheCachePolicy];

    [request setCachePolicy:ASIDoNotWriteToCacheCachePolicy];
    
    // Set headers and delegate
    [self setHeaders:headers andDelegate:httpResponseBlock forRequest:request];
    
    if(httpRequestBlock != nil) {

        httpRequestBlock(request);
    }

    [request startAsynchronous];
} // get:withHeaders:onRequest:onResponse:

- (void)    post:(NSString *)url
        withData:(id)data
      andHeaders:(NSDictionary *)headers
       onRequest:(MBLHttpRequestBlock)httpRequestBlock
      onResponse:(MBLHttpResponseBlock)httpResponseBlock {

    ASIFormDataRequest *request = [ASIFormDataRequest requestWithURL:[NSURL URLWithString:url]];
    [request setAllowCompressedResponse:YES];
    // Set headers and delegate
    [self setHeaders:headers andDelegate:httpResponseBlock forRequest:request];
    
    if ([data isKindOfClass:[NSString class]])
    {
        // Set the post file
        [request appendPostDataFromFile:data];
    }
    else
    {
        // Set the post data
        [request appendPostData:data];
    }

    if(httpRequestBlock != nil) {
        
        httpRequestBlock(request);
    }
    [request startAsynchronous];
} // post:withData:andHeaders:onRequest:onResponse:

- (void)postMultipart:(NSString *)url
            withParts:(NSArray *)parts
           andHeaders:(NSDictionary *)headers
            onRequest:(MBLHttpRequestBlock)httpRequestBlock
           onResponse:(MBLHttpResponseBlock)httpResponseBlock {

    ASIFormDataRequest *request = [ASIFormDataRequest requestWithURL:[NSURL URLWithString:url]];
    [request setAllowCompressedResponse:YES];
    // Set Headers and delegate
    [self setHeaders:headers andDelegate:httpResponseBlock forRequest:request];

    // Add the parts
    [parts enumerateObjectsUsingBlock:^(id obj, NSUInteger idx, BOOL *stop) {
        
        MBLHttpPart *part = (MBLHttpPart *)obj;
        if ([part type] == FILE_PART) {
            
            [request setFile:[part value]
                withFileName:[part fileName]
              andContentType:[part contentType]
                      forKey:[part name]];
        } else {
            
            [request setPostValue:[part value] forKey:[part name]];
        }
    }];
    
    if(httpRequestBlock != nil) {
        
        httpRequestBlock(request);
    }

    [request startAsynchronous];
} // postMultipart:withParts:andHeaders:onRequest:onResponse:

- (void) deleteRequest:(NSString *)url
           withHeaders:(NSDictionary *)headers
             onRequest:(MBLHttpRequestBlock)httpRequestBlock
            onResponse:(MBLHttpResponseBlock)httpResponseBlock
{    
    ASIFormDataRequest *request = [ASIFormDataRequest requestWithURL:[NSURL URLWithString:url]];
    [request setAllowCompressedResponse:YES];
    // Set headers and delegate
    [self setHeaders:headers andDelegate:httpResponseBlock forRequest:request];
    
    if(httpRequestBlock != nil) {
        
        httpRequestBlock(request);
    }
    [request startAsynchronous];
} // deleteRequest:withHeaders:onRequest:onResponse:

#pragma mark -
#pragma mark HttpClient utils

- (void) setHeaders:(NSDictionary *)headers
        andDelegate:(MBLHttpResponseBlock)httpResponseBlock
         forRequest:(ASIHTTPRequest *)request {

    if (headers) {
        
        // Set headers
        [headers enumerateKeysAndObjectsUsingBlock:^(id key, id obj, BOOL *stop) {
            
            [request addRequestHeader:[NSString stringWithFormat:@"%@", (NSString *)key] value:(NSString *)obj];
        }];
    }
    
    MBLHTTPResponseDelegate *delegate = [[MBLHTTPResponseDelegate alloc] initWithBlock:httpResponseBlock];
    [request setDelegate:delegate];
} // setHeaders:andDelegate:forRequest

@end