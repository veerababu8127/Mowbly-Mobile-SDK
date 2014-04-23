//
//  MBLContact.h
//  Mowbly
//
//  Created by Sathish on 01/09/11.
//  Copyright (c) 2011-2014 CloudPact. All rights reserved.
//
//	Class that represents a contact.
//	Bridges between the phone contact and the JS representation of the contact.

#import <Foundation/Foundation.h>
#import <AddressBook/AddressBook.h>

@interface MBLContact : NSObject {

	ABRecordRef aRecord;			// Contact record - native
}

@property (nonatomic, assign) ABRecordRef aRecord;

- (MBLContact *)initWithABRecordRef:(ABRecordRef)record;

- (MBLContact *)initWithDictionary:(NSDictionary *)contact;

- (NSDictionary *)deserializeAsDictionaryWithProperties:(NSArray *)propArray;

- (NSArray *)getMultiValueForProperty:(ABPropertyID)property as:(Class)returnType withAttributes:(NSArray *)attributes;

- (NSString *)getValueForProperty:(ABPropertyID)property;

- (NSDictionary *)getValueForW3CProperty:(NSString *)property withAttributes:(NSArray *)attributes;

- (bool)matches:(NSString *)filter;

- (bool)matchDateValue:(NSString *)filter forProperty:(ABPropertyID)property;

- (bool)matchMultiValue:(NSString *)filter forProperty:(ABPropertyID)property;

- (bool)matchPhoneNumber:(NSString *)filter forProperty:(ABPropertyID)property;

- (bool)matchValueString:(NSString *)filter forProperty:(ABPropertyID)property;

- (bool)save:(CFErrorRef *) error;

- (bool)setMultiValueDictionaryforProperty:(ABPropertyID)property fromFieldsArray:(NSArray *)fields;

- (bool)setMultiValueStringforProperty:(ABPropertyID)property fromFieldsArray:(NSArray *)fields;

- (bool)setValue:(id)value forProperty:(ABPropertyID)property;

// Contact Constants
// generic ContactField types
#define kW3ContactFieldId @"id"
#define kW3ContactFieldType @"type"
#define kW3ContactFieldValue @"value"

// Various labels for ContactField types
#define kW3ContactWorkLabel @"work"
#define kW3ContactHomeLabel @"home"
#define kW3ContactOtherLabel @"other"
#define kW3ContactPhoneFaxLabel @"fax"
#define kW3ContactPhoneMobileLabel @"mobile"
#define kW3ContactPhonePagerLabel @"pager"

#define kW3ContactUrlBlog @"blog"
#define kW3ContactUrlProfile @"profile"

// Contact object
#define kW3ContactId @"id"

#define kW3ContactName @"name"
#define kW3ContactFirstName @"firstName"
#define kW3ContactLastName @"lastName"
#define kW3ContactMiddleName @"middleName"
#define kW3ContactPrefix @"prefix"
#define kW3ContactSuffix @"suffix"

#define kW3ContactNickname @"nickName"

#define kW3ContactPhones @"phones"

#define kW3ContactAddresses @"addresses"
#define kW3ContactStreet @"street"
#define kW3ContactCity @"city"
#define kW3ContactRegion @"region"
#define kW3ContactPostalCode @"postalCode"
#define kW3ContactCountry @"country"
#define kW3ContactAdditionalInfo @"additionalInfo"

#define kW3ContactEmails @"emails"

#define kW3ContactImpps @"impps"
#define kW3ContactImAIMLabel @"aim"
#define kW3ContactImICQLabel @"icq"
#define kW3ContactImJABBERLabel @"jabber"
#define kW3ContactImMSNLabel @"msn"
#define kW3ContactImYahooLabel @"yahoo"
#define kW3ContactImUserName @"value"
#define kW3ContactImService @"service"

#define kW3ContactOrganization @"organization"
#define kW3ContactOrganizationName @"name"
#define kW3ContactJobTitle @"jobTitle"
#define kW3ContactDepartment @"department"

#define kW3ContactBirthday @"birthday"

#define kW3ContactNote @"note"

#define kW3ContactPhotos @"photos"

#define kW3ContactUrls @"urls"

@end