//
//  MBLFile.m
//  Mowbly
//
//  Created by Sathish on 14/06/11.
//  Copyright (c) 2011-2014 CloudPact. All rights reserved.
//

#import "MBLApp.h"
#import "MBLAppDelegate.h"
#import "MBLFeatureResult.h"
#import "MBLFile.h"
#import "MBLFileManager.h"
#import "MBLJSMessage.h"
#import "MBLPage.h"
#import "MBLUtils.h"

@interface MBLFile (private)

- (void) invokeAsync:(MBLJSMessage *)message;

- (void) invokeSyncResponse:(NSArray *)array;

- (void) invokeSyncError:(NSArray *)array;

@end

@implementation MBLFile

- (void)invoke:(MBLJSMessage *)message {
    
    NSOperationQueue *tasksQueue = [NSOperationQueue new];
    [tasksQueue setMaxConcurrentOperationCount:1];
    NSInvocationOperation *operation = nil;
    
    // Adding to queue
    operation = [[NSInvocationOperation alloc] initWithTarget:self
                                                     selector:@selector(invokeAsync:)
                                                       object:message];
    [tasksQueue addOperation:operation];
    
} // invoke:

-(void)invokeAsync:(MBLJSMessage *)message{
    
    MBLFileManager *fileManager = [MBLFileManager defaultManager];
    id response = nil;
	NSError *error = nil;
    NSDictionary *options = [message.args objectAtIndex:(message.args.count-1)];
    
    // Get the parent directory for the path with the specified options
    int storageType = [[options objectForKey:@"storageType"] intValue];
    int level = [[options objectForKey:@"level"] intValue];
    NSString *rootDir = [fileManager absoluteDirectoryPathForLevel:level
                                                           storage:storageType
                                                           andFeature:self];
    // Get the path
    NSString *path = (NSString *)(([message.method isEqualToString:@"unzip"]) ? [options objectForKey:@"path"] : [message.args objectAtIndex:0]);
    path = ([path hasPrefix:@"/"]) ? path : [rootDir stringByAppendingPathComponent:path];
    
	if ([message.method isEqualToString:@"deleteDirectory"]) {
        
        if ([fileManager deleteDirectory:path error:&error]) {
            
            response = [NSNumber numberWithBool:YES];
        }
	} else if ([message.method isEqualToString:@"deleteFile"]) {
        
        if ([fileManager deleteFile:path error:&error]) {
            
            response = [NSNumber numberWithBool:YES];
        }
	} else if ([message.method isEqualToString:@"getDirectory"]) {
        
        
        if ([fileManager getDirectory:path error:&error]) {
            
            response = [NSNumber numberWithBool:YES];
        }
    } else if ([message.method isEqualToString:@"getFile"]) {
        
        
        if ([fileManager getFile:path error:&error]) {
            
            response = [NSNumber numberWithBool:YES];
        }
	} else if ([message.method isEqualToString:@"getFilesJSONString"]) {
        
        response = [fileManager getFiles:path error:&error];
	} else if ([message.method isEqualToString:@"getRootDirectory"]) {
        
        response = rootDir;
        
	} else if ([message.method isEqualToString:@"read"]) {
        
        response = [fileManager read:path error:&error];
        
	} else if ([message.method isEqualToString:@"readData"]) {
        
        response = [fileManager readData:path error:&error];
        
	} else if ([message.method isEqualToString:@"testDirExists"]) {
        
        response = [NSNumber numberWithBool:[fileManager testDirExists:path error:&error]];
        
	} else if ([message.method isEqualToString:@"testFileExists"]) {
        
        if ([fileManager testFileExists:path error:&error]) {
            
            response = [NSNumber numberWithBool:YES];
        } else {
            
            response = [NSNumber numberWithBool:NO];
        }
    } else if ([message.method isEqualToString:@"unzip"]) {
        
        NSDictionary *srcOptions = [message.args objectAtIndex:0];
        
        // Get the source path with the specified options
        int srcStorageType = [[srcOptions objectForKey:@"storageType"] intValue];
        int srcLevel = [[srcOptions objectForKey:@"level"] intValue];
        NSString *srcRootDir = [fileManager absoluteDirectoryPathForLevel:srcLevel
                                                                  storage:srcStorageType
                                                                  andFeature:self];
        // Get the path
        NSString *srcPath = (NSString *)[srcOptions objectForKey:@"path"];
        srcPath = ([srcPath hasPrefix:@"/"]) ? srcPath : [srcRootDir stringByAppendingPathComponent:srcPath];
        response = [NSNumber numberWithBool:[fileManager unzipFile:srcPath toPath:path error:&error shouldExtractDir:nil shouldExtractFile:nil]];
        
    } else if ([message.method isEqualToString:@"write"]) {
        
        response = [NSNumber numberWithBool:[fileManager write:path
                                                       content:(NSString*)[message.args objectAtIndex:1]
                                           shouldAppendContent:[(NSNumber*)[message.args objectAtIndex:2] boolValue]
                                                         error:&error]];
        
	} else if ([message.method isEqualToString:@"writeData"]) {
        
        response = [NSNumber numberWithBool:[fileManager writeData:path
                                                           content:(NSString*)[message.args objectAtIndex:1]
                                               shouldAppendContent:[(NSNumber*)[message.args objectAtIndex:2] boolValue]
                                                             error:&error]];
    } else {
        
		// Not a supported method
		LogError(@"Feature %@ does not support method %@.", message.feature, message.method);
	}
    
	if(! response) {
    
        NSArray *arr=[[NSArray alloc]initWithObjects:error,self.RESPONSE_CODE_ERROR,message.callbackId, nil];
        
        //handover control to main thread
        [self performSelectorOnMainThread:@selector(invokeSyncError:) withObject:arr waitUntilDone:NO];
	} else {
        
        NSArray *arr=[[NSArray alloc]initWithObjects:response,self.RESPONSE_CODE_OK,message.callbackId, nil];
        
        //handover control to main thread
        [self performSelectorOnMainThread:@selector(invokeSyncResponse:) withObject:arr waitUntilDone:NO];
	}
	error = nil;
}//invoke Async

- (void) invokeSyncResponse:(NSArray *)array{
    [self pushResponseToJavascript:[array objectAtIndex:0] withCode:[array objectAtIndex:1] andCallBackId:[array objectAtIndex:2]];
}//invokeSyncresponse

- (void) invokeSyncError:(NSArray *)array{
    [self pushErrorToJavascript:[array objectAtIndex:0] withCode:[array objectAtIndex:1] andCallBackId:[array objectAtIndex:2]];
}//invokesyncerror

@end