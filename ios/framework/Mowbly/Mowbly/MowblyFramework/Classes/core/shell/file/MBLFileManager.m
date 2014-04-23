//
//  MBLFileManager.m
//  Mowbly
//
//  Created by Sathish on 01/10/13.
//  Copyright (c) 2011-2014 CloudPact. All rights reserved.

#import "MBLApp.h"
#import "MBLFile.h"
#import "FileInZipInfo.h"
#import "MBLFileManager.h"
#import "MBLUtils.h"
#import "QSStrings.h"
#import "ZipException.h"
#import "ZipFile.h"
#import "ZipReadStream.h"
#import "MBLUtils.h"

@implementation MBLFileManager

@synthesize errorCode = _errorCode,
            successCode = _successCode;

static MBLFileManager *_fileManager;  // static instance

#pragma mark -
#pragma mark Singleton

// Returns the singleton instance of MBLFileManager class
+ (MBLFileManager *)defaultManager {

	if (_fileManager == nil) {

		_fileManager = [self create];
    }
    return _fileManager;
} // defaultManager

- (id)init {

    if ((self = [super init])) {

        [self setErrorCode:[NSNumber numberWithInt:0]];
        [self setSuccessCode:[NSNumber numberWithInt:1]];
	}

    return self;
} // init

+ (id)create {
    
    return [[self alloc] init];
    
} // create

- (id)copyWithZone:(NSZone *)zone {
    return self;
} // copyWithZone

#pragma mark -
#pragma mark File feature methods

- (NSString *)absoluteDirectoryPathForLevel:(FileLevel)level
                                    storage:(StorageType)storageType
                                    andFeature:(MBLFeature *)feature {

    NSString *rootDir = @"";

    if(level != STORAGE_ROOT) {

        rootDir = (storageType == FILE_CACHE) ? [MBLApp cacheDirectory] : [MBLApp documentDirectory];

    }

    return rootDir;
} // absoluteDirectoryPathForLevel:storage:andFeature

- (void)clearCacheDir {

    long long currTime = ([[NSDate date] timeIntervalSince1970] * 1000);
    [[MBLFileManager defaultManager] deleteItemsAtPath:[MBLApp cacheDirectory]
                                             olderThan:currTime];
    
} // clearCacheDir

- (BOOL) deleteDirectory:(NSString *)path error:(NSError **)error {
	
	BOOL status = NO;
	BOOL isDir;
    NSFileManager *fileManager = [[NSFileManager alloc] init];
	if ([fileManager fileExistsAtPath:path isDirectory:&isDir] && isDir)
    {
		status = [fileManager removeItemAtPath:path error:error];
	} else {
        
		if (error) {
            
			*error = [MBLUtils errorWithCode:[self errorCode]
                                 description:MBLLocalizedString(@"DIRECTORY_NOT_FOUND")];
		}
	}
    
	return status;
} // deleteDirectory:error

- (BOOL) deleteFile:(NSString *)path error:(NSError **)error
{
	BOOL status = NO;
	BOOL isDir;
    NSFileManager *fileManager = [[NSFileManager alloc] init];
    
	if ([fileManager fileExistsAtPath:path isDirectory:&isDir] && !isDir) {
        
		status = [fileManager removeItemAtPath:path error:error];
	} else {
        
		if (error) {
            
			*error = [MBLUtils errorWithCode:[self errorCode]
                                 description:MBLLocalizedString(@"FILE_NOT_FOUND")];
		}
	}
    
	return status;
} // deleteFile:error

- (void)deleteItemsAtPath:(NSString *)path olderThan:(long long)when {

    NSMutableArray *filesToDelete = [NSMutableArray array];
    NSError *error = nil;
    NSFileManager *fileManager = [[NSFileManager alloc] init];
    NSDirectoryEnumerator *dirEnum = [fileManager enumeratorAtPath:path];
    NSString *file;
    while((file = [dirEnum nextObject])) {

        NSDictionary *fileAttributes = [fileManager attributesOfItemAtPath:file error:&error];
        if(fileAttributes) {

            NSDate *fileDate = [fileAttributes fileCreationDate];
            if (when == 0 || (fileDate && [fileDate timeIntervalSinceNow] <= 0-when)) {

                [filesToDelete addObject:file];
            }
        } else {

            LogError(@"Clear Dir: Error retrieving attributes for file %@. Reason - %@", file, [error description]);
        }
    }

    [filesToDelete enumerateObjectsUsingBlock:^(id obj, NSUInteger idx, BOOL *stop) {

        NSString *path = (NSString *)obj;
        NSError *error = nil;
        if(! [fileManager removeItemAtPath:path error:&error]) {

            LogError(@"Clear Dir: Could not delete file %@. Reason - %@", path, [error description]);
        }
    }];
} // deleteItemsAtPath:olderThan:

- (BOOL)getDirectory:(NSString *)path error:(NSError **)error
{
	BOOL status = NO;
	BOOL isDir;
    NSFileManager *fileManager = [[NSFileManager alloc] init];
	
	if ([fileManager fileExistsAtPath:path isDirectory:&isDir])
    {
        status = isDir;
        if (! isDir)
        {
            if (error)
            {
                *error = [MBLUtils errorWithCode:[self errorCode]
                                     description:MBLLocalizedString(@"IDENTICAL_NAME_FILE_ALREADY_EXISTS")];
            }
        }
	}
    else
    {
		status = [fileManager createDirectoryAtPath:path withIntermediateDirectories:YES attributes:nil error:error];
	}
	
	return status;
} // getDirectory:error

- (BOOL)getFile:(NSString *)path error:(NSError **)error
{
	BOOL status = NO;
    BOOL isDir;
    NSFileManager *fileManager = [[NSFileManager alloc] init];
	
	if ([fileManager fileExistsAtPath:path isDirectory:&isDir])
    {
		status = !isDir;
        if (isDir)
        {
            if (error)
            {
                *error = [MBLUtils errorWithCode:[self errorCode]
                                     description:MBLLocalizedString(@"IDENTICAL_NAME_DIRECTORY_ALREADY_EXISTS")];
            }
        }
	}
    else
    {
        NSString *parentDir = [path stringByDeletingLastPathComponent];
        BOOL isDir;
        if (! [fileManager fileExistsAtPath:parentDir isDirectory:&isDir] || !isDir)
        {
            if([fileManager createDirectoryAtPath:parentDir withIntermediateDirectories:YES attributes:nil error:error])
            {
                status = [fileManager createFileAtPath:path contents:nil attributes:nil];
            }
        }
        else
        {
            status = [fileManager createFileAtPath:path contents:nil attributes:nil];
        }
	}
	
	return status;
} // getFile:error

- (NSArray *)getFiles:(NSString *)path error:(NSError **)error
{
    NSArray *aFiles = [NSArray array];
    BOOL isDir;
    NSFileManager *fileManager = [[NSFileManager alloc] init];
	if ([fileManager fileExistsAtPath:path isDirectory:&isDir] && isDir)
    {
		NSArray *aPacks = [fileManager contentsOfDirectoryAtPath:path error:error];
        NSMutableArray *files = [NSMutableArray arrayWithCapacity:[aPacks count]];
        [aPacks enumerateObjectsUsingBlock:^(id obj, NSUInteger idx, BOOL *stop)
         {
             NSString *fileName = (NSString *)obj;
             NSMutableDictionary *file = [NSMutableDictionary dictionaryWithCapacity:2];
             [file setObject:[fileName lastPathComponent] forKey:@"name"];
             
             NSDictionary *fileAttrs = [fileManager attributesOfItemAtPath:[path stringByAppendingPathComponent:fileName]
                                                                     error:nil];
             int type = ([[fileAttrs valueForKey:NSFileType] isEqual:NSFileTypeDirectory]) ? TYPE_DIRECTORY : TYPE_FILE;
             [file setObject:[NSNumber numberWithInt:type] forKey:@"type"];
             
             [files addObject:file];
         }];
        aFiles = [NSArray arrayWithArray:files];
	}
    else
    {
		if (error)
        {
			*error = [MBLUtils errorWithCode:[self errorCode]
                                 description:MBLLocalizedString(@"DIRECTORY_NOT_FOUND")];
		}
	}
    
    return aFiles;
} // getFiles:error

- (NSString *)read:(NSString *)path error:(NSError **)error
{
    NSString *response = nil;
    BOOL isDir;
    NSFileManager *fileManager = [[NSFileManager alloc] init];
    if ([fileManager fileExistsAtPath:path isDirectory:&isDir] && !isDir)
    {
		response = [NSString stringWithContentsOfFile:path encoding:NSUTF8StringEncoding error:error];
	}
    else
    {
		if (error)
        {
			*error = [MBLUtils errorWithCode:[self errorCode]
                                 description:MBLLocalizedString(@"FILE_NOT_FOUND")];
		}
	}
    
	return response;
} // read:error

- (NSString *)readData:(NSString *)path error:(NSError **)error {
    
    NSString *content = nil;
    BOOL isDir;
    NSFileManager *fileManager = [[NSFileManager alloc] init];
    if ([fileManager fileExistsAtPath:path isDirectory:&isDir] && !isDir)
    {
        // Encode content as base64 string
		content = [QSStrings encodeBase64WithData:[NSData dataWithContentsOfFile:path]];
	}
    else
    {
		if (error)
        {
			*error = [MBLUtils errorWithCode:[self errorCode]
                                 description:MBLLocalizedString(@"FILE_NOT_FOUND")];
		}
	}
    
    return content;
} // readData:error

- (BOOL)testDirExists:(NSString *)path error:(NSError **)error
{
	BOOL status = NO;
	BOOL isDir;
    NSFileManager *fileManager = [[NSFileManager alloc] init];
	
	if ([fileManager fileExistsAtPath:path isDirectory:&isDir] && isDir)
    {
		status = YES;
	} else
    {
		if (error)
        {
			*error = [MBLUtils errorWithCode:[self errorCode]
                                 description:MBLLocalizedString(@"DIRECTORY_NOT_FOUND")];
		}
	}
	
	return status;
} // testDirExists:error

- (BOOL)testFileExists:(NSString *)path error:(NSError **)error
{
	BOOL status = NO;
	BOOL isDir;
    NSFileManager *fileManager = [[NSFileManager alloc] init];
	
	if ([fileManager fileExistsAtPath:path isDirectory:&isDir] && !isDir)
    {
		status = YES;
	}
	
	return status;
} // testFileExists:error

- (BOOL)unzipFile:(NSString *)srcFile
           toPath:(NSString *)destPath
            error:(NSError **)error
 shouldExtractDir:(BOOL(^)(NSString *dirPath))shouldExtractDir
shouldExtractFile:(BOOL(^)(NSString *filePath, NSMutableData **data))shouldExtractFile {

    BOOL status = YES;
	NSString *errorMessage = nil;
	NSFileManager *fileManager = [[NSFileManager alloc] init];
    @try {

        NSError *err = nil;

        // List down the files in the zip.
        ZipFile *unzipFile= [[ZipFile alloc] initWithFileName:srcFile
                                                         mode:ZipFileModeUnzip];
        NSArray *infos= [unzipFile listFileInZipInfos];
        for (FileInZipInfo *info in infos) {

            // LogInfo(@"- %@ %@ %d (%d)", info.name, info.date, info.size, info.level);
            NSString *dirPath;
            NSString *fileName;
            NSString *filePath;
            if ([info.name hasSuffix:@"/"]) {

                dirPath = [destPath stringByAppendingPathComponent:info.name];
                if(shouldExtractDir && shouldExtractDir(dirPath)) {

                    // create the directory
                    if(! [fileManager createDirectoryAtPath:dirPath
                                withIntermediateDirectories:YES
                                                 attributes:nil
                                                      error:&err]) {

                        errorMessage = [NSString stringWithFormat:@"Creation of directory %@ failed. Reason - %@", info.name, [err localizedDescription]];
                        status = NO;
                        break;
                    }
                }
            } else {

                fileName = info.name;
                // Create the package directory
                filePath = [destPath stringByAppendingPathComponent:fileName];

                NSString *dirPath = [filePath stringByDeletingLastPathComponent];
                if(shouldExtractDir && !shouldExtractDir(dirPath)) continue;

                // Check and create the parent directories of the file
                if(! [dirPath isEqualToString:@""]) {
                    if(! [fileManager createDirectoryAtPath:dirPath
                                withIntermediateDirectories:YES
                                                 attributes:nil
                                                      error:&err]) {
                        errorMessage = [NSString stringWithFormat:@"Creation of parent directory %@ failed. Reason - %@", dirPath, [err localizedDescription]];
                        status = NO;
                        break;
                    }
                }

                // Locate the file in the zip
                [unzipFile locateFileInZip:info.name];

                // Expand the file in memory
                ZipReadStream *read= [unzipFile readCurrentFileInZip];
                NSMutableData *data = [[NSMutableData alloc] initWithLength:info.length];
                [read readDataWithBuffer:data];
                [read finishedReading];
                
                if (shouldExtractFile) {

                    if(shouldExtractFile(filePath, &data)) {

                        status = [data writeToFile:filePath options:NSDataWritingAtomic error:&err];
                    }
                } else {

                    status = [data writeToFile:filePath options:NSDataWritingAtomic error:&err];
                }

                if(! status) {

                    errorMessage = [NSString stringWithFormat:@"Creation of file %@ failed. Reason - %@", info.name, [err localizedDescription]];
                    break;
                }
            }
        }

        [unzipFile close];
    }
    @catch (ZipException *exception) {

        errorMessage = [exception reason];
        LogError(@"Package %@ installation error: %@", destPath, errorMessage);
        status = NO;
    }

    if(!status && error) {
        
        *error = [MBLUtils errorWithCode:0 description:errorMessage];
    }
    
    return status;
} // unzipFile:toPath:error:

- (BOOL)write:(NSString *)path content:(NSString *)content shouldAppendContent:(BOOL)bAppend error:(NSError **)error {

    bool status = NO;
    NSString *parentDir = [path stringByDeletingLastPathComponent];
    NSFileManager *fileManager = [[NSFileManager alloc] init];
    BOOL isDir;
    
    if (! [fileManager fileExistsAtPath:parentDir isDirectory:&isDir] || !isDir)
    {
        if([fileManager createDirectoryAtPath:parentDir withIntermediateDirectories:YES attributes:nil error:error])
        {
            status = [content writeToFile:path atomically:YES encoding:NSUTF8StringEncoding error:error];
        }
    }
    else
    {
        if(bAppend)
        {
            NSString  *theContent = [NSString stringWithContentsOfFile:path encoding:NSUTF8StringEncoding error:error];
            theContent = [NSString stringWithFormat:@"%@%@",theContent,content];
            status = [theContent writeToFile:path atomically:YES encoding:NSUTF8StringEncoding error:error];
        }
        else
            status = [content writeToFile:path atomically:YES encoding:NSUTF8StringEncoding error:error];
    }
    
    return status;
} // write:content:mode:error

- (BOOL)writeData:(NSString *)path content:(NSString *)base64String shouldAppendContent:(BOOL)bAppend error:(NSError **)error {
    
  	// Decode base64 content
    NSData *decodedData = [QSStrings decodeBase64WithString:base64String];
    bool status = NO;
    if (decodedData && decodedData.length > 0)
    {
        NSString *parentDir = [path stringByDeletingLastPathComponent];
        NSFileManager *fileManager = [[NSFileManager alloc] init];
        BOOL isDir;
        
        if (! [fileManager fileExistsAtPath:parentDir isDirectory:&isDir] || !isDir)
        {
            if([fileManager createDirectoryAtPath:parentDir withIntermediateDirectories:YES attributes:nil error:error])
            {
                status = [decodedData writeToFile:path options:NSDataWritingAtomic error:error];
            }
        }
        else
        {
            if(bAppend)
            {
                NSMutableData *theData = [NSData dataWithContentsOfFile:path options:(NSDataReadingOptions)NSDataWritingAtomic error:error];
                [theData appendData:decodedData];
                [theData writeToFile:path options:NSDataWritingAtomic error:error];
            }
            else
                status = [decodedData writeToFile:path options:NSDataWritingAtomic error:error];
        }
    }
    return status;
} // writeData:content:mode:error

- (unsigned long long)sizeofItemAtPath:(NSString *)path {

    __block unsigned long long size = 0;
    NSError *error = nil;
    NSFileManager *fileManager = [[NSFileManager alloc] init];
    BOOL isDir;
    if([fileManager fileExistsAtPath:path isDirectory:&isDir]) {

        if(isDir) {

            NSDirectoryEnumerator *dirEnum = [fileManager enumeratorAtPath:path];
            NSString *file;
            while((file = [dirEnum nextObject])) {

                NSDictionary *fileAttributes = [fileManager attributesOfItemAtPath:[NSString stringWithFormat:@"%@/%@",path,file] error:&error];
                if(fileAttributes) {

                    size += [[fileAttributes objectForKey:NSFileSize] longLongValue];
                } else {

                    LogError(@"Dir Size: Error retrieving attributes for file %@. Reason - %@", file, [error description]);
                }
            }
        } else {

            NSDictionary *fileAttributes = [fileManager attributesOfItemAtPath:path error:&error];
            if(fileAttributes) {

                size = [[fileAttributes objectForKey:NSFileSize] longLongValue];
            } else {

                LogError(@"File Size: Error retrieving attributes for file %@. Reason - %@", path, [error description]);
            }
        }
    } else {

        LogError(@"File/Dir Size: No item found at path %@", path);
    }

    return size;
} // sizeOfItemAtPath:

- (unsigned long long)totalFreeMemory {

    unsigned long long totalFreeSpace = 0;

    NSError *error = nil;
    NSArray *paths = NSSearchPathForDirectoriesInDomains(NSDocumentDirectory, NSUserDomainMask, YES);
    NSFileManager *fileManager = [[NSFileManager alloc] init];
    NSDictionary *fileAttributes = [fileManager attributesOfFileSystemForPath:[paths lastObject] error:&error];

    if (fileAttributes) {

        NSNumber *freeFileSystemSizeInBytes = [fileAttributes objectForKey:NSFileSystemFreeSize];
        totalFreeSpace = [freeFileSystemSizeInBytes unsignedLongLongValue];
        LogDebug(@"Avaiable memory %llu MiB", ((totalFreeSpace/1024ll)/1024ll));
    } else {

        NSLog(@"Error Obtaining System Memory Info: Domain = %@, Code = %ld", [error domain], (long)[error code]);
    }

    return totalFreeSpace;
} // totalFreeMemory

- (unsigned long long)totalMemory {

    unsigned long long totalSpace = 0;

    NSError *error = nil;
    NSArray *paths = NSSearchPathForDirectoriesInDomains(NSDocumentDirectory, NSUserDomainMask, YES);
    NSFileManager *fileManager = [[NSFileManager alloc] init];
    NSDictionary *fileAttributes = [fileManager attributesOfFileSystemForPath:[paths lastObject] error:&error];

    if (fileAttributes) {

        NSNumber *fileSystemSizeInBytes = [fileAttributes objectForKey: NSFileSystemSize];
        totalSpace = [fileSystemSizeInBytes unsignedLongLongValue];
        LogDebug(@"Total memory %llu MiB", ((totalSpace/1024ll)/1024ll));
    } else {

        NSLog(@"Error Obtaining System Memory Info: Domain = %@, Code = %ld", [error domain], (long)[error code]);
    }
    
    return totalSpace;
} // totalMemory

#pragma mark -
#pragma mark Utils

BOOL shouldExtractMowblyAppFile(NSString *filePath, NSMutableData **data) {

    BOOL shouldExtract = YES;
    
    // Ignore system files. Hidden files are created when HTML content is changed during Meta tag injection.
    NSString *fileName = [filePath lastPathComponent];
    shouldExtract = !([fileName hasPrefix:@"__MACOSX"] || [fileName hasPrefix:@".DS_Store"]);

    // Write the content. Update meta tag in HTML content.
    if(shouldExtract && ([[filePath pathExtension] isEqualToString:@"htm"] || [[filePath pathExtension] isEqualToString:@"html"])) {

        NSString *html = [[NSString alloc] initWithData:*data encoding:NSUTF8StringEncoding];
        html = [html stringByReplacingOccurrencesOfString:@"<meta name=\"viewport\" content=\"width=device-width,user-scalable=no,maximum-scale=1.0,initial-scale=1.0,height=device-height\" />"
                                               withString:@"<meta name=\"viewport\" content=\"width=device-width,user-scalable=no,maximum-scale=1.0,initial-scale=1.0\" />"];
        *data = [[html dataUsingEncoding:NSUTF8StringEncoding] mutableCopy];
    }

    return shouldExtract;
}

@end