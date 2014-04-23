//
//  MBLOfflineHttpHelper.m
//  Mowbly
//
//  Created by Sathish on 01/11/13.
//  Copyright (c) 2011-2014 CloudPact. All rights reserved.
//

#import "ASIHttpRequest.h"
#import "ASIFormDataRequest.h"
#import "MBLConnectivity.h"
#import "MBLFileManager.h"
#import "MBLHttpClient.h"
#import "MBLHttpHelper.h"
#import "MBLHttpPart.h"
#import "MBLJSMessage.h"
#import "MBLLogger.h"
#import "MBLPage.h"
#import "MBLConstants.h"

@interface MBLHttpHelper (private)

- (NSString *)getPathForFileObject:(NSDictionary *)file inFeature:(MBLFeature *)feature;

- (NSMutableString *)replacePlaceholers:(NSString *)oldString username:(NSString *)username password:(NSString *)password;

@end

@implementation MBLHttpHelper

#pragma mark -
#pragma mark Utils

// Helper function to convert an object to string
static NSString *toString(id obj) {

    NSString *result = @"";
    if ([obj isKindOfClass:[NSNumber class]]) {

        // Check for bool
        if ([(NSNumber *)obj boolValue] == YES || [(NSNumber *)obj boolValue] == NO) {

            result = [(NSNumber *)obj boolValue] ? @"true" : @"false";
        } else {

            result = [NSString stringWithFormat:@"%@", obj];
        }
    } else {

        result = [NSString stringWithFormat:@"%@", obj];
    }

	return result;
} // toString

// Helper function to encode the url query parameter
static NSString *urlEncode(NSString *string) {
	return [string stringByAddingPercentEscapesUsingEncoding:NSUTF8StringEncoding];
} // urlEncode

#pragma mark -
#pragma mark MBLFeature methods

- (NSString *)getPathForFileObject:(NSDictionary *)file inFeature:(MBLFeature *)feature {

    NSString *filePath = [file objectForKey:@"path"];
    int storageType = [[file objectForKey:@"storageType"] intValue];
    int level = [[file objectForKey:@"level"] intValue];
    NSString *rootDir = [[MBLFileManager defaultManager] absoluteDirectoryPathForLevel:level
                                                                               storage:storageType
                                                                               andFeature:feature];
    filePath = ([filePath hasPrefix:@"/"]) ? [rootDir stringByAppendingString:filePath] : [rootDir stringByAppendingPathComponent:filePath];

    return filePath;
} // getPathForFileObject:feature:

- (HttpOperation) httpOperationForRequestType:(NSString *)type {

    HttpOperation op;
    if([type isEqualToString:@"POST"]) {
        op = POST;
    } else if([type isEqualToString:@"PUT"]) {
        op = PUT;
    } else if([type isEqualToString:@"DELETE"]) {
        op = DELETE;
    } else if([type isEqualToString:@"HEAD"]) {
        op = HEAD;
    } else {
        op = GET;
    }

    return op;
} // httpOperationForRequestType:

- (void)invoke:(MBLJSMessage *)message
       forFeature:(MBLFeature *)feature
     withBlock:(void(^)(NSDictionary* response, NSNumber* statusCode, NSError *error))onResponse {

    NSDictionary *options = (NSDictionary *)[message.args objectAtIndex:0];
    NSString *url = (NSString *)[options objectForKey:@"url"];
    HttpOperation type = [self httpOperationForRequestType:[options objectForKey:@"type"]];
    int timeout = [[options objectForKey:@"timeout"] intValue];
    NSMutableDictionary *headers = [NSMutableDictionary dictionaryWithDictionary:[options objectForKey:@"headers"]];

    // Download file
    NSDictionary *file = (NSDictionary *)[options objectForKey:@"downloadFile"];
    NSString *downloadFile = nil;
    if(file) {

        NSString *filePath = [self getPathForFileObject:file inFeature:feature];

        // Check if the download file is available. Create if not available.
        BOOL bIsFileAvl = YES;
        NSFileManager *fileManager = [[NSFileManager alloc] init];
        if (![fileManager fileExistsAtPath:filePath]) {

            NSString *parentDir = [filePath stringByDeletingLastPathComponent];
            BOOL isDir;
            bIsFileAvl = [fileManager fileExistsAtPath:parentDir isDirectory:&isDir];
            if (! bIsFileAvl || !isDir) {

                NSError *error = nil;
                if([fileManager createDirectoryAtPath:parentDir withIntermediateDirectories:YES attributes:nil error:&error]) {

                    bIsFileAvl = [fileManager createFileAtPath:filePath contents:nil attributes:nil];
                } else {

                    bIsFileAvl = NO;
                    LogError(@"Http: Could not create file %@; Reason - %@", filePath, [error localizedDescription]);
                }
            } else {

                bIsFileAvl = [fileManager createFileAtPath:filePath contents:nil attributes:nil];
            }
        }
        
        if(! bIsFileAvl) {

            LogError(@"Http: Could not create file %@ to download. Aborting request to url %@.", filePath, url);
            return;
        }

        downloadFile = [filePath copy];
    }
        // Data
        BOOL bIsMultipartFileContent = NO;
        NSArray *parts = nil;
        id postData = nil;
        NSArray *aParts = (NSArray *)[options objectForKey:@"parts"];
        if(aParts && [aParts count] > 0){

            if(type == POST || type == PUT) {

                // Multipart
                type = (type == POST) ? POST_MULTIPART : PUT_MULTIPART;
                bIsMultipartFileContent = YES;

                NSMutableArray *_parts = [NSMutableArray arrayWithCapacity:[aParts count]];
                [aParts enumerateObjectsUsingBlock:^(id obj, NSUInteger idx, BOOL *stop) {

                    NSDictionary *oPart = (NSDictionary *)obj;
                     MBLHttpPart *part = [[MBLHttpPart alloc] initWithDictionary:oPart];
                     if([part type] == FILE_PART) {

                         [part setValue:[self getPathForFileObject:[oPart objectForKey:@"value"]
                                                            inFeature:feature]];
                     }else if([part type] == STRING_PART){
                         
                         [part setValue:[self getValue:[part value] withOptions:options]];
                         
                     }
                     [_parts addObject:part];
                 }];

                parts = [NSArray arrayWithArray:_parts];
            }
        } else {

            NSString *dataType = (NSString *)[options objectForKey:@"dataType"];
            if([dataType isEqualToString:@"json"]) {

                // Name-value pairs
                NSDictionary *data = (NSDictionary *)[options objectForKey:@"data"];
                
                if(type == POST || type == PUT) {

                    // Multipart
                    type = (type == POST) ? POST_MULTIPART : PUT_MULTIPART;
                    
                    // Create parts
                    NSMutableArray *_parts = [NSMutableArray arrayWithCapacity:[data count]];
                    [data enumerateKeysAndObjectsUsingBlock:^(id key, id obj, BOOL *stop) {
                        
                        NSString *name = (NSString *)key;
                        NSString *value = [NSString stringWithFormat:@"%@", obj];
                        MBLHttpPart *part = [[MBLHttpPart alloc] initWithName:name andValue:[self getValue:value withOptions:options]];
                        [_parts addObject:part];
                    }];
                    
                    parts = [NSArray arrayWithArray:_parts];
                } else if (type != DELETE) {

                    // Append query parameters to url
                    NSMutableArray *queryParts = [NSMutableArray array];

                    // Iterate through the query parameters
                    [data enumerateKeysAndObjectsUsingBlock:^(id key, id obj, BOOL *stop) {

                        NSString *part = [NSString stringWithFormat:@"%@=%@", urlEncode(toString(key)), urlEncode(toString(obj))];
                        [queryParts addObject:part];
                    }];

                    if([queryParts count]>0) {

                        // Append the query parameters to the url
                        url = [url stringByAppendingFormat:@"?%@", [queryParts componentsJoinedByString:@"&"]];
                    }
                }
            } else if([dataType isEqualToString:@"file"]) {

                // Name-value pairs
                NSDictionary *data = (NSDictionary *)[options objectForKey:@"data"];
                
                if(type == POST || type == PUT) {

                    postData = [self getPathForFileObject:[data valueForKey:@"file"]
                                                   inFeature:feature];
                }
            } else {

                // String
                NSString *data = (NSString *)[options objectForKey:@"data"];
                if(type == POST || type == PUT) {
                    
                    postData = [[NSString stringWithString:[self getValue:data withOptions:options]] dataUsingEncoding:NSUTF8StringEncoding];
                }
            }
        }
        
        // Request and Response handlers
        MBLHttpRequestBlock httpRequestBlock = ^(ASIHTTPRequest *request) {
            
            request = [self requestWithAuth:options request:request];
            
            if(downloadFile) {

                [request setDownloadDestinationPath:downloadFile];
            }
            [request setShouldContinueWhenAppEntersBackground:YES];
            [request setTimeOutSeconds:timeout/1000.0];

            if(type == PUT || type == PUT_MULTIPART) {

                [request setRequestMethod:@"PUT"];
            } else if (type == DELETE) {

                [request setRequestMethod:@"DELETE"];
            }
            
            if (bIsMultipartFileContent) {

                if ([request respondsToSelector:@selector(setPostFormat:)]) {

                    [request performSelector:@selector(setPostFormat:) withObject:[NSNumber numberWithInt:ASIMultipartFormDataPostFormat]];
                }
            }
        };

        MBLHttpResponseBlock httpResponseBlock = ^(ASIHTTPRequest *request, NSError *error) {
    
            NSNumber *httpStatusCode = [NSNumber numberWithInt:[request responseStatusCode]];
            id r = nil;
            if(downloadFile) {

                r = [NSNumber numberWithBool:([request responseStatusCode] == 200 || [request responseStatusCode] == 206)];
            } else {
                if(! error) {

                    r = [request responseString];
                }
            }

            NSDictionary *responseHeaders = [request responseHeaders];
            if(! responseHeaders) {

                responseHeaders = [NSDictionary dictionary];
            }
            NSDictionary *response = nil;
            
            if (r) {

                response = [NSDictionary dictionaryWithObjects:[NSArray arrayWithObjects:r, responseHeaders, nil] forKeys:[NSArray arrayWithObjects:@"data", @"headers", nil]];
            }
            
            if(onResponse) {

                onResponse(response, httpStatusCode, error);
            }
        };

        // Send the request
        MBLHttpClient *httpClient = [[MBLHttpClient alloc] init];
        if (type == GET) {

            [httpClient get:url
                withHeaders:headers
                  onRequest:httpRequestBlock
                 onResponse:httpResponseBlock];
        } else if(type == POST || type == PUT) {
            [httpClient post:url
                    withData:postData
                  andHeaders:headers
                   onRequest:httpRequestBlock
                  onResponse:httpResponseBlock];
        } else if(type == DELETE) {

            [httpClient deleteRequest:url withHeaders:headers onRequest:httpRequestBlock onResponse:httpResponseBlock];
        } else {

            [httpClient postMultipart:url
                            withParts:parts
                           andHeaders:headers
                            onRequest:httpRequestBlock
                           onResponse:httpResponseBlock];
        }
} // invoke

- (NSString *)getValue:(NSString *)value
           withOptions:(NSDictionary *)options {

    return value;
}

- (ASIHTTPRequest *)requestWithAuth :(NSDictionary *)options request:(ASIHTTPRequest *)asiRequest {
    // Adding Authentication modes
    if ([[options objectForKey:AUTH_MODE] intValue] > 0) {
        
        if ([options objectForKey:USERNAME_AUTH]) {
            [asiRequest setUsername:[options objectForKey:USERNAME_AUTH]];
        } else {
            [asiRequest setUsername:[self defaultUserName]];
        }
        if ([options objectForKey:PASSWORD]) {
            [asiRequest setPassword:[options objectForKey:PASSWORD]];
        } else {
            [asiRequest setPassword:[self defaultPassword]];
        }
        if ([[options objectForKey:AUTH_MODE] intValue] == 2) {
            [asiRequest setDomain:[options objectForKey:AUTH_DOMAIN]];
        }
    }
    
    return asiRequest;
} // http auth

- (NSString *)defaultUserName {
    return @"";
} // default username

- (NSString *)defaultPassword {
    return @"";
} // default password

@end