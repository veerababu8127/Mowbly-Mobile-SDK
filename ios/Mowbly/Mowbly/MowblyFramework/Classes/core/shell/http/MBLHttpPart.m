//
//  MBLHttpPart.m
//  Mowbly
//
//  Created by Sathish on 26/09/12.
//  Copyright (c) 2011-2014 CloudPact. All rights reserved.

#import "MBLHttpPart.h"

@implementation MBLHttpPart

@synthesize contentType = _contentType,
            fileName = _fileName,
            name = _name,
            type = _type,
            value = _value;

- (id)initWithDictionary:(NSDictionary *)oPart
{
    if(self = [super init])
    {
        [self setContentType:[oPart objectForKey:@"contentType"]];
        [self setFileName:[oPart objectForKey:@"filename"]];
        [self setName:[oPart objectForKey:@"name"]];
        [self setType:[[oPart objectForKey:@"type"] isEqualToString:@"file"] ? FILE_PART : STRING_PART];
        id partValue = [oPart objectForKey:@"value"];
        if([partValue isKindOfClass:[NSString class]])
        {
            [self setValue:partValue];
        }
    }
    
    return self;
} // initWithDictionary:

- (id)initWithName:(NSString *)name andValue:(NSString *)value {

    if((self = [super init])) {

        [self setName:name];
        [self setValue:value];
        [self setType:STRING_PART];
    }
    
    return self;
} // initWithName:andValue:


@end