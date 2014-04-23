//
//  MBLUi.m
//  Mowbly
//
//  Created by Sathish on 10/09/12.
//  Copyright (c) 2011-2014 CloudPact. All rights reserved.
//

#import "CJSONDeserializer.h"
#import "iToast.h"
#import "MBLFeatureBinder.h"
#import "MBLJSMessage.h"
#import "MBLProgress.h"
#import "MBLUi.h"
#import "MBLPage.h"

@implementation MBLUi

- (void) invoke:(MBLJSMessage *)message {
    
    if([message.method isEqualToString:@"alert"]) {
        
        NSError *error = nil;
        NSDictionary *oAlert = [[CJSONDeserializer deserializer] deserializeAsDictionary:[[[message.args objectAtIndex:0] stringByReplacingPercentEscapesUsingEncoding:NSUTF8StringEncoding] dataUsingEncoding:NSUTF8StringEncoding]
                                                                                     error:&error];
        
        if(oAlert) {

            NSString *title = [oAlert objectForKey:@"title"];
            NSString *alertMsg = [oAlert objectForKey:@"message"];
            alertCallbackId = [[oAlert objectForKey:@"callbackId"] copy];
            alertDlg = [[UIAlertView alloc] initWithTitle:title
                                                  message:alertMsg
                                                 delegate:self
                                        cancelButtonTitle:@"OK"
                                        otherButtonTitles:nil];
            [alertDlg show];
        }
    } else if ([message.method isEqualToString:@"confirm"]) {
        
        // Ignore obsolete calls and calls from hidden pages
        if ([[[MBLFeatureBinder defaultBinder] view] isActive]) {
            
            NSError *error = nil;
            NSDictionary *oConfirm = [[CJSONDeserializer deserializer] deserializeAsDictionary:[[[message.args objectAtIndex:0] stringByReplacingPercentEscapesUsingEncoding:NSUTF8StringEncoding] dataUsingEncoding:NSUTF8StringEncoding] error:&error];
            
            if(oConfirm) {
                
                NSArray *aButtons = [oConfirm objectForKey:@"buttons"];
                if (aButtons && aButtons.count >= 2) {
                    
                    // Read the confirm message
                    NSString *title = [oConfirm objectForKey:@"title"];
                    NSString *msg = [oConfirm objectForKey:@"message"];
                    alertCallbackId = [[oConfirm objectForKey:@"callbackId"] copy];
                    
                    NSDictionary *button1 = [aButtons objectAtIndex:0];
                    NSString *l1 = [button1 objectForKey:@"label"];
                    if (l1 == nil) {
                        
                        l1 = @"Yes";
                    }
                    [alertDlg addButtonWithTitle:l1];
                    
                    NSDictionary *button2 = [aButtons objectAtIndex:1];
                    NSString *l2 = [button2 objectForKey:@"label"];
                    if (l2 == nil) {
                        
                        l2 = @"No";
                    }
                    [alertDlg addButtonWithTitle:l2];
                    
                    alertDlg = [[UIAlertView alloc] initWithTitle:title
                                                          message:msg
                                                         delegate:self
                                                cancelButtonTitle:nil
                                                otherButtonTitles:l1, l2, nil];
                    
                    if (aButtons.count > 2) {
                        
                        NSDictionary *button3 = [aButtons objectAtIndex:2];
                        NSString *l3 = [button3 objectForKey:@"label"];
                        if (l3 == nil) {
                            
                            l3 = @"Cancel";
                        }
                        [alertDlg addButtonWithTitle:l3];
                    }
                    
                    if ([oConfirm objectForKey:@"cancelable"]) {
                        
                        alertDlgCancelable = [[oConfirm objectForKey:@"cancelable"] boolValue];
                    } else {
                        
                        alertDlgCancelable = YES;
                    }
                    
                    [alertDlg show];
                }
            }
        }
	} else if ([message.method isEqualToString:@"hideProgress"]) {
        
        // Ignore obsolete calls and calls from hidden pages
        if ([[[MBLFeatureBinder defaultBinder] view] isActive]) {
            
            [MBLProgress hideProgress];
        }
	} else if ([message.method isEqualToString:@"hidePullToRefresh"]) {
        
        [[[MBLFeatureBinder defaultBinder] view] stopLoading];
    } else if ([message.method isEqualToString:@"showProgress"]) {
        
        // Ignore obsolete calls and calls from hidden pages
        if ([[[MBLFeatureBinder defaultBinder] view] isActive]) {
            
            NSString *title = [message.args objectAtIndex:0];
            NSString *progressText = [message.args objectAtIndex:1];
            
            [MBLProgress showProgressWithTitle:title message:progressText];
        }
	} else if ([message.method isEqualToString:@"toast"]) {
        
		NSString *text = [NSString stringWithFormat:@"%@", [message.args objectAtIndex:0]];
        if (! [text isEqual:[NSNull null]]) {
            
            iToast *toast = [[iToast alloc] initWithText:text];
            
            if ([message.args count] > 1) {
                NSNumber *oDuration = [message.args objectAtIndex:1];
                int duration = ([oDuration intValue] == 0) ? 2000 : 3500;
                [toast setDuration:duration];
            }
            
            [toast show];
        }
	} else {
		// Not a supported method
		LogError(@"Feature %@ does not support method %@.", message.feature, message.method);
	}
} // invoke

#pragma mark -
#pragma mark UIAlertViewDelegate methods

- (void)alertView:(UIAlertView *)alertView clickedButtonAtIndex:(NSInteger)buttonIndex {
    
    if(alertCallbackId && ![alertCallbackId isEqualToString:@""]) {
        
        NSString *message = [NSString stringWithFormat:@"__mowbly__.__CallbackClient.onreceive('%@', %ld)", alertCallbackId, (long)buttonIndex];
        [self pushJavascriptMessage:message];
    }
} // alertView:clickedButtonAtIndex

#pragma mark -
#pragma mark App lifecycle

- (void)onAppPause {
    
	// Cancel the confirm dialog
	if(alertDlgCancelable && alertDlg) {
        
		[alertDlg dismissWithClickedButtonIndex:[alertDlg cancelButtonIndex] animated:YES];
	}
    
	[super onAppPause];
} // onAppPause

@end