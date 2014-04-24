//
//  MBLHttpPart.h
//  Mowbly
//
//  Created by Sathish on 26/09/12.
//  Copyright (c) 2011-2014 CloudPact. All rights reserved.

#import <Foundation/Foundation.h>

typedef enum _part_type {
    
    FILE_PART = 0,
    STRING_PART
} PartType;

@interface MBLHttpPart : NSObject {

    NSString *_contentType;
    NSString *_fileName;
    NSString *_name;
    PartType _type;
    NSString *_value;
}

@property (nonatomic, copy)NSString *contentType;
@property (nonatomic, copy)NSString *fileName;
@property (nonatomic, copy)NSString *name;
@property (nonatomic, assign)PartType type;
@property (nonatomic, copy)NSString *value;

- (id)initWithDictionary:(NSDictionary *)oPart;

- (id)initWithName:(NSString *)name andValue:(NSString *)value;

@end