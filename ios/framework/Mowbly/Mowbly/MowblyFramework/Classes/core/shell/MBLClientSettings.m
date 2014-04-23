//
//  MBLClientSettings.m
//  Mowbly
//
//  Created by Sathish on 08/10/13.
//  Copyright (c) 2011-2014 CloudPact. All rights reserved.

#import "CJSONSerializer.h"
#import "MBLApp.h"
#import "MBLClientSettings.h"
#import "MBLConstants.h"

@implementation MBLClientSettings

@synthesize companyId = _companyId,
            companyIdentifier = _companyIdentifier,
            serviceUrl = _serviceUrl,
            space = _space;

static MBLClientSettings *_clientSettings;  // static instance

#pragma mark -
#pragma mark Singleton

// Returns the singleton instance of MBLClientSettings class
+ (MBLClientSettings *)defaultSettings {

	if (_clientSettings == nil) {

        NSData *dataSettings = [MBLApp getPreference:CLIENT_SETTINGS];
        if(dataSettings != nil) {

            _clientSettings = [NSKeyedUnarchiver unarchiveObjectWithData:dataSettings];
        } else {

            _clientSettings = [[super alloc] init];
            [_clientSettings update:[_clientSettings defaults]];
        }
    }
    return _clientSettings;
} // defaultSettings

+ (void)backup {
    
    [MBLApp setValue:[NSKeyedArchiver archivedDataWithRootObject:_clientSettings]
       forPreference:CLIENT_SETTINGS_BAK];
} // backup

+ (void)clearBackup {

    [MBLApp clearPreference:CLIENT_SETTINGS_BAK];
} // clearBackup

+ (void)restore {
    
    NSData *dataSettings = [MBLApp getPreference:CLIENT_SETTINGS_BAK];
    if(dataSettings != nil) {
        
        _clientSettings = [NSKeyedUnarchiver unarchiveObjectWithData:dataSettings];
        [_clientSettings persist];
    } else {
        
        _clientSettings = [[super alloc] init];
        [_clientSettings update:[_clientSettings defaults]];
    }
} // restore


- (id)copyWithZone:(NSZone *)zone {
    return self;
} // copyWithZone


#pragma mark -
#pragma mark NSCoding

- (id)initWithCoder:(NSCoder *)aDecoder {
    
    if((self = [self init])) {

        [self setCompanyId:[aDecoder decodeObjectForKey:COMPANY_ID]];
        [self setCompanyIdentifier:[aDecoder decodeObjectForKey:COMPANY_IDENTIFIER]];
        [self setServiceUrl:[aDecoder decodeObjectForKey:SERVICE_URL]];
        [self setSpace:[aDecoder decodeObjectForKey:SPACE]];
    }
    
    return self;
} // initWithCoder

- (void)encodeWithCoder:(NSCoder*)encoder {

    [encoder encodeObject:[self companyId] forKey:COMPANY_ID];
    [encoder encodeObject:[self companyIdentifier] forKey:COMPANY_IDENTIFIER];
    [encoder encodeObject:[self serviceUrl] forKey:SERVICE_URL];
    [encoder encodeObject:[self space] forKey:SPACE];
} // encodeWithCode

#pragma mark -
#pragma mark MBLClientSettings methods

- (NSDictionary *)asDictionary {

    // To Javascript layer
    return [NSDictionary dictionaryWithObjectsAndKeys:
            [self companyId], COMPANY_ID,
            [self companyIdentifier], COMPANY_IDENTIFIER,
            [self serviceUrl], SERVICE_URL,
            [self space], SPACE, nil];
} // asDictionary

- (NSDictionary *)defaults {

    return [NSDictionary dictionaryWithObjectsAndKeys:
            MBLProperty(@"COMPANY_ID"), COMPANY_ID,
            MBLProperty(@"COMPANY_IDENTIFIER"), COMPANY_IDENTIFIER,
            MBLProperty(@"SERVICE_URL"), SERVICE_URL,
            MBLProperty(@"SPACE"), SPACE, nil];
} // defaults

- (void)persist {

    [MBLApp setValue:[NSKeyedArchiver archivedDataWithRootObject:self]
       forPreference:CLIENT_SETTINGS];
} // persist

- (void)reset {

    [self update:[self defaults]];
} // reset

- (void)update:(NSDictionary *)settings {

    NSString *theCompanyId = [settings objectForKey:COMPANY_ID];
    if(! theCompanyId)
        theCompanyId = MBLProperty(@"COMPANY_ID");

    NSString *theCompanyIdentifier = [settings objectForKey:COMPANY_IDENTIFIER];
    if(! theCompanyIdentifier)
        theCompanyIdentifier = MBLProperty(@"COMPANY_IDENTIFIER");

    NSString *theServiceUrl = [settings objectForKey:SERVICE_URL];
    if(! theServiceUrl)
        theServiceUrl = MBLProperty(@"SERVICE_URL");

    NSString *theSpace = [settings objectForKey:SPACE];
    if(! theSpace)
        theSpace = MBLProperty(@"SPACE");

    [self setCompanyId:theCompanyId];
    [self setCompanyIdentifier:theCompanyIdentifier];
    [self setServiceUrl:theServiceUrl];
    [self setSpace:theSpace];

    // Persist
    [self persist];
} // update:

#pragma mark -
#pragma mark JSONSerialization

- (NSData *) JSONDataRepresentation {
    
    NSError *error;
    NSData *data = [[CJSONSerializer serializer] serializeDictionary:[self asDictionary] error:&error];
    
    return data;
} // JSONDataRepresentation


@end