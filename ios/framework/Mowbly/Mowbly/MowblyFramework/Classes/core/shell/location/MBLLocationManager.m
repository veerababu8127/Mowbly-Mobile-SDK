//
//  MBLLocationManager.m
//  Mowbly
//
//  Created by Sathish on 18/09/11.
//  Copyright (c) 2011-2014 CloudPact. All rights reserved.
//

#import "MBLLocationManager.h"
#import "MBLUtils.h"

@implementation MBLLocationManager

@synthesize timeout = _timeout;

- (id)initWithCallbackId:(NSString *)cId andTimeout:(long)timeout {

    if ((self = [self init])) {

        // Set the callbackId and timeout
        [self setCallbackId:cId];
        [self setTimeout:timeout];
    }
    
    return self;
} // initWithCallbackId:

- (void)startMonitoringLocationChanges {

    // Start monitoring location changes
    [self startUpdatingLocation];
    
    // Start the timer for timeout
    [self performSelector:@selector(timeoutMonitoringLocationChanges) withObject:nil afterDelay:[self timeout]];
} // startMonitoringLocationChanges

- (void)stopMonitoringLocationChanges {

    [self stopUpdatingLocation];
} // stopMonitoringLocationChanges

- (void)timeoutMonitoringLocationChanges {

    // Stop monitoring location changes
    [self stopMonitoringLocationChanges];
    
    NSError *error = [MBLUtils errorWithCode:[NSNumber numberWithInt:0] description:@"Position reading timed out."];
    [[self delegate] locationManager:self didFailWithError:error];
}

- (void)cancelStopMonitoringLocationChanges {

    [NSObject cancelPreviousPerformRequestsWithTarget:self];
} // cancelStopMonitoringLocationChanges


@end