//
//  MBLFileHandler.m
//  Mowbly
//
//  Created by Sathish on 28/08/12.
//  Copyright (c) 2011-2014 CloudPact. All rights reserved.
//

#import "MBLLogger.h"
#import "MBLApp.h"
#import "MBLFileLogHandler.h"
#import "MBLSimpleLogLayout.h"

@implementation MBLFileLogHandler

- (id)initWithFile:(NSString *)theFilePath {

    if ((self = [super init])) {

        // Set name and filepath
        [self setName:@"__file_log_handler"];
        filePath = [theFilePath copy];

        // set up
        [self setUp];
    }

    return self;
} // init

- (void)createLogFile {

    NSFileManager *fileManager = [[NSFileManager alloc] init];
    if (![fileManager fileExistsAtPath:filePath]) {

        NSString *parentDir = [filePath stringByDeletingLastPathComponent];
        BOOL isDir;
        if (! [fileManager fileExistsAtPath:parentDir isDirectory:&isDir] || !isDir) {

            NSError *error = nil;
            if([fileManager createDirectoryAtPath:parentDir withIntermediateDirectories:YES attributes:nil error:&error]) {

                [fileManager createFileAtPath:filePath contents:nil attributes:nil];
            } else {

                LogError(@"FileLogHandler could not create file %@; Reason - %@", filePath, [error localizedDescription]);
            }
        }
    } else {
            
            [self deleteLogFile];
        }
        
    [fileManager createFileAtPath:filePath contents:nil attributes:nil];
    
    fileHandle = [NSFileHandle fileHandleForWritingAtPath:filePath];
    if(fileHandle) {
        
        [fileHandle seekToEndOfFile];
    } else {
        
        LogError(@"FileLogHandler could not initialize file %@.", filePath);
    }

} // createLogFile

- (void)deleteLogFile {

    NSFileManager *fileManager = [[NSFileManager alloc] init];
    if ([fileManager fileExistsAtPath:filePath]) {

        NSError *error = nil;
        if(! [fileManager removeItemAtPath:filePath error:&error]) {

            LogError(@"Error deleting log file %@. Reason - %@", filePath, [error localizedDescription]);
        }
    }
} // deleteLogFile

- (void)setFile:(NSString *)theFilePath {

    if(theFilePath) {

        // Set Log file path
        [theFilePath copy];
        filePath = theFilePath;

        // Create log file
        [self createLogFile];
    } else {

        LogError(@"FileLogHandler failed to initialize due to invalid log file name.");
    }
} // setFile:

#pragma mark -
#pragma mark MBLHandler methods

- (void)handle:(MBLLogEvent *)logEvent {
    
    if(fileHandle) {

        NSString *logStr = [[self layout] format:logEvent];
        [fileHandle writeData:[logStr dataUsingEncoding:NSUTF8StringEncoding]];
        [fileHandle synchronizeFile];
    }
} // handle:

- (void)reset {

    [self deleteLogFile];
    [self createLogFile];
} // reset

- (void)setUp {

    // Set the file
    [self setFile:filePath];
} // setUp

@end