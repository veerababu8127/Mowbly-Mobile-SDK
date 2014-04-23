//
//  MBLDevice.m
//  Mowbly
//
//  Created by Sathish on 23/10/11.
//  Copyright (c) 2011-2014 CloudPact. All rights reserved.
//

#import "MBLApp.h"
#import "MBLAppDelegate.h"
#import "MBLDevice.h"
#import "MBLFeatureResult.h"
#import "MBLJSMessage.h"
#import "OpenUDID.h"

static float iosVersion;

@implementation MBLDevice

NSString* convertBytesToGB(unsigned long long ullSize)
{    
    NSString *suffix = @"KB";
    double size = ullSize;
    float oneKb = 1024.0;
    if (size >= oneKb)
    {
        size /= oneKb;
        if (size >= oneKb)
        {
            suffix = @"MB";
            size /= oneKb;
            if(size >= oneKb)
            {
                suffix = @"GB";
                size /= oneKb;
            }
        }
    }
    return [NSString stringWithFormat:@"%0.2f%@", size, suffix];
} // convertBytesToGB

- (void) invoke:(MBLJSMessage *)message
{
    NSNumber *code = RESPONSE_CODE_OK;
	id response = nil;
    if ([message.method isEqualToString:@"getDeviceId"])
    {
        response = [OpenUDID value];
    }
    else if ([message.method isEqualToString:@"getMemoryStatus"])
    {
        NSFileManager *fileManager = [[NSFileManager alloc] init];
        NSMutableDictionary *result = [[NSDictionary dictionaryWithObjects:[NSArray arrayWithObjects:@"0MB", @"0MB", nil]
                                                                   forKeys:[NSArray arrayWithObjects:@"totalExternalMemory", @"availableExternalMemory", nil]] mutableCopy];
        // App Memory
        NSString *appDir = [MBLApp appDirectory];
        NSArray *filesArray = [fileManager subpathsOfDirectoryAtPath:appDir error:nil];
        NSEnumerator *filesEnumerator = [filesArray objectEnumerator];
        NSString *fileName;
        unsigned long long folderSize = 0;
        
        while (fileName = [filesEnumerator nextObject])
        {
            NSDictionary *fileDictionary = [fileManager attributesOfItemAtPath:[appDir stringByAppendingPathComponent:fileName] error:nil];
            folderSize += [fileDictionary fileSize];
        }
        [result setValue:convertBytesToGB(folderSize) forKey:@"applicationMemory"];
        
        // Internal memory
        NSDictionary *fattributes = [fileManager attributesOfFileSystemForPath:NSHomeDirectory() error:nil];
        [result setValue:convertBytesToGB([[fattributes objectForKey:NSFileSystemSize] unsignedLongLongValue])
                  forKey:@"totalInternalMemory"];
        
        [result setValue:convertBytesToGB([[fattributes objectForKey:NSFileSystemFreeSize] unsignedLongLongValue])
                  forKey:@"availableInternalMemory"];
        response = [NSDictionary dictionaryWithDictionary:result];
    }
    else
    {
        // Not a supported method
        LogError(@"Feature %@ does not support method %@.", message.feature, message.method);
    }

    // Push the response/error to javascript
    if(response)
    {
        [self pushResponseToJavascript:response withCode:code andCallBackId:message.callbackId];
    }
    else
    {
        [self pushErrorToJavascript:nil withCode:RESPONSE_CODE_ERROR andCallBackId:message.callbackId];
    }
} // invoke:

+ (float) iosVersion {

    if (!iosVersion) {

        iosVersion = [[[UIDevice currentDevice] systemVersion] floatValue];
    }

    return iosVersion;
}

@end