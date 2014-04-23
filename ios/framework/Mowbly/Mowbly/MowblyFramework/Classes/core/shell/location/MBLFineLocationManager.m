//
//  MBLFineLocationManager.m
//  Mowbly
//
//  Created by Sathish on 26/09/12.
//  Copyright (c) 2011-2014 CloudPact. All rights reserved.

#import "MBLFineLocationManager.h"

@implementation MBLFineLocationManager

- (id)init {

    if((self = [super init])) {

        [self setDesiredAccuracy:kCLLocationAccuracyBest];
    }
    return self;
} // init

@end