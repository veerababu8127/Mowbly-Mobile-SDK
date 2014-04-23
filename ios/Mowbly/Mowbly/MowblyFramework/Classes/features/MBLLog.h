//
//  MBLLog.h
//  Mowbly
//
//  Created by Sathish on 10/09/12.
//  Copyright (c) 2011-2014 CloudPact. All rights reserved.
//

#import "MBLFeature.h"

@class MBLLogger;
@class MBLLogHandler;

@interface MBLLog : MBLFeature {

    MBLLogHandler *logHandler;  // Log handler
    
    NSString *callbackId;
}

@property (nonatomic, retain) MBLLogger *logger;

@end