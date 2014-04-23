//
//  MBLContact.m
//  Mowbly
//
//  Created by Sathish on 01/09/11.
//  Copyright (c) 2011-2014 CloudPact. All rights reserved.
//

#import <UIKit/UIKit.h>
#import "MBLContact.h"
#import "MBLApp.h"
#import "MBLLogger.h"
#import "QSStrings.h"

@implementation MBLContact

@synthesize aRecord;

#pragma mark -
#pragma mark Static methods

static NSDictionary *dW3C_kAB;		// Dictionary of AddressBook fields for W3C contact fields
static NSDictionary *dkAB_W3C;		// Dictionary of W3C contact fields for AddressBook fields
static NSDictionary *dContactParts;	// Dictionary containing the elements that constitute a contact

+ (NSDictionary *)mapW3C_kAB {

	if(dW3C_kAB == nil) {

		dW3C_kAB = [NSDictionary dictionaryWithObjectsAndKeys:
					// Address
					[NSNumber numberWithInt:kABPersonAddressProperty], kW3ContactAddresses,
					kABPersonAddressStreetKey, kW3ContactStreet,
					kABPersonAddressCityKey, kW3ContactCity,
					kABPersonAddressStateKey, kW3ContactRegion,
					kABPersonAddressZIPKey, kW3ContactPostalCode,
					kABPersonAddressCountryKey, kW3ContactCountry,
					
					// Birthday
					[NSNumber numberWithInt:kABPersonBirthdayProperty], kW3ContactBirthday,
					
					// Emails
					[NSNumber numberWithInt:kABPersonEmailProperty], kW3ContactEmails,
					
					// Impps
					[NSNumber numberWithInt:kABPersonInstantMessageProperty], kW3ContactImpps,
					kABPersonInstantMessageUsernameKey, kW3ContactImUserName,
					kABPersonInstantMessageServiceKey, kW3ContactImService,
					
					// Name
					[NSNumber numberWithInt:kABPersonFirstNameProperty], kW3ContactFirstName,
					[NSNumber numberWithInt:kABPersonMiddleNameProperty], kW3ContactMiddleName,
					[NSNumber numberWithInt:kABPersonLastNameProperty], kW3ContactLastName,
					[NSNumber numberWithInt:kABPersonPrefixProperty], kW3ContactPrefix,
					[NSNumber numberWithInt:kABPersonSuffixProperty], kW3ContactSuffix,
					
					//Note
					[NSNumber numberWithInt:kABPersonNoteProperty], kW3ContactNote,
					
					// NickName
					[NSNumber numberWithInt:kABPersonNicknameProperty], kW3ContactNickname,
					
					// Organizations
					[NSNumber numberWithInt:kABPersonOrganizationProperty], kW3ContactOrganizationName,
					[NSNumber numberWithInt:kABPersonJobTitleProperty], kW3ContactJobTitle,
					[NSNumber numberWithInt:kABPersonDepartmentProperty], kW3ContactDepartment,
					
					// Phones
					[NSNumber numberWithInt:kABPersonPhoneProperty], kW3ContactPhones,
					
					// Photos
					
					// Urls
					[NSNumber numberWithInt:kABPersonURLProperty], kW3ContactUrls,
					nil];
	}
	return dW3C_kAB;
} // mapW3C_kAB

+ (NSDictionary *)mapkAB_W3C {
	
	if(dkAB_W3C == nil) {
		
		dkAB_W3C = [NSDictionary dictionaryWithObjectsAndKeys:
					// Address
					kW3ContactAddresses, [NSNumber numberWithInt:kABPersonAddressProperty],
					kW3ContactStreet, kABPersonAddressStreetKey,
					kW3ContactCity, kABPersonAddressCityKey,
					kW3ContactRegion, kABPersonAddressStateKey,
					kW3ContactPostalCode, kABPersonAddressZIPKey,
					kW3ContactCountry, kABPersonAddressCountryKey,
					
					// Birthday
					kW3ContactBirthday, [NSNumber numberWithInt:kABPersonBirthdayProperty],
					
					// Emails
					kW3ContactEmails, [NSNumber numberWithInt:kABPersonEmailProperty],
					
					// Impps
					kW3ContactImpps, [NSNumber numberWithInt:kABPersonInstantMessageProperty],
					kW3ContactImUserName, kABPersonInstantMessageUsernameKey,
					kW3ContactImService, kABPersonInstantMessageServiceKey,
					
					// Name
					kW3ContactFirstName, [NSNumber numberWithInt:kABPersonFirstNameProperty],
					kW3ContactMiddleName, [NSNumber numberWithInt:kABPersonMiddleNameProperty],
					kW3ContactLastName, [NSNumber numberWithInt:kABPersonLastNameProperty],
					kW3ContactPrefix, [NSNumber numberWithInt:kABPersonPrefixProperty],
					kW3ContactSuffix, [NSNumber numberWithInt:kABPersonSuffixProperty],
					
					//Note
					kW3ContactNote, [NSNumber numberWithInt:kABPersonNoteProperty],
					
					// NickName
					kW3ContactNickname, [NSNumber numberWithInt:kABPersonNicknameProperty],
					
					// Organizations
					kW3ContactOrganizationName, [NSNumber numberWithInt:kABPersonOrganizationProperty],
					kW3ContactJobTitle, [NSNumber numberWithInt:kABPersonJobTitleProperty],
					kW3ContactDepartment, [NSNumber numberWithInt:kABPersonDepartmentProperty],
					
					// Phones
					kW3ContactPhones, [NSNumber numberWithInt:kABPersonPhoneProperty],
					
					// Photos
					
					// Urls
					kW3ContactUrls, [NSNumber numberWithInt:kABPersonURLProperty],
					nil];
		
	}
	return dkAB_W3C;
} // mapkAB_W3C

+ (NSDictionary *) contactParts {

	if (dContactParts == nil) {

		dContactParts = [NSDictionary dictionaryWithObjectsAndKeys:
						 [NSArray arrayWithObjects: kW3ContactStreet,
													kW3ContactCity,
													kW3ContactRegion,
													kW3ContactPostalCode,
													kW3ContactCountry,
													kW3ContactAdditionalInfo,nil],	kW3ContactAddresses,
						 [NSNull null],												kW3ContactBirthday,
						 [NSArray arrayWithObjects: kW3ContactFieldType,
													kW3ContactFieldValue, nil],		kW3ContactEmails,
						 [NSArray arrayWithObjects: kW3ContactImUserName,
													kW3ContactImService, nil],		kW3ContactImpps,
						 [NSArray arrayWithObjects: kW3ContactFirstName,
													kW3ContactMiddleName, 
													kW3ContactLastName, 
													kW3ContactPrefix, 
													kW3ContactSuffix, nil],			kW3ContactName,
						 [NSNull null],												kW3ContactNickname,
						 [NSNull null],												kW3ContactNote,
						 [NSArray arrayWithObjects: kW3ContactOrganizationName,
													kW3ContactJobTitle, 
													kW3ContactDepartment, nil],		kW3ContactOrganization,
						 [NSArray arrayWithObjects: kW3ContactFieldType,
													kW3ContactFieldValue, nil],		kW3ContactPhones,
						 [NSArray arrayWithObjects: kW3ContactFieldType,
													kW3ContactFieldValue, nil],		kW3ContactPhotos,
						 [NSArray arrayWithObjects: kW3ContactFieldType,
													kW3ContactFieldValue, nil],		kW3ContactUrls,
						 nil];
	}
	

	return dContactParts;
} // contactParts


/* Conversion of type labels to iPhone properties
 *	
 *	phone:  work, home, other, mobile, fax, pager --> 
 *		kABWorkLabel, kABHomeLabel, kABOtherLabel, kABPersonPhoneMobileLabel, kABPersonHomeFAXLabel || kABPersonHomeFAXLabel, kABPersonPhonePagerLabel
 *
 *	emails:  work, home, other ---> kABWorkLabel, kABHomeLabel, kABOtherLabel
 *
 *	impps:	aim, gtalk, icq, xmpp, msn, skype, qq, yahoo --> 
 *			kABPersonInstantMessageService + (AIM, ICG, MSN, Yahoo).  No support for gtalk, xmpp, skype, qq
 */
+(CFStringRef) convertContactTypeToPropertyLabel:(NSString*)label
{
	CFStringRef type;
	
	if ([label isKindOfClass:[NSNull class]] || ![label isKindOfClass:[NSString class]]){
		type = NULL; // no label
	}
	else if ([label caseInsensitiveCompare: kW3ContactWorkLabel] == NSOrderedSame){
		type = kABWorkLabel;
	} else if ([label caseInsensitiveCompare: kW3ContactHomeLabel] == NSOrderedSame){
		type = kABHomeLabel;
	} else if ( [label caseInsensitiveCompare: kW3ContactOtherLabel] == NSOrderedSame){
		type = kABOtherLabel;
	} else if ( [label caseInsensitiveCompare:kW3ContactPhoneMobileLabel] == NSOrderedSame){
		type = kABPersonPhoneMobileLabel;
	} else if ( [label caseInsensitiveCompare:kW3ContactPhonePagerLabel] == NSOrderedSame){
		type = kABPersonPhonePagerLabel;
	} else if ( [label caseInsensitiveCompare:kW3ContactImAIMLabel] == NSOrderedSame){
		type = kABPersonInstantMessageServiceAIM;
	} else if ( [label caseInsensitiveCompare:kW3ContactImICQLabel] == NSOrderedSame){
		type = kABPersonInstantMessageServiceICQ;
	} else if ( [label caseInsensitiveCompare:kW3ContactImMSNLabel] == NSOrderedSame){
		type = kABPersonInstantMessageServiceMSN;
	} else if ( [label caseInsensitiveCompare:kW3ContactImYahooLabel] == NSOrderedSame){
		type = kABPersonInstantMessageServiceYahoo;
	} else if ( [label caseInsensitiveCompare:kW3ContactUrlProfile] == NSOrderedSame){
		type = kABPersonHomePageLabel;
	} else {
		type = kABOtherLabel;
	}

	return type;	
} // convertContactTypeToPropertyLabel

+(NSString*) convertPropertyLabelToContactType: (NSString*)label
{
	NSString* type = nil;
	if (label != nil){
		if ([label isEqualToString:(NSString*)kABPersonPhoneMobileLabel]){
			type = kW3ContactPhoneMobileLabel;
		} else if ([label isEqualToString: (NSString*)kABPersonPhoneHomeFAXLabel] || 
				  [label isEqualToString: (NSString*)kABPersonPhoneWorkFAXLabel]) {
			type=kW3ContactPhoneFaxLabel;
		}  else if ([label isEqualToString:(NSString*)kABPersonPhonePagerLabel]){
			type = kW3ContactPhonePagerLabel;
		} else if ([label isEqualToString:(NSString*)kABHomeLabel]){
			type = kW3ContactHomeLabel;
		} else if ([label isEqualToString:(NSString*)kABWorkLabel]){
			type = kW3ContactWorkLabel;
		} else if ([label isEqualToString:(NSString*)kABOtherLabel]){
			type = kW3ContactOtherLabel;
		} else if ([label isEqualToString:(NSString*)kABPersonInstantMessageServiceAIM]){
			type = kW3ContactImAIMLabel;
		} else if ([label isEqualToString: (NSString*)kABPersonInstantMessageServiceICQ]) {
			type=kW3ContactImICQLabel;
		} else if ([label isEqualToString:(NSString*)kABPersonInstantMessageServiceJabber]){
			type = kW3ContactOtherLabel;
		} else if ([label isEqualToString:(NSString*)kABPersonInstantMessageServiceMSN]){
			type = kW3ContactImMSNLabel;
		} else if ([label isEqualToString:(NSString*)kABPersonInstantMessageServiceYahoo]){
			type = kW3ContactImYahooLabel;
		} else if ([label isEqualToString:(NSString*)kABPersonHomePageLabel]){
			type = kW3ContactUrlProfile;
		} else {
			type = kW3ContactOtherLabel;
		}
		
	}
	return type;
} // convertPropertyLabelToContactType

+ (NSMutableDictionary *)translatekABContactField:(NSDictionary *)kABField {

	NSMutableDictionary *w3CField = [NSMutableDictionary dictionaryWithCapacity:[kABField count]];
	NSDictionary *kAB_W3C = [MBLContact mapkAB_W3C];
	
	[kABField enumerateKeysAndObjectsUsingBlock:^(id key, id obj, BOOL *stop) {

		NSNumber *kABLabel = (NSNumber *)key;
		NSString *w3CLabel = [kAB_W3C objectForKey:kABLabel];
        [w3CField setValue:obj forKey:w3CLabel];
	}];
	
	return w3CField;
} // translatekABContactField

+ (NSDictionary *)translateW3ContactField:(NSDictionary *)w3CField {

	NSMutableDictionary *kABField = [NSMutableDictionary dictionaryWithCapacity:[w3CField count]];
	NSDictionary *w3C_kAB = [MBLContact mapW3C_kAB];

	[w3CField enumerateKeysAndObjectsUsingBlock:^(id key, id obj, BOOL *stop) {

		NSString *w3CLabel = key;
		NSNumber *kABLabel = [w3C_kAB objectForKey:w3CLabel];
		if (kABLabel) {

			[kABField setObject:obj forKey:kABLabel];
		}
	}];

	return [NSDictionary dictionaryWithDictionary:kABField];
} // translateW3ContactField

#pragma mark -

- (id) init {

	if ((self = [super init])) {

		aRecord = ABPersonCreate();
	}
	return self;
} // init

- (MBLContact *)initWithABRecordRef:(ABRecordRef)record {

	if ((self = [super init])) {

		self.aRecord = CFRetain(record);
	}
	return self;
} // initWithABRecordRef

- (id)initWithDictionary:(NSDictionary *)contact {

	if ((self = [self init])) {
		
		__block bool didSet = YES;
		
		// Addresses
		didSet &= [self setMultiValueDictionaryforProperty:kABPersonAddressProperty
										   fromFieldsArray:[contact objectForKey:kW3ContactAddresses]];
		
		// Birthday
		double birthdatems = [(NSNumber *)[contact objectForKey:kW3ContactBirthday] doubleValue];
		if (birthdatems > 0) {

			birthdatems = birthdatems/1000;	// convert to seconds
			NSDate *birthdate = [NSDate dateWithTimeIntervalSince1970:birthdatems];
			didSet &= [self setValue:birthdate forProperty:kABPersonBirthdayProperty];
		}
		
		// Emails
		didSet &= [self setMultiValueStringforProperty:kABPersonEmailProperty 
									   fromFieldsArray:[contact objectForKey:kW3ContactEmails]];
		
		// Impps
		didSet &= [self setMultiValueDictionaryforProperty:kABPersonInstantMessageProperty
										   fromFieldsArray:[contact objectForKey:kW3ContactImpps]];
		
		// Name
		NSDictionary *dName = [contact objectForKey:kW3ContactName];
		[dName enumerateKeysAndObjectsUsingBlock:^(id key, id obj, BOOL *stop) {

			didSet &= [self setValue:(NSString *)obj 
						 forProperty:[[[MBLContact mapW3C_kAB] objectForKey:(NSString *)key] intValue]];
		}];
		
		// NickName
		didSet &= [self setValue:[contact objectForKey:kW3ContactNickname] 
					 forProperty:kABPersonNicknameProperty];
		
		// Note
		didSet &= [self setValue:[contact objectForKey:kW3ContactNote] 
					 forProperty:kABPersonNoteProperty];
		
		// Organizations - Name, title and dept
		NSDictionary *organization = [contact objectForKey:kW3ContactOrganization];
		if (organization) {

			didSet &= [self setValue:[organization objectForKey:kW3ContactOrganizationName] 
						 forProperty:kABPersonOrganizationProperty];
			didSet &= [self setValue:[organization objectForKey:kW3ContactJobTitle] 
						 forProperty:kABPersonJobTitleProperty];
			didSet &= [self setValue:[organization objectForKey:kW3ContactDepartment] 
						 forProperty:kABPersonDepartmentProperty];
		}
		
		// Phones
		didSet &= [self setMultiValueStringforProperty:kABPersonPhoneProperty 
									   fromFieldsArray:[contact objectForKey:kW3ContactPhones]];
		
		// Photos
		NSArray *photos = [contact objectForKey:kW3ContactPhotos];
		if ([photos count] > 0) {

			// Get the first photo url
			NSDictionary *photo = [photos objectAtIndex:0];
            NSString *data = [photo objectForKey:kW3ContactFieldValue];
            NSData *imgData;
            NSError* err = nil;
            if ([data hasPrefix:@"http:"] || [data hasPrefix:@"file:"]) {

                // Url
                NSURL* photoUrl = [NSURL URLWithString:data];

                // Read the photo data and set
                imgData = [NSData dataWithContentsOfURL:photoUrl options: NSDataReadingUncached error:&err];
            } else {

                // Likely data
                imgData = [QSStrings decodeBase64WithString:data];
            }
            
            if (! err) {

                if(imgData && [imgData length] > 0) {
                    
                    UIImage *image = [UIImage imageWithData:imgData];                    
                    CFErrorRef error = (__bridge CFErrorRef)err;
                    didSet &= ABPersonSetImageData(self.aRecord, (__bridge CFDataRef)UIImagePNGRepresentation(image), &error);
                }   
            }

			if (!imgData) {

				LogError(@"Error setting contact image: %@", (err != nil ? [err localizedDescription] : @""));
			}
		}
		
		// Urls
		didSet &= [self setMultiValueStringforProperty:kABPersonURLProperty
									   fromFieldsArray:[contact objectForKey:kW3ContactUrls]];
		
		if (! didSet) {
			
			LogDebug(@"Possible errors in creating Person record.");
		}
	}
	
	return self;
} // initWithDictionary

- (NSDictionary *)deserializeAsDictionaryWithProperties:(NSArray *)propArray {

	// Get the list of contact properties to return
	NSDictionary *w3ContactParts = [MBLContact contactParts];
	NSDictionary *properties;
	if (propArray && ![propArray isEqual:[NSNull null]] && ([propArray count] > 0)) {

		NSMutableDictionary *temp = [NSMutableDictionary dictionaryWithCapacity:[propArray count]];
		[propArray enumerateObjectsUsingBlock:^(id obj, NSUInteger idx, BOOL *stop) {

			NSString *w3CLabel = (NSString *)obj;
			NSArray *fields = [w3ContactParts objectForKey:w3CLabel];

			[temp setValue:fields forKey:w3CLabel];
		}];
		properties = [NSDictionary dictionaryWithDictionary:temp];
	} else {

		properties = w3ContactParts;
	}
	
	NSMutableDictionary *contact = [NSMutableDictionary dictionaryWithCapacity:1];
	NSDictionary *w3C_kAB = [MBLContact mapW3C_kAB];
	NSArray *multiValue;
	NSArray *attributes;

	// id
	[contact setObject:[NSNumber numberWithInt:ABRecordGetRecordID(self.aRecord)]
				forKey:@"id"];

	// Address
	attributes = (NSArray *)[properties objectForKey:kW3ContactAddresses];
	if (attributes) {

		multiValue = [self getMultiValueForProperty:kABPersonAddressProperty 
												 as:[NSDictionary class]
									 withAttributes:attributes];
        [contact setValue:multiValue forKey:kW3ContactAddresses];
	}

	// Birthday
	if ([properties objectForKey:kW3ContactBirthday]) {

		NSNumber* msDate = nil;
		NSDate* aDate = nil;
		CFTypeRef cfDate = ABRecordCopyValue(self.aRecord, kABPersonBirthdayProperty);
		if (cfDate) {

			aDate = (__bridge NSDate*) cfDate;
			msDate = [NSNumber numberWithDouble:([aDate timeIntervalSince1970]*1000)];
			CFRelease(cfDate);

			[contact setObject:msDate forKey:kW3ContactBirthday];
		}
	}

	// Emails
	attributes = (NSArray *)[properties objectForKey:kW3ContactEmails];
	if (attributes) {

		multiValue = [self getMultiValueForProperty:kABPersonEmailProperty
											 as:[NSString class]
								 withAttributes:attributes];
		[contact setValue:multiValue forKey:kW3ContactEmails];
	}

	// Impps
	attributes = (NSArray *)[properties objectForKey:kW3ContactImpps];
	if (attributes) {

		multiValue = [self getMultiValueForProperty:kABPersonInstantMessageProperty
												 as:[NSDictionary class]
									 withAttributes:attributes];
        [contact setValue:multiValue forKey:kW3ContactImpps];
	}

	// Name
	NSArray *w3CNameParts = [properties objectForKey:kW3ContactName];
	if (w3CNameParts) {

		NSMutableDictionary *oName = [NSMutableDictionary dictionaryWithCapacity:[w3CNameParts count]];
		[w3CNameParts enumerateObjectsUsingBlock:^(id obj, NSUInteger idx, BOOL *stop) {
			
            NSString *value = [self getValueForProperty:(ABPropertyID)[[w3C_kAB objectForKey:obj] intValue]];

            [oName setValue:value forKey:obj];
		}];
		[contact setObject:oName forKey:kW3ContactName];
	}

	// Nickname
	if ([properties objectForKey:kW3ContactNickname]) {

		NSString *nickName = [self getValueForProperty:kABPersonNicknameProperty];
		[contact setValue: nickName forKey:kW3ContactNickname];
	}

	// Note
	if ([properties objectForKey:kW3ContactNote]) {

		NSString *note = [self getValueForProperty:kABPersonNoteProperty];
		[contact setValue:note forKey:kW3ContactNote];
	}

	// Organizations
	attributes = (NSArray *)[properties objectForKey:kW3ContactOrganization];
	if (attributes) {

		NSDictionary *organization = [self getValueForW3CProperty:kW3ContactOrganization
                                                   withAttributes:attributes];
		[contact setValue:organization
                    forKey:kW3ContactOrganization];
	}

	// Phones
	attributes = (NSArray *)[properties objectForKey:kW3ContactPhones];
	if (attributes) {

		multiValue = [self getMultiValueForProperty:kABPersonPhoneProperty
												 as:[NSString class]
									 withAttributes:attributes];
		[contact setValue:multiValue forKey:kW3ContactPhones];
	}

	// Photos
	if ([properties objectForKey:kW3ContactPhotos]) {

		NSMutableArray* photos = nil;
		if (ABPersonHasImageData(self.aRecord)) {

			CFDataRef photoData = ABPersonCopyImageData(self.aRecord);
            if (photoData) {

                NSData* data = (__bridge NSData*)photoData;

                // Write the photo in tmp directory
                NSString* tmpFolderPath = [MBLApp cacheDirectory];
                NSError* err = nil;
                NSFileManager* fileMgr = [[NSFileManager alloc] init];		

                // Generate unique file name
                NSString* filePath;
                int i=1;
                do {
                    
                    filePath = [NSString stringWithFormat:@"%@/photo_%03d.jpg", tmpFolderPath, i++];
                } while([fileMgr fileExistsAtPath: filePath]);
                
                // Save file
                if ([data writeToFile: filePath options: NSAtomicWrite error: &err]) {
                    
                    photos = [NSMutableArray arrayWithCapacity:1];
                    NSMutableDictionary* photo = [NSMutableDictionary dictionaryWithCapacity:2];
                    [photo setObject:[[NSURL fileURLWithPath:filePath] absoluteString] forKey:kW3ContactFieldValue];
                    [photo setObject:@"url" forKey:kW3ContactFieldType];
                    [photos addObject:photo];
                }
                
                CFRelease(photoData);
                
                [contact setValue:photos forKey:kW3ContactPhotos];
            }
		}
	}

	// Urls
	attributes = (NSArray *)[properties objectForKey:kW3ContactUrls];
	if (attributes) {

		multiValue = [self getMultiValueForProperty:kABPersonURLProperty
												 as:[NSString class]
									 withAttributes:[properties objectForKey:kW3ContactUrls]];
		[contact setValue:multiValue forKey:kW3ContactUrls];
	}
	
	return contact;
} // deserializeAsDictionaryWithProperties

- (id)getMultiValueForProperty:(ABPropertyID)property
							as:(Class)returnType
				withAttributes:(NSArray *)attributes {

	NSMutableArray *fields = nil;
	ABMultiValueRef multi = ABRecordCopyValue(self.aRecord, property);
	if (multi) {
		
		fields = [NSMutableArray arrayWithCapacity:ABMultiValueGetCount(multi)];
		NSMutableDictionary *field;
		id fieldRef;
		for (CFIndex i = 0; i < ABMultiValueGetCount(multi); i++) {

			// Get the identifier for this field and set it as id
			id identifier = [NSNumber numberWithInt: ABMultiValueGetIdentifierAtIndex(multi,i)];
			
			// Get the label of the entry, to set as type
			NSString *kABLabel = (__bridge NSString *)ABMultiValueCopyLabelAtIndex(multi, i);
			NSString *w3CLabel = [MBLContact convertPropertyLabelToContactType:kABLabel];
    
			// Get the value. Cast it based on the returnType
			if ([returnType isEqual:[NSDictionary class]]) {

				fieldRef = (__bridge NSDictionary *)ABMultiValueCopyValueAtIndex(multi, i);
				field = [MBLContact translatekABContactField:fieldRef];
			} else {

				field = [NSMutableDictionary dictionaryWithCapacity:[attributes count]];
				fieldRef = (__bridge NSString *)ABMultiValueCopyValueAtIndex(multi, i);
				[field setObject:fieldRef forKey:kW3ContactFieldValue];
			}
			// set id and type
			[field setObject:identifier forKey:kW3ContactFieldId];
			[field setObject:w3CLabel forKey:kW3ContactFieldType];
			
			// Add the field to fields array
			[fields addObject:field];
		}

		CFRelease(multi);
	}
	
	return [NSArray arrayWithArray:fields];
} // getMultiValueForProperty

- (NSString *)getValueForProperty:(ABPropertyID)property {

	NSString *value = (__bridge NSString*)ABRecordCopyValue(self.aRecord, property);	
	return value;
} // getValueForProperty

- (NSDictionary *)getValueForW3CProperty:(NSString *)property 
						  withAttributes:(NSArray *)attributes {

	NSMutableDictionary *field = [NSMutableDictionary dictionaryWithCapacity:[attributes count]];
	NSDictionary *w3C_kAB = [MBLContact mapW3C_kAB];
	
	[attributes enumerateObjectsUsingBlock:^(id obj, NSUInteger idx, BOOL *stop) {

		NSString *w3CLabel = (NSString *)obj;
		ABPropertyID kABLabel = (ABPropertyID)[[w3C_kAB objectForKey:w3CLabel] intValue];

		if (kABLabel) {

			NSString *value = (__bridge NSString*)ABRecordCopyValue(self.aRecord, kABLabel);
			[field setValue:value forKey:w3CLabel];
		}
	}];
	
	return field;
} // getValueForW3CProperty

- (bool)matches:(NSString *)filter {

	bool bFound = NO;
    
	// Addresses
	bFound = ([self matchPhoneNumber:filter forProperty:kABPersonPhoneProperty] ||
              [self matchMultiValue:filter forProperty:kABPersonAddressProperty] ||
			  [self matchDateValue:filter forProperty:kABPersonBirthdayProperty] ||
			  [self matchMultiValue:filter forProperty:kABPersonEmailProperty] ||
			  [self matchMultiValue:filter forProperty:kABPersonInstantMessageProperty] ||
			  [self matchValueString:filter forProperty:kABPersonFirstNameProperty] ||
			  [self matchValueString:filter forProperty:kABPersonLastNameProperty] ||
			  [self matchValueString:filter forProperty:kABPersonMiddleNameProperty] ||
			  [self matchValueString:filter forProperty:kABPersonPrefixProperty] ||
			  [self matchValueString:filter forProperty:kABPersonSuffixProperty] ||
			  [self matchValueString:filter forProperty:kABPersonNicknameProperty] ||
			  [self matchValueString:filter forProperty:kABPersonNoteProperty] ||
			  [self matchValueString:filter forProperty:kABPersonOrganizationProperty] ||
			  [self matchValueString:filter forProperty:kABPersonDepartmentProperty] ||
			  [self matchValueString:filter forProperty:kABPersonJobTitleProperty] ||
			  [self matchMultiValue:filter forProperty:kABPersonURLProperty]);
	return bFound;
} // matches

- (bool)matchDateValue:(NSString *)filter forProperty:(ABPropertyID)property {

	bool bFound = NO;
	NSNumberFormatter *numFormatter = [[NSNumberFormatter alloc] init];
	[numFormatter setNumberStyle:NSNumberFormatterDecimalStyle];
	NSNumber *oDatems = [numFormatter numberFromString:filter];
	
	if (oDatems) {

		double datems = [oDatems doubleValue];
		datems = datems/1000;	// convert to seconds
        NSDate *propValue = (__bridge NSDate *)ABRecordCopyValue(self.aRecord, property);
		double value = [propValue timeIntervalSince1970];
        
		bFound = (value == datems);
	}
	return bFound;
} // matchDateValue:forProperty

- (bool)matchMultiValue:(NSString *)filter forProperty:(ABPropertyID)property {

	bool bFound = NO;
	ABMultiValueRef multi = ABRecordCopyValue(self.aRecord, property);
    if (multi) {

        NSArray *values = (__bridge NSArray *)ABMultiValueCopyArrayOfAllValues(multi);
        if (values) {

            NSString *aggregateValue = [values componentsJoinedByString:@"|"];
            bFound = ([aggregateValue rangeOfString:filter].location != NSNotFound);
        }
        CFRelease(multi);        
    }

	return bFound;
} // matchMultiValue:forProperty

- (bool)matchPhoneNumber:(NSString *)filter forProperty:(ABPropertyID)property {
    
	bool bFound = NO;
	ABMultiValueRef multi = ABRecordCopyValue(self.aRecord, property);
    if (multi) {

        NSArray *values = (__bridge NSArray *)ABMultiValueCopyArrayOfAllValues(multi);
        if (values) {

            NSString *aggregateValue = [values componentsJoinedByString:@"|"];
            // Replace "(", ")" and "-" in the aggregate string
            aggregateValue = [[[[aggregateValue 
                                stringByReplacingOccurrencesOfString:@"(" withString:@""] 
                                stringByReplacingOccurrencesOfString:@")" withString:@""] 
                                stringByReplacingOccurrencesOfString:@"-" withString:@""]
                                stringByReplacingOccurrencesOfString:@" " withString:@""];
            bFound = ([aggregateValue rangeOfString:filter].location != NSNotFound);
        }

        CFRelease(multi);        
    }

	return bFound;
} // matchMultiValue:forProperty

- (bool)matchValueString:(NSString *)filter forProperty:(ABPropertyID)property {

	bool bFound = NO;
	NSString *value = (__bridge NSString *)ABRecordCopyValue(self.aRecord, property);
    
    if (value) {
        bFound = ([value rangeOfString:filter].location != NSNotFound);
    }
	

	return bFound;
} // matchValueString:forProperty

- (bool) save:(CFErrorRef *) error
{
    bool didSave = NO;
    ABAddressBookRef addressBook;
    CFErrorRef err = nil;
    
    __block BOOL accessGranted = NO;
    if (ABAddressBookRequestAccessWithCompletion != NULL)
    {
        // we're on iOS 6
        addressBook = ABAddressBookCreateWithOptions(NULL, &err);
        if (!err)
        {
            dispatch_semaphore_t sema = dispatch_semaphore_create(0);
            ABAddressBookRequestAccessWithCompletion(addressBook, ^(bool granted, CFErrorRef error) {
                accessGranted = granted;
                dispatch_semaphore_signal(sema);
            });
            dispatch_semaphore_wait(sema, DISPATCH_TIME_FOREVER);
        }
        else
        {
            error = &err;
        }
    }
    else
    {
        // we're on iOS 5 or older
        addressBook = ABAddressBookCreate();
        accessGranted = YES;
    }
    if (accessGranted)
    {
        if(ABAddressBookAddRecord(addressBook, self.aRecord, error))
        {
            didSave = ABAddressBookSave(addressBook, error);
        }
    }
    CFRelease(addressBook);

	return didSave;
} // save

- (bool)setMultiValueDictionaryforProperty:(ABPropertyID)property
						   fromFieldsArray:(NSArray *)fields {
	__block bool didAdd = YES;
	bool didSet;
	CFErrorRef error;
	ABMutableMultiValueRef multiValueDict = ABMultiValueCreateMutable(kABMultiDictionaryPropertyType);
	
	[fields enumerateObjectsUsingBlock:^(id obj, NSUInteger idx, BOOL *stop) {
		
		NSDictionary *field = (NSDictionary *)obj;
		
		// Get the type of the field
		NSString *w3CLabel = [field objectForKey:kW3ContactFieldType];
		CFStringRef kABLabel;
		if (w3CLabel) {
			
			kABLabel = (CFStringRef)[MBLContact convertContactTypeToPropertyLabel:w3CLabel];
		} else {
			
			// No type mentioned
			kABLabel = kABOtherLabel;
		}
		
		// Translate the W3C properties of field to kAB values
		field = [MBLContact translateW3ContactField:field];
			
		// Add the property name(type)-value to the multi-value string
		didAdd &= ABMultiValueAddValueAndLabel(multiValueDict, (__bridge CFTypeRef)(field), kABLabel, NULL);
	}];
	
	didSet = didAdd;
	if (didAdd) {
		
		didSet = ABRecordSetValue(self.aRecord, property, multiValueDict, &error);
	}
	CFRelease(multiValueDict);
	
	return didSet;
} // setMultiValueDictionaryforProperty:fromFieldsArray

- (bool)setMultiValueStringforProperty:(ABPropertyID)property
					   fromFieldsArray:(NSArray *)fields {
	__block bool didAdd = YES;
	bool didSet = YES;
	CFErrorRef error;
	ABMutableMultiValueRef multiValueString = ABMultiValueCreateMutable(kABMultiStringPropertyType);
	
	[fields enumerateObjectsUsingBlock:^(id obj, NSUInteger idx, BOOL *stop) {
		
		NSDictionary *field = (NSDictionary *)obj;
		
		id value = [field objectForKey:kW3ContactFieldValue];
		if (value) {
			
			NSString *w3CLabel = [field objectForKey:kW3ContactFieldType];
			CFStringRef kABLabel;
			if (w3CLabel) {
				
				kABLabel = [MBLContact convertContactTypeToPropertyLabel:w3CLabel];
			} else {
				
				// No type mentioned
				kABLabel = (CFStringRef)@"";
			}
			
			// Add the property name(type)-value to the multi-value string
			didAdd &= ABMultiValueAddValueAndLabel(multiValueString, (__bridge CFTypeRef)(value), kABLabel, NULL);
		}
	}];
	
	didSet = didAdd;
	if (didAdd) {
		
		didSet = ABRecordSetValue(self.aRecord, property, multiValueString, &error);
	}
	CFRelease(multiValueString);
	
	return didSet;
} // setMultiValueStringforProperty:fromFieldsArray

- (bool)setValue:(id)value forProperty:(ABPropertyID)property {

	bool didSet = YES;
	if (value) {

		CFErrorRef error;
		didSet = ABRecordSetValue(self.aRecord, 
								  property, 
								  CFBridgingRetain(value),
								  &error);
	}

	return didSet;
} // setValue:forProperty


@end