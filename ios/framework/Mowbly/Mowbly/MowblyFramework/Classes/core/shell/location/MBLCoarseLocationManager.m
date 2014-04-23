//
//  MBLCoarseLocationManager.m
//  Mowbly
//
//  Created by Sathish on 26/09/12.
//  Copyright (c) 2011-2014 CloudPact. All rights reserved.
//

#import "MBLCoarseLocationManager.h"

@implementation MBLCoarseLocationManager

- (id)init {

    if((self = [super init])) {

        [self setDesiredAccuracy:kCLLocationAccuracyThreeKilometers];
        [self setDistanceFilter:10]; // 10 meters
    }
    
    return self;
} // init

@end