//
//  MBLContacts.h
//  Mowbly
//
//  Created by Sathish on 13/08/11.
//  Copyright (c) 2011-2014 CloudPact. All rights reserved.
//
//  Class that bridges the Contacts feature with the page.

#import <AddressBook/AddressBook.h>
#import <AddressBookUI/AddressBookUI.h>
#import "MBLFeature.h"

@interface MBLContacts : MBLFeature <ABPeoplePickerNavigationControllerDelegate, ABPersonViewControllerDelegate> {

	bool bPickMultipleContacts;			// Tells if picker should be allow multiple contacts selection
	bool bChooseContactProperty;		// Tells if picker should allow selection of property
	bool bAllowPickerDefaultAction;		// Tells if picker should allow default action on a property. Picker does not close in this case.
	
	NSMutableArray *pickedContacts;		// Collection of picked contacts
	NSArray *outputProperties;			// Collection of required properties of a picked contact to return to JS
    
    ABAddressBookRef addressBook;
    CFErrorRef error;
    
    NSString *callbackId;                   // JS callback id for contacts
}

- (BOOL) accessGranted;
@end