//
//  MBLLocationManager.h
//  Mowbly
//
//  Created by CloudPact3 on 18/09/11.
//  Copyright (c) 2011-2014 CloudPact. All rights reserved.
//

#import <CoreLocation/CoreLocation.h>

@interface MBLLocationManager : CLLocationManager {

    NSString *_callbackId;     // Id of this listener
    long _timeout;             // Tells how long the listener should look for a location
}

@property (nonatomic, copy)NSString *callbackId;
@property (nonatomic)long timeout;

- (id)initWithCallbackId:(NSString *)cId andTimeout:(long)timeout;

- (void)startMonitoringLocationChanges;

- (void)stopMonitoringLocationChanges;

- (void)timeoutMonitoringLocationChanges;

- (void)cancelStopMonitoringLocationChanges;

@end