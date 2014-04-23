//
//  MBLDatabaseLogHandler.h
//  Mowbly
//
//  Created by Aravind Baskaran on 13/03/13.
//  Copyright (c) 2011-2014 CloudPact. All rights reserved.

#import "MBLAbstractLogHandler.h"

@class FMDatabase;

@interface MBLDatabaseLogHandler : MBLAbstractLogHandler {

    FMDatabase *db;
    NSString *dbPath;
}

- (id)init;

@end