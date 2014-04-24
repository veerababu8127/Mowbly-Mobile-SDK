//
//  MBLAbstractLogHandler.h
//  Mowbly
//
//  Created by Sathish on 05/09/12.
//  Copyright (c) 2011-2014 CloudPact. All rights reserved.
//

#import "MBLLogHandler.h"

@interface MBLAbstractLogHandler : NSObject <MBLLogHandler> {

    BOOL _isPropagationAllowed;
    id<MBLLogLayout> _layout;
    NSString* _name;
    MBLLogLevel* _threshold;
}

@end