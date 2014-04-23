//
//  MBLMessageUI.h
//  Mowbly
//
//  Created by Sathish on 04/11/11.
//  Copyright (c) 2011-2014 CloudPact. All rights reserved.
//
//  Class that bridges the SMS/Mail feature of the device to the page.

#import <MessageUI/MessageUI.h>
#import "MBLFeature.h"

@interface MBLMessageUI : MBLFeature <MFMessageComposeViewControllerDelegate, MFMailComposeViewControllerDelegate>

@end