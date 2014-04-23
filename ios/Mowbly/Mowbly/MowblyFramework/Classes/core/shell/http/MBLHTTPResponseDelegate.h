//
//  MBLHTTPResponseDelegate.h
//  Mowbly
//
//  Created by Sathish on 08/06/11.
//  Copyright (c) 2011-2014 CloudPact. All rights reserved.
//

#import "ASIHTTPRequestDelegate.h"
#import "MBLHttpClient.h"

typedef void (^MBLHttpResponseBlock)(ASIHTTPRequest* request, NSError*);

@interface MBLHTTPResponseDelegate : NSObject <ASIHTTPRequestDelegate>{

	MBLHttpResponseBlock httpResponseBlock;
}

- (id) initWithBlock:(MBLHttpResponseBlock)theHttpResponseBlock;

@end