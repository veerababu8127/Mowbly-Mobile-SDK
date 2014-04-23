//
//  MBLHTTPResponseDelegate.m
//  Mowbly
//
//  Created by Sathish on 08/06/11.
//  Copyright (c) 2011-2014 CloudPact. All rights reserved.
//

#import "ASIFormDataRequest.h"
#import "MBLHttpClient.h"
#import "MBLHTTPResponseDelegate.h"
#import "MBLLogger.h"
#import "MBLUtils.h"

@implementation MBLHTTPResponseDelegate

- (id) initWithBlock:(MBLHttpResponseBlock)theHttpResponseBlock {
	
	if((self = [super init])) {		
        
		// hold the tag
		httpResponseBlock = [theHttpResponseBlock copy];
	}
	
	return self;
}

#pragma mark -
#pragma mark ASIHTTPRequestDelegate methods

- (void)requestFinished:(ASIHTTPRequest *)request {

    if(httpResponseBlock) {

		httpResponseBlock(request, nil);
	}
}

- (void)requestFailed:(ASIHTTPRequest *)request {

    NSError *error = [request error];
    
    // Log the error
	LogError(@"Connection Failed - Error:  %@", error);
    
	if(httpResponseBlock){

		httpResponseBlock(request, error);
	}
}

@end