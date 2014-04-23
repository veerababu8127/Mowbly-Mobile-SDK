//
//  MBLRotatingFileLogHandler.m
//  Mowbly
//
//  Created by Sathish on 05/09/12.
//  Copyright (c) 2011-2014 CloudPact. All rights reserved.
//

#import "MBLLogger.h"
#import "MBLRotatingFileLogHandler.h"

@interface MBLRotatingFileLogHandler (private)

- (void)rotate;

@end

@implementation MBLRotatingFileLogHandler

@synthesize maxBackupFiles = _maxBackupFiles,
            maxFileSize = _maxFileSize;

- (id)initWithFile:(NSString *)theFilePath {
    
    if ((self = [super initWithFile:theFilePath])) {

        [self setMaxBackupFiles:0];
        [self setMaxFileSize:10 * 1024 * 1024];
    }
    
    return self;
} // init

#pragma mark -
#pragma mark MBLLogHandler methods

- (void)handle:(MBLLogEvent *)logEvent {

    if(fileHandle) {

        [super handle:logEvent];
        if ([fileHandle seekToEndOfFile] > [self maxFileSize]) {

            [self rotate];
        }
    }
} // handle:

- (void)reset {

    // Release the file handle
    [fileHandle closeFile]; fileHandle = nil;
    
    NSFileManager *fileManager = [[NSFileManager alloc] init];
    NSError *error;
    for (int i = 0; i<_maxBackupFiles; i++) {

        NSString *path = [filePath stringByAppendingFormat:@".%d", i];
        if([fileManager fileExistsAtPath:path]) {

            if(! [fileManager removeItemAtPath:path error:&error]) {

                LogError(@"Error deleting log file %@. Reason - %@", path, [error localizedDescription]);
            }
        }
    }
    // Create the log file
    [self createLogFile];
} // reset

- (void)rotate {

    if(_maxBackupFiles > 0) {

        BOOL status = YES;

        // Release the file handle
        [fileHandle closeFile];
        fileHandle = nil;

        NSFileManager *fileManager = [[NSFileManager alloc] init];
        NSError *error;

        // Delete the oldest log file
        NSString *oldestFile = [filePath stringByAppendingFormat:@".%d", _maxBackupFiles];
        if([fileManager fileExistsAtPath:oldestFile]) {

            LogDebug(@"RotatingFileLogHandler: Deleting file %@", oldestFile);
            status = [fileManager removeItemAtPath:oldestFile error:&error];
            if(! status) {

                LogError(@"RotatingFileLogHandler: Failed to delete file %@. Reason - %@", oldestFile, [error description]);
            }
        }

        // Start reaming the log files starting from the oldest one
        NSString *srcFilePath, *tgtFilePath;
        for (int i = _maxBackupFiles-1; i>=0 && status; i--) {

            srcFilePath = (i > 0) ? [filePath stringByAppendingFormat:@".%d", i] : filePath;
            if([fileManager fileExistsAtPath:srcFilePath]) {

                tgtFilePath = [filePath stringByAppendingFormat:@".%d", i+1];
                status = [fileManager moveItemAtPath:srcFilePath toPath:tgtFilePath error:&error];
                if(! status) {

                    LogError(@"RotatingFileLogHandler: Failed to rotate file %@ to %@. Reason - %@", srcFilePath, tgtFilePath, [error description]);
                }
            }
        }


        // Set the file handle to log file
        [self setFile:filePath];
        if(! status) {

            LogError(@"RotatingFileLogHandler: Failed to rotate files.");

            // Reset the pointer to the begining.
            [fileHandle truncateFileAtOffset:0];
        }
    } else {

        // Clear contents in the file.
        [fileHandle truncateFileAtOffset:0];
    }
} // rotate

@end