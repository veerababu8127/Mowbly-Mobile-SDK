//
//  MBLDatabaseLogHandler.m
//  Mowbly
//
//  Created by Aravind Baskaran on 13/03/13.
//  Copyright (c) 2011-2014 CloudPact. All rights reserved.

#import "FMDatabase.h"
#import "MBLConstants.h"
#import "MBLDatabaseLogHandler.h"
#import "MBLApp.h"

@interface MBLDatabaseLogHandler (private)

- (void)createLogsDatabase;

- (void)deleteLogsDatabase;

@end

@implementation MBLDatabaseLogHandler

- (id)init {

    if((self = [super init])) {

        // Set the database path
        NSString *appDocDir = [MBLApp documentDirectory];
        NSFileManager *fileManager = [[NSFileManager alloc] init];
        BOOL isDir;
        if (! [fileManager fileExistsAtPath:appDocDir isDirectory:&isDir] || !isDir) {

            [fileManager createDirectoryAtPath:appDocDir withIntermediateDirectories:YES attributes:nil error:nil];
        }

        dbPath = [[[appDocDir stringByAppendingFormat:@"/%@%@", LOGS_TABLE_NAME, LOGS_DB_VERSION] stringByAppendingPathExtension:@"sqlite"] copy];

        // setup
        [self setUp];
    }

    return self;
} // init

#pragma mark -
#pragma mark MBLHandler methods

- (void)createLogsDatabase {

    @synchronized(self) {

        db = [FMDatabase databaseWithPath:dbPath];
        [db open];
        [db beginTransaction];
        [db executeUpdate:[NSString stringWithFormat:@"CREATE TABLE IF NOT EXISTS %@ (type VARCHAR(255), level VARCHAR(50), tag TEXT, message LONGTEXT, timestamp LONGTEXT)", LOGS_TABLE_NAME]];
        [db commit];
        [db close];
    }
} // createLogsDatabase

- (void)deleteLogsDatabase {

    @synchronized(self) {

        db = [FMDatabase databaseWithPath:dbPath];
        [db open];
        [db beginTransaction];
        [db executeUpdate:[NSString stringWithFormat:@"DROP TABLE %@", LOGS_TABLE_NAME]];
        [db commit];
        [db close];
    }
} // deleteLogsDatabase

- (void)handle:(MBLLogEvent *)logEvent {

    @synchronized(self) {

        db = [FMDatabase databaseWithPath:dbPath];
        if(db && [db open]) {

            NSArray *arr = [NSArray arrayWithObjects:[logEvent type],[logEvent level],[logEvent tag], [logEvent message], [logEvent timestamp], nil];
            NSString *sqlStmt = [NSString stringWithFormat:@"INSERT INTO %@ VALUES (?,?,?,?,?)",LOGS_TABLE_NAME];
            [db beginTransaction];
            [db executeUpdate:sqlStmt withArgumentsInArray:arr];
            [db commit];
            [db close];
        }
    }
} // handle:

- (void)reset {

    // Recreate logs db
    [self deleteLogsDatabase];
    [self createLogsDatabase];
} // reset

- (void)setUp {

    // Create database if not exists
    [self createLogsDatabase];
} // setUp


@end