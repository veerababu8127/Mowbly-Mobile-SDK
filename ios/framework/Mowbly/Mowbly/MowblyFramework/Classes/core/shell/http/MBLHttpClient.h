//
//  MBLHttpClient.h
//  Mowbly
//
//  Created by Sathish on 03/06/11.
//  Copyright (c) 2011-2014 CloudPact. All rights reserved.
//

#import "MBLHTTPResponseDelegate.h"
#import "ASIHTTPRequest.h"

typedef void (^MBLHttpRequestBlock)(ASIHTTPRequest *request);

typedef enum _HttpOperation {

    GET = 0,
    POST = 1,
    POST_MULTIPART = 2,
    PUT = 4,
    PUT_MULTIPART = 5,
    DELETE = 6,
    HEAD = 7
} HttpOperation;

@interface MBLHttpClient : NSObject

+ (NSArray *)getPartsForPostParameters:(NSDictionary *)partsDict;

+ (BOOL)isResponseForRequest:(ASIHTTPRequest *)request ofContentType:(NSString *)type;

- (void)    get:(NSString *)url
    withHeaders:(NSDictionary *)headers
      onRequest:(MBLHttpRequestBlock)httpRequestBlock
     onResponse:(MBLHttpResponseBlock)httpResponseBlock;

- (void)    post:(NSString *)url
        withData:(id)data
      andHeaders:(NSDictionary *)headers
       onRequest:(MBLHttpRequestBlock)httpRequestBlock
      onResponse:(MBLHttpResponseBlock)httpResponseBlock;

- (void)postMultipart:(NSString *)url
            withParts:(NSArray *)parts
           andHeaders:(NSDictionary *)headers
            onRequest:(MBLHttpRequestBlock)httpRequestBlock
           onResponse:(MBLHttpResponseBlock)httpResponseBlock;

- (void) deleteRequest:(NSString *)url
           withHeaders:(NSDictionary *)headers
             onRequest:(MBLHttpRequestBlock)httpRequestBlock
            onResponse:(MBLHttpResponseBlock)httpResponseBlock;

@end