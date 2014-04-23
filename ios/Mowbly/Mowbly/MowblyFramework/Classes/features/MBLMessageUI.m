//
//  MBLMessageUI.m
//  Mowbly
//
//  Created by Sathish on 04/11/11.
//  Copyright (c) 2011-2014 CloudPact. All rights reserved.
//
    
#import "MBLLogger.h"
#import "MBLApp.h"
#import "MBLAppDelegate.h"
#import "MBLDevice.h"
#import "MBLFeatureResult.h"
#import "MBLMessageUI.h"
#import "MBLJSMessage.h"
#import "MBLPage.h"
#import "MBLPageViewController.h"
#import "MBLUtils.h"

@interface MBLMessageUI (private)


@end

@implementation MBLMessageUI

- (void)invoke:(MBLJSMessage *)message {

	if ([message.method isEqualToString:@"sendText"]) {
        
            if ([MFMessageComposeViewController canSendText]) {
                
                LogDebug(@"%@", MBLLocalizedString(@"SENDING_SMS"));
                NSArray *phoneNumbers = (NSArray *)[message.args objectAtIndex:0];
                NSString *text = (NSString *)[message.args objectAtIndex:1];
                
                MFMessageComposeViewController *messageUI = [[MFMessageComposeViewController alloc] init];
                messageUI.messageComposeDelegate = self;
                if (![phoneNumbers isEqual:[NSNull null]]) {

                    messageUI.recipients = phoneNumbers;
                }
                if (text) {

                    messageUI.body = text;
                }
                //ios 5 or 6
                if ([MBLDevice iosVersion] >= 5.0) {

                    [[[self appDelegate] appViewController] setModalPresentationStyle:UIModalPresentationFullScreen];
                    [[[self appDelegate] appViewController] presentViewController:messageUI animated:YES completion:nil];
                } else {

                    [[[self appDelegate] appViewController] presentModalViewController:messageUI animated:YES];
                }
            } else {

                LogDebug(@"%@", MBLLocalizedString(@"SMS_NOT_AVAILABLE"));
            }

    } else if ([message.method isEqualToString:@"sendMail"]) {

            if ([MFMailComposeViewController canSendMail]) {
    
                LogDebug(@"%@", MBLLocalizedString(@"SENDING_MAIL"));
                NSArray *toList = (NSArray *)[message.args objectAtIndex:0];
                NSString *subject = (NSString *)[message.args objectAtIndex:1];
                NSString *body = (NSString *)[message.args objectAtIndex:2];
                NSArray *ccList = (NSArray *)[message.args objectAtIndex:3];
                NSArray *bccList = (NSArray *)[message.args objectAtIndex:4];

                MFMailComposeViewController *mailer = [[MFMailComposeViewController alloc] init];
                [mailer setToRecipients:toList];
                [mailer setCcRecipients:ccList];
                [mailer setBccRecipients:bccList];
                [mailer setSubject:subject];
                [mailer setMessageBody:body isHTML:YES];
                mailer.mailComposeDelegate = self;

                [[[self appDelegate] appViewController] setModalPresentationStyle:UIModalPresentationFullScreen];
                [[[self appDelegate] appViewController] presentViewController:mailer animated:YES completion:nil];

                
            } else {

                LogDebug(@"%@", MBLLocalizedString(@"MAIL_NOT_AVAILABLE"));
            }
        }
} // invoke

#pragma mark -
#pragma mark MFMessageComposeViewControllerDelegate methods

- (void)messageComposeViewController:(MFMessageComposeViewController *)controller 
                 didFinishWithResult:(MessageComposeResult)result {
    
    [controller dismissViewControllerAnimated:YES completion:nil];
} // messageComposeViewController:didFinishWithResult

#pragma mark -
#pragma mark MFMailComposeViewControllerDelegate methods

- (void)mailComposeController:(MFMailComposeViewController*)controller 
          didFinishWithResult:(MFMailComposeResult)result 
                        error:(NSError*)error {

    if (error) {

        LogDebug(@"Mail error: %@", [error localizedDescription]);
	} else {

        NSString *response;
        bool status = YES;
        switch (result)
        {
            case MFMailComposeResultCancelled:
                status = NO;
                response = MBLLocalizedString(@"MAIL_CANCELED");
                break;
            case MFMailComposeResultSaved:
                response = MBLLocalizedString(@"MAIL_SAVED");
                break;
            case MFMailComposeResultSent:
                response = MBLLocalizedString(@"MAIL_SENT");
                break;
            case MFMailComposeResultFailed:
                status = NO;
                response = MBLLocalizedString(@"MAIL_SEND_FAILED");
                break;
            default:
                status = NO;
                response = MBLLocalizedString(@"MAIL_NOT_SENT");
                break;
        }
        
        if(status) {

            LogInfo(@"Sending mail success; Result - %@", response);
        } else {

            LogInfo(@"Sending mail failed; Reason - %@", response);
        }
	}

    [controller dismissViewControllerAnimated:YES completion:nil];

} // mailComposeController:didFinishWithResult:error

@end