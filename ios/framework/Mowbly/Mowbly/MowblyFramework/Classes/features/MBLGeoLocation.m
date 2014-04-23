//
//  MBLGeoLocation.m
//  Mowbly
//
//  Created by Sathish on 14/09/11.
//  Copyright (c) 2011-2014 CloudPact. All rights reserved.
//

#import "CJSONSerializer.h"
#import "MBLLogger.h"
#import "MBLCoarseLocationManager.h"
#import "MBLFeatureResult.h"
#import "MBLFineLocationManager.h"
#import "MBLGeoLocation.h"
#import "MBLJSMessage.h"
#import "MBLLocationManager.h"
#import "MBLUtils.h"

@implementation MBLGeoLocation

- (void)invoke:(MBLJSMessage *)message {
    
    if([message.method isEqualToString:@"getLocationForCoordinates"]) {

        LogDebug(@"Requested location at time %@", [NSDate date]);
        callbackId = [message.callbackId copy];

        double latitude = [(NSNumber *)[message.args objectAtIndex:0] doubleValue];
        double longitude = [(NSNumber *)[message.args objectAtIndex:1] doubleValue];
        CLLocationCoordinate2D coordinate = {latitude, longitude};
        reverseGeocoder = [[MKReverseGeocoder alloc] initWithCoordinate:coordinate];
        reverseGeocoder.delegate = self;
        [reverseGeocoder start];
    } else if([message.method isEqualToString:@"getCurrentPosition"]) {
        
        if ([CLLocationManager locationServicesEnabled]) {

            // Read the options
            NSDictionary *options = (NSDictionary *)[message.args objectAtIndex:0];
            
            // Get the timeout
            long timeout = [[options objectForKey:@"timeout"] longValue]/1000.0;
            
            // Get the LocationManager based on accuracy
            bool bNeedHighAccuracy = [[options objectForKey:@"enableHighAccuracy"] boolValue];
            LogDebug(@"Requested position at time %@ with accuracy %@ and timeout %ld", [NSDate date], [NSNumber numberWithBool:bNeedHighAccuracy], timeout);
            MBLLocationManager *locationManager = (bNeedHighAccuracy) ?
            [[MBLFineLocationManager alloc] initWithCallbackId:message.callbackId andTimeout:timeout] :
            [[MBLCoarseLocationManager alloc] initWithCallbackId:message.callbackId andTimeout:timeout];
            
            [locationManager setDelegate:self];
            [locationManager startMonitoringLocationChanges];
        } else {

            NSString *msg = @"Location service provider disabled.";
            LogDebug(@"%@",msg);
            NSError *error = [MBLUtils errorWithCode:[NSNumber numberWithInt:0] description:msg];
            [self pushJavascriptMessage:[[MBLFeatureResult resultWithCode:RESPONSE_CODE_ERROR error:error] toCallbackString:message.callbackId]];
        }
    } else {

		// Not a supported method
		LogError(@"Feature %@ does not support method %@.", message.feature, message.method);
	}
} // invoke

#pragma mark -
#pragma mark CLLocationManagerDelegate methods

NSDictionary *dictionaryForCLLocation(CLLocation *location)
{
    NSNumber *latitude = [NSNumber numberWithDouble:location.coordinate.latitude];
    NSNumber *longitude = [NSNumber numberWithDouble:location.coordinate.longitude];
    NSNumber *altitude = [NSNumber numberWithDouble:location.altitude];
    NSNumber *horizontalAccuracy = [NSNumber numberWithDouble:location.horizontalAccuracy];
    NSNumber *verticalAccuracy = [NSNumber numberWithDouble:location.verticalAccuracy];
    NSNumber *speed = [NSNumber numberWithDouble:location.speed];
    NSNumber *heading = [NSNumber numberWithDouble:location.course];
    
    NSDictionary *coords = [NSDictionary dictionaryWithObjects:[NSArray arrayWithObjects:
                                                                latitude,
                                                                longitude,
                                                                altitude,
                                                                horizontalAccuracy,
                                                                verticalAccuracy,
                                                                speed,
                                                                heading,
                                                                nil]
                                                       forKeys:[NSArray arrayWithObjects:
                                                                @"latitude",
                                                                @"longitude",
                                                                @"altitude",
                                                                @"horizontalAccuracy",
                                                                @"verticalAccuracy",
                                                                @"speed",
                                                                @"heading",
                                                                nil]
                            ];
    NSDate* eventDate = location.timestamp;
    NSNumber *timestamp = [NSNumber numberWithLong:([eventDate timeIntervalSince1970] * 1000)];
    NSDictionary *dLocation = [NSDictionary dictionaryWithObjects:[NSArray arrayWithObjects:
                                                                   coords,
                                                                   timestamp,
                                                                   nil]
                                                         forKeys:[NSArray arrayWithObjects:
                                                                  @"coords",
                                                                  @"timestamp",
                                                                  nil]
                               ];
    
    return dLocation;
} // dictionaryForCLLocation

- (void)locationManager:(MBLLocationManager *)manager
    didUpdateToLocation:(CLLocation *)newLocation
           fromLocation:(CLLocation *)oldLocation {

    LogDebug(@"Position received at %@", [NSDate date]);

    // Stop the location monitoring and timeout thread
    [manager cancelStopMonitoringLocationChanges];
    [manager stopMonitoringLocationChanges];
    
    // Prepare the response
    NSDictionary *response = [NSDictionary dictionaryWithObjects:[NSArray arrayWithObjects:
                                                                  dictionaryForCLLocation(newLocation),
                                                                  dictionaryForCLLocation(oldLocation),
                                                                  nil]
                                                         forKeys:[NSArray arrayWithObjects:
                                                                  @"position",
                                                                  @"oldPosition",
                                                                  nil]
                              ];
    
    [self pushJavascriptMessage:[[MBLFeatureResult resultWithCode:RESPONSE_CODE_OK result:response] toCallbackString:[manager callbackId]]];
} // locationManager:didUpdateToLocation:fromLocation

- (void)locationManager:(MBLLocationManager *)manager didFailWithError:(NSError *)error {

    LogDebug(@"Could not retrieve position. Reason - %@", [error localizedDescription]);

    // Stop the location monitoring and timeout thread
    [manager cancelStopMonitoringLocationChanges];
    [manager stopMonitoringLocationChanges];
    
    // Prepare the response
    [self pushJavascriptMessage:[[MBLFeatureResult resultWithCode:RESPONSE_CODE_ERROR error:error] toCallbackString:[manager callbackId]]];
} // locationManager:didFailWithError

#pragma mark -
#pragma mark MKReverseGeocoderDelegate methods

- (void)reverseGeocoder:(MKReverseGeocoder *)geocoder didFindPlacemark:(MKPlacemark *)placemark {

    LogDebug(@"Location received at %@", [NSDate date]);
    NSMutableArray *addresses = [NSMutableArray arrayWithCapacity:1];
    NSMutableDictionary *address = [NSMutableDictionary dictionaryWithCapacity:9];
    [address setValue:[placemark administrativeArea] forKey:@"admin"];
    [address setValue:[placemark subAdministrativeArea] forKey:@"subAdmin"];
    [address setValue:[placemark locality] forKey:@"locality"];
    [address setValue:[placemark thoroughfare] forKey:@"thoroughfare"];
    [address setValue:[placemark postalCode] forKey:@"postalCode"];
    [address setValue:[placemark countryCode] forKey:@"countryCode"];
    [address setValue:[placemark country] forKey:@"country"];
    [addresses addObject:address];

    NSString *message = [[MBLFeatureResult resultWithCode:self.RESPONSE_CODE_OK 
                                                    result:addresses] toCallbackString:callbackId];

    [self pushJavascriptMessage:message];
} // reverseGeocoder:didFindPlacemark

- (void)reverseGeocoder:(MKReverseGeocoder *)geocoder didFailWithError:(NSError *)error {

    LogDebug(@"Could not retrieve location. Reason - %@", [error localizedDescription]);
    NSString *message = [[MBLFeatureResult resultWithCode:self.RESPONSE_CODE_OK 
                                                     error:error] toCallbackString:callbackId];
    
    [self pushJavascriptMessage:message];
} // reverseGeocoder:didFailWithError
@end