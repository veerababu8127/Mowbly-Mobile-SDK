//
//  MBLFileManager.h
//  Mowbly
//
//  Created by Sathish on 01/10/13.
//  Copyright (c) 2011-2014 CloudPact. All rights reserved.

#import <Foundation/Foundation.h>

@class MBLFeature;

typedef enum {
	FILE_INTERNAL_STORAGE = 0,	// Documents directory
	FILE_EXTERNAL_STORAGE,		// Documents directory
    FILE_CACHE
} StorageType;

typedef enum {
	TYPE_FILE = 0,
	TYPE_DIRECTORY
} FileType;

typedef enum {
    APP_ROOT = 1,
    STORAGE_ROOT,
    DOCUMENT_ROOT
} FileLevel;

@interface MBLFileManager : NSObject {

    NSNumber *_errorCode;    // Code for file operation error - 0
    NSNumber *_successCode;  // Code for file operation success - 1
}

@property (nonatomic, retain)NSNumber *errorCode;
@property (nonatomic, retain)NSNumber *successCode;

+ (MBLFileManager *)defaultManager;

+ (id)create;

#pragma mark -
#pragma mark File feature methods

- (NSString *)absoluteDirectoryPathForLevel:(FileLevel)level
                                    storage:(StorageType)storageType
                                    andFeature:(MBLFeature *)feature;

- (void)clearCacheDir;

- (BOOL)deleteDirectory:(NSString *)path error:(NSError **)error;

- (BOOL)deleteFile:(NSString *)path error:(NSError **)error;

- (void)deleteItemsAtPath:(NSString *)path olderThan:(long long)when;

- (BOOL)getDirectory:(NSString *)path error:(NSError **)error;

- (BOOL)getFile:(NSString *)path error:(NSError **)error;

- (NSArray *)getFiles:(NSString *)path error:(NSError **)error;

- (NSString *)read:(NSString *)path error:(NSError **)error;

- (NSString *)readData:(NSString *)path error:(NSError **)error;

- (BOOL)testDirExists:(NSString *)path error:(NSError **)error;

- (BOOL)testFileExists:(NSString *)path error:(NSError **)error;

- (BOOL)unzipFile:(NSString *)srcFile
           toPath:(NSString *)destPath
            error:(NSError **)error
 shouldExtractDir:(BOOL(^)(NSString *dirPath))shouldExtractDir
shouldExtractFile:(BOOL(^)(NSString *filePath, NSMutableData **data))shouldExtractFile;

- (BOOL)write:(NSString *)path content:(NSString *)content shouldAppendContent:(BOOL)bAppend error:(NSError **)error;

- (BOOL)writeData:(NSString *)path content:(NSString *)base64String shouldAppendContent:(BOOL)bAppend error:(NSError **)error;

- (unsigned long long)sizeofItemAtPath:(NSString *)path;

- (unsigned long long)totalFreeMemory;

- (unsigned long long)totalMemory;

@end