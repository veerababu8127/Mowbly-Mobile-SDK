//
//  MBLDatabase.h
//  Mowbly
//
//  Created by Sathish on 15/06/11.
//  Copyright (c) 2011-2014 CloudPact. All rights reserved.
//
//	Class that bridges the Database feature of the device to the page.

#import "MBLFeature.h"

@class FMDatabase;

typedef enum {
	SQL_ERR = 100,
    UNKNOWN_ERR = 1000,
    DATABASE_ERR = 2000,
    VERSION_ERR = 3000,
    TOO_LARGE_ERR = 4000,
    QUOTA_ERR = 5000,
    SYNTAX_ERR = 6000,
    CONSTRAINT_ERR = 7000,
    TIMEOUT_ERR = 8000
} DBError;

@interface MBLDatabase : MBLFeature {

    NSString *callbackId;
    NSMutableDictionary *dbId;
}

@property (nonatomic,retain) NSMutableDictionary *dbId;

- (NSDictionary *)executeQuery:(NSString *)sql
                 withParameter:(NSArray *)params
                       queryId:(NSString *)qId
                    onDatabase:(FMDatabase *)db
                         error:(NSError **)error;

- (int) getDBSQLErrorCode:(int)errorCode;

- (NSString *) getDatabasePathForDatabase:(NSString *)name rootDir:(NSString *)offlineDir error:(NSError **) error;

@end