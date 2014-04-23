//
//  MBLGeoLocation.h
//  Mowbly
//
//  Created by Sathish on 14/09/11.
//  Copyright (c) 2011-2014 CloudPact. All rights reserved.
//
// Class that bridges the GeoLocation feature in the device to page

#import <MapKit/MapKit.h>
#import "MBLFeature.h"

@interface MBLGeoLocation : MBLFeature <CLLocationManagerDelegate, MKReverseGeocoderDelegate> {

    MKReverseGeocoder *reverseGeocoder;     // Reverse Geocoder object
    NSString *callbackId;                   // JS callback id for getLocation
}

@end