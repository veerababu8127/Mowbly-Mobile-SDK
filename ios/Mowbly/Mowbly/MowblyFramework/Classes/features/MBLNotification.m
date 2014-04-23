//
//  MBLNotification.m
//  Mowbly
//
//  Created by Sathish on 10/09/12.
//  Copyright (c) 2011-2014 CloudPact. All rights reserved.
//

#import <AudioToolbox/AudioServices.h>
#import "MBLJSMessage.h"
#import "MBLNotification.h"

@implementation MBLNotification

- (void)invoke:(MBLJSMessage *)message {
    
	if([message.method isEqualToString:@"vibrate"]){
        
        AudioServicesPlayAlertSound(kSystemSoundID_Vibrate);
    } else {
        
		// Not a supported method
		LogError(@"Feature %@ does not support method %@.", message.feature, message.method);
	}
} // invoke

@end