//
//  MBLConnectivity.m
//  Mowbly
//
//  Created by Sathish on 13/06/11.
//  Copyright (c) 2011-2014 CloudPact. All rights reserved.
//	
//	Singleton class that checks and updates the network connectivity.
//	Holds the latest available network status.
//	Any application that needs network status changes, should conform to ConnectionDelegate protocol;
//	At any point of time, only one Mowbly application listens for network updates.
//	The setting of delegate happens during onApplicationPause and onApplicationResume events.

#import "MBLLogger.h"
#import "MBLConnectivity.h"

static MBLConnectivity *_connectivity;

@implementation MBLConnectivity

@synthesize delegate = _delegate,
            reachability = _reachability,
            networkConnected = _networkConnected,
            networkStatus = _networkStatus;


#pragma mark -
#pragma mark Singleton

// Returns the singleton instance of MBLConnectivity class
+ (MBLConnectivity *)getInstance {
    
	if (_connectivity == nil) {
        
		_connectivity = [[super alloc] init];
        
        // start listening to the network reachability.
		[[NSNotificationCenter defaultCenter] addObserver:_connectivity selector:@selector(reachabilityChanged:) name:kReachabilityChangedNotification object:nil];
        [_connectivity setReachability:[Reachability reachabilityForInternetConnection]];
    }
    return _connectivity;
} // getInstance

- (id)copyWithZone:(NSZone *)zone {
    return self;
} // copyWithZone


#pragma mark -
#pragma mark Connectivity methods

//Called by Reachability whenever status changes.
- (void) reachabilityChanged: (NSNotification* )note
{
	Reachability* curReach = [note object];
	NSParameterAssert([curReach isKindOfClass: [Reachability class]]);
	[self updateReachabilityStatus: curReach];
} // reachabilityChanged

- (void)startReachabilityCheck {

    [[self reachability] startNotifier];
    [self updateReachabilityStatus:[self reachability]];
} // startReachabilityCheck

- (void)stopReachabilityCheck {

    [[self reachability] stopNotifier];
} // stopReachabilityCheck

- (void) updateReachabilityStatus: (Reachability *) curReach {

	[self setNetworkStatus:[curReach currentReachabilityStatus]];
	switch ([self networkStatus]) {
		case NotReachable:
            LogInfo(@"Network disconnected");
			[self setNetworkConnected:NO];
			break;
		default:
            LogInfo(@"Network connected - %ld", [self networkStatus]);
			[self setNetworkConnected:YES];
			break;
	}
    
    // Notify delegates on change
	if([self delegate] != nil) {

        [[self delegate] didNetworkStatusUpdate:self.networkConnected];
	}
} // updateReachabilityStatus

- (NSString *) networkType
{
    NSString *response;
    if(self.networkStatus == 0)
    {
        response = @"NONE";
    }
    else if(self.networkStatus == 1)
    {
        response = @"WIFI";
    }
    else
    {
        response = @"CELLULAR";
    }
    return response;
}


@end