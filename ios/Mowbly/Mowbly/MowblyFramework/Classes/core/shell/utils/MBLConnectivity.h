//
//  MBLConnectivity.h
//  Mowbly
//
//  Created by Sathish on 13/06/11.
//  Copyright (c) 2011-2014 CloudPact. All rights reserved.
//

#import "Reachability.h"
#import "MBLConnectivity.h"

@protocol MBLConnectivityDelegate

@required
- (void) didNetworkStatusUpdate:(BOOL)connected;

@end

@interface MBLConnectivity : NSObject {
    
	Reachability *_reachability;
	BOOL _networkConnected;
    NetworkStatus _networkStatus;
}

@property (nonatomic, assign) id<MBLConnectivityDelegate> delegate;
@property (nonatomic, retain) Reachability *reachability;
@property BOOL networkConnected;
@property NetworkStatus networkStatus;

+ (MBLConnectivity *)getInstance;

- (void)startReachabilityCheck;

- (void)stopReachabilityCheck;

- (NSString *) networkType;

@end