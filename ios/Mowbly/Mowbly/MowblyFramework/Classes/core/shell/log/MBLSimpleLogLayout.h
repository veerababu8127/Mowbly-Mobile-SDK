//
//  MBLSimpleLayout.h
//  Mowbly
//
//  Created by Sathish on 28/08/12.
//  Copyright (c) 2011-2014 CloudPact. All rights reserved.
//

#import "MBLLogLayout.h"

@interface MBLSimpleLogLayout : NSObject <MBLLogLayout> {

    NSString *_contentType;
    NSString *_footer;
    NSString *_header;
}

@end