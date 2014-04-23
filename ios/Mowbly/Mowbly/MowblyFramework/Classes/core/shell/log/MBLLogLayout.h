//
//  MBLLayout.h
//  Mowbly
//
//  Created by Sathish on 28/08/12.
//  Copyright (c) 2011-2014 CloudPact. All rights reserved.
//

#import "MBLLogEvent.h"

@protocol MBLLogLayout <NSObject>

@property (nonatomic, copy) NSString* contentType;
@property (nonatomic, copy) NSString* footer;
@property (nonatomic, copy) NSString* header;

@required

- (NSString *)format:(MBLLogEvent *)logEvent;

@end