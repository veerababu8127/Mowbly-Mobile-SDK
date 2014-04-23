//
//  MBLDatabase.m
//  Mowbly
//
//  Created by Sathish on 15/06/11.
//  Copyright (c) 2011-2014 CloudPact. All rights reserved.
//

#import "MBLApp.h"
#import "MBLAppDelegate.h"
#import "MBLDatabase.h"
#import "MBLFeatureResult.h"
#import "MBLFileManager.h"
#import "MBLJSMessage.h"
#import "MBLPage.h"
#import "MBLUtils.h"
#import "FMDatabase.h"

@interface MBLDatabase (private)

- (NSString *) getUUIDForDatabaseAtPath:(NSString *) dbPath;

@end

@implementation MBLDatabase

@synthesize dbId;

- (id)init {

    if((self = [super init])) {

        [self setDbId:[NSMutableDictionary dictionary]];
    }
    
    return self;
} // init

#pragma mark -
#pragma mark MBLDatabase private

- (NSString *) getDatabasePathForDatabase:(NSString *)name rootDir:(NSString *)offlineDir error:(NSError **) error {

    NSString *dbPath = nil;
    NSFileManager *fileManager = [[NSFileManager alloc] init];
    BOOL isDir;
    NSError *err = nil;
    if (! [fileManager fileExistsAtPath:offlineDir isDirectory:&isDir] || !isDir) {

        [fileManager createDirectoryAtPath:offlineDir withIntermediateDirectories:YES attributes:nil error:&err];
    }
    
    if (! err) {

        dbPath = [[offlineDir stringByAppendingFormat:@"/%@", name] stringByAppendingPathExtension:@"sqlite"];
    } else if(error != NULL) {

        *error = err;
    }
    return dbPath;
} // getDatabasePathForDatabase:rootDir:error:

- (NSString *) getUUIDForDatabaseAtPath:(NSString *) dbPath {

    FMDatabase *db = [FMDatabase databaseWithPath:dbPath];
    [db open];
    NSString * uid = [MBLUtils GUID];
    [dbId setObject:db forKey:uid];

	return uid;
} // getUUIDForDatabaseAtPath:

#pragma mark -
#pragma mark MBLFeature

- (void)invoke:(MBLJSMessage *)message {

    callbackId = [message.callbackId copy];
	id response = nil;
	NSError *error = nil;

    if ([message.method isEqualToString:@"openDatabase"]) {

        NSString *dbName = [NSString stringWithFormat:@"%@%@",[message.args objectAtIndex:0],[message.args objectAtIndex:2]];
        //float version = [[message.args objectAtIndex:2] floatValue];
        int level=[[message.args objectAtIndex:1] intValue];
        int storageType = FILE_INTERNAL_STORAGE;
        
        NSString *rootDir = [[MBLFileManager defaultManager] absoluteDirectoryPathForLevel:level
                                                               storage:storageType
                                                            andFeature:self];
        
        NSString *dbPath = [self getDatabasePathForDatabase:dbName rootDir:rootDir error:&error];
        
        if (!error) {

            response = [self getUUIDForDatabaseAtPath:dbPath];
        }
    } else if ([message.method isEqualToString:@"beginTransaction"]) {

		FMDatabase *database = [self.dbId objectForKey:[message.args objectAtIndex:0]];
        if (database) {

            if([database beginTransaction]) {

                response = [NSNumber numberWithBool:YES];
            } else {

                error = [MBLUtils errorWithCode:[NSNumber numberWithInt:100]
                                    description:@"Unable to begin transaction"];
            }
        }
    } else if ([message.method isEqualToString:@"commit"]) {

        FMDatabase *database = [self.dbId objectForKey:[message.args objectAtIndex:0]];
        if (database) {

            if([database commit]) {

                response = [NSDictionary dictionaryWithObjects:[NSArray arrayWithObjects:
                                                                [NSNull null],
                                                                [NSNumber numberWithBool:YES],nil]
                                                       forKeys:[NSArray arrayWithObjects:
                                                                @"queryId",
                                                                @"data",nil]];
            } else {

                error = [MBLUtils errorWithCode:[NSNumber numberWithInt:100] description:@"Unable to Commit"];
            }
        }
    } else if ([message.method isEqualToString:@"rollback"]) {

		FMDatabase *database = [self.dbId objectForKey:[message.args objectAtIndex:0]];
        if (database) {

            if([database rollback]) {

                response = [NSDictionary dictionaryWithObjects:[NSArray arrayWithObjects:
                                                                [NSNull null],
                                                                [NSNumber numberWithBool:YES],nil]
                                                       forKeys:[NSArray arrayWithObjects:
                                                                @"queryId",
                                                                @"data",nil]];
            } else {

                error = [MBLUtils errorWithCode:[NSNumber numberWithInt:100] description:@"Unable to begin rollback"];
            }
        }
    } else if ([message.method isEqualToString:@"executeQuery"]) {

        NSDictionary *arg = [message.args objectAtIndex:0];
		NSString *sql = [arg valueForKey:@"sql"];
        NSArray *params = [arg valueForKey:@"params"];
        NSString *uid = [arg valueForKey:@"id"];
        NSString *queryId = [arg valueForKey:@"queryId"];
        
        FMDatabase *database = [self.dbId objectForKey:uid];
		if (database) {

            response = [self executeQuery:sql withParameter:params queryId:queryId onDatabase:database error:&error];
        }
	} else {

		// Not a supported method
		LogError(@"Feature %@ does not support method %@.", message.feature, message.method);
	}

    if (!error) {

        [self pushResponseToJavascript:response withCode:RESPONSE_CODE_OK error:nil andCallBackId:callbackId];
	} else {

		[self pushResponseToJavascript:response withCode:RESPONSE_CODE_ERROR error:error andCallBackId:callbackId];
	}
} // invoke:

- (NSDictionary *)executeQuery:(NSString *)sql
                 withParameter:(NSArray *)params
                       queryId:(NSString *)qId
                    onDatabase:(FMDatabase *)db
                         error:(NSError **)error {

	NSDictionary *result = nil;
	// open db;
    if ([db open]) {

        NSMutableArray *rsArray = [NSMutableArray array];
        FMResultSet *rs = nil;
        if (params && [params count]>0) {

            rs = [db executeQuery:sql withArgumentsInArray:params];
        } else {

            rs = [db executeQuery:sql];
        }
        
        while ([rs next]) {

            [rsArray addObject:[rs resultDictionary]];
        }
        
        if ([db hadError] && error) {
            
            NSString *errorMessage = [NSString stringWithFormat:@"%@. Statement: %@", [db lastErrorMessage], sql];
            *error = [MBLUtils errorWithCode:[NSNumber numberWithInt:[self getDBSQLErrorCode:[db lastErrorCode]]]
                                 description:errorMessage];
            rsArray = nil;
            result = [NSDictionary dictionaryWithObjectsAndKeys:qId,@"queryId", nil];
        } else {

            long long insertId = [db lastInsertRowId];
            NSInteger rowsAffected = [db changes];
            NSDictionary *data = [NSDictionary dictionaryWithObjects:[NSArray arrayWithObjects:[NSNumber numberWithLongLong:insertId],[NSNumber numberWithInt:rowsAffected],rsArray, nil] forKeys:[NSArray arrayWithObjects:@"insertId",@"rowsAffected",@"rows", nil]];

            result = [NSDictionary dictionaryWithObjects:[NSArray arrayWithObjects:qId,data,nil] forKeys:[NSArray arrayWithObjects:@"queryId",@"data",nil]];
        }
    }

	return result;
} // executeQuery: withParameter: queryId: onDatabase:

- (int) getDBSQLErrorCode:(int)errorCode {

    DBError finalCode = SQL_ERR; //default error code
    switch(errorCode) {
        case SQLITE_NOTADB:
        case SQLITE_AUTH:
        case SQLITE_CANTOPEN:
        case SQLITE_PROTOCOL:
            finalCode = DATABASE_ERR;
            break;
        case SQLITE_TOOBIG:
            finalCode = TOO_LARGE_ERR;
            break;
        case SQLITE_FULL:
            finalCode = QUOTA_ERR;
            break;
        case SQLITE_ERROR:
            finalCode = SYNTAX_ERR;
            break;
        case SQLITE_CONSTRAINT:
            finalCode = CONSTRAINT_ERR;
            break;
        case SQLITE_INTERRUPT:
            finalCode = TIMEOUT_ERR;
            break;
        default:
            finalCode = UNKNOWN_ERR;
            break;
    }
    LogError(@"DBErrorCode - %d", finalCode);
    return (int)finalCode;
} // getDBSQLErrorCode:


@end