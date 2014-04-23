//
//  MBLPreferences.m
//  Mowbly
//
//  Created by Sathish on 07/09/12.
//  Copyright (c) 2011-2014 CloudPact. All rights reserved.
//

#import "MBLLogger.h"
#import "MBLConstants.h"
#import "MBLApp.h"
#import "MBLJSMessage.h"
#import "MBLPage.h"
#import "MBLPreferences.h"

@implementation MBLPreferences

- (void)invoke:(MBLJSMessage *)message
{
	if ([message.method isEqualToString:@"commit"])
    {
        LogDebug(@"Committing preferences");
        NSString *preferences = (NSString *)[message.args objectAtIndex:0];
        NSUserDefaults *settings = [NSUserDefaults standardUserDefaults];
        
        [settings setValue:preferences forKey:MOWBLY_PREFERENCES];
        [settings synchronize];
	}
    else
    {
		// Not a supported method
		LogError(@"Feature %@ does not support method %@.", message.feature, message.method);
	}
} // invoke

@end