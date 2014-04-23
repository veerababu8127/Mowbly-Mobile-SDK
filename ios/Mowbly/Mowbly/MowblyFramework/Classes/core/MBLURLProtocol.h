//
//  MBLURLProtocol.h
//  Mowbly
//
//  Created by Sathish on 27/11/12.
//  Copyright (c) 2011-2014 CloudPact. All rights reserved.

#import <Foundation/Foundation.h>

#import "MBLJSMessage.h"
#import "MBLPage.h"

@interface MBLURLProtocol : NSURLProtocol

- (void)invoke:(NSURL *)url;

+ (void)reset;


@end