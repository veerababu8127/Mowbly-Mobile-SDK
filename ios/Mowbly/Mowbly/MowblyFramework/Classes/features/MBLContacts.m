//
//  MBLContacts.m
//  Mowbly
//
//  Created by Sathish on 13/08/11.
//  Copyright (c) 2011-2014 CloudPact. All rights reserved.
//

#import <AddressBook/AddressBook.h>
#import "MBLApp.h"
#import "MBLAppDelegate.h"
#import "MBLContact.h"
#import "MBLContacts.h"
#import "MBLDevice.h"
#import "MBLFeatureResult.h"
#import "MBLJSMessage.h"
#import "MBLPage.h"
#import "MBLPageViewController.h"
#import "MBLUtils.h"

static int activityCancelled = -1;

@interface MBLContacts (private)

- (void)dismissPeoplePicker:(ABPeoplePickerNavigationController *)peoplePicker;

@end

@implementation MBLContacts

- (void)invoke:(MBLJSMessage *)message
{
    callbackId = [message.callbackId copy];
	id response = nil;
	error = nil;
	if([message.method isEqualToString:@"callContact"])
    {
        NSString *phoneNumber = (NSString *)[message.args objectAtIndex:0];
        NSURL *url = [NSURL URLWithString:[NSString stringWithFormat:@"tel:%@", phoneNumber]];
        if([[UIApplication sharedApplication] canOpenURL:url])
        {
            [[UIApplication sharedApplication] openURL:url];
        }
        return;
    }
    else if([message.method isEqualToString:@"deleteContact"])
    {
        bool bDeleted = NO;
        
        ABRecordID recordID = (ABRecordID)[[message.args objectAtIndex:0] intValue];
        
        if ([self accessGranted])
        {
            ABRecordRef record = ABAddressBookGetPersonWithRecordID(addressBook, recordID);
            if (record)
            {
                bool bRemoved = ABAddressBookRemoveRecord(addressBook, record, &error);
                if (bRemoved)
                {
                    bDeleted = ABAddressBookSave(addressBook, &error);
                }
            }
            if (bDeleted)
            {
                response = [NSNumber numberWithBool:bDeleted];
            }
            else if (!error)
            {
                error = (__bridge CFErrorRef)[MBLUtils errorWithCode:[NSNumber numberWithInt:0] description:MBLLocalizedString(@"PRIVACY_SETTINGS_CONTACTS")];
            }
            CFRelease(addressBook);
        }
    }
    else if([message.method isEqualToString:@"findContact"])
    {
        NSString *filter = (NSString *)[message.args objectAtIndex:0];
        NSDictionary *options = (NSDictionary *)[message.args objectAtIndex:1];
        NSArray *properties = (NSArray *)[options objectForKey:@"properties"];
        NSUInteger numContacts = [[options objectForKey:@"limit"] integerValue];
        
        if ([self accessGranted])
        {
            if ([filter isEqualToString:@""])
            {
                // Return all contacts, if filter string is empty
                NSArray *records = (__bridge NSArray *)ABAddressBookCopyArrayOfAllPeople(addressBook);
                if (numContacts == 1)
                {
                    MBLContact *contact = [[MBLContact alloc] initWithABRecordRef:(ABRecordRef)[records objectAtIndex:0]];
                    response = [contact deserializeAsDictionaryWithProperties:properties];
                }
                else
                {
                    NSMutableArray *foundRecords = [NSMutableArray arrayWithCapacity:[records count]];
                    [records enumerateObjectsUsingBlock:^(id obj, NSUInteger idx, BOOL *stop) {
                        
                        ABRecordRef record = (__bridge ABRecordRef)obj;
                        MBLContact *contact = [[MBLContact alloc] initWithABRecordRef:record];
                        [foundRecords addObject:[contact deserializeAsDictionaryWithProperties:properties]];
                    }];
                    
                    response = foundRecords;
                }
            }
            else
            {
                // Check if the filter matches the name of any contact
                NSArray *records = (__bridge NSArray *)ABAddressBookCopyPeopleWithName(addressBook, (__bridge CFStringRef)filter);
                
                if ([records count] > 0)
                {
                    NSMutableArray *foundRecords = [NSMutableArray array];
                    [records enumerateObjectsUsingBlock:^(id obj, NSUInteger idx, BOOL *stop) {
                        
                        ABRecordRef record = (__bridge ABRecordRef)obj;
                        MBLContact *contact = [[MBLContact alloc] initWithABRecordRef:record];
                        
                        [foundRecords addObject:[contact deserializeAsDictionaryWithProperties:properties]];
                        if ([foundRecords count] == numContacts)
                        {
                            *stop = YES;
                        }
                    }];
                    response = foundRecords;
                }
                else
                {
                    
                    // Proceed checking on all fields
                    NSMutableArray *foundRecords = [NSMutableArray array];
                    records = (__bridge NSArray *)ABAddressBookCopyArrayOfAllPeople(addressBook);
                    [records enumerateObjectsUsingBlock:^(id obj, NSUInteger idx, BOOL *stop) {
                        
                        ABRecordRef record = (__bridge ABRecordRef)obj;
                        MBLContact *contact = [[MBLContact alloc] initWithABRecordRef:record];
                        
                        if ([contact matches:filter])
                        {
                            [foundRecords addObject:[contact deserializeAsDictionaryWithProperties:properties]];
                            if ([foundRecords count] == numContacts)
                            {
                                *stop = YES;
                            }
                        }

                    }];
                    
                    response = foundRecords;
                }
            }
            CFRelease(addressBook);
        }
        else if (!error)
        {
            error = (__bridge CFErrorRef)[MBLUtils errorWithCode:[NSNumber numberWithInt:0] description:MBLLocalizedString(@"PRIVACY_SETTINGS_CONTACTS")];
        }
    }
    else if([message.method isEqualToString:@"pickContact"])
    {
        
        outputProperties = (NSArray *)[message.args objectAtIndex:0];
        bPickMultipleContacts = [[message.args objectAtIndex:1] boolValue];
        bChooseContactProperty = [[message.args objectAtIndex:2] boolValue];
        bAllowPickerDefaultAction = [[message.args objectAtIndex:3] boolValue];
        
        ABPeoplePickerNavigationController *picker = [[ABPeoplePickerNavigationController alloc] init];
        picker.peoplePickerDelegate = self;
        

        [[[self appDelegate] appViewController] setModalPresentationStyle:UIModalPresentationFullScreen];
        [[[self appDelegate] appViewController] presentViewController:picker animated:YES completion:nil];
        
        return;
    }
    else if ([message.method isEqualToString:@"saveContact"])
    {
        
        NSDictionary *contactInfo = [message.args objectAtIndex:0];
        
        if ([contactInfo objectForKey:@"id"]) {
            
            if ([self accessGranted])
            {
                BOOL bDeleted = NO;
                ABRecordID recordID = (ABRecordID)[[contactInfo objectForKey:@"id"]intValue];
                ABRecordRef record = ABAddressBookGetPersonWithRecordID(addressBook, recordID);
                if (record)
                {
                    bool bRemoved = ABAddressBookRemoveRecord(addressBook, record, &error);
                    if (bRemoved)
                    {
                        bDeleted = ABAddressBookSave(addressBook, &error);
                    }
                }
                if (bDeleted)
                {
                    response = [NSNumber numberWithBool:bDeleted];
                }
                else if (!error)
                {
                    error = (__bridge CFErrorRef)[MBLUtils errorWithCode:[NSNumber numberWithInt:0] description:MBLLocalizedString(@"PRIVACY_SETTINGS_CONTACTS")];
                }
                CFRelease(addressBook);
            }
        }
        
        MBLContact *contact = [[MBLContact alloc] initWithDictionary:contactInfo];
        bool didSave = [contact save:&error];
        
        if (didSave)
        {
            response = [NSNumber numberWithInt:ABRecordGetRecordID(contact.aRecord)];
        }
        else if (!error)
        {
            error = (__bridge CFErrorRef)[MBLUtils errorWithCode:[NSNumber numberWithInt:0] description:MBLLocalizedString(@"PRIVACY_SETTINGS_CONTACTS")];
        }
    }
    else if([message.method isEqualToString:@"viewContact"])
    {
        
        ABRecordID recordID = (ABRecordID)[[message.args objectAtIndex:0] intValue];
        if ([self accessGranted])
        {
            ABRecordRef record = ABAddressBookGetPersonWithRecordID(addressBook, recordID);
            ABPersonViewController *personViewController = [[ABPersonViewController alloc] init];
            personViewController.allowsEditing = YES;
            personViewController.personViewDelegate = self;
            personViewController.displayedPerson = record;
            
            UINavigationController *navigationController = [[[self appDelegate] appViewController] navigationController];
            [navigationController pushViewController:personViewController animated:YES];
            [navigationController setNavigationBarHidden:NO];
            
            CFRelease(addressBook);
        }
        else if (!error)
        {
            error = (__bridge CFErrorRef)[MBLUtils errorWithCode:[NSNumber numberWithInt:0] description:MBLLocalizedString(@"PRIVACY_SETTINGS_CONTACTS")];
        }
    }
    else
    {
		// Not a supported method
		LogError(@"Feature %@ does not support method %@.", message.feature, message.method);
	}

	if(response)
    {
		[self pushResponseToJavascript:response withCode:self.RESPONSE_CODE_OK andCallBackId:message.callbackId];
	}
    else
    {
		[self pushErrorToJavascript:(__bridge NSError *)error withCode:self.RESPONSE_CODE_ERROR andCallBackId:message.callbackId];
	}
	error = nil;
} // invoke

- (BOOL) accessGranted;
{
    __block BOOL accessGranted = NO;
    if (ABAddressBookRequestAccessWithCompletion != NULL)
    {
        // we're on iOS 6
        addressBook = ABAddressBookCreateWithOptions(NULL, &error);
        if (!error)
        {
            dispatch_semaphore_t sema = dispatch_semaphore_create(0);
            ABAddressBookRequestAccessWithCompletion(addressBook, ^(bool granted, CFErrorRef error) {
                accessGranted = granted;
                dispatch_semaphore_signal(sema);
            });
            dispatch_semaphore_wait(sema, DISPATCH_TIME_FOREVER);
        }
    }
    else
    {
        // we're on iOS 5 or older
        addressBook = ABAddressBookCreate();
        accessGranted = YES;
    }
    return accessGranted;
} //accessGranted

- (void)dismissPeoplePicker:(ABPeoplePickerNavigationController *)peoplePicker {

    // ios 5+
    if([peoplePicker respondsToSelector:@selector(presentingViewController)]) {

        [[peoplePicker presentingViewController] dismissViewControllerAnimated:YES completion:nil];
    } else {

        [[peoplePicker parentViewController] dismissModalViewControllerAnimated:YES];
    }
} // dismissPeoplePicker:

#pragma mark -
#pragma mark ABPeoplePickerNavigationControllerDelegate methods

- (void)peoplePickerNavigationControllerDidCancel:(ABPeoplePickerNavigationController *)peoplePicker
{
	bool bContactNotPicked = YES;
	if (bPickMultipleContacts)
    {
		// User would have picked contacts. Check the pickedContacts list.
		if ([pickedContacts count] > 0)
        {
			bContactNotPicked = NO;
			NSMutableArray *records = [NSMutableArray arrayWithCapacity:[pickedContacts count]];
			[pickedContacts enumerateObjectsUsingBlock:^(id obj, NSUInteger idx, BOOL *stop) {

				[records addObject:obj];
			}];
			
            NSString *message = [[MBLFeatureResult resultWithCode:self.RESPONSE_CODE_OK
                                                           result:records] toCallbackString:callbackId];
			[self pushJavascriptMessage:message];

			[pickedContacts removeAllObjects];
		}
	}

	if (bContactNotPicked)
    {

        NSString *message = [[MBLFeatureResult resultWithCode:[NSNumber numberWithInt:activityCancelled]
                                                       error:(__bridge NSError *)error] toCallbackString:callbackId];
		[self pushJavascriptMessage:message];
	}

    // Dismiss People picker
    [self dismissPeoplePicker:peoplePicker];
} // peoplePickerNavigationControllerDidCancel

- (BOOL)peoplePickerNavigationController:(ABPeoplePickerNavigationController *)peoplePicker
	  shouldContinueAfterSelectingPerson:(ABRecordRef)record
{
	// Get the contact info for JS layer
	MBLContact *contact = [[MBLContact alloc] initWithABRecordRef:record];
	NSDictionary *contactInfo = [contact deserializeAsDictionaryWithProperties:outputProperties];

	if (! bPickMultipleContacts)
    {
		// Dismiss People picker
        [self dismissPeoplePicker:peoplePicker];

		// Raise onContactPicked event
        NSString *message = [[MBLFeatureResult resultWithCode:self.RESPONSE_CODE_OK
                                                       result:contactInfo] toCallbackString:callbackId];
		[self pushJavascriptMessage:message];

		// Clear the picked contacts
		[pickedContacts removeAllObjects];
	}
    else
    {
		// collect the contact info
		[pickedContacts addObject:contactInfo];
	}

	return bPickMultipleContacts;
} // peoplePickerNavigationController:shouldContinueAfterSelectingPerson

- (BOOL)peoplePickerNavigationController:(ABPeoplePickerNavigationController *)peoplePicker 
	  shouldContinueAfterSelectingPerson:(ABRecordRef)record 
								property:(ABPropertyID)property
							  identifier:(ABMultiValueIdentifier)identifier
{
	if (! bAllowPickerDefaultAction)
    {
		// Selected property value to be returned
		NSString *value = nil;
		if (identifier == kABMultiValueInvalidIdentifier)
        {
			// single value property
			value = (__bridge NSString *)ABRecordCopyValue(record, property);
		}
        else
        {
			// Multi-value
			ABMultiValueRef multi = ABRecordCopyValue(record, property);
            if (multi)
            {
                value = (__bridge NSString *)ABMultiValueCopyValueAtIndex(multi, ABMultiValueGetIndexForIdentifier(multi, identifier));
                CFRelease(multi);   
            }
		}

		// Create the message to JS layer
		MBLContact *contact = [[MBLContact alloc] initWithABRecordRef:record];
		NSDictionary *contactInfo = [contact deserializeAsDictionaryWithProperties:outputProperties];

		NSMutableDictionary *response = [NSMutableDictionary dictionaryWithCapacity:2];
		[response setValue:contactInfo forKey:@"contact"];
		[response setValue:value forKey:@"property"];

		// Raise onContactPicked event
        NSString *message = [[MBLFeatureResult resultWithCode:self.RESPONSE_CODE_OK
                                                       result:contactInfo] toCallbackString:callbackId];
		[self pushJavascriptMessage:message];

        // Dismiss People picker
        [self dismissPeoplePicker:peoplePicker];
	}
	
	return bAllowPickerDefaultAction;
} // peoplePickerNavigationController:shouldContinueAfterSelectingPerson

#pragma mark -
#pragma mark ABPersonViewControllerDelegate methods

- (BOOL)personViewController:(ABPersonViewController *)personViewController 
shouldPerformDefaultActionForPerson:(ABRecordRef)person 
					property:(ABPropertyID)property 
				  identifier:(ABMultiValueIdentifier)identifierForValue
{
	return YES;
} // personViewController:shouldPerformDefaultActionForPerson:property:identifier



@end